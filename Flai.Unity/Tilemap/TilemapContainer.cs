using Flai.DataStructures;
using Flai.Diagnostics;
using Flai.Tilemap.Serializing;
using System;
using UnityEngine;

namespace Flai.Tilemap
{
    public class TilemapContainer : FlaiScript, IEquatable<TilemapContainer>
    {
        public GameObject TilemapPrefab;
        public TmxAsset TmxAsset;

        public ReadOnlyList<TilemapData> Tilemaps
        {
            get { return this.TmxAsset.Tilemaps; }
        }

        public bool Equals(TilemapContainer other)
        {
            return TmxAsset.AreEqual(this.TmxAsset, other.TmxAsset);
        }

        public void OnMapUpdated()
        {
            Ensure.IsEditor();
            this.GameObject.DestroyAllChildrenImmediate();

            // if the map was removed, then no need to create a new map
            if (this.TmxAsset == null || this.Tilemaps == null || this.Tilemaps.Count == 0)
            {
                return;
            }

            if (this.TilemapPrefab == null)
            {
                FlaiDebug.LogErrorWithTypeTag<TilemapContainer>("TilemapPrefab is null - aborting!");
                return;
            }

            foreach (TilemapData data in this.Tilemaps)
            {
                FlaiDebug.LogWithTypeTag<TilemapContainer>("Building a tilemap \"{2}\" ({0}x{1})", data.Width, data.Height, data.Name);
                GameObject tilemap = this.TilemapPrefab.Instantiate();
                if (data.HasProperty("CreateColliders"))
                {
                    bool value;
                    if (bool.TryParse(data.GetProperty("CreateColliders"), out value))
                    {
                        tilemap.Get<Tilemap>().CreateColliders = value;
                    }
                }

                tilemap.name = data.Name.NullIfEmpty() ?? "Tilemap";
                tilemap.SetParent(this.GameObject);
                tilemap.SetLayer("Tiles");
               
                try
                {
                    tilemap.Get<Tilemap>().CreateFrom(data, this.TmxAsset.TilesetManager);
                }
                catch
                {
                    FlaiDebug.LogErrorWithTypeTag<TilemapContainer>("Error - Aborting");
                }

                this.TmxAsset.NeedsRefresh = false;
            }
        }
    }
}
