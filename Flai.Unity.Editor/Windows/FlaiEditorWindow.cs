using UnityEditor;

namespace Flai.Editor.Windows
{
    public abstract class FlaiEditorWindow : EditorWindow
    {
        protected virtual void OnGUI() { }
        protected virtual void Update() { }
        protected virtual void OnSelectionChange() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnInspectorUpdate() { }

        protected virtual void OnFocus() { }
        protected virtual void OnLostFocus() { }

        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
    }
}
