using UnityEngine;

namespace Flai.Scripts
{
    public class ConstantVelocity : FlaiScript
    {
        public Vector3 Velocity;

        protected override void Update()
        {
            this.Position += this.Velocity*Time.deltaTime;
        }
    }
}
