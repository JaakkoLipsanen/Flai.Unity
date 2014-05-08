using UnityEditor;
using UnityEngine;

namespace Flai.Editor.Inspectors
{
 // [CustomEditor(typeof(Camera))]
    public class CameraInspector : InspectorBase<Camera>
    {
        public override void OnInspectorGUI()
        {
            bool is2dMode = EditorPrefs.GetBool("IsCameraInspectorIn2DMode", false);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            is2dMode = GUILayout.Toggle(is2dMode, "2D", "Button", GUILayout.Width(30));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (is2dMode)
            {
                this.Draw2DMode();
            }
            else
            {
                this.Draw3DMode();
            }

            EditorPrefs.SetBool("IsCameraInspectorIn2DMode", is2dMode);
        }

        protected virtual void Draw3DMode()
        {
            base.DrawDefaultInspector();
        }

        protected virtual void Draw2DMode()
        {
            var labelWidth = GUILayout.Width(Screen.width / 2.5f);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Background Color", labelWidth);
            this.Target.backgroundColor = EditorGUILayout.ColorField(this.Target.backgroundColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ortographics Size", labelWidth);
            this.Target.orthographicSize = EditorGUILayout.FloatField(this.Target.orthographicSize);
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Setup 2D", GUILayout.Width(80)))
            {
                this.Setup2D();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        protected virtual void Setup2D()
        {
            Camera camera = this.Target;
            camera.isOrthoGraphic = true;
            camera.orthographic = true;
            camera.transform.eulerAngles = Vector3.zero;
            camera.orthographicSize = 10;
            if (camera.transform.position.z >= 0)
            {
                camera.transform.SetPositionZ(-5);
            }
        }
    }
}
