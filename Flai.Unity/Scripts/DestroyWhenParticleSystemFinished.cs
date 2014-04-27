using UnityEngine;

namespace Flai.Scripts
{
    [RequireComponent(typeof(ParticleSystem))]
    public class DestroyWhenParticleSystemFinished : ExtendedMonoBehaviour
    {
        protected void LateUpdate()
        {
            if (!this.particleSystem.IsAlive())
            {
                this.DestroyGameObject();
            }
        }
    }
}
