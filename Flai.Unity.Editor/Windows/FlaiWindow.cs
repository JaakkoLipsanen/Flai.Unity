using Flai.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Flai.Editor.Windows
{
    public class FlaiWindow : FlaiEditorWindow
    {
        [MenuItem("Flai/General")]
        private static void Initialize()
        {
            EditorWindow.GetWindow<FlaiWindow>("General");
        }

        protected override void OnGUI()
        {
            if (GUILayout.Button("Find All Missing Scripts"))
            {
                this.FindMissingScripts();
            }
        }

        private void FindMissingScripts()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            bool any = false;
            foreach (GameObject gameObject in gameObjects)
            {
                this.FindMissingScriptsRecursive(gameObject, ref any);
            }

            if (!any)
            {
                FlaiDebug.Log("No missing scripts found!");
            }
        }

        private void FindMissingScriptsRecursive(GameObject gameObject, ref bool any)
        {
            Component[] components = gameObject.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    string name = gameObject.name;
                    Transform transform = gameObject.transform;
                    while (transform.parent != null)
                    {
                        name = transform.parent.name + "/" + name;
                        transform = transform.parent;
                    }
                    Debug.LogWarning(name + " has an empty script attached in position: " + i, gameObject);
                    any = true;
                }
            }

            foreach (Transform childT in gameObject.transform)
            {
                this.FindMissingScriptsRecursive(childT.gameObject, ref any);
            }
        }
    }
}