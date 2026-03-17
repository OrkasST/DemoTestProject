using Assets.Scripts.Actions.Attack;
using Assets.Scripts.Actions.Block;
using Assets.Scripts.Animator;
using System;
using System.Collections;
using UnityEngine;


namespace Assets.Scripts.Weapon
{
    public abstract class AbstractWeapon
    {
        protected WeaponData _weaponData;

        protected Func<IEnumerator, Coroutine> _routineStarter;
        protected Action<Coroutine> _routineCanceler;

        protected GameObject _lightAttackHitbox;
        protected GameObject _specialAttackHitbox;
        protected GameObject _blockHitbox;

        protected Attack _lightAttack;
        protected Attack _specialAttack;
        protected Block _block;

        protected ParryDashing _parryDash;

        public int Damage(AttackType attackType)
        {
            return attackType == AttackType.Special ? _specialAttack.Damage : _lightAttack.Damage;
        } 

        public void Initialize(GameObject lightAttackHitbox, GameObject specialAttackHitbox, GameObject blockHitbox, Func<IEnumerator, Coroutine> routineStarter,
            Action<Coroutine> routineCanceler, ActorStateMachine stateMachine, Rigidbody2D rb, AnimatorController animatorController,
            Action<AttackStates> onAttackStateChange, Action<BlockStates> onBlockStateChange, BoxCollider2D actorCollider, CapsuleCollider2D blockCollider)
        {
            _lightAttackHitbox = lightAttackHitbox;
            _specialAttackHitbox = specialAttackHitbox;
            _blockHitbox = blockHitbox;

            _routineStarter = routineStarter;
            _routineCanceler = routineCanceler;

            _lightAttack = new Attack(stateMachine, rb, routineStarter, routineCanceler, animatorController, _weaponData.LightAttackData, lightAttackHitbox, onAttackStateChange);
            _specialAttack = new Attack(stateMachine, rb, routineStarter, routineCanceler, animatorController, _weaponData.SpecialAttackData, specialAttackHitbox, onAttackStateChange);
            _block = new Block(stateMachine, rb, routineStarter, routineCanceler, animatorController, _weaponData.BlockData, blockHitbox, onBlockStateChange);

            if (_weaponData.CanDashOnParry)
            {
                _parryDash = new ParryDashing();
                _parryDash.Initialize(stateMachine, rb, routineStarter, animatorController);
                _parryDash.SetUp(actorCollider, blockCollider);
                if (actorCollider == null) Debug.Log("actorCollider");
            }
        }

        public AttackStates CurrentAttackState { get; protected set; } = AttackStates.Waiting;

        public virtual void Attack(AttackType attackType)
        {
            if (attackType == AttackType.Special) _specialAttack.StartAttack();
            else _lightAttack.StartAttack();
        }

        public virtual void StartBlock() => _block.StartBlock();
        public virtual void EndBlock() => _block.EndBlock();

        public virtual void StopAttack()
        {

        }

        internal void Dash(float movementSpeed)
        {
            if (!_weaponData.CanDashOnParry) return;
            _parryDash.Dash(movementSpeed);
        }
    }
}
