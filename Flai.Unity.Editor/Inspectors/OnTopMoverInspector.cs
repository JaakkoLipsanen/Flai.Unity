using Flai.Scripts;
using UnityEditor;

namespace Flai.Editor.Inspectors
{
 // [CustomEditor(typeof(OnTopMover))]
    public class OnTopMoverInspector : InspectorBase<OnTopMover>
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.IntField("Count", this.Target.Count);
        }
    }
}
