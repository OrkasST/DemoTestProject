using UnityEngine;

namespace Assets.Scripts.BusinessLogic
{
    public class CombatController
    {
        public int MaxHp = 100;
        public int CurrentHp = 100;

        public int WeaponDamage = 5;

        public CapsuleCollider2D _lightAttackCollider;

        public void Initialize(CapsuleCollider2D lightAttackCollider)
        {
            _lightAttackCollider = lightAttackCollider;
        }

        public void GetDamage(int hp)
        {
            if (CurrentHp <= hp) CurrentHp = 0;
            else CurrentHp -= hp;
        }

        public bool CheckAttackCollision()
        {
            //_lightAttackCollider
            return false;
        }
    }
}
