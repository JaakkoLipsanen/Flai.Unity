using Flai.Graphics;
using UnityEditor;
using UnityEngine;

namespace Flai.Editor
{
    public static class FlaiEditorPrefs
    {
        public static Color32 GetColor32(string key)
        {
            return ColorHelper.IntToColor32(EditorPrefs.GetInt(key));
        }

        public static Color32 GetColor32OrDefault(string key, Color32 defaultColor)
        {
            return EditorPrefs.HasKey(key) ? FlaiEditorPrefs.GetColor32(key) : defaultColor;
        }

        public static void SetColor32(string key, Color32 color)
        {
            EditorPrefs.SetInt(key, ColorHelper.Color32ToInt(color));
        }
    }
}
