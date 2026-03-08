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
        if (collision.tag == "Block")
        {
            _isblocked = true;
            Debug.Log(collision.gameObject.GetComponent<BlockHitbox>().GetActorController().ActorName);
            switch (collision.gameObject.GetComponent<BlockHitbox>().GetBlockState())
            {
                case BlockStates.Parrying: _onParry(); break;
                case BlockStates.Blocking: _onBlock(); collision.gameObject.GetComponent<BlockHitbox>().GetActorController().CombatController.GetDamage(_damage); break;
            }
        }
        else if (collision.tag == "Entity" && !_isblocked)
        {
            var collisionBattleState = collision.gameObject.GetComponent<ActorController>().GetCurrentBattleState();

            collision.gameObject.GetComponent<ActorController>().CombatController.GetDamage(_damage);
            //Debug.Log(_damage);
            Debug.Log("Hit");
        }
    }

    private void OnEnable()
    {
        _isblocked = false;
    }
}
