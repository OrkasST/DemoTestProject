using System;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private int _damage;
    private Action _onParry;
    private Action _onBlock;

    private bool _isblocked = false;

    public void SetUp(int damage, Action onParry, Action onBlock)
    {
        _damage = damage;
        _onParry = onParry;
        _onBlock = onBlock;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit");
        if (collision.tag == "Block")
        {
            GameObject me = this.gameObject.GetComponentInParent<ActorController>().gameObject;
            Debug.Log("Block");
            _isblocked = true;
            ActorController actorController = collision.gameObject.GetComponentInParent<ActorController>();

            switch (actorController.GetCurrentBlockState())
            {
                case BlockStates.Parrying: _onParry(); 
                    actorController.CombatController.OnEnemyAttackParred(me.GetComponent<BoxCollider2D>().size.x, me.transform.position); break;
                case BlockStates.Blocking: _onBlock(); actorController.CombatController.GetDamage(_damage); break;
            }
        }
        else if ((collision.tag == "Entity" || collision.tag == "Player") && !_isblocked)
        {
            Debug.Log("Entity");
            var collisionBattleState = collision.gameObject.GetComponent<ActorController>().GetCurrentBattleState();

            collision.gameObject.GetComponent<ActorController>().CombatController.GetDamage(_damage);
        }
    }

    private void OnEnable()
    {
        _isblocked = false;
    }
    private void OnDisable() { _isblocked = false; }

}
