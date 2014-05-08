
namespace Flai.Editor
{
    public class InspectorBase<T> : UnityEditor.Editor
        where T : UnityEngine.Object
    {
        protected virtual T Target
        {
            get { return (T)target; }
        }

        protected virtual T[] Targets
        {
            get { return (T[])targets; } // probably doesn't work, change to "targets.Cast<T>().ToArray()"
        }
    }

    public interface IProxyInspector
    {
        void SetTarget(object value);
        void OnInspectorGUI();
    }

    public abstract class ProxyInspector<T> : IProxyInspector
        where T : UnityEngine.Object
    {
        protected T Target { get; set; }
        void IProxyInspector.SetTarget(object value)
        {
            this.Target = value as T;
        }

        public abstract void OnInspectorGUI();
    }
}
