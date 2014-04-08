using Flai.DataStructures;
using Flai.Graphics;
using UnityEngine;

namespace Flai.UI
{
    public static class FlaiGUI
    {
        public static void DrawTexture(RectangleF area, Texture texture)
        {
            GUI.DrawTexture(area, texture);
        }

        public static void DrawTexture(RectangleF area, Texture texture, Color color)
        {
            Color previousColor = GUI.backgroundColor;
            GUI.color = color;
            FlaiGUI.DrawTexture(area, texture);
			GUI.color = previousColor;
        }
    }

    public static class FlaiGUILayout
    {
        // no idea if this works, didn't need this afterall
        public static void Box(Color color)
        {
            Color previousColor = GUI.backgroundColor;
            GUI.color = color;
            GUILayout.Box(TextureHelper.BlankTexture);
            GUI.color = previousColor;
        }
    }
}
