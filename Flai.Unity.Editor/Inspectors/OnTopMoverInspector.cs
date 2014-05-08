using Flai.Scripts;
using UnityEditor;

namespace Flai.Editor.Inspectors
{
 // [CustomEditor(typeof(OnTopMover))]
    public class OnTopMoverInspector : ProxyInspector<OnTopMover>
    {
        public override void OnInspectorGUI()
        {
            this.Target.AllowedDirection = (Direction2D)EditorGUILayout.EnumPopup("Allowed Direction", this.Target.AllowedDirection);
            EditorGUILayout.IntField("Count", this.Target.Count);
        }
    }
}
