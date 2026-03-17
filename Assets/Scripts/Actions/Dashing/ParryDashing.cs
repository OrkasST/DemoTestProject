using Assets.Scripts.Actions.Dashing;
using System;
using System.Collections;
using UnityEngine;

public class ParryDashing: Dashing
{
    private BoxCollider2D _actorCollider;
    private CapsuleCollider2D _blockCollider;

    public void SetUp(BoxCollider2D actorCollider, CapsuleCollider2D blockCollider)
    {
        _actorCollider = actorCollider;
        _blockCollider = blockCollider;
    }

    public override void Dash(float movementSpeed)
    {
        _animatorController.IsRunning = true;
        _startRoutine(DashHandle(movementSpeed, _actorCollider, _blockCollider));
    }
    
    protected IEnumerator DashHandle(float movementSpeed, BoxCollider2D actorCollider, CapsuleCollider2D blockCollider)
    {
        Debug.Log("Value 1: "+ _actorCollider.excludeLayers.value);

        _actorCollider.excludeLayers = 128;
        _blockCollider.excludeLayers = 128;
        Debug.Log("Value 2: " + _actorCollider.excludeLayers.value);

        _stateMachine.ChangeState(MachineActorStates.Dashing);
        _stateMachine.ChangeBattleState(ActorBattleState.CanContrattack);
        _dashDirection = (int)_stateMachine.Direction;
        Debug.Log(_dashDirection);
        _rb.linearVelocityX = _dashDirection * DashSpeed;

        yield return new WaitForSeconds(0.2f);
        _rb.linearVelocityX = _dashDirection * (DashSpeed * 0.7f);
        //_stateMachine.ChangeBattleState(ActorBattleState.Dashing);

        yield return new WaitForSeconds(0.15f);
        if (_stateMachine.PreviousState != MachineActorStates.Moving)
        {
            _rb.linearVelocityX = 0;
            _stateMachine.ChangeState(MachineActorStates.Standing);
            _stateMachine.ChangeBattleState(ActorBattleState.Waiting);
            _animatorController.IsRunning = false;
        }
        else
        {
            _rb.linearVelocityX = _dashDirection * movementSpeed;
            _stateMachine.ChangeState(MachineActorStates.Moving);
            _stateMachine.ChangeBattleState(ActorBattleState.Waiting);  
        }

        //_actorCollider.excludeLayers = 0;
        //_blockCollider.excludeLayers = 0;
    }
}
