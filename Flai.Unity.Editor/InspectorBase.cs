
namespace Flai.Editor
{
    public class InspectorBase<T> : UnityEditor.Editor
     where T : UnityEngine.Object
    {
        protected T Target
        {
            get { return (T)target; }
        }

        protected T[] Targets
        {
            get { return (T[])targets; } // probably doesn't work, change to "targets.Cast<T>().ToArray()"
        }
    }
}
