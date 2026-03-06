using Assets.Scripts.Actions.Attack;
using Assets.Scripts.Animator;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.BusinessLogic
{
    public class CombatController
    {
        public int MaxHp = 100;
        public int CurrentHp = 100;

        //public CapsuleCollider2D _lightAttackCollider;
        public GameObject _lightAttackCollider;
        public GameObject _specialAttackCollider;

        private Attack _attack;

        private AttackType _currentAttack = AttackType.Light;
        public int CurrentAttackDamage { get => _attack.Damage[_currentAttack]; }

        private Action _destroyAction;

        public void Initialize(ActorStateMachine stateMachine, Rigidbody2D rb, Func<IEnumerator, Coroutine> coroutineStarterFunc, AnimatorController animatorController,
            GameObject lightAttackCollider, GameObject specialAttackCollider, Action destroy, float attackSpeed)
        {
            _lightAttackCollider = lightAttackCollider;
            _specialAttackCollider = specialAttackCollider;

            _lightAttackCollider.SetActive(false);
            _specialAttackCollider.SetActive(false);

            _attack = new Attack(5, 1, 1.2f, _lightAttackCollider, _specialAttackCollider, attackSpeed);
            _attack.Initialize(stateMachine, rb, coroutineStarterFunc, animatorController);

            _lightAttackCollider.GetComponent<AttackHitbox>().SetDamage(_attack.Damage[AttackType.Light]);
            _specialAttackCollider.GetComponent<AttackHitbox>().SetDamage(_attack.Damage[AttackType.Special]);

            _destroyAction = destroy;
        }

        public void GetDamage(int hp)
        {
            if (CurrentHp <= hp)
            {
                CurrentHp = 0;
                _destroyAction();
            }
            else CurrentHp -= hp;
        }

        public void LightAttack() => _attack.StartAttack(AttackType.Light);

        internal void SpecialAttack() => _attack.StartAttack(AttackType.Special);
    }
}
