using System.Collections;
using UnityEngine;

public class Jumping : AbstractAction
{
    public Jumping() : base(true, true) { }

    private float _airMovementSpeed = 7f;
    private float _jumpForce = 320f;
    private const float ReduceJumpPercent = 0.4f;

    private bool _isInAir = false;
    private bool _isHoldingJump = false;

    public void MoveInAir(int direction, float movementSpeed)
    {
        float speedChange = 0;

        if ((direction < 0 && _rb.linearVelocity.x > -movementSpeed) || (direction > 0 && _rb.linearVelocity.x < movementSpeed))
        {
            speedChange = _airMovementSpeed;
        }

        _rb.linearVelocity = new Vector2(x: _rb.linearVelocity.x + (direction * speedChange), y: _rb.linearVelocity.y);
    }

    public void Jump()
    {
        _isHoldingJump = true;
        Coroutines.StartRoutine(JumpHandling());
    }

    private IEnumerator JumpHandling()
    {
        yield return new WaitForFixedUpdate();

        _stateMachine.ChangeState(MachineActorStates.PreparingJump);
        yield return new WaitForSeconds(0.02f);

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
        _stateMachine.ChangeState(MachineActorStates.Jumping);
        if (!_isHoldingJump) StopJump();// || (_actionBuffer[Actions.StopJump] > 0 && Time.time - _actionBuffer[Actions.StopJump] <= _excuseTime))

        yield return new WaitUntil(() => _rb.linearVelocity.y <= 0);
        _stateMachine.ChangeState(MachineActorStates.Falling);
    }
    private void StopJump()
    {
        if (_stateMachine.CurrentState == MachineActorStates.PreparingJump)
        {
            _isHoldingJump = false;
            return;
        }
        _rb.linearVelocityY *= ReduceJumpPercent;
    }


    private void Land()
    {
        _stateMachine.ChangeState(_rb.linearVelocityX > 0.1f || _rb.linearVelocityX < -0.1f ? MachineActorStates.Moving : MachineActorStates.Standing);

        //if (_actionBuffer[Actions.Jump] > 0f && Time.time - _actionBuffer[Actions.Jump] < _excuseTime) Jump();
        //else if (_actionBuffer[Actions.Dash] > 0f && Time.time - _actionBuffer[Actions.Dash] < _excuseTime) Dash();
    }

    public override void OnActorCollisionEnter(Collision2D collision)
    {
        if (_stateMachine.CurrentState == MachineActorStates.Falling && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isInAir = false;
            Land();
        }
    }
    public override void OnActorCollisionLeave(Collision2D collision)
    {
        if (!_isInAir && _stateMachine.CurrentState == MachineActorStates.Jumping && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isInAir = true;
        }
    }
}
