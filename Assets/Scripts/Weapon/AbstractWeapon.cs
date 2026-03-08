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

        public int Damage(AttackType attackType)
        {
            return attackType == AttackType.Special ? _specialAttack.Damage : _lightAttack.Damage;
        } 

        public void Initialize(GameObject lightAttackHitbox, GameObject specialAttackHitbox, GameObject blockHitbox, Func<IEnumerator, Coroutine> routineStarter,
            Action<Coroutine> routineCanceler, ActorStateMachine stateMachine, Rigidbody2D rb, AnimatorController animatorController)
        {
            _lightAttackHitbox = lightAttackHitbox;
            _specialAttackHitbox = specialAttackHitbox;
            _blockHitbox = blockHitbox;

            _routineStarter = routineStarter;
            _routineCanceler = routineCanceler;

            _lightAttack = new Attack(stateMachine, rb, routineStarter, routineCanceler, animatorController, _weaponData.LightAttackData, lightAttackHitbox);
            _specialAttack = new Attack(stateMachine, rb, routineStarter, routineCanceler, animatorController, _weaponData.SpecialAttackData, specialAttackHitbox);
            _block = new Block(stateMachine, rb, routineStarter, routineCanceler, animatorController, _weaponData.BlockData, blockHitbox);
        }

        public AttackStates CurrentAttackState { get; protected set; } = AttackStates.Waiting;

        public virtual void Attack(AttackType attackType)
        {
            if (attackType == AttackType.Special) _specialAttack.StartAttack();
            else _lightAttack.StartAttack();
        }

        public virtual void StartBlock() => _block.StartBlock();
        public virtual void EndBlock() => _block.EndBlock();
    }
}
