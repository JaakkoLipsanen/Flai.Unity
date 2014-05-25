using UnityEngine;

namespace Flai.Scripts
{
    public class ConstantRotator2D : ExtendedMonoBehaviour
    {
        public float RotationPerSecond = 180;
        protected void LateUpdate()
        {
            this.Rotation2D += this.RotationPerSecond*Time.deltaTime;
        }
    }
}
