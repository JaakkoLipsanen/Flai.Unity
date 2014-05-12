using Flai.Inspector;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Flai.Editor.Inspectors.Internal
{
    internal delegate object DrawFunction(string label, object value, params GUIContent[] parameters);
    internal static class InternalPropertyDrawer
    {
        public static bool GetDrawFunction(PropertyInfo property, ShowInInspectorAttribute attribute, out DrawFunction drawFunction)
        {
            var propertyType = property.PropertyType;
            if (propertyType == typeof(Vector2i))
            {
                drawFunction = CreateDelegate<Vector2i>(DrawVector2i);
                return true;
            }

            drawFunction = null;
            return false;
        }

        private static DrawFunction CreateDelegate<T>(Func<string, T, T> func)
        {
            return (label, value, parameters) => func(label, (T)value);
        }

        private static Vector2i DrawVector2i(string label, Vector2i value)
        {
            var labelWidth = GUILayout.Width(Screen.width / 2.5f);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, labelWidth);
            value.X = EditorGUILayout.IntField(value.X);
            value.Y = EditorGUILayout.IntField(value.Y);
            EditorGUILayout.EndHorizontal();

            return value;
        }
    }
}
