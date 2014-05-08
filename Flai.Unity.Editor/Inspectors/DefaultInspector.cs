using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flai.Diagnostics;
using Flai.Inspector;
using Flai.IO;
using Flai.UI;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;
namespace Flai.Editor.Inspectors
{
 // [CustomEditor(typeof(MonoBehaviour), true)]
    public abstract class DefaultInspector : InspectorBase<MonoBehaviour>
    {
        private bool _hasSearchedForProxyInspector = false;
        private IProxyInspector _proxyInspector = null;

        public virtual IEnumerable<Assembly> AssembliesToSearchForProxyInspectors
        {
            get { yield break; }
        }

        public override void OnInspectorGUI()
        {
            if (this.Target == null)
            {
                return;
            }

            if (!_hasSearchedForProxyInspector)
            {
                this.SearchForProxyInspector();
            }

            if (_proxyInspector != null)
            {
                _proxyInspector.SetTarget(this.Target);
                _proxyInspector.OnInspectorGUI();
                return;
            }

            this.DrawDefaultInspector();
            this.DrawProperties();
            this.DrawMethods();

            EditorGUILayout.LabelField("DefaultInspector", EditorStyles.boldLabel);
        }

        #region Draw Properties

        private void DrawProperties()
        {
            Type type = this.Target.GetType();
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.HasCustomAttribute<ShowInInspectorAttribute>())
                {
                    var attribute = property.GetCustomAttribute<ShowInInspectorAttribute>();
                    if (attribute.IsVisible)
                    {
                        this.DrawProperty(property, attribute);
                    }
                }
            }
        }

        private void DrawProperty(PropertyInfo property, ShowInInspectorAttribute attribute)
        {
            FlaiGUI.PushGuiEnabled(!attribute.IsReadOnly && (attribute.IsEditableWhenNotPlaying || Application.isPlaying) && property.CanWrite);
            if (property.PropertyType.IsEnum)
            {
                this.DrawProperty<Enum>(property, attribute, EditorGUILayout.EnumPopup);
            }
            else if (property.PropertyType == typeof(bool))
            {
                this.DrawProperty<bool>(property, attribute, EditorGUILayout.Toggle);
            }
            else if (property.PropertyType == typeof(int))
            {
                if (property.HasCustomAttribute<ShowAsIntSliderAttribute>())
                {
                    var sliderAttribute = property.GetCustomAttribute<ShowAsIntSliderAttribute>();
                    this.DrawSlider<int>(property, attribute, sliderAttribute.Min, sliderAttribute.Max, EditorGUILayout.IntSlider);
                }
                else if (Attribute.IsDefined(property, typeof(ShowAsLayerAttribute)))
                {
                    this.DrawProperty<int>(property, attribute, EditorGUILayout.LayerField);
                }
                else
                {
                    this.DrawProperty<int>(property, attribute, EditorGUILayout.IntField);
                }
            }
            else if (property.PropertyType == typeof(float))
            {
                if (property.HasCustomAttribute<ShowAsFloatSliderAttribute>())
                {
                    var sliderAttribute = property.GetCustomAttribute<ShowAsFloatSliderAttribute>();
                    this.DrawSlider<float>(property, attribute, sliderAttribute.Min, sliderAttribute.Max, EditorGUILayout.Slider);
                }
                else
                {
                    this.DrawProperty<float>(property, attribute, EditorGUILayout.FloatField);
                }
            }
            else if (property.PropertyType == typeof(double))
            {
                this.DrawProperty<double>(property, attribute, (n, v, o) => (double)(EditorGUILayout.FloatField(n, (float)v, o)));
            }
            else if (property.PropertyType == typeof(Bounds))
            {
                this.DrawProperty<Bounds>(property, attribute, EditorGUILayout.BoundsField);
            }
            else if (property.PropertyType == typeof(Color))
            {
                this.DrawProperty<Color>(property, attribute, EditorGUILayout.ColorField);
            }
            else if (property.PropertyType == typeof(Color32))
            {
                this.DrawProperty<Color32>(property, attribute, (n, v, o) => (Color32)(EditorGUILayout.ColorField(n, v, o)));
            }
            else if (property.PropertyType == typeof(ColorF))
            {
                this.DrawProperty<ColorF>(property, attribute, (n, v, o) => (ColorF)(EditorGUILayout.ColorField(n, v, o)));
            }
            else if (property.PropertyType == typeof(AnimationCurve))
            {
                this.DrawProperty<AnimationCurve>(property, attribute, EditorGUILayout.CurveField);
            }
            else if (typeof(UnityObject).IsAssignableFrom(property.PropertyType))
            {
                var value = EditorGUILayout.ObjectField(this.GetName(property, attribute), (UnityObject)property.GetValue(this.Target, null), property.PropertyType, true);
                if (GUI.changed && property.CanWrite)
                {
                    property.SetValue(this.Target, value, null);
                }
            }
            else if (property.PropertyType == typeof(Rect))
            {
                this.DrawProperty<Rect>(property, attribute, EditorGUILayout.RectField);
            }
            else if (property.PropertyType == typeof(RectangleF))
            {
                this.DrawProperty<RectangleF>(property, attribute, (n, v, o) => (RectangleF)(EditorGUILayout.RectField(n, v, o)));
            }
            else if (property.PropertyType == typeof(Vector2))
            {
                this.DrawProperty<Vector2>(property, attribute, EditorGUILayout.Vector2Field);
            }
            else if (property.PropertyType == typeof(Vector2f))
            {
                this.DrawProperty<Vector2f>(property, attribute, (n, v, o) => (Vector2f)(EditorGUILayout.Vector2Field(n, v, o)));
            }
            else if (property.PropertyType == typeof(Vector3))
            {
                this.DrawProperty<Vector3>(property, attribute, EditorGUILayout.Vector3Field);
            }
            else if (property.PropertyType == typeof(Vector4))
            {
                this.DrawProperty<Vector4>(property, attribute, EditorGUILayout.Vector4Field);
            }
            else
            {
                FlaiGUI.PushGuiEnabled(false);
                this.DrawProperty<object>(property, attribute, (n, v, o) => EditorGUILayout.TextField(n + " (unkown type)", (v == null) ? "" : v.ToString()), true);
                FlaiGUI.PopGuiEnabled();
            }

            FlaiGUI.PopGuiEnabled();
        }

        private void DrawProperty<T>(PropertyInfo property, ShowInInspectorAttribute attribute, Func<string, T, GUILayoutOption[], T> drawFunction)
        {
            this.DrawProperty<T>(property, attribute, drawFunction, false);
        }

        private void DrawProperty<T>(PropertyInfo property, ShowInInspectorAttribute attribute, Func<string, T, GUILayoutOption[], T> drawFunction, bool forceReadOnly)
        {
            var newValue = drawFunction(this.GetName(property, attribute), (T)property.GetValue(this.Target, null), null);
            if (GUI.changed && property.CanWrite && !forceReadOnly)
            {
                property.SetValue(this.Target, newValue, null);
            }
        }

        private void DrawSlider<T>(PropertyInfo property, ShowInInspectorAttribute attribute, T min, T max, Func<string, T, T, T, GUILayoutOption[], T> drawFunction)
        {
            var newValue = drawFunction(this.GetName(property, attribute), (T)property.GetValue(this.Target, null), min, max, null);
            if (GUI.changed && property.CanWrite)
            {
                property.SetValue(this.Target, newValue, null);
            }
        }

        #endregion

        #region Draw Methods

        private void DrawMethods()
        {
            if (this.Target == null)
            {
                return;
            }

            Type type = this.Target.GetType();
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (method.HasCustomAttribute<ShowInInspectorAttribute>())
                {
                    var attribute = method.GetCustomAttribute<ShowInInspectorAttribute>();
                    if (attribute.IsVisible)
                    {
                        this.DrawMethod(method, attribute);
                    }
                }
            }
        }

        private void DrawMethod(MethodInfo method, ShowInInspectorAttribute attribute)
        {
            if (method.ContainsGenericParameters || method.GetParameters().Length > 0)
            {
                FlaiDebug.LogWarningWithTypeTag<DefaultInspector>("Method '{0}' has parameters or generic parameters. It cannot be called", method.Name);
                return;
            }

            FlaiGUI.PushGuiEnabled(!attribute.IsReadOnly && (attribute.IsEditableWhenNotPlaying || Application.isPlaying));
            if (GUILayout.Button(this.GetName(method, attribute)))
            {
                method.Invoke(this.Target, null);
            }
            FlaiGUI.PopGuiEnabled();
        }

        #endregion

        private string GetName(MemberInfo member, ShowInInspectorAttribute attribute)
        {
            return attribute.Name ?? Common.AddSpaceBeforeCaps(member.Name);
        }

        #region Search For Proxy Inspector

        private void SearchForProxyInspector()
        {
            var targetType = this.Target.GetType();
            var inspectorBaseType = typeof(ProxyInspector<>).MakeGenericType(targetType);

            Assembly currentAssembly = Assembly.GetCallingAssembly(); // Flai.Unity.Editor assembly is always included
            var searchAssemblies = this.AssembliesToSearchForProxyInspectors.Concat(currentAssembly.AsEnumerable());
            var matchingTypes = searchAssemblies.SelectMany(
                assembly => assembly.GetTypes().Where(type => inspectorBaseType.IsAssignableFrom(type) && typeof(IProxyInspector).IsAssignableFrom(type)));

            foreach (Type matchingType in matchingTypes)
            {
                if (!matchingType.ContainsGenericParameters && matchingType.GetConstructor(Type.EmptyTypes) != null)
                {
                    var inspector = Activator.CreateInstance(matchingType) as IProxyInspector;
                    if (inspector != null && this.AcceptProxyInspector(inspector))
                    {
                        _proxyInspector = inspector;
                        _hasSearchedForProxyInspector = true;
                        return;
                    }
                }
            }

            _hasSearchedForProxyInspector = true;
        }

        protected virtual bool AcceptProxyInspector(IProxyInspector inspector)
        {
            return true;
        }

        #endregion
    }
}
