using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ActorStates { Standing, Moving, PreparingJump, Jumping, Falling, Dashing }

public enum Actions { Jump, StopJump, Dash }
public enum Direction { Left = -1, Right = 1 }

public class PlayerMovement : MonoBehaviour
{
    #region Fields Declaration
    private Rigidbody2D _rb;
    private Animator _animator;

    public InputActionReference move;
    public InputActionReference jump;
    public InputActionReference dash;

    private float _coyotTime = 0.1f;
    private float _coyotTimeCountDown = 0.1f;

    private float _excuseTime = 0.25f;

    //[SerializeField]
    private float _movementSpeed = 180;
    //[SerializeField]
    private float _movementSpeedAir = 7f;

    [SerializeField]
    private float _jumpForce = 320f;
    private const float ReduceJumpPercent = 0.4f;
    private const float DashSpeed = 400f;
    private const float _activationGate = 0.5f;

    public ActorStates CurrentState { get; private set; } = ActorStates.Standing;
    public ActorStates PreviousState { get; private set; } = ActorStates.Standing;
    private bool _isInAir = false;

    public Direction Direction { get; private set; } = Direction.Left;

    private Dictionary<Actions, float> _actionBuffer = new Dictionary<Actions, float>() { };
    private Actions[] _bufferedActionsKeys = { };
    private bool _isHoldingJump;

    #endregion

    public void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        var values = Enum.GetValues(typeof(Actions)).Cast<Actions>();
        foreach (var value in values ) _actionBuffer.Add(value, -1);
        _bufferedActionsKeys = _actionBuffer.Keys.ToArray();
    }

    public void Update()
    {
        float direction = move.action.ReadValue<float>() > _activationGate ? 1 : move.action.ReadValue<float>() < -_activationGate ? -1 : 0;
        Debug.Log(direction);
        var newDirection = (Direction)direction;

        if (!_isInAir && CurrentState != ActorStates.Dashing)
        {
            _rb.linearVelocity = new Vector2(x: direction * _movementSpeed, y: _rb.linearVelocity.y);
            ChangeDirection(newDirection);
        }
        else if(_isInAir)
        {
            float speedChange = 0;
            
            ChangeDirection(newDirection);

            if ((direction < 0 && _rb.linearVelocity.x > -_movementSpeed) || (direction > 0 && _rb.linearVelocity.x < _movementSpeed))
            {
                speedChange = _movementSpeedAir;
            }

            _rb.linearVelocity = new Vector2(x: _rb.linearVelocity.x + (direction * speedChange), y: _rb.linearVelocity.y);
        }

        if (_rb.linearVelocityX < 0.1f && _rb.linearVelocityY > -0.1f && CurrentState == ActorStates.Moving)
        {
            ChangeState(ActorStates.Standing);
            _animator.SetBool("isRunning", false);
        }
        if (CurrentState == ActorStates.Standing && (_rb.linearVelocityX > 0.1f || _rb.linearVelocityX < -0.1f))
        {
            ChangeState(ActorStates.Moving);
            _animator.SetBool("isRunning", true);
        }

        if (!_isInAir && _coyotTimeCountDown <= 0)
        {
            _coyotTimeCountDown = _coyotTime;
        }
        if (_isInAir && CurrentState == ActorStates.Moving && _coyotTimeCountDown > 0)
        {
            _coyotTimeCountDown -= Time.deltaTime;
        }

        foreach (var key in _bufferedActionsKeys)
        {
            if (Time.time - _actionBuffer[key] >= _excuseTime) _actionBuffer[key] = -1;
        }
    }

    private void ChangeDirection(Direction newDirection)
    {
        if (newDirection == 0) return;
        if (Direction != newDirection)
        {
            Direction = newDirection;
            Debug.Log(newDirection);
            if (newDirection == Direction.Left) transform.localScale = Vector3.one;
            else transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void FixedUpdate()
    {
        if (_rb.linearVelocity.y < -0.1f && CurrentState != ActorStates.Falling)
        {
            ChangeState(ActorStates.Falling);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CurrentState == ActorStates.Falling && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isInAir = false;
            Land();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!_isInAir && CurrentState == ActorStates.Jumping && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isInAir = true;
        }
    }


    private void Jump(InputAction.CallbackContext context) => Jump();
    private void Jump()
    {
        if ((CurrentState != ActorStates.Standing && CurrentState != ActorStates.Moving) || (_isInAir && _coyotTime <= 0))
        {
            _actionBuffer[Actions.Jump] = Time.time;
            return;
        }
        _actionBuffer[Actions.Jump] = -1;
        _isHoldingJump = true;
        StartCoroutine(JumpHandling());
    }

    private IEnumerator JumpHandling()
    {
        yield return new WaitForFixedUpdate();

        ChangeState(ActorStates.PreparingJump);
        yield return new WaitForSeconds(0.02f);

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
        ChangeState(ActorStates.Jumping);
        if (!_isHoldingJump || (_actionBuffer[Actions.StopJump] > 0 && Time.time - _actionBuffer[Actions.StopJump] <= _excuseTime))
            StopJump();

        yield return new WaitUntil(() => _rb.linearVelocity.y <= 0);
        ChangeState(ActorStates.Falling);
    }

    private void Land()
    {
        _coyotTimeCountDown = _coyotTime;

        ChangeState(_rb.linearVelocityX > 0.1f || _rb.linearVelocityX < -0.1f ? ActorStates.Moving : ActorStates.Standing);

        if (_actionBuffer[Actions.Jump] > 0f && Time.time - _actionBuffer[Actions.Jump] < _excuseTime) Jump();
        else if (_actionBuffer[Actions.Dash] > 0f && Time.time - _actionBuffer[Actions.Dash] < _excuseTime) Dash();
    }

    private void ChangeState(ActorStates newState)
    {
        PreviousState = CurrentState;
        CurrentState = newState;
    }


    private void StopJump(InputAction.CallbackContext context) => StopJump();
    private void StopJump()
    {
        if (CurrentState == ActorStates.PreparingJump)
        {
            _isHoldingJump = false;
            return;
        }
        if (CurrentState != ActorStates.Jumping)
        {
            _actionBuffer[Actions.StopJump] = Time.time;
            return;
        }

        _actionBuffer[Actions.StopJump] = -1;
        _rb.linearVelocityY *= ReduceJumpPercent;
    }

    private void Dash(InputAction.CallbackContext context) => Dash();
    private void Dash()
    {
        if (_isInAir)
        {
            _actionBuffer[Actions.Dash] = Time.time;
            return;
        }
        _actionBuffer[Actions.Dash] = -1;
        StartCoroutine(DashHandle());
    }
    private IEnumerator DashHandle()
    {
        ChangeState(ActorStates.Dashing);

        _rb.linearVelocityX = (int)Direction * DashSpeed;

        yield return new WaitForSeconds(0.2f);

        _rb.linearVelocityX = (int)Direction * (DashSpeed * 0.7f);

        yield return new WaitForSeconds(0.15f);

        if (PreviousState != ActorStates.Moving)
        {
            _rb.linearVelocityX = 0;
            ChangeState(ActorStates.Standing);
        }
        else
        {
            _rb.linearVelocityX = (int)Direction * _movementSpeed;
            ChangeState(ActorStates.Moving);
        }
    }


    public void OnEnable()
    {
        jump.action.started += Jump;
        jump.action.canceled += StopJump;

        dash.action.started += Dash;
    }

    public void OnDisable()
    {
        jump.action.started -= Jump;
        jump.action.canceled -= StopJump;
    }
}
