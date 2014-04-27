using System;
using System.Linq;
using UnityEngine;

namespace Flai.Tilemap.Serializing
{
    [Serializable]
    public class TilesetManager
    {
        [SerializeField]
        private Tileset[] _tilesets;

        public TilesetManager(Tileset[] tilesets)
        {
            _tilesets = tilesets.OrderBy(tileset => tileset.FirstGid).ToArray();
        }

        public void GetTile(int id, out string textureName, out RectangleF sourceRectangle)
        {
            Tileset tileset = this.GetTilesetForId(id);
            textureName = tileset.ImageName;
            sourceRectangle = tileset.GetSourceRectangle(id);
        }

        public RectangleF GetTileSourceRectangle(int id)
        {
            string textureName;
            RectangleF sourceRectangle;
            this.GetTile(id, out textureName, out sourceRectangle);

            return sourceRectangle;
        }

        public Tileset GetTilesetForId(int id)
        {
            return _tilesets.First(ts => id >= ts.FirstGid && id < ts.FirstGid + ts.TileCount); // the second check is not necessary
        }
    }
}
