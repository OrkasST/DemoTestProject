using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ActorStates { Standing, Moving, PreparingJump, Jumping, Falling, Landing }

public enum Actions { Jump }

public class PlayerMovement : MonoBehaviour
{
    #region Fields Declaration
    public Rigidbody2D rb;
    public InputActionReference move;
    public InputActionReference jump;

    private float _coyotTime = 0.1f;
    private float _coyotTimeCountDown = 0.1f;

    private float _excuseTime = 0.2f;

    //[SerializeField]
    private float _movementSpeed = 120;
    //[SerializeField]
    private float _movementSpeedAir = 0.25f;

    //[SerializeField]
    private float _jumpForce = 300f;

    public ActorStates CurrentState { get; private set; } = ActorStates.Standing;
    public ActorStates PreviousState { get; private set; } = ActorStates.Standing;
    private bool _isInAir = false;

    private Dictionary<Actions, float> _actionBuffer = new Dictionary<Actions, float>()
    {
        [ Actions.Jump ] = -1
    };

    #endregion

    public void Update()
    {
        if (!_isInAir)
        {
            rb.linearVelocity = new Vector2(x: move.action.ReadValue<float>() * _movementSpeed, y: rb.linearVelocity.y);
        }
        else
        {
            float speedChange = 0;
            float direction = move.action.ReadValue<float>();

            if ( (direction < 0 && rb.linearVelocity.x > -_movementSpeed) || (direction > 0 && rb.linearVelocity.x < _movementSpeed) )
            {
                speedChange = _movementSpeedAir;
            }

            rb.linearVelocity = new Vector2(x: rb.linearVelocity.x + (direction * speedChange), y: rb.linearVelocity.y);
        }


        if (!_isInAir && _coyotTimeCountDown <= 0)
        {
            _coyotTimeCountDown = _coyotTime;
        }
        if (_isInAir && CurrentState == ActorStates.Moving && _coyotTimeCountDown > 0)
        {
            _coyotTimeCountDown -= Time.deltaTime;
        }
        if (Time.time - _actionBuffer[Actions.Jump] >= _excuseTime) _actionBuffer[Actions.Jump] = -1;
    }

    public void FixedUpdate()
    {
        if (rb.linearVelocity.y < -0.1f && CurrentState != ActorStates.Falling)
        {
            ChangeState(ActorStates.Falling);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CurrentState == ActorStates.Falling && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log(collision.gameObject.name);
            _isInAir = false;
            StartCoroutine(Land());
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!_isInAir && CurrentState == ActorStates.Jumping && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isInAir = true;
        }
    }


    private void Jump(InputAction.CallbackContext context)
    {
        if ((CurrentState != ActorStates.Standing && CurrentState != ActorStates.Moving) || (_isInAir && _coyotTime <= 0))
        {
            _actionBuffer[Actions.Jump] = Time.time;
            return;
        }
        StartCoroutine(JumpHandling());
    }

    private IEnumerator JumpHandling()
    {
        yield return new WaitForFixedUpdate();

        ChangeState(ActorStates.PreparingJump);
        yield return new WaitForSeconds(0.1f);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, _jumpForce);
        ChangeState(ActorStates.Jumping);

        yield return new WaitUntil(() => rb.linearVelocity.y <= 0);
        ChangeState(ActorStates.Falling);
    }

    private IEnumerator Land()
    {
        ChangeState(ActorStates.Landing);

        yield return new WaitForSeconds(0.1f);
        _coyotTimeCountDown = _coyotTime;
        ChangeState(rb.linearVelocity.x != 0 ? ActorStates.Moving : ActorStates.Standing);
        if (_actionBuffer[Actions.Jump] > 0f && Time.time - _actionBuffer[Actions.Jump] < _excuseTime) StartCoroutine(JumpHandling());
    }

    private void ChangeState(ActorStates newState)
    {
        PreviousState = CurrentState;
        CurrentState = newState;
    }

    public void OnEnable()
    {
        jump.action.started += Jump;
    }
    public void OnDisable()
    {
        jump.action.started -= Jump;
    }
}
