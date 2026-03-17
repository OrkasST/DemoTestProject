using Assets.Scripts.Actions.Attack;
using Assets.Scripts.Animator;
using Assets.Scripts.Weapon;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.BusinessLogic
{
    public class CombatController
    {
        public int MaxHp = 100;
        public int CurrentHp = 100;

        public GameObject _lightAttackCollider;
        public GameObject _specialAttackCollider;
        public GameObject _blockCollider;

        //private AttackType _currentAttack = AttackType.Light;
        public int CurrentAttackDamage { get => 5; }

        private ActorStateMachine _stateMachine;
        private Rigidbody2D _rb;
        private AnimatorController _animatorController;
        private Action _destroyAction;
        private Action<Coroutine> _coroutineCancelFunction;
        private Func<IEnumerator, Coroutine> _coroutineStarterFunc;

        private AbstractWeapon _weapon;
        private AttackType _currentAttackType;
        private BoxCollider2D _actorCollider;

        public void Initialize(ActorStateMachine stateMachine, Rigidbody2D rb, Func<IEnumerator, Coroutine> coroutineStarterFunc, Action<Coroutine> coroutineCancelFunction,
            AnimatorController animatorController, GameObject lightAttackCollider, GameObject specialAttackCollider, GameObject blockCollider, BoxCollider2D actorCollider, Action destroy)
        {
            _stateMachine = stateMachine;
            _rb = rb;
            _animatorController = animatorController;
            _lightAttackCollider = lightAttackCollider;
            _specialAttackCollider = specialAttackCollider;
            _blockCollider = blockCollider;

            _coroutineStarterFunc = coroutineStarterFunc;
            _coroutineCancelFunction = coroutineCancelFunction;

            _actorCollider = actorCollider;
            _destroyAction = destroy;
        }


        public void EquipWeapon(AbstractWeapon weapon)
        {
            _weapon = weapon;
            if (_actorCollider == null) Debug.Log("_actorCollider");
            _weapon.Initialize(_lightAttackCollider, _specialAttackCollider, _blockCollider, _coroutineStarterFunc, _coroutineCancelFunction,
                _stateMachine, _rb, _animatorController, OnAttackStateChange, OnBlockStateChange, _actorCollider, _blockCollider.GetComponent<CapsuleCollider2D>());
        }

        private void OnAttackStateChange(AttackStates state)
        {
            _stateMachine.ChangeAttackState(state);
        }
        private void OnBlockStateChange(BlockStates state)
        {
            _stateMachine.ChangeBlockState(state);
        }
        private void OnBattleStateChange(ActorBattleState state)
        {
            _stateMachine.ChangeBattleState(state);
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
        public void InterruptAttack()
        {
            //if (_currentAttackType == AttackType.Light) _weapon.
        }
        public void LightAttack() {
            if (_stateMachine.CurrentBattleState == ActorBattleState.CanContrattack)
                _stateMachine.ChangeDirection(_stateMachine.Direction == MachineDirection.Left ? MachineDirection.Right : MachineDirection.Left);
            _weapon.Attack(AttackType.Light);
            }
        public void SpecialAttack() => _weapon.Attack(AttackType.Special);
        public void StartBlock() => _weapon.StartBlock();
        public void EndBlock() => _weapon.EndBlock();

        public void Dash(float movementSpeed) => _weapon.Dash(movementSpeed);

    }
}
