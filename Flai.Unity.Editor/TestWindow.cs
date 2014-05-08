
using Flai.Diagnostics;
using UnityEditor;
using UnityEngine;

public class FlaiWindow : EditorWindow
{
    [MenuItem("Flai/General")]
    static void Init()
    {
        EditorWindow.GetWindow<FlaiWindow>("General");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Find All Missing Scripts"))
        {
            this.FindMissingScripts();
        }
    }

    private void FindMissingScripts()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        foreach (GameObject gameObject in gameObjects)
        {
            this.FindMissingScriptsRecursive(gameObject);
        }

        FlaiDebug.Log("Finding All Missing Scripts Completed!");
    }

    private void FindMissingScriptsRecursive(GameObject gameObject)
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
            }
        }

        foreach (Transform childT in gameObject.transform)
        {
            this.FindMissingScriptsRecursive(childT.gameObject);
        }
    }
}