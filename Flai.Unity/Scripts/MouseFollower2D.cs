using Flai.Input;
using UnityEngine;

namespace Flai.Scripts
{
    public class MouseFollower2D : FlaiScript
    {
        protected override void LateUpdate()
        {
            this.Position2D = FlaiInput.MousePositionInWorld2D;
        }
    }
}
