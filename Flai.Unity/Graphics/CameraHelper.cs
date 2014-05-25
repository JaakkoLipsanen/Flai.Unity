using UnityEngine;

namespace Flai.Graphics
{
    // does this belong to Flai.Graphics?
    public static class CameraHelper
    {
        public static SizeF GetCameraSize2D(Camera camera)
        {
            return new SizeF(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2f);
        }
    }
}
