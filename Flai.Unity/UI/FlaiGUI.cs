using Flai.DataStructures;
using Flai.Graphics;
using UnityEngine;

namespace Flai.UI
{
    public static class FlaiGUI
    {
        private static ValueStackAggregator<bool> _isGuiEnabledAggregator = new ValueStackAggregator<bool>(true, (a, b) => a && b); 
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

        public static void PushGuiEnabled(bool isEnabled)
        {
            _isGuiEnabledAggregator.Push(isEnabled);
            GUI.enabled = _isGuiEnabledAggregator.CurrentValue;
        }

        public static void PopGuiEnabled()
        {
            _isGuiEnabledAggregator.Pop();
            GUI.enabled = _isGuiEnabledAggregator.CurrentValue;
        }

        #region Splitter

        private static readonly GUIStyle _splitterStyle;
        public static readonly Color DefaultSplitterColor = new Color(0.157f, 0.157f, 0.157f); // light theme -> new Color(0.5f, 0.5f, 0.5f);

        static FlaiGUI()
        {
            _splitterStyle = new GUIStyle
            {
                normal = { background = TextureHelper.BlankTexture },
                stretchWidth = true,
                margin = new RectOffset(0, 0, 7, 7)
            };
        }

        public static void Splitter(Color color, float thickness)
        {
            FlaiGUI.Splitter(color, thickness, Screen.width * 0.9f, Axis.Horizontal, true, Vector2i.Zero);
        }

        public static void Splitter(Color color, float thickness, float length)
        {
            FlaiGUI.Splitter(color, thickness, length, Axis.Horizontal, true, Vector2i.Zero);
        }

        public static void Splitter(Color color, float thickness, float length, Axis drawAxis)
        {
            FlaiGUI.Splitter(color, thickness, length, drawAxis, true, Vector2i.Zero);
        }

        public static void Splitter(Color color, float thickness, float length, Axis drawAxis, bool useLayout)
        {
            FlaiGUI.Splitter(color, thickness, length, drawAxis, useLayout, Vector2i.Zero);
        }

        public static void Splitter(Color color, float thickness, float length, Axis drawAxis, bool useLayout, Vector2i offset)
        {
            float width = (drawAxis == Axis.Horizontal) ? length : thickness;
            float height = (drawAxis == Axis.Horizontal) ? thickness : length;

            Rect position = GUILayoutUtility.GetRect(GUIContent.none, _splitterStyle, GUILayout.Width(useLayout ? width : 1), GUILayout.Height(useLayout ? height : 1));
            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = color;
                _splitterStyle.Draw(new Rect(position.x + offset.X, position.y + offset.Y, width, height), false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        #endregion
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
