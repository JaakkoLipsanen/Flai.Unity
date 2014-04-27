using Flai.Diagnostics;
using Flai.Tilemap.Serializing;
using Flai.Unity.Tiled;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Flai.Editor.Importers
{
    public class TilemapLoader
    {
        public static TmxAsset LoadTilemap(string assetPath)
        {
            var tmxData = TmxData.Load(assetPath);
            var data = ScriptableObject.CreateInstance<TmxAsset>();
            TilemapLoader.InitializeTilemapData(data, tmxData);

            return data;
        }

        private static void InitializeTilemapData(TmxAsset data, TmxData tmxData)
        {
            TilesetManager tilesetManager = TilemapLoader.CreateTilesetManager(tmxData);
            data.Initialize(tmxData.TmxTileLayers.Select(tl => new TilemapData(tl.TileData, tmxData.MapSize, tl.Properties, tl.Name)), tilesetManager);
        }

        private static TilesetManager CreateTilesetManager(TmxData tmxData)
        {
            return new TilesetManager(tmxData.TmxTilesets.Select(ts =>
            {
                FlaiDebug.Log("!" + ts.ImageSize.Width + " " + ts.ImageSize.Height);
                Ensure.True(ts.TileSize.Width == ts.TileSize.Height);
                return new Tileset(ts.Name, ts.FirstGlobalTileID, ts.TileSize.Width, Path.GetFileNameWithoutExtension(ts.ImagePath), ts.ImageSize.Width, ts.ImageSize.Height);
            }).ToArray());
        }
    }
}
