using Assets.Scripts.Actions.Dashing;
using System;
using System.Collections;
using UnityEngine;

public class ParryDashing: Dashing
{
    private BoxCollider2D _actorCollider;
    private CapsuleCollider2D _blockCollider;
    private Action<Coroutine> _endRoutine;
    private Coroutine _currentDash;

    public void SetUp(BoxCollider2D actorCollider, CapsuleCollider2D blockCollider, Action<Coroutine> endCoroutineFunc)
    {
        _actorCollider = actorCollider;
        _blockCollider = blockCollider;
        _endRoutine = endCoroutineFunc;
    }

    public void Dash(float movementSpeed, Vector3 dashDistance)
    {
        _animatorController.Blend = 1;
        _currentDash = _startRoutine(DashHandle(movementSpeed, _actorCollider, _blockCollider, dashDistance));
    }
    
    protected IEnumerator DashHandle(float movementSpeed, BoxCollider2D actorCollider, CapsuleCollider2D blockCollider, Vector3 dashFinalPoint)
    {
        float startTime = Time.time;

        _actorCollider.excludeLayers = 128;
        _blockCollider.excludeLayers = 128;

        _stateMachine.ChangeState(MachineActorStates.Dashing);
        _stateMachine.ChangeBattleState(ActorBattleState.Dashing);
        _dashDirection = (int)_stateMachine.Direction;
        _rb.linearVelocityX = _dashDirection * DashSpeed;


        yield return new WaitUntil(() => IsDistanceOver(dashFinalPoint));

        _rb.linearVelocityX = _dashDirection * (DashSpeed * 0.7f);
        _stateMachine.ChangeBattleState(ActorBattleState.CanContrattack);

        yield return new WaitForSeconds(0.15f);
        if (_stateMachine.PreviousState != MachineActorStates.Moving) ExitIntoStanding();
        else ExitIntoMoving(movementSpeed);
    }

    public void FinishDash()
    {
        _stateMachine.ChangeBattleState(ActorBattleState.Waiting);
        _actorCollider.excludeLayers = 0;
        _blockCollider.excludeLayers = 0;
    }

    private bool IsDistanceOver(Vector3 finalPoint)
    {
        if (_stateMachine.Direction == MachineDirection.Left && _actorCollider.transform.position.x <= finalPoint.x) return true;
        if (_stateMachine.Direction == MachineDirection.Right && _actorCollider.transform.position.x >= finalPoint.x) return true;
        return false;
    }

    public void Interrupt()
    {
        _endRoutine(_currentDash);
    }
    public void ExitIntoStanding()
    {
        _rb.linearVelocityX = 0;
        _stateMachine.ChangeState(MachineActorStates.Standing);
        _animatorController.Blend = 0;
        FinishDash();
    }
    public void ExitIntoMoving(float movementSpeed)
    {
        _rb.linearVelocityX = _dashDirection * movementSpeed;
        _stateMachine.ChangeState(MachineActorStates.Moving);
        FinishDash();
    }
}
