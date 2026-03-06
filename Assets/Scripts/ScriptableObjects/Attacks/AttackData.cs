using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "ScriptableObjects/AttackData")]
public class AttackData : ScriptableObject
{
    public Vector3 Scale;

    public Vector3 InitialPosition;
    public Quaternion InitialRotation;

    public Vector3 DealingDamagePosition;
    public Quaternion DealingDamageRotation;

    public Vector3 EndPosition;
    public Quaternion EndRotation;

    public float AccelerateTime;
    public float DealDamageTime;
    public float RecoveryTime;
}
