using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public List<ActorController> _hitingEntities = new();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Entity"))
        {
            //if (collision.transform.GetComponent<ActorController>().GetCurrentBattleState() == )
        }
    }
}
