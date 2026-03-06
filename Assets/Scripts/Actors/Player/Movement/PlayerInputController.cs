using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ActorController))]
public class PlayerInputController : MonoBehaviour
{
    public InputActionReference move;
    public InputActionReference jump;
    public InputActionReference dash;

    private const float _coyotTime = 0.15f;
    private float _coyotTimeCountDown = 0.15f;
    private const float _activationGate = 0.5f;
    private const float _excuseTime = 0.25f;

    private ActorController _actorController;

    private Dictionary<ActorActions, float> _actionBuffer = new Dictionary<ActorActions, float>() { };
    private ActorActions[] _bufferedActionsKeys = { };

    void Start()
    {
        _actorController = GetComponent<ActorController>();

        var values = Enum.GetValues(typeof(ActorActions)).Cast<ActorActions>();
        foreach (var value in values) _actionBuffer.Add(value, -1);
        _bufferedActionsKeys = _actionBuffer.Keys.ToArray();
    }

    void Update()
    {
        _actorController.ChangeDirection(
            move.action.ReadValue<float>() > _activationGate ? 1
            : move.action.ReadValue<float>() < -_activationGate ? -1
            : 0);
       


        _actorController.UpdateActorState();

        if (!_actorController.IsInAir && _coyotTimeCountDown <= 0)
        {
            _coyotTimeCountDown = _coyotTime;
        }
        if (_actorController.IsInAir && _coyotTimeCountDown > 0)
        {
            _coyotTimeCountDown -= Time.deltaTime;
        }

        foreach (var key in _bufferedActionsKeys)
        {
            if (key == ActorActions.StopJump && _actorController.CanStopJump() && Time.time - _actionBuffer[key] < _excuseTime)
            {
                _actorController.StopJump();
                _actionBuffer[key] = -1;
            }
            else if (key == ActorActions.Jump && _actorController.CanJump() && Time.time - _actionBuffer[key] < _excuseTime)
            {
                _actorController.Jump();
                _actionBuffer[key] = -1;
            }
            else if (key == ActorActions.Dash && _actorController.CanDash() && Time.time - _actionBuffer[key] < _excuseTime)
            {
                _actorController.Dash();
                _actionBuffer[key] = -1;
            }

            if (Time.time - _actionBuffer[key] >= _excuseTime) _actionBuffer[key] = -1;
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

        dash.action.started -= Dash;
    }

    private void Dash(InputAction.CallbackContext context) => Dash();

    private void Dash()
    {
        if (!_actorController.CanDash()) _actionBuffer[ActorActions.Dash] = Time.time;
        else {
            _actorController.Dash();
            _actionBuffer[ActorActions.Dash] = -1;
        }
    }
 

    private void Jump(InputAction.CallbackContext context) => Jump();

    private void Jump()
    {
        _actorController.Jump();

        if (!_actorController.CanJump() && (_actorController.IsInAir && _coyotTimeCountDown <= 0))
        {
            _actionBuffer[ActorActions.Jump] = Time.time;
        }
        else
        {
            if (_actorController.IsInAir && _coyotTimeCountDown >= 0) _actorController.Jump(true);

            _actionBuffer[ActorActions.Jump] = -1;
        }
    }

    private void StopJump(InputAction.CallbackContext context) => StopJump();

    private void StopJump()
    {
        if (!_actorController.CanStopJump())
        {
            _actionBuffer[ActorActions.StopJump] = Time.time;
        }
        else
        {
            _actionBuffer[ActorActions.StopJump] = -1;
            _actorController.StopJump();
        }
    }
}
