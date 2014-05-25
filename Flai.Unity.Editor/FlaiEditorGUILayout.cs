using Flai.Editor.Inspectors.Internal;
using Flai.UI;
using UnityEditor;
using UnityEngine;

namespace Flai.Editor
{
    public static class FlaiEditorGUILayout
    {
        public static T DrawField<T>(string label, T value, params GUILayoutOption[] parameters)
        {
            return FlaiEditorGUILayout.DrawField(label, value, false, parameters);
        }

        public static T DrawField<T>(string label, T value, bool isReadOnly = false, params GUILayoutOption[] parameters)
        {
            DrawFunction drawFunction;
            if (InternalPropertyDrawer.GetDrawFunction<T>(out drawFunction))
            {
                FlaiGUI.PushGuiEnabled(!isReadOnly);
                var newValue = (T)drawFunction(label, value, parameters);
                FlaiGUI.PopGuiEnabled();
                return newValue;
            }
            
            FlaiGUI.PushGuiEnabled(false);
            EditorGUILayout.TextField(label + "(unknown type)", value.ToString(), parameters);
            FlaiGUI.PopGuiEnabled();
            return value;
        }
    }
}
