using System.IO;
using Flai.Diagnostics;
using Flai.Graphics;
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

            if (GUILayout.Button("Create Blank Texture Asset"))
            {
                this.CreateBlankTexture();
            }

            if (GUILayout.Button("Create Circle Texture Asset"))
            {
                this.CreateCircleTexture(128);
            }
        }

        private void CreateBlankTexture()
        {
            var blankTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
            blankTexture.SetPixel(0, 0, ColorF.White);
            blankTexture.Apply();

            this.SaveTextureAsset("BlankTexture", blankTexture);
            Object.DestroyImmediate(blankTexture);
        }

        private void CreateCircleTexture(int diameter)
        {
            float radius = diameter / 2f;
            var blankTexture = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false, true);
            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    bool isInside = Vector2f.Distance(new Vector2f(x, y), Vector2f.One * radius) < radius;
                    blankTexture.SetPixel(x, y, isInside ? ColorF.White : ColorF.Transparent);
                }
            }

            blankTexture.Apply();

            this.SaveTextureAsset("BlankTexture", blankTexture);
            Object.DestroyImmediate(blankTexture);
        }

        private void SaveTextureAsset(string fileName, Texture2D texture)
        {
            var pngBytes = texture.EncodeToPNG();

            var assetsPath = Application.dataPath + "/";
            if (!Directory.Exists(assetsPath + "Textures/"))
            {
                AssetDatabase.CreateFolder("Assets", "Textures");
            }

            File.WriteAllBytes(assetsPath + "/Textures/" + fileName + ".png", pngBytes);
            AssetDatabase.Refresh();
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