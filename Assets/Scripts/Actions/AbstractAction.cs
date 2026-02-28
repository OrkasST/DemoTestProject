using UnityEngine;

public abstract class AbstractAction
{
    public bool IsCollisionAffected = false;
    public bool IsInputAffected = true;

    protected ActorStateMachine _stateMachine;
    protected Rigidbody2D _rb;

    public AbstractAction(bool isCollisionAffected, bool isInputAffected)
    {
        this.IsCollisionAffected = isCollisionAffected;
        this.IsInputAffected = isInputAffected;
    }

    public void Initialize(ActorStateMachine stateMachine, Rigidbody2D rb)
    {
        _stateMachine = stateMachine;
        _rb = rb;
    }

    public virtual void OnActorCollisionEnter(Collision2D collision) { }
    public virtual void OnActorCollisionLeave(Collision2D collision) { }

}
