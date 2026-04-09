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

        private Action<MachineDirection, bool?> _directionChangeFunction;
        private bool _isOutOfBlock = false;
        private bool _hasParredEnemyAttack = false;

        public void Initialize(ActorStateMachine stateMachine, Rigidbody2D rb, Func<IEnumerator, Coroutine> coroutineStarterFunc, Action<Coroutine> coroutineCancelFunction,
            AnimatorController animatorController, GameObject lightAttackCollider, GameObject specialAttackCollider, GameObject blockCollider, BoxCollider2D actorCollider, Action destroy,
            Action<MachineDirection, bool?> directionChangeFunction)
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

            _directionChangeFunction = directionChangeFunction;
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

        public bool CanCounterAttack => _stateMachine.CurrentBattleState == ActorBattleState.CanContrattack;
        public void LightAttack()
        {
            if (CanCounterAttack)
            {
                _weapon.Counterattack(_directionChangeFunction, _stateMachine.Direction);
                return;
            }
            _weapon.Attack(AttackType.Light);
        }
        public void SpecialAttack() => _weapon.Attack(AttackType.Special);
        public void StartBlock()
        {
            _isOutOfBlock = false;
            _hasParredEnemyAttack = false;
            _weapon.StartBlock();
        }
        public void EndBlock()
        {
            _isOutOfBlock = true;
            _weapon.EndBlock();
        }

        public void OnEnemyAttackParred(float enemyWidth, Vector3 enemyPosition)
        {
            _hasParredEnemyAttack = true;
            _weapon.PrepareDash(enemyWidth, enemyPosition, _actorCollider.transform.position, _actorCollider.size.x, (int)_stateMachine.Direction);
        }

        public void Dash(float movementSpeed)
        {
            if (!_hasParredEnemyAttack) return;

            _weapon.InterruptBlock();
            _stateMachine.ChangeBlockState(BlockStates.Waiting);
            _weapon.Dash(movementSpeed);
        }


        public void Update()
        {
            if (_isOutOfBlock && _stateMachine.CurrentBlockState == BlockStates.Blocking && _weapon.CanRemoveBlock())
            {
                _isOutOfBlock = false;
                _weapon.InterruptBlock();
                _stateMachine.ChangeBlockState(BlockStates.Blocking);
                _weapon.EndBlock();
            }
        }

        public void Restore()
        {
            _stateMachine.ChangeAttackState(AttackStates.Waiting);
            _stateMachine.ChangeBlockState(BlockStates.Waiting);
            _stateMachine.ChangeBattleState(ActorBattleState.Waiting);

            CurrentHp = MaxHp;
        }
    }
}
