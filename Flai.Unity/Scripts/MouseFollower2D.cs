using Flai.Input;
using UnityEngine;

namespace Flai.Scripts
{
    public class MouseFollower2D : FlaiScript
    {
        public Camera Camera;
        protected override void LateUpdate()
        {
            this.Position2D = FlaiInput.GetMousePositionInWorld2D(this.Camera ?? Camera.main);
        }
    }
}
