using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Actions.Attack
{
    public class Attack: AbstractAction
    {
        public void LightAttack()
        {
            _startRoutine(AttackHandle());
        }

        private IEnumerator AttackHandle()
        {
            _stateMachine.ChangeAttackState(AttackStates.Charging);
            yield return new WaitForSeconds(0.5f);

            _stateMachine.ChangeAttackState(AttackStates.Accelerating);
            yield return new WaitForSeconds(0.5f);

            _stateMachine.ChangeAttackState(AttackStates.DealingDamage);
            yield return new WaitForSeconds(0.5f);

            _stateMachine.ChangeAttackState(AttackStates.Recovering);
            yield return new WaitForSeconds(0.5f);

            _stateMachine.ChangeAttackState(AttackStates.Waiting);
        }
    }
}
