using UnityEngine;

namespace Flai.Scripts
{
    public class CursorSetter : FlaiScript
    {
        public Texture2D CursorTexture;
        protected override void Awake()
        {
        }

        protected override void LateUpdate()
        {
            Cursor.SetCursor(this.CursorTexture, Vector2f.Zero, CursorMode.Auto);
        }
    }
}
