using Flai.Diagnostics;
using UnityEngine;

namespace Flai.Scripts
{
    [ExecuteInEditMode]
    public class TargetFollower : FlaiScript
    {
        public bool X = true;
        public bool Y = true;
        public bool Z = true;
        public bool FollowInEditMode = false;
        public LerpType LerpType = LerpType.Lerp;
        public bool UseFixedUpdate = false;

        public GameObject Target;
        public float Power = 1;

        protected override void Awake()
        {
            if (this.Target == null)
            {
                FlaiDebug.LogErrorWithTypeTag<TargetFollower>("No target found!", this);
            }
        }

        protected override void LateUpdate()
        {
            if (this.Target == null)
            {
                return;
            }

            if (!Application.isPlaying && this.FollowInEditMode)
            {
                this.Follow(0.016f);
            }
            else if (!this.UseFixedUpdate)
            {
                this.Follow();
            }
        }

        protected override void FixedUpdate()
        {
            if (this.Target == null || !this.UseFixedUpdate)
            {
                return;
            }

            this.Follow();
        }

        private void Follow(float? delta = null)
        {
            float deltaTime = delta ?? ((Time.deltaTime + Time.smoothDeltaTime) * 0.5f);
            Vector3 current = this.Position;
            Vector3 target = this.Target.GetPosition();

            if (this.X)
            {
                current.x = FlaiMath.Lerp(this.LerpType, current.x, target.x, deltaTime * this.Power);
            }

            if (this.Y)
            {
                current.y = FlaiMath.Lerp(this.LerpType, current.y, target.y, deltaTime * this.Power);
            }

            if (this.Z)
            {
                current.z = FlaiMath.Lerp(this.LerpType, current.z, target.z, deltaTime * this.Power);
            }

            this.Position = current;
        }
    }
}
