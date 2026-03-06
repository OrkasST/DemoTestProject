using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Weapon
{
    public abstract class AbstractWeapon
    {
        protected WeaponData _weaponData;

        protected Func<IEnumerator, Coroutine> _routineStarter;
        protected GameObject _lightAttackHitbox;
        protected GameObject _specialAttackHitbox;

        public virtual void Initialize(GameObject lightAttackHitbox, GameObject specialAttackHitbox, Func<IEnumerator, Coroutine> routineStarter)
        {
            _lightAttackHitbox = lightAttackHitbox;
            _specialAttackHitbox = specialAttackHitbox;
            _routineStarter = routineStarter;
        }

        public AttackStates CurrentAttackState { get; protected set; } = AttackStates.Waiting;

        public virtual void Attack(AttackType attackType) => _routineStarter(StartAttack(attackType));

        protected virtual IEnumerator StartAttack(AttackType attackType)
        {
            if (attackType == AttackType.Special) _specialAttackHitbox.SetActive(true);
            else _lightAttackHitbox.SetActive(true);

            CurrentAttackState = AttackStates.Accelerating;
            float moveSpeed = 0;
            if (attackType == AttackType.Special)
                moveSpeed = CalculateSpeed(_specialAttackHitbox.transform, _weaponData.SpecialAttackData.DealingDamagePosition, _weaponData.SpecialAttackData.AccelerateTime);
            else moveSpeed = CalculateSpeed(_lightAttackHitbox.transform, _weaponData.LightAttackData.DealingDamagePosition, _weaponData.LightAttackData.AccelerateTime);

            //while ()

            CurrentAttackState = AttackStates.DealingDamage;

            //while (_lightAttackHitbox.transform.localPosition != _)
            //{
                //_lightAttackHitbox.transform.localPosition = new Vector3(_lightAttackHitbox.transform.localPosition.x, _lightAttackHitbox.transform.localPosition.y - AttackSpeed * Time.fixedDeltaTime);
                //yield return new WaitForFixedUpdate();
            //}

            _lightAttackHitbox.SetActive(false);
            //_lightAttackHitbox.transform.localPosition = new Vector3(initialPosition.x, initialPosition.y);
            CurrentAttackState = AttackStates.Recovering;
            yield return new WaitForSeconds(0.5f);

            CurrentAttackState = AttackStates.Waiting;
        }

        protected float CalculateSpeed(Transform hitboxTransform, Vector3 destination, float time)
        {
            return 0;
        }
    }
}
