using System;
using UnityEngine;

namespace Assets.Scripts.Actions.Running
{
    public class Running: AbstractAction
    {
        public Running() : base() { }

        public float MovementSpeed { get; private set; } = 180f;
        private float _speedDecrease = 0.5f;

        public void Move(int direction)
        {
            _rb.linearVelocity = new UnityEngine.Vector2(x: direction * MovementSpeed *
                (_stateMachine.CurrentBattleState == ActorBattleState.Blocking ? _speedDecrease : 1), y: _rb.linearVelocity.y);

            if (_rb.linearVelocityX < 0.1f && _rb.linearVelocityY > -0.1f && _stateMachine.CurrentState == MachineActorStates.Moving)
            {
                if (_animatorController == null) Debug.Log("AnController NULL");
                if (_animatorController.Animator == null) Debug.Log("Animator NULL");

                _stateMachine.ChangeState(MachineActorStates.Standing);
                _animatorController.Blend = 0;
            }
            if (_stateMachine.CurrentState == MachineActorStates.Standing && (_rb.linearVelocityX > 0.1f || _rb.linearVelocityX < -0.1f))
            {
                _stateMachine.ChangeState(MachineActorStates.Moving);
                _animatorController.Blend = 1;
            }
            if (_stateMachine.CurrentState == MachineActorStates.Standing && _animatorController.Blend == 1) _animatorController.Blend = 0;
        }
    }
}
