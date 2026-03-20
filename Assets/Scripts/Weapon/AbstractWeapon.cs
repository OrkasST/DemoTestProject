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
        private ActorStateMachine _stateMachine;
        protected GameObject _lightAttackHitbox;
        protected GameObject _specialAttackHitbox;
        protected GameObject _blockHitbox;

        protected Attack _lightAttack;
        protected Attack _specialAttack;
        protected Block _block;

        protected ParryDashing _parryDash;

        protected Vector3 _dashFinalPoint;

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

            _stateMachine = stateMachine;

            _lightAttack = new Attack(stateMachine, rb, routineStarter, routineCanceler, animatorController, _weaponData.LightAttackData, lightAttackHitbox, onAttackStateChange);
            _specialAttack = new Attack(stateMachine, rb, routineStarter, routineCanceler, animatorController, _weaponData.SpecialAttackData, specialAttackHitbox, onAttackStateChange);
            _block = new Block(stateMachine, rb, routineStarter, routineCanceler, animatorController, _weaponData.BlockData, blockHitbox, onBlockStateChange);

            if (_weaponData.CanDashOnParry)
            {
                _parryDash = new ParryDashing();
                _parryDash.Initialize(stateMachine, rb, routineStarter, animatorController);
                _parryDash.SetUp(actorCollider, blockCollider, routineCanceler);
                if (actorCollider == null) Debug.Log("actorCollider");
            }
        }

        public AttackStates CurrentAttackState { get; protected set; } = AttackStates.Waiting;

        public virtual void Attack(AttackType attackType)
        {
            if (_stateMachine.CurrentBattleState == ActorBattleState.Blocking && !_weaponData.CanAttackWhileBlocking) return;
            if (attackType == AttackType.Special) _specialAttack.StartAttack();
            else _lightAttack.StartAttack();
        }
        public virtual void Counterattack(Action<MachineDirection, bool?> changeDirectionFunc, MachineDirection direction)
        {
            Debug.Log("COUNTEERATTACK");
            if (!_weaponData.CanCounterAttack) return;
            _parryDash.Interrupt();
            _parryDash.ExitIntoStanding();
            changeDirectionFunc(direction == MachineDirection.Left ? MachineDirection.Right : MachineDirection.Left, true);
            _lightAttack.StartAttack();
        }

        public virtual void StartBlock() => _block.StartBlock();
        public virtual void EndBlock() => _block.EndBlock();
        public virtual void InterruptBlock() => _block.InterruptBlock();
        public virtual void StopAttack()
        {

        }

        public bool CanRemoveBlock()
        {
            return Time.time - _block.BlockingStartTime > 0.2f;
        }
        public void PrepareDash(float enemyWidth, Vector3 enemyPosition, Vector3 myPosition, float myWidth, int direction)
        {
            _dashFinalPoint = new Vector3(enemyPosition.x + direction/2f * (enemyWidth + myWidth), myPosition.y, myPosition.z);
        }

        public void Dash(float movementSpeed)
        {
            if (!_weaponData.CanDashOnParry) return;
            _parryDash.Dash(movementSpeed, _dashFinalPoint);
        }
    }
}
