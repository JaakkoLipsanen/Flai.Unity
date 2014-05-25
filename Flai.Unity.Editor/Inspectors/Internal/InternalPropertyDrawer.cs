using Flai.Inspector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;
namespace Flai.Editor.Inspectors.Internal
{
    internal delegate object DrawFunction(string label, object value, params GUILayoutOption[] parameters);
    internal delegate DrawFunction DrawFunctionGenerator(PropertyInfo property, ShowInInspectorAttribute attribute);
    internal static class InternalPropertyDrawer
    {
        private static readonly Dictionary<Type, DrawFunction> _drawFunctions = new Dictionary<Type, DrawFunction>
        {
            { typeof(Bounds), InternalPropertyDrawer.CreateDrawFunction<Bounds>(EditorGUILayout.BoundsField) },
            { typeof(AnimationCurve), InternalPropertyDrawer.CreateDrawFunction<AnimationCurve>(EditorGUILayout.CurveField)},
            
            { typeof(Rect), InternalPropertyDrawer.CreateDrawFunction<Rect>(EditorGUILayout.RectField)},
            { typeof(RectangleF), InternalPropertyDrawer.CreateDrawFunction<RectangleF>((n, v, o) => (RectangleF)(EditorGUILayout.RectField(n, v, o)))},

            { typeof(Color), InternalPropertyDrawer.CreateDrawFunction<Color>(EditorGUILayout.ColorField) },
            { typeof(Color32), InternalPropertyDrawer.CreateDrawFunction<Color32>((n, v, o) => (Color32)(EditorGUILayout.ColorField(n, v, o))) },
            { typeof(ColorF), InternalPropertyDrawer.CreateDrawFunction<ColorF>((n, v, o) => (ColorF)(EditorGUILayout.ColorField(n, v, o))) },
            
            { typeof(Vector2), InternalPropertyDrawer.CreateDrawFunction<Vector2>(EditorGUILayout.Vector2Field) },
            { typeof(Vector2f), InternalPropertyDrawer.CreateDrawFunction<Vector2f>((n, v, o) => (Vector2f)(EditorGUILayout.Vector2Field(n, v, o))) },
            { typeof(Vector3), InternalPropertyDrawer.CreateDrawFunction<Vector3>(EditorGUILayout.Vector3Field) },
            { typeof(Vector4), InternalPropertyDrawer.CreateDrawFunction<Vector4>(EditorGUILayout.Vector4Field) },
            
            { typeof(bool), InternalPropertyDrawer.CreateDrawFunction<bool>(EditorGUILayout.Toggle) },
            { typeof(double), InternalPropertyDrawer.CreateDrawFunction<double>((n, v, o) => (double)(EditorGUILayout.FloatField(n, (float)v, o))) },  
    
            
            { typeof(Vector2i), InternalPropertyDrawer.CreateDrawFunction<Vector2i>(DrawFunctions.Draw) }, 
            { typeof(Size), InternalPropertyDrawer.CreateDrawFunction<Size>(DrawFunctions.Draw) },
            { typeof(SizeF), InternalPropertyDrawer.CreateDrawFunction<SizeF>(DrawFunctions.Draw) },
        };

        private static readonly Dictionary<Type, DrawFunctionGenerator> _drawFunctionGenerators = new Dictionary<Type, DrawFunctionGenerator>
        {
            { typeof(int), DrawFunctionGenerators.GenerateInt },
            { typeof(float), DrawFunctionGenerators.GenerateFloat },
        };

        public static bool GetDrawFunction<T>(out DrawFunction drawFunction)
        {
            var propertyType = typeof (T);
            if (_drawFunctions.TryGetValue(propertyType, out drawFunction))
            {
                return true;
            }

            DrawFunctionGenerator generator;
            if (_drawFunctionGenerators.TryGetValue(propertyType, out generator))
            {
                drawFunction = generator(null, null);
                return true;
            }

            return false;
        }

