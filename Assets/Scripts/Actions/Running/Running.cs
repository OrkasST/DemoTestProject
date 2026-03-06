using System;
using UnityEngine;

namespace Assets.Scripts.Actions.Running
{
    public class Running: AbstractAction
    {
        public Running() : base() { }

        public float MovementSpeed { get; private set; } = 180f;

        public void Move(int direction)
        {
            _rb.linearVelocity = new UnityEngine.Vector2(x: direction * MovementSpeed, y: _rb.linearVelocity.y);
            if (_rb.linearVelocityX < 0.1f && _rb.linearVelocityY > -0.1f && _stateMachine.CurrentState == MachineActorStates.Moving)
            {
                _stateMachine.ChangeState(MachineActorStates.Standing);
                _animatorController.IsRunning = false;
            }
            if (_stateMachine.CurrentState == MachineActorStates.Standing && (_rb.linearVelocityX > 0.1f || _rb.linearVelocityX < -0.1f))
            {
                _stateMachine.ChangeState(MachineActorStates.Moving);
                _animatorController.IsRunning = true;
            }
            if (_stateMachine.CurrentState == MachineActorStates.Standing && _animatorController.IsRunning) _animatorController.IsRunning = false;
        }
    }
}
