using Flai.Inspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Flai.Editor.Inspectors.Internal
{
    internal delegate object DrawFunction(string label, object value, params GUILayoutOption[] parameters);
    internal delegate DrawFunction DrawFunctionGenerator(MemberReference member, ShowInInspectorAttribute attribute);
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
            { typeof(string), InternalPropertyDrawer.CreateDrawFunction<string>(EditorGUILayout.TextField) },
            
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
            return InternalPropertyDrawer.GetDrawFunction(typeof(T), out drawFunction);
        }

        public static bool GetDrawFunction(Type type, out DrawFunction drawFunction)
        {
            return GetDrawFunction(type, MemberReference.Empty, null, out drawFunction);
        }

        public static bool GetDrawFunction(Type type, MemberReference memberReference, ShowInInspectorAttribute attribute, out DrawFunction drawFunction)
        {
            if (_drawFunctions.TryGetValue(type, out drawFunction))
            {
                return true;
            }

            DrawFunctionGenerator generator;
            if (_drawFunctionGenerators.TryGetValue(type, out generator))
            {
                drawFunction = generator(memberReference, attribute);
                return true;
            }

            if (type.IsEnum)
            {
                drawFunction = (n, v, o) => EditorGUILayout.EnumPopup(n, (Enum)v, o);
                return true;
            }
            else if (typeof(UnityObject).IsAssignableFrom(type))
            {
                drawFunction = (n, v, o) => EditorGUILayout.ObjectField(n, (UnityObject)v, type, true, o);
                return true;
            }
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                DrawFunction elementDrawFunction;
                if (InternalPropertyDrawer.GetDrawFunction(elementType, out elementDrawFunction))
                {
                    drawFunction = (label, value, parameters) =>
                    {
                        const int IndentationAmount = 20;
                        Array array = (Array)value;

                        // draw the name of the array
                        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

                        // draw the length (and '+' & '-' buttons)
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(IndentationAmount);
                        int newLength = EditorGUILayout.IntField("Length", array.Length);
                        if (GUILayout.Button("+", GUILayout.Width(24)))
                        {
                            newLength++;
                        }
                        else if (GUILayout.Button("-", GUILayout.Width(24)))
                        {
                            newLength = FlaiMath.Max(0, newLength - 1);
                        }
                        EditorGUILayout.EndHorizontal();

                        // if the size has been changed, then update it
                        if (newLength != array.Length && newLength > 0)
                        {
                            Array newArray = (Array)Activator.CreateInstance(array.GetType(), newLength);
                            if (array.Length > 0)
                            {
                                for (int i = 0; i < newArray.Length; i++)
                                {
                                    var elementValue = (i >= array.Length) ? array.GetValue(array.Length - 1) : array.GetValue(i);
                                    newArray.SetValue(elementValue, i);
                                }
                            }

                            array = newArray;
                        }

                        // draw all the elements
                        for (int i = 0; i < array.Length; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(IndentationAmount);
                            array.SetValue(elementDrawFunction("Element " + i, array.GetValue(i), null), i);
                            EditorGUILayout.EndHorizontal();
                        }

                        return array;
                    };

                    return true;
                }
            }

            return false;
        }

        public static bool GetDrawFunction(MemberReference member, ShowInInspectorAttribute attribute, out DrawFunction drawFunction)
        {
            if (InternalPropertyDrawer.GetDrawFunction(member.InnerType, member, attribute, out drawFunction))
            {
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
            public static DrawFunction GenerateInt(MemberReference member, ShowInInspectorAttribute attribute)
            {
                if (member.HasValue && member.MemberInfo.HasCustomAttribute<ShowAsIntSliderAttribute>())
                {
                    var sliderAttribute = member.MemberInfo.GetCustomAttribute<ShowAsIntSliderAttribute>();
                    return (label, value, parameters) => EditorGUILayout.IntSlider(label, (int)value, sliderAttribute.Min, sliderAttribute.Max, parameters);
                }
                else if (member.HasValue && Attribute.IsDefined(member.MemberInfo, typeof(ShowAsLayerAttribute)))
                {
                    return InternalPropertyDrawer.CreateDrawFunction<int>(EditorGUILayout.LayerField);
                }
                else
                {
                    return InternalPropertyDrawer.CreateDrawFunction<int>(EditorGUILayout.IntField);
                }
            }

            public static DrawFunction GenerateFloat(MemberReference member, ShowInInspectorAttribute attribute)
            {
                if (member.HasValue && member.MemberInfo.HasCustomAttribute<ShowAsFloatSliderAttribute>())
                {
                    var sliderAttribute = member.MemberInfo.GetCustomAttribute<ShowAsFloatSliderAttribute>();
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
