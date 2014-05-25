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

        private static Sprite _blankSprite;
        public static Sprite BlankSprite
        {
            get
            {
                if (_blankSprite == null)
                {
                    _blankSprite = Sprite.Create(TextureHelper.BlankTexture, new Rect(0, 0, 1, 1), Vector2f.One*0.5f, 1);
                }

                return _blankSprite;
            }
        }

        public static RectangleF PixelToUvCoordinates(Texture2D texture, RectangleF pixelCoordinates) // TODO: RectangleF -> Rectangle (int's)
        {
            return pixelCoordinates / new Vector2f(texture.width, texture.height);
        }
    }
}
