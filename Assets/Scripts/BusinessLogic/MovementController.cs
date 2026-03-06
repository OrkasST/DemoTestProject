using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Jumping _jumping = new Jumping();

    private void Start()
    {
        //_jumping.Initialize()
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _jumping.OnActorCollisionEnter(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        _jumping.OnActorCollisionLeave(collision);
    }
}