        public static bool GetDrawFunction(PropertyInfo property, ShowInInspectorAttribute attribute, out DrawFunction drawFunction)
        {
            var propertyType = property.PropertyType;
            if (_drawFunctions.TryGetValue(propertyType, out drawFunction))
            {
                return true;
            }

            DrawFunctionGenerator generator;
            if (_drawFunctionGenerators.TryGetValue(propertyType, out generator))
            {
                drawFunction = generator(property, attribute);
                return true;
            }

            if (propertyType.IsEnum)
            {
                drawFunction = (n, v, o) => EditorGUILayout.EnumPopup(n, (Enum)v, o);
                return true;
            }
            else if (typeof(UnityObject).IsAssignableFrom(propertyType))
            {
                drawFunction = (n, v, o) => EditorGUILayout.ObjectField(n, (UnityObject)v, propertyType, true, o);
                return true;
            }

            drawFunction = null;
            return false;
        }

        public static DrawFunction CreateDrawFunction<T>(Func<string, T, T> drawFunction)
        {
            return (label, value, parameters) => drawFunction(label, (T)value);
        }

        public static DrawFunction CreateDrawFunction<T>(Func<string, T, GUILayoutOption[], T> drawFunction)
        {
            return (label, value, parameters) => drawFunction(label, (T)value, null);
        }

        public static DrawFunction CreateDrawFunction<T>(Func<T, T> drawFunction)
        {
            return (label, value, parameters) =>
            {
                var labelWidth = GUILayout.Width(InternalPropertyDrawer.CalculateLabelWidth());
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, labelWidth);
                var newValue = drawFunction((T)value);
                EditorGUILayout.EndHorizontal();

                return newValue;
            };
        }

        private static float CalculateLabelWidth()
        {
            return Screen.width / 2.5f;
        }

        #region Draw Functions

        internal static class DrawFunctions
        {
            public static Vector2i Draw(Vector2i value)
            {
                return new Vector2i(EditorGUILayout.IntField(value.X), EditorGUILayout.IntField(value.Y));
            }

            public static Size Draw(Size value)
            {
                return new Size(EditorGUILayout.IntField(value.Width), EditorGUILayout.IntField(value.Height));
            }

            public static SizeF Draw(SizeF value)
            {
                return new SizeF(EditorGUILayout.FloatField(value.Width), EditorGUILayout.FloatField(value.Height));
            }
        }

        #endregion

        #region Draw Functions Generators

        internal static class DrawFunctionGenerators
        {
            public static DrawFunction GenerateInt(PropertyInfo property, ShowInInspectorAttribute attribute)
            {
                if (property != null && property.HasCustomAttribute<ShowAsIntSliderAttribute>())
                {
                    var sliderAttribute = property.GetCustomAttribute<ShowAsIntSliderAttribute>();
                    return (label, value, parameters) => EditorGUILayout.IntSlider(label, (int)value, sliderAttribute.Min, sliderAttribute.Max, parameters);
                }
                else if (property != null && Attribute.IsDefined(property, typeof(ShowAsLayerAttribute)))
                {
                    return InternalPropertyDrawer.CreateDrawFunction<int>(EditorGUILayout.LayerField);
                }
                else
                {
                    return InternalPropertyDrawer.CreateDrawFunction<int>(EditorGUILayout.IntField);
                }
            }

            public static DrawFunction GenerateFloat(PropertyInfo property, ShowInInspectorAttribute attribute)
            {
                if (property != null && property.HasCustomAttribute<ShowAsFloatSliderAttribute>())
                {
                    var sliderAttribute = property.GetCustomAttribute<ShowAsFloatSliderAttribute>();
                    return (label, value, parameters) => EditorGUILayout.Slider(label, (float)value, sliderAttribute.Min, sliderAttribute.Max, parameters);
                }
                else
                {
                    return InternalPropertyDrawer.CreateDrawFunction<float>(EditorGUILayout.FloatField);
                }
            }
        }

        #endregion
    }
}
