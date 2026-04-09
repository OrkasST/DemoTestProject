using Assets.Scripts.Animator;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Actions.Block
{
    public class Block: AbstractAction
    {
        private BlockData _blockData;
        private GameObject _hitbox;

        private Coroutine _currentBlock;
        private Action<Coroutine> _coroutineCancelFunction;
        private Action<BlockStates> _onBlockStateChange;

        public float DamageDecrease { get; private set; }
        public float BlockingStartTime { get; private set; }

        public Block(ActorStateMachine stateMachine, Rigidbody2D rb, Func<IEnumerator, Coroutine> coroutineStarterFunc, Action<Coroutine> coroutineCancelFunction,
            AnimatorController animatorController, BlockData blockData, GameObject hitbox, Action<BlockStates> onBlockStateChange)
        {
            Initialize(stateMachine, rb, coroutineStarterFunc, animatorController);
            _blockData = blockData;
            _hitbox = hitbox;
            _hitbox.SetActive(false);

            //onBlockStateChange(BlockStates.Blocking);

            _coroutineCancelFunction = coroutineCancelFunction;

            DamageDecrease = _blockData.DamageDecrease;
            _onBlockStateChange = onBlockStateChange;
        }

        public void StartBlock()
        {
            if (_stateMachine.CurrentBlockState != BlockStates.Waiting) return;
            _currentBlock = _startRoutine(BlockPreparingHandle());
        }
        public void EndBlock()
        {
            if (_stateMachine.CurrentBlockState != BlockStates.Blocking) return;
            _startRoutine(BlockEndHandle());
        }

        public void InterruptBlock()
        {
            _coroutineCancelFunction(_currentBlock);
            _hitbox.SetActive(false);
            _onBlockStateChange(BlockStates.Interrupted);
        }

        private IEnumerator BlockPreparingHandle()
        {
            #region PreparingState
            _onBlockStateChange(BlockStates.Preparing);

            _hitbox.transform.localPosition = _blockData.InitialPosition;
            _hitbox.transform.localRotation = _blockData.InitialRotation;
            _hitbox.transform.localScale = _blockData.InitialScale;
            yield return new WaitForSeconds(_blockData.PreparingTime);

            #endregion

            #region ParryingState
            var time = Time.time;
            _onBlockStateChange(BlockStates.Parrying);

            Vector3 movementSpeed = CalculateSpeed(_hitbox.transform, _blockData.ParryngEndPosition, _blockData.ParryingMoveTime);
            _hitbox.SetActive(true);
            while (_hitbox.transform.localPosition != _blockData.ParryngEndPosition && Time.time - time < _blockData.ParryingMoveTime)
            {
                //MoveHitbox(movementSpeed);
                yield return null;
            }
            _hitbox.transform.localPosition = _blockData.ParryngEndPosition;
            if (Time.time - time < _blockData.ParryingTime) yield return new WaitForSeconds(_blockData.ParryingTime - (Time.time - time));

            #endregion

            #region DelingDamageState
            _onBlockStateChange(BlockStates.Blocking);
            BlockingStartTime = Time.time;

            if (_blockData.BlockMoveTime > 0)
            {
                movementSpeed = CalculateSpeed(_hitbox.transform, _blockData.BlockPosition, _blockData.BlockMoveTime);

                time = Time.time;
                while (_hitbox.transform.localPosition != _blockData.BlockPosition && Time.time - time < _blockData.BlockMoveTime)
                {
                    MoveHitbox(movementSpeed);
                    yield return null;
                }
                _hitbox.transform.localPosition = _blockData.BlockPosition;
            }

            #endregion
        }

        private IEnumerator BlockEndHandle()
        {
            #region RecoveryState

            _hitbox.SetActive(false);
            _onBlockStateChange(BlockStates.Recovering);
            yield return new WaitForSeconds(_blockData.RecoveryTime);
            #endregion
            _stateMachine.ChangeBlockState(BlockStates.Waiting);
        }

        protected Vector3 CalculateSpeed(Transform hitboxTransform, Vector3 destination, float time)
        {
            if (time == 0) time = 0.1f;

            return new Vector3((destination.x - hitboxTransform.localPosition.x) / time, (destination.y - hitboxTransform.localPosition.y) / time);
        }

        protected void MoveHitbox(Vector3 speed)
        {
            _hitbox.transform.localPosition = new Vector3(_hitbox.transform.localPosition.x + speed.x * Time.deltaTime,
                    _hitbox.transform.localPosition.y + speed.y * Time.deltaTime);
        }
    }
}
