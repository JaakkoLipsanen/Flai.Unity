using Flai.Diagnostics;
using Flai.Tilemap;
using Flai.Tilemap.Serializing;
using UnityEditor;
using UnityEngine;

namespace Flai.Editor.Inspectors
{
    public class TilemapContainerInspector : ProxyInspector<TilemapContainer>
    {
        public override void OnInspectorGUI()
        {
            this.DrawMapDataField();
            this.DrawTilemapPrefabField();
            this.DrawSize();
            this.DrawButtons();
        }

        private void DrawTilemapPrefabField()
        {
            this.Target.TilemapPrefab = (GameObject)EditorGUILayout.ObjectField("Tilemap Prefab", this.Target.TilemapPrefab, typeof(GameObject), false);
            EditorUtility.SetDirty(this.Target);
        }

        private void DrawMapDataField()
        {
            TilemapContainer tilemap = this.Target;
            TmxAsset tmxAsset = (TmxAsset)EditorGUILayout.ObjectField("Map Data", tilemap.TmxAsset, typeof(TmxAsset), false);

            if (tilemap.TmxAsset != null && tmxAsset == null)
            {
                FlaiDebug.LogWithTypeTag<TilemapContainerInspector>("Map Data set null!");
            }

            if (tilemap.TmxAsset != tmxAsset || (tilemap.TmxAsset != null && tilemap.TmxAsset.NeedsRefresh))
            {
                tilemap.TmxAsset = tmxAsset;
                tilemap.OnMapUpdated();
                EditorUtility.SetDirty(tilemap);
            }
        }

        private void DrawSize()
        {
            TilemapContainer tilemap = this.Target;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size", GUILayout.Width(Screen.width / 2.9f));

            GUI.enabled = false;
            string width = (tilemap.TmxAsset == null) ? "" : tilemap.TmxAsset.Size.Width.ToString();
            string height = (tilemap.TmxAsset == null) ? "" : tilemap.TmxAsset.Size.Height.ToString();
            EditorGUILayout.TextField(width);
            EditorGUILayout.TextField(height);
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            TilemapContainer tilemap = this.Target;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

            if (GUILayout.Button("Refresh"))
            {
                tilemap.OnMapUpdated();
            }

            if (GUILayout.Button("Reset"))
            {
                tilemap.TmxAsset = null;
                tilemap.OnMapUpdated();
            }
        }
    }
}
