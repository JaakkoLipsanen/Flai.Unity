using UnityEngine;

namespace Flai.Scripts.Character
{
    [RequireComponent(typeof(CharacterController2D), typeof(Animator))]
    public class CharacterAnimatorState2D : FlaiScript
    {
        private CharacterController2D _controller;
        private CharacterController2D Controller
        {
            get { return _controller ?? (_controller = this.Get<CharacterController2D>()); }
        }

        public string IsRunningParameter = "IsRunning";
        public string IsOnGroundParameter = "IsOnGround";

        public string HorizontalVelocity = "VelocityX";
        public string VerticalVelocity = "VelocityY";

        protected override void Awake()
        {
        }

        protected override void LateUpdate()
        {
            if (!this.IsRunningParameter.IsNullOrWhiteSpace())
            {
                this.Animator.SetBool(this.IsRunningParameter, this.Controller.IsOnGround && FlaiMath.Abs(this.Controller.Velocity.X) > 0.01f);
            }

            if (!this.IsOnGroundParameter.IsNullOrWhiteSpace())
            {
                this.Animator.SetBool(this.IsOnGroundParameter, this.Controller.IsOnGround);
            }

            if (!this.HorizontalVelocity.IsNullOrWhiteSpace())
            {
                this.Animator.SetFloat(this.HorizontalVelocity, this.Controller.Velocity.X);
            }

            if (!this.VerticalVelocity.IsNullOrWhiteSpace())
            {
                this.Animator.SetFloat(this.VerticalVelocity, this.Controller.Velocity.Y);
            }
        }
    }
}
