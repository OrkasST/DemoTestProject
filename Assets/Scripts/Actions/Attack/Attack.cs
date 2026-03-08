using Assets.Scripts.Animator;
using System;
using System.Collections;
using UnityEngine;

public enum AttackType { Light, Special }

namespace Assets.Scripts.Actions.Attack
{
    public class Attack: AbstractAction
    {
        private AttackData _attackData;
        private GameObject _hitbox;

        private Coroutine _currentAttack;
        Action<Coroutine> _coroutineCancelFunction;

        public int Damage { get; private set; }

        public Attack(ActorStateMachine stateMachine, Rigidbody2D rb, Func<IEnumerator, Coroutine> coroutineStarterFunc, Action<Coroutine> coroutineCancelFunction,
            AnimatorController animatorController, AttackData attackData, GameObject hitbox)
        {
            Initialize(stateMachine, rb, coroutineStarterFunc, animatorController);
            _attackData = attackData;
            _hitbox = hitbox;
            _hitbox.SetActive(false);
            _coroutineCancelFunction = coroutineCancelFunction;

            Damage = _attackData.Damage;
            _hitbox.GetComponent<AttackHitbox>().SetUp(Damage, ParryAttack, InterruptAttack);
        }

        public void StartAttack()
        {
            if (_stateMachine.CurrentAttackState != AttackStates.Recovering && _stateMachine.CurrentAttackState != AttackStates.Waiting) return;
            if (_stateMachine.CurrentAttackState == AttackStates.Recovering) _coroutineCancelFunction(_currentAttack);
            _currentAttack = _startRoutine(AttackHandle());
        }

        public void InterruptAttack()
        {
            _startRoutine(DisableAttack(AttackStates.Interrupted));
        }
        public void ParryAttack()
        {
            _startRoutine(DisableAttack(AttackStates.Parred));
        }

        private IEnumerator DisableAttack(AttackStates attackState)
        {
            _coroutineCancelFunction(_currentAttack);
            _hitbox.SetActive(false);
            _stateMachine.ChangeAttackState(attackState);

            if (attackState == AttackStates.Parred) yield return new WaitForSeconds(1.3f);
            else yield return new WaitForSeconds(0.4f);

            _stateMachine.ChangeAttackState(AttackStates.Waiting);
        }

        private IEnumerator AttackHandle()
        {
            #region ChargingState
            _stateMachine.ChangeAttackState(AttackStates.Charging);
            _hitbox.transform.localPosition = _attackData.InitialPosition;
            _hitbox.transform.localRotation = _attackData.InitialRotation;
            _hitbox.transform.localScale = _attackData.InitialScale;
            yield return new WaitForSeconds(_attackData.ChargingTime);

            #endregion

            #region AcceleratingState
            var time = Time.time;
            _stateMachine.ChangeAttackState(AttackStates.Accelerating);
            Vector3 movementSpeed = CalculateSpeed(_hitbox.transform, _attackData.DealingDamagePosition, _attackData.AccelerateTime);
            _hitbox.SetActive(true);
            while(_hitbox.transform.localPosition != _attackData.DealingDamagePosition && Time.time - time < _attackData.AccelerateTime )
            {
                MoveHitbox(movementSpeed);
                yield return null;
            }

            #endregion

            #region DelingDamageState
            _stateMachine.ChangeAttackState(AttackStates.DealingDamage);
            movementSpeed = CalculateSpeed(_hitbox.transform, _attackData.EndPosition, _attackData.DealDamageTime);

            time = Time.time;
            while (_hitbox.transform.localPosition != _attackData.EndPosition && Time.time - time < _attackData.DealDamageTime)
            {
                MoveHitbox(movementSpeed);
                yield return null;
            }

            #endregion

            #region RecoveryState

            _hitbox.SetActive(false);
            _stateMachine.ChangeAttackState(AttackStates.Recovering);
            yield return new WaitForSeconds(_attackData.RecoveryTime);
            #endregion

            _stateMachine.ChangeAttackState(AttackStates.Waiting);
        }

        protected Vector3 CalculateSpeed(Transform hitboxTransform, Vector3 destination, float time)
        {
            if (time == 0) time = 0.1f;
           
            return new Vector3((destination.x - hitboxTransform.localPosition.x)/time, (destination.y - hitboxTransform.localPosition.y)/time);
        }

        protected void MoveHitbox(Vector3 speed)
        {
            _hitbox.transform.localPosition = new Vector3(_hitbox.transform.localPosition.x + speed.x * Time.deltaTime,
                    _hitbox.transform.localPosition.y + speed.y * Time.deltaTime);
        }
    }
}
