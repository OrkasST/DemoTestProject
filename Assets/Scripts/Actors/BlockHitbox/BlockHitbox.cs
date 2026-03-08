using System;
using UnityEngine;

public class BlockHitbox : MonoBehaviour
{
    [SerializeField] private ActorController _actorController;
    public BlockStates GetBlockState() => _actorController.GetCurrentBlockState();
    public ActorController GetActorController() => _actorController;
}
