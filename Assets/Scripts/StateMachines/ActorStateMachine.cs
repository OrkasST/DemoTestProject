public enum MachineActorStates { Standing, Moving, PreparingJump, Jumping, Falling, Dashing }
public enum MachineActions { Jump, StopJump, Dash }
public enum MachineDirection { Left = -1, Right = 1 }

public class ActorStateMachine
{
    public MachineActorStates CurrentState { get; private set; } = MachineActorStates.Standing;
    public MachineActorStates PreviousState { get; private set; } = MachineActorStates.Standing;
    public MachineDirection Direction { get; private set; } = MachineDirection.Left;

    public void ChangeState(MachineActorStates newState)
    {
        PreviousState = CurrentState;
        CurrentState = newState;
    }

    public void ChangeDirection(MachineDirection newDirection)
    {
        if (newDirection == 0) return;
        if (Direction != newDirection)
        {
            Direction = newDirection;
            //if (newDirection == MachineDirection.Left) transform.localScale = Vector3.one;
            //else transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}