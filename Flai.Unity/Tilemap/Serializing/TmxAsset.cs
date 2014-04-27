using System;
using System.Collections.Generic;
using System.Linq;
using Flai.DataStructures;
using UnityEngine;

namespace Flai.Tilemap.Serializing
{
    /* todo: black & white tilemaps seperated */
    [Serializable]
    public class TmxAsset : ScriptableObject // TilemapAsset?
    {
        public event GenericEvent Changed;

        [SerializeField]
        private List<TilemapData> _tilemaps = new List<TilemapData>();

        [SerializeField]
        private TilesetManager _tilesetManager;

        [SerializeField]
        private int _creationGuid; // can't use GUID since it's not serializable

        [SerializeField]
        private bool _needsRefresh = true;

        public ReadOnlyList<TilemapData> Tilemaps
        {
            get { return new ReadOnlyList<TilemapData>(_tilemaps); }
        }

        public TilesetManager TilesetManager
        {
            get { return _tilesetManager; }
        }

        public Size Size
        {
            get { return _tilemaps[0].Size; }
        }

        public int UniqueGUID
        {
            get { return _creationGuid; }
        }

        public bool NeedsRefresh
        {
            get { return _needsRefresh; }
            set { _needsRefresh = value; } // meh
        }

        public void Initialize(IEnumerable<TilemapData> tilemaps, TilesetManager tilesetManager)
        {
            Ensure.True(tilemaps.Any());
            _tilemaps = new List<TilemapData>(tilemaps);
            _tilesetManager = tilesetManager;
            _creationGuid = Global.Random.Next();
        }

        public void CopyFrom(TmxAsset other)
        {
            this.Initialize(other._tilemaps, other.TilesetManager);
            _needsRefresh = true;
            this.Changed.InvokeIfNotNull();
        }

        public static bool AreEqual(TmxAsset map1, TmxAsset map2)
        {
            if (map1 == null || map2 == null)
            {
                return map1 == map2;
            }
            else if (map1.Size != map2.Size || map1.Tilemaps.Count != map2.Tilemaps.Count)
            {
                return false;
            }

            for (int i = 0; i < map1.Tilemaps.Count; i++)
            {
                if (!TilemapData.AreEqual(map1.Tilemaps[i], map2.Tilemaps[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
