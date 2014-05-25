using Flai.Diagnostics;
using Flai.Editor.Inspectors.Internal;
using Flai.Inspector;
using Flai.UI;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var propertyTyhpe = property.PropertyType;
            if (!this.TryDrawProperty(property, attribute))
            {
                DrawFunction drawFunction;
                if (InternalPropertyDrawer.GetDrawFunction(property, attribute, out drawFunction))
                {
                    this.DrawProperty(property, attribute, drawFunction);
                    return;
                }

                var value = property.GetValue(this.Target, null);
                if (value is UnityObject) // simple special case.. not sure what I needed this for though... :|
                {
                    this.DrawProperty(property, attribute, (n, v, o) => EditorGUILayout.ObjectField(n, (UnityObject)v, typeof(UnityObject), true));
                }
                else
                {
                    FlaiGUI.PushGuiEnabled(false);
                    this.DrawProperty(property, attribute,
                        (n, v, o) => EditorGUILayout.TextField(n + " (unkown type)", (v == null) ? "" : v.ToString()), true);
                    FlaiGUI.PopGuiEnabled();
                }
            }
            FlaiGUI.PopGuiEnabled();
        }

 
        private void DrawProperty(PropertyInfo property, ShowInInspectorAttribute attribute, DrawFunction drawFunction)
        {
            this.DrawProperty(property, attribute, drawFunction, false);
        }

        private void DrawProperty(PropertyInfo property, ShowInInspectorAttribute attribute, DrawFunction drawFunction, bool forceReadOnly)
        {
            var newValue = drawFunction(this.GetName(property, attribute), property.GetValue(this.Target, null), null);
            if (GUI.changed && property.CanWrite && !forceReadOnly)
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

        protected virtual bool TryDrawProperty(PropertyInfo property, ShowInInspectorAttribute attribute)
        {
            return false;
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
