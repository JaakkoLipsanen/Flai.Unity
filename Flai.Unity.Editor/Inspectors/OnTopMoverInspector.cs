using Flai.Scripts;
using UnityEditor;
using UnityEngine;

namespace Flai.Editor.Inspectors
{
 // [CustomEditor(typeof(OnTopMover))]
    public class OnTopMoverInspector : ProxyInspector<OnTopMover>
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = !this.Target.SetDirectionAutomatically;
            this.Target.AllowedDirection = (Direction2D)EditorGUILayout.EnumPopup("Allowed Direction", this.Target.AllowedDirection);

            GUI.enabled = true;
            this.Target.SetDirectionAutomatically = EditorGUILayout.Toggle("Set Direction Automatically", this.Target.SetDirectionAutomatically);
            GUI.enabled = this.Target.SetDirectionAutomatically;
            {
                this.Target.AutomaticSetDefaultDirection = (Direction2D)EditorGUILayout.EnumPopup("Default Direction", this.Target.AutomaticSetDefaultDirection);
            }

            GUILayout.Space(16);
            GUI.enabled = false;
            EditorGUILayout.IntField("Count on Top", this.Target.Count);
        }
    }
}
