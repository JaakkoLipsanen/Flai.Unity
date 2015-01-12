using System.Runtime.InteropServices;
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
                        this.DrawMember(MemberReference.FromProperty(property), attribute);
                    }
                }
            }
        }

        private void DrawMember(MemberReference member, ShowInInspectorAttribute attribute)
        {
            FlaiGUI.PushGuiEnabled(!attribute.IsReadOnly && (attribute.IsEditableWhenNotPlaying || Application.isPlaying) && member.CanWrite);
            if (!this.TryDrawMember(member, attribute))
            {
                DrawFunction drawFunction;
                if (InternalPropertyDrawer.GetDrawFunction(member, attribute, out drawFunction))
                {
                    this.DrawMember(member, attribute, drawFunction);
                }
                else
                {
                    var value = member.GetValue(this.Target);
                    if (value is UnityObject) // simple special case.. not sure what I needed this for though... :|
                    {
                        this.DrawMember(member, attribute, (n, v, o) => EditorGUILayout.ObjectField(n, (UnityObject)v, typeof(UnityObject), true));
                    }
                    else
                    {
                        FlaiGUI.PushGuiEnabled(false);
                        this.DrawMember(member, attribute,
                            (n, v, o) => EditorGUILayout.TextField(n + " (unkown type)", (v == null) ? "" : v.ToString()), true);
                        FlaiGUI.PopGuiEnabled();
                    }
                }
            }
            FlaiGUI.PopGuiEnabled();
        }


        private void DrawMember(MemberReference member, ShowInInspectorAttribute attribute, DrawFunction drawFunction)
        {
            this.DrawMember(member, attribute, drawFunction, false);
        }

        private void DrawMember(MemberReference memberReference, ShowInInspectorAttribute attribute, DrawFunction drawFunction, bool forceReadOnly)
        {
            var newValue = drawFunction(this.GetName(memberReference, attribute), memberReference.GetValue(this.Target), null);
            if (GUI.changed && memberReference.CanWrite && !forceReadOnly)
            {
                memberReference.SetValue(this.Target, newValue);
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
            if (GUILayout.Button(this.GetName(method.Name, attribute)))
            {
                method.Invoke(this.Target, null);
            }
            FlaiGUI.PopGuiEnabled();
        }

        #endregion

        private string GetName(MemberReference member, ShowInInspectorAttribute attribute)
        {
            return this.GetName(member.Name, attribute);
        }

        private string GetName(string memberName, ShowInInspectorAttribute attribute)
        {
            return attribute.Name ?? Common.AddSpaceBeforeCaps(memberName);
        }

        protected virtual bool TryDrawMember(MemberReference memberReference, ShowInInspectorAttribute attribute)
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

    public struct MemberReference
    {
        public static readonly MemberReference Empty = new MemberReference();

        private FieldInfo _fieldInfo;
        private PropertyInfo _propertyInfo;

        public bool HasValue
        {
            get { return this.MemberInfo != null; }
        }

        public MemberInfo MemberInfo
        {
            get { return (_fieldInfo != null) ? (MemberInfo)_fieldInfo : _propertyInfo; }
        }

        public bool IsProperty
        {
            get { return _propertyInfo != null; }
        }

        public bool CanWrite
        {
            get { return (_fieldInfo != null) ? _fieldInfo.IsInitOnly : _propertyInfo.CanWrite; }
        }

        public string Name
        {
            get { return this.MemberInfo.Name; }
        }

        public Type InnerType
        {
            get { return (_fieldInfo != null) ? _fieldInfo.FieldType : _propertyInfo.PropertyType; }
        }

        public static MemberReference FromField(FieldInfo fieldInfo)
        {
            return new MemberReference { _fieldInfo = fieldInfo };
        }

        public static MemberReference FromProperty(PropertyInfo propertyInfo)
        {
            return new MemberReference { _propertyInfo = propertyInfo };
        }

        public object GetValue(object source)
        {
            return (_fieldInfo == null) ? _propertyInfo.GetValue(source, null) : _fieldInfo.GetValue(source);
        }

        public void SetValue(object source, object value)
        {
            if (_fieldInfo != null)
            {
                _fieldInfo.SetValue(source, value);
            }
            else
            {
                _propertyInfo.SetValue(source, value, null);
            }
        }
    }
}
