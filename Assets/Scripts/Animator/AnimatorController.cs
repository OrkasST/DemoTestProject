using UnityEngine;

namespace Assets.Scripts.Animator
{
    public class AnimatorController
    {
        private UnityEngine.Animator _animator;
        public UnityEngine.Animator Animator { get { return _animator; } }


        public float Blend {
            get {
                if (_animator == null) Debug.Log("Animator Is Null");
                return _animator.GetFloat("Blend");
            }
            set {
                if (_animator == null) Debug.Log("Animator Is Null");
                _animator.SetFloat("Blend", value);
            }
        }
        public bool IsJumping
        {
            get
            {
                if (_animator == null) Debug.Log("Animator Is Null");
                return _animator.GetBool("IsJumping");
            }
            set
            {
                if (_animator == null) Debug.Log("Animator Is Null");
                _animator.SetBool("IsJumping", value);
            }
        }
        public bool IsFalling
        {
            get { return _animator.GetBool("IsFalling"); }
            set { _animator.SetBool("IsFalling", value); }
        }

        public bool IsBlocking
        {
            get { return _animator.GetBool("IsBlocking"); }
            set { _animator.SetBool("IsBlocking", value); }
        }

        public int AttackIndex
        {
            get { return _animator.GetInteger("AttackINT"); }
            set { _animator.SetInteger("AttackINT", value); }
        }


        public void Initialize(UnityEngine.Animator animator)
        {
            _animator = animator;
        }
    }
}
