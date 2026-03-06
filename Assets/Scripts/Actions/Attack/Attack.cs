using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { Light, Special }

namespace Assets.Scripts.Actions.Attack
{
    public class Attack: AbstractAction
    {
        public Dictionary<AttackType, int> Damage { get; private set; }
        public float AttackSpeed = 170f;

        //private CapsuleCollider2D _lightCollider;
        private GameObject _lightCollider;
        public Attack(int damage, float specialMultiplier, float lightMultiplier, GameObject lightCollider, GameObject specialCollider, float attackSpeed)//CapsuleCollider2D lightCollider)
        {
            Damage = new()
            {
                [AttackType.Light] = (int)Math.Ceiling(damage * lightMultiplier),
                [AttackType.Special] = (int)Math.Ceiling(damage * specialMultiplier),
            };
            _lightCollider = lightCollider;

            AttackSpeed = attackSpeed;
        }
        public void StartAttack(AttackType attackType)
        {
            if (_stateMachine.CurrentAttackState != AttackStates.Recovering && _stateMachine.CurrentAttackState != AttackStates.Waiting) return;
            _startRoutine(AttackHandle());
        }

        private IEnumerator AttackHandle()
        {
            _stateMachine.ChangeAttackState(AttackStates.Charging);
            yield return new WaitForSeconds(0.01f);

            _stateMachine.ChangeAttackState(AttackStates.Accelerating);
            yield return new WaitForSeconds(0.1f);

            //_lightCollider.enabled = true;
            _lightCollider.SetActive(true);
            var initialPosition = new Vector3(_lightCollider.transform.localPosition.x, _lightCollider.transform.localPosition.y);
            _stateMachine.ChangeAttackState(AttackStates.DealingDamage);

            while(_lightCollider.transform.localPosition.y > 10)
            {
                _lightCollider.transform.localPosition = new Vector3(_lightCollider.transform.localPosition.x, _lightCollider.transform.localPosition.y - AttackSpeed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            } 
            //yield return new WaitForSeconds(0.9f);

            //_lightCollider.enabled = false;
            _lightCollider.SetActive(false);
            _lightCollider.transform.localPosition = new Vector3(initialPosition.x, initialPosition.y);
            _stateMachine.ChangeAttackState(AttackStates.Recovering);
            yield return new WaitForSeconds(0.5f);

            _stateMachine.ChangeAttackState(AttackStates.Waiting);
        }
    }
}
