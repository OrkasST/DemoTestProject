using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActorController))]

public class DUMM : MonoBehaviour
{
    private ActorController _controller;
    private Coroutine currentProgramm = null;
    private bool isProgrammEnded;

    private float startTime = 0;

    private Vector3 _initialPosition;

    void Start()
    {
        _initialPosition = new Vector3(transform.position.x, transform.position.y);
        _controller = GetComponent<ActorController>();
        _controller.OnDeath = () =>
        {
            _controller.ChangeDirection(MachineDirection.Left);
            if (currentProgramm != null)
            {
                StopCoroutine(currentProgramm);
                currentProgramm = null;
            }
        };
    }
    void Update()
    {
        if (currentProgramm == null && !_controller.IsDead) currentProgramm = StartCoroutine(MovementProgramm());
        if (isProgrammEnded || _controller.IsDead)
        {
            StopCoroutine(currentProgramm);
            currentProgramm = null;
        }

        _controller.UpdateActorState();
    }

    private IEnumerator MovementProgramm()
    {
        startTime = Time.time;
        isProgrammEnded = false;
        _controller.ChangeDirection(MachineDirection.Right);

        yield return new WaitUntil(() => (transform.position.x >= _initialPosition.x + 140) || (Time.time - startTime >= 3));
        startTime = Time.time;
        _controller.ChangeDirection(MachineDirection.Stop);
        yield return new WaitForSeconds(0.3f);
        _controller.CombatController.LightAttack();
        yield return new WaitUntil(() => _controller.GetCurrentBattleState() == ActorBattleState.Waiting);
        yield return new WaitForSeconds(0.3f);

        _controller.ChangeDirection(MachineDirection.Left);
        yield return new WaitUntil(() => (transform.position.x <= _initialPosition.x) || (Time.time - startTime >= 3));
        _controller.ChangeDirection(MachineDirection.Stop);
        yield return new WaitForSeconds(0.3f);
        _controller.CombatController.LightAttack();
        yield return new WaitUntil(() => _controller.GetCurrentBattleState() == ActorBattleState.Waiting);
        yield return new WaitForSeconds(0.3f);
        isProgrammEnded = true;
    }
}
