using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private int _damage;

    public void SetDamage(int damage) { _damage = damage; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Coll");
        Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.tag == "Entity")
        {
            var collisionBattleState = collision.transform.GetComponent<ActorController>().GetCurrentBattleState();
            Debug.Log("Coll HIT!");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Coll Stay");
        Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.tag == "Entity")
        {
            var collisionBattleState = collision.transform.GetComponent<ActorController>().GetCurrentBattleState();
            Debug.Log("Coll Stay HIT!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Entity")
        {
            var collisionBattleState = collision.gameObject.GetComponent<ActorController>().GetCurrentBattleState();

            collision.gameObject.GetComponent<ActorController>().CombatController.GetDamage(_damage);
            Debug.Log("Hit");
        }
    }
}
