namespace Assets.Scripts.Animator
{
    public class AnimatorController
    {
        private UnityEngine.Animator _animator;

        public bool IsRunning
        {
            get { return _animator.GetBool("isRunning"); }
            set { _animator.SetBool("isRunning", value); }
        }
        public bool IsJumping
        {
            get { return _animator.GetBool("isJumping"); }
            set { _animator.SetBool("isJumping", value); }
        }


        public void Initialize(UnityEngine.Animator animator)
        {
            _animator = animator;
        }
    }
}
