using UnityEngine;

namespace Flai.Graphics
{
    public static class TextureHelper
    {
        public static readonly Texture BlankTexture;

        static TextureHelper()
        {
            TextureHelper.BlankTexture = new Texture2D(1, 1);
            TextureHelper.BlankTexture.As<Texture2D>().SetPixel(0, 0, Color.white);
        }
    }
}
