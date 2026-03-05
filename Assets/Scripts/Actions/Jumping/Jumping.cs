using System;
using System.Collections;
using UnityEngine;

public class Jumping : AbstractAction
{
    public Jumping() : base() { }

    private float _airMovementSpeed = 7f;
    private float _jumpForce = 320f;
    private const float ReduceJumpPercent = 0.4f;

    public bool IsInAir { get; private set; }

    public void MoveInAir(int direction, float movementSpeed)
    {
        float speedChange = 0;

        if ((direction < 0 && _rb.linearVelocity.x > -movementSpeed*0.7f) || (direction > 0 && _rb.linearVelocity.x < movementSpeed*0.7f))
        {
            speedChange = _airMovementSpeed;
        }

        _rb.linearVelocity = new Vector2(x: _rb.linearVelocity.x + (direction * speedChange), y: _rb.linearVelocity.y);
    }
    public void Jump()
    {
        _animatorController.IsJumping = true;
        _startRoutine(JumpHandling());
    }

    private IEnumerator JumpHandling()
    {
        yield return new WaitForFixedUpdate();

        _stateMachine.ChangeState(MachineActorStates.PreparingJump);
        yield return new WaitForSeconds(0.02f);

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
        _stateMachine.ChangeState(MachineActorStates.Jumping);

        yield return new WaitUntil(() => _rb.linearVelocity.y <= 0);
        _stateMachine.ChangeState(MachineActorStates.Falling);
    }

    public void StopJump()
    {
        if (IsInAir && _stateMachine.CurrentState == MachineActorStates.Jumping) _rb.linearVelocityY *= ReduceJumpPercent;
    }

    public void Land()
    {
        _animatorController.IsJumping = false;
        _stateMachine.ChangeState(_rb.linearVelocityX > 0.1f || _rb.linearVelocityX < -0.1f ? MachineActorStates.Moving : MachineActorStates.Standing);
    }

    public bool IsFalling() => _rb.linearVelocity.y < -0.1f;

    public override void OnActorCollisionEnter(Collision2D collision)
    {
        if (_stateMachine.CurrentState == MachineActorStates.Falling && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            IsInAir = false;
            Land();
        }
    }
    public override void OnActorCollisionLeave(Collision2D collision)
    {
        if (!IsInAir && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            IsInAir = true;
        }
    }
    public override void OnActorCollisionStay(Collision2D collision)
    {
        if (IsInAir && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            IsInAir = false;
        }
    }
}
