using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Actions.Dashing
{
    public class Dashing: AbstractAction
    {
        public float DashSpeed { get; private set; } = 400f;
        private int _dashDirection = 0;

        public Dashing() : base() { }


        public void Dash(float movementSpeed)
        {
            _animatorController.IsRunning = true;
            _startRoutine(DashHandle(movementSpeed));
        }

        private IEnumerator DashHandle(float movementSpeed)
        {
            _stateMachine.ChangeState(MachineActorStates.Dashing);
            _dashDirection = (int)_stateMachine.Direction;
            Debug.Log(_dashDirection);
            _rb.linearVelocityX = _dashDirection * DashSpeed;

            yield return new WaitForSeconds(0.2f);
            _rb.linearVelocityX = _dashDirection * (DashSpeed * 0.7f);

            yield return new WaitForSeconds(0.15f);
            if (_stateMachine.PreviousState != MachineActorStates.Moving)
            {
                _rb.linearVelocityX = 0;
                _stateMachine.ChangeState(MachineActorStates.Standing);
                _animatorController.IsRunning = false;
            }
            else
            {
                _rb.linearVelocityX = _dashDirection * movementSpeed;
                _stateMachine.ChangeState(MachineActorStates.Moving);
            }
        }
    }
}
