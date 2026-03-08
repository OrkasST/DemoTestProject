using System.IO;
using UnityEngine;

namespace Assets.Scripts.Weapon.Sword
{
    public class Sword : AbstractWeapon
    {
        public Sword()
        {
            _weaponData = Resources.Load<WeaponData>("WeaponData/Sword/SwordData");
        }
    }
}
