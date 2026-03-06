using Assets.Scripts.Animator;
using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractAction
{
    protected ActorStateMachine _stateMachine;
    protected Rigidbody2D _rb;
    protected Func<IEnumerator, Coroutine> _startRoutine;
    protected AnimatorController _animatorController;

    public void Initialize(ActorStateMachine stateMachine, Rigidbody2D rb, Func<IEnumerator, Coroutine> coroutineStarterFunc, AnimatorController animatorController)
    {
        _stateMachine = stateMachine;
        _rb = rb;
        _startRoutine = coroutineStarterFunc;
        _animatorController = animatorController;
    }

    public virtual void OnActorCollisionEnter(Collision2D collision) { }
    public virtual void OnActorCollisionLeave(Collision2D collision) { }
    public virtual void OnActorCollisionStay(Collision2D collision) { }

}
