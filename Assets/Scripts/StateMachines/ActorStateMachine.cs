using UnityEngine;

public enum MachineActorStates { Standing, Moving, PreparingJump, Jumping, Falling, Dashing }
public enum MachineDirection { Left = -1, Right = 1, Stop = 0}

public enum ActorBattleState { Waiting, Attacking, Blocking, CanContrattack, Dashing, Interrupted }
public enum AttackStates { Waiting, Charging, Accelerating, DealingDamage, Recovering, Interrupted, Parred, Blocked }
public enum BlockStates { Waiting, Preparing, Parrying, Blocking, Recovering, Interrupted }

public class ActorStateMachine
{
    public MachineActorStates CurrentState { get; private set; } = MachineActorStates.Standing;
    public MachineActorStates PreviousState { get; private set; } = MachineActorStates.Standing;
    public MachineDirection Direction { get; private set; } = MachineDirection.Left;

    public ActorBattleState CurrentBattleState { get; private set; } = ActorBattleState.Waiting;
    public AttackStates CurrentAttackState { get; private set; } = AttackStates.Waiting;
    public BlockStates CurrentBlockState { get; private set; } = BlockStates.Waiting;

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

    public void ChangeAttackState(AttackStates newState)
    {
        CurrentAttackState = newState;
        if (newState == AttackStates.Interrupted || newState == AttackStates.Parred) CurrentBattleState = ActorBattleState.Interrupted;
        else if (newState == AttackStates.Waiting) CurrentBattleState = ActorBattleState.Waiting;
        else CurrentBattleState = ActorBattleState.Attacking;
    }
    public void ChangeBlockState(BlockStates newState)
    {
        CurrentBlockState = newState;
        if (newState == BlockStates.Interrupted) CurrentBattleState = ActorBattleState.Interrupted;
        else if (newState == BlockStates.Waiting) CurrentBattleState = ActorBattleState.Waiting;
        else CurrentBattleState = ActorBattleState.Blocking;
    }

    public void ChangeBattleState(ActorBattleState newState)
    {
        CurrentBattleState = newState;
    }
}