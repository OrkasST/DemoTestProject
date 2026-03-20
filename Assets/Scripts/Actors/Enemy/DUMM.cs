using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActorController))]

public class DUMM : MonoBehaviour
{
    private ActorController _controller;
    private Coroutine currentProgramm = null;
    private bool isProgrammEnded;

    void Start()
    {
        _controller = GetComponent<ActorController>();
    }
    void Update()
    {
        if (currentProgramm == null) currentProgramm = StartCoroutine(MovementProgramm());
        if (isProgrammEnded)
        {
            StopCoroutine(currentProgramm);
            currentProgramm = null;
        }

        _controller.UpdateActorState();
    }

    private IEnumerator MovementProgramm()
    {
        isProgrammEnded = false;
        _controller.ChangeDirection(MachineDirection.Right);
        yield return new WaitUntil(() => transform.position.x >= 240);
        _controller.ChangeDirection(MachineDirection.Stop);
        yield return new WaitForSeconds(0.3f);
        _controller.CombatController.LightAttack();
        yield return new WaitUntil(() => _controller.GetCurrentBattleState() == ActorBattleState.Waiting);
        yield return new WaitForSeconds(0.3f);

        _controller.ChangeDirection(MachineDirection.Left);
        yield return new WaitUntil(() => transform.position.x <= 145);
        _controller.ChangeDirection(MachineDirection.Stop);
        yield return new WaitForSeconds(0.3f);
        _controller.CombatController.LightAttack();
        yield return new WaitUntil(() => _controller.GetCurrentBattleState() == ActorBattleState.Waiting);
        yield return new WaitForSeconds(0.3f);
        isProgrammEnded = true;
    }
}
