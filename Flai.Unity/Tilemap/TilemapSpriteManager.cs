using Flai.Diagnostics;
using Flai.Tilemap.Serializing;
using System.Collections.Generic;
using UnityEngine;

namespace Flai.Tilemap
{
    public class TilemapSpriteManager : Singleton<TilemapSpriteManager>
    {
        private readonly Dictionary<int, Sprite> _indexToSpriteDict = new Dictionary<int, Sprite>();
        private readonly Dictionary<string, Texture2D> _tilesetTextures = new Dictionary<string, Texture2D>();

        public Dictionary<int, Sprite>.ValueCollection Sprites
        {
            get { return _indexToSpriteDict.Values; }
        }

        public Sprite GetSprite(TilesetManager tilesetManager, int index)
        {
            Sprite sprite;
            if (!_indexToSpriteDict.TryGetValue(index, out sprite))
            {
                sprite = this.CreateSprite(tilesetManager, index);
                _indexToSpriteDict.Add(index, sprite);
            }

            return sprite;
        }

        private Sprite CreateSprite(TilesetManager tilesetManager, int index)
        {
            string textureName;
            RectangleF sourceRectangle;
            tilesetManager.GetTile(index, out textureName, out sourceRectangle);

            Texture2D texture = this.LoadTexture(textureName);

            // !!! the SpriteMeshType.FullRect is needed  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            return Sprite.Create(texture, sourceRectangle, Vector2.zero, tilesetManager.GetTilesetForId(index).TileSize, 0, SpriteMeshType.FullRect);
        }

        private Texture2D LoadTexture(string tileset)
        {
            tileset = tileset.Trim();
            Texture2D texture;
            if (!_tilesetTextures.TryGetValue(tileset, out texture) || texture == null)
            {
                texture = Resources.Load<Texture2D>(tileset);
                _tilesetTextures.AddOrSetValue(tileset, texture);
                if (texture == null)
                {
                    FlaiDebug.LogWarningWithTypeTag<TilemapSpriteManager>("Couldn't find tileset texture {0}", tileset);
                }
            }

            return texture;
        }

        public void Reset()
        {
            _indexToSpriteDict.Clear();
            _tilesetTextures.Clear();
        }
    }
}
