public enum MachineActorStates { Standing, Moving, PreparingJump, Jumping, Falling, Dashing }
public enum MachineDirection { Left = -1, Right = 1, Stop = 0}

public class ActorStateMachine
{
    public MachineActorStates CurrentState { get; private set; } = MachineActorStates.Standing;
    public MachineActorStates PreviousState { get; private set; } = MachineActorStates.Standing;
    public MachineDirection Direction { get; private set; } = MachineDirection.Left;

    public int DirectionValue = 0;

    public void ChangeState(MachineActorStates newState)
    {
        PreviousState = CurrentState;
        CurrentState = newState;
    }

    public bool ChangeDirection(int newDirection)
    {
        if (newDirection == DirectionValue) return false;

        DirectionValue = newDirection;

        if (newDirection != 0)
        {
            Direction = (MachineDirection)newDirection;
            return true;
        }
        return false;
    }
    public bool ChangeDirection(MachineDirection newDirection)
    {
        if (newDirection == Direction) return false;

        DirectionValue = (int)newDirection;

        if (newDirection != MachineDirection.Stop)
        {
            Direction = newDirection;
            return true;
        }
        return false;
    }
}