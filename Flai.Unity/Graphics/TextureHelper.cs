using UnityEngine;

namespace Flai.Graphics
{
    public static class TextureHelper
    {
        private static Texture2D _blankTexture;
        public static Texture2D BlankTexture
        {
            get
            {
                if (_blankTexture == null)
                {
                    _blankTexture = new Texture2D(1, 1);
                    _blankTexture.SetPixel(0, 0, ColorF.White);
                    _blankTexture.Apply();
                }

                return _blankTexture;
            }
        }

        private static Texture2D _emptyTexture;
        public static Texture2D EmptyTexture
        {
            get
            {
                if (_emptyTexture == null)
                {
                    _emptyTexture = new Texture2D(1, 1);
                    _emptyTexture.SetPixel(0, 0, ColorF.Transparent);
                    _emptyTexture.Apply();
                }

                return _emptyTexture;
            }
        }
    }
}
