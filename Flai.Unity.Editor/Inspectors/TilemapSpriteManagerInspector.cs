using Flai.Tilemap;
using UnityEditor;
using UnityEngine;

namespace Flai.Editor.Inspectors
{
    public class TilemapSpriteManagerInspector : ProxyInspector<TilemapSpriteManager>
    {
        private bool _toggled = false;
        public override void OnInspectorGUI()
        {
            _toggled = EditorGUILayout.Foldout(_toggled, "Sprites");
            if (_toggled)
            {
                EditorGUILayout.LabelField("Count: " + this.Target.Sprites.Count, EditorStyles.boldLabel);
                foreach (Sprite sprite in this.Target.Sprites)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(sprite.texture.name);
                    EditorGUILayout.LabelField(string.Format("X: {0}, Y: {1}, W: {2}, H: {3}", sprite.textureRect.x, sprite.textureRect.y, sprite.textureRect.width, sprite.textureRect.height));
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Reset"))
            {
                this.Target.Reset();
            }
        }
    }
}
