using UnityEngine;

namespace Flai.Scripts
{
    public class GameSettings : FlaiScript
    {
        public bool ShowCursor = true;

        protected override void Awake()
        {
            Screen.showCursor = this.ShowCursor;
        }

        protected override void LateUpdate() 
        {
            if (!this.ShowCursor)
            {
              //  Cursor.SetCursor(TextureHelper.EmptyTexture, Vector2f.Zero, CursorMode.Auto);
            }
        }
    }
}
