using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "ScriptableObjects/BlockData")]
public class BlockData : ScriptableObject
{
    public Vector3 InitialScale;
    public Vector3 InitialPosition;
    public Quaternion InitialRotation;

    public Vector3 ParryngEndPosition;
    public Vector3 ParryngEndScale;
    public Quaternion ParryngEndRotation;

    public Vector3 BlockPosition;
    public Vector3 BlockScale;
    public Quaternion BlockRotation;

    public float PreparingTime;
    public float ParryingMoveTime;
    public float ParryingTime;
    public float BlockMoveTime;
    public float RecoveryTime;

    public float DamageDecrease;
}
