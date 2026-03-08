using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "ScriptableObjects/AttackData")]
public class AttackData : ScriptableObject
{
    public Vector3 InitialScale;
    public Vector3 InitialPosition;
    public Quaternion InitialRotation;

    public Vector3 DealingDamagePosition;
    public Vector3 DealingDamageScale;
    public Quaternion DealingDamageRotation;

    public Vector3 EndPosition;
    public Vector3 EndScale;
    public Quaternion EndRotation;

    public float ChargingTime;
    public float AccelerateTime;
    public float DealDamageTime;
    public float RecoveryTime;

    public int Damage;
}
