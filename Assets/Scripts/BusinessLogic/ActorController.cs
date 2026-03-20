using Assets.Scripts.Actions.Dashing;
using Assets.Scripts.Actions.Running;
using Assets.Scripts.Animator;
using Assets.Scripts.BusinessLogic;
using Assets.Scripts.Weapon.Sword;
using UnityEngine;

public enum ActorActions { Jump, StopJump, Dash, Counterattack }

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class ActorController : MonoBehaviour
{
    private Jumping _jumping = new Jumping();
    private Running _running = new Running();
    private Dashing _dashing = new Dashing();

    private ActorStateMachine _stateMachine = new ActorStateMachine();
    private AnimatorController _animatorController = new AnimatorController();

    private string _cs = "";
    private string _bs = "";
    private string _bbs = "";

    public CombatController CombatController { get; private set; } = new CombatController();

    private BoxCollider2D _actorCollider;

    public bool IsInAir { get => _jumping.IsInAir; }
    public string ActorName;

    public MachineActorStates GetCurrentState() => _stateMachine.CurrentState;
    public MachineActorStates GetPreviousState() => _stateMachine.PreviousState;

    public ActorBattleState GetCurrentBattleState() => _stateMachine.CurrentBattleState;
    public BlockStates GetCurrentBlockState() => _stateMachine.CurrentBlockState;
    public BoxCollider2D GetActorCollider() => _actorCollider;

    public GameObject LightAttackHitbox;
    public GameObject SpecialAttackHitbox;
    public GameObject BlockHitbox;

    public float AttackSpeed = 170f;

    private void Start()
    {
        _animatorController.Initialize(GetComponent<Animator>());
        _jumping.Initialize(_stateMachine, GetComponent<Rigidbody2D>(), StartCoroutine, _animatorController);
        _running.Initialize(_stateMachine, GetComponent<Rigidbody2D>(), StartCoroutine, _animatorController);
        _dashing.Initialize(_stateMachine, GetComponent<Rigidbody2D>(), StartCoroutine, _animatorController);

        _actorCollider = GetComponent<BoxCollider2D>();

        this.CombatController.Initialize(_stateMachine, GetComponent<Rigidbody2D>(), StartCoroutine, StopCoroutine, _animatorController,
            LightAttackHitbox, SpecialAttackHitbox, BlockHitbox, _actorCollider, () => Destroy(gameObject), ChangeDirection);

        this.CombatController.EquipWeapon(new Sword());
    }

    public void Jump(bool? unsafeCanJump = false)
    {
        if (!CanJump() && !unsafeCanJump.Value) return;
        _jumping.Jump();
    }
    public void StopJump() => _jumping.StopJump();
    public bool CanJump()
    {
        return (_stateMachine.CurrentState == MachineActorStates.Standing || _stateMachine.CurrentState == MachineActorStates.Moving)
            && _stateMachine.CurrentBattleState != ActorBattleState.Interrupted;
    }
    public bool CanStopJump() => _stateMachine.CurrentState == MachineActorStates.Jumping;

    public bool CanDash()
    {
        return _stateMachine.CurrentState == MachineActorStates.Standing || _stateMachine.CurrentState == MachineActorStates.Moving
            && _stateMachine.CurrentBattleState != ActorBattleState.Interrupted &&
            (_stateMachine.CurrentAttackState == AttackStates.Waiting || _stateMachine.CurrentAttackState == AttackStates.Charging);
    }
    public void Dash()
    {
        if (!CanDash()) return;
        //if (_stateMachine.CurrentAttackState == AttackStates.Charging) this.CombatController.
        _dashing.Dash(_running.MovementSpeed);
    }

    public void UpdateActorState()
    {
        if (!IsInAir && _stateMachine.CurrentState != MachineActorStates.Dashing) _running.Move(_stateMachine.DirectionValue);
        else if (IsInAir) _jumping.MoveInAir(_stateMachine.DirectionValue, _running.MovementSpeed);
    }

    public void ChangeDirection(MachineDirection direction, bool? isAbsolute = false)
    {
        if (_stateMachine.CurrentBattleState == ActorBattleState.Interrupted) return;
        if (_stateMachine.CurrentState == MachineActorStates.Dashing && !isAbsolute.Value) return;
        if (_stateMachine.ChangeDirection(direction))
        {
            if (direction == MachineDirection.Right) transform.localScale = Vector3.one;
            else transform.localScale = new Vector3(-1, 1, 1);
        }
    }//For AI based
    public void ChangeDirection(int direction)
    {
        if (_stateMachine.CurrentBattleState == ActorBattleState.Interrupted) return;
        if (_stateMachine.CurrentState == MachineActorStates.Dashing) return;
        if (_stateMachine.CurrentBlockState == BlockStates.Parrying && (MachineDirection)direction == _stateMachine.Direction)
        {
            CombatController.Dash(_running.MovementSpeed);
            return;
        }
        if (_stateMachine.ChangeDirection(direction))
        {
            if ((MachineDirection)direction == MachineDirection.Right) transform.localScale = Vector3.one;
            else transform.localScale = new Vector3(-1, 1, 1);
        }
    }//For input based


    public void FixedUpdate()
    {
        if (_jumping.IsFalling() && _stateMachine.CurrentState != MachineActorStates.Falling)
        {
            _stateMachine.ChangeState(MachineActorStates.Falling);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _jumping.OnActorCollisionEnter(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        _jumping.OnActorCollisionLeave(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        _jumping.OnActorCollisionStay(collision);
    }


    private void Update()
    {
        _cs = _stateMachine.CurrentState.ToString();
        _bs = _stateMachine.CurrentBattleState.ToString();
        _bbs = _stateMachine.CurrentBlockState.ToString();

        this.CombatController.Update();
    }
}