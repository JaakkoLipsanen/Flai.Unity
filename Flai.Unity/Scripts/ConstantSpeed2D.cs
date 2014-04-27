
using UnityEngine;

namespace Flai.Scripts
{
    public class ConstantSpeed2D : FlaiScript
    {
        public float Speed = 5;

        protected override void Update()
        {
            this.Position2D += this.RotationDirection2D*this.Speed*Time.deltaTime;
        }
    }
}
