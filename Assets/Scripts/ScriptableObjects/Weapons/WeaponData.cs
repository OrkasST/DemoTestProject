using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public AttackData LightAttackData;
    public AttackData SpecialAttackData;
    public BlockData BlockData;
    public bool CanDashOnParry;
    public bool CanCounterAttack;
    public bool CanAttackWhileBlocking;
}
