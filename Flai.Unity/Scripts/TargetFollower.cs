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
        public Vector3 Offset;

        protected override void Awake() 
        { 
            if (this.Target == null)
            {
             // FlaiDebug.LogErrorWithTypeTag<TargetFollower>("No target found!", this);
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
            Vector3 target = this.Target.GetPosition() + this.Offset;
            float amount = FlaiMath.Clamp(deltaTime*this.Power, 0, 1);

            if (this.X)
            {
                current.x = FlaiMath.Lerp(this.LerpType, current.x, target.x, amount);
            }

            if (this.Y)
            {
                current.y = FlaiMath.Lerp(this.LerpType, current.y, target.y, amount);
            }

            if (this.Z)
            {
                current.z = FlaiMath.Lerp(this.LerpType, current.z, target.z, amount);
            }

            this.Position = current;
        }
    }
}
