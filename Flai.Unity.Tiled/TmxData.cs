using Flai.General;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Flai.Unity.Tiled
{
    // this DOES NOT fully implement the .TMX format!! Lots of things missing and lots of assumptions are made
    public class TmxData
    {
        public readonly string VersionTag;
        public readonly TmxMapOrientation Orientation;

        public readonly Size MapSize;
        public readonly Size TileSize;

        public readonly TmxTileset[] TmxTilesets;
        public readonly TmxTileLayer[] TmxTileLayers;

        internal TmxData(string versionTag, TmxMapOrientation orientation, Size mapSize, Size tileSize, TmxTileset[] tmxTilesets, TmxTileLayer[] tmxTileLayers)
        {
            this.VersionTag = versionTag;
            this.Orientation = orientation;
            this.MapSize = mapSize;
            this.TileSize = tileSize;
            this.TmxTilesets = tmxTilesets;
            this.TmxTileLayers = tmxTileLayers;
        }

        public static TmxData Load(string path)
        {
            return TmxData.Load(XDocument.Load(path));
        }

        public static TmxData Load(XDocument input)
        {
            var root = input.Document.Root;
            TmxMapOrientation orientation = EnumHelper.Parse<TmxMapOrientation>(root.Attribute("orientation").Value, true);
            if (orientation != TmxMapOrientation.Orthogonal)
            {
                throw new NotSupportedException("Only orthogonal maps are supported");
            }

            string versionTag = root.Attribute("version").Value;
            Size mapSize = new Size(int.Parse(root.Attribute("width").Value), int.Parse(root.Attribute("height").Value));
            Size tileSize = new Size(int.Parse(root.Attribute("tilewidth").Value), int.Parse(root.Attribute("tileheight").Value));

            TmxTileset[] tmxTilesets = root.Elements("tileset").Select(x => TmxTileset.Parse(x)).ToArray();
            TmxTileLayer[] tmxTileLayers = root.Elements("layer").Select(x => TmxTileLayer.Parse(x)).ToArray();

            return new TmxData(versionTag, orientation, mapSize, tileSize, tmxTilesets, tmxTileLayers);
        }

        public TmxTileLayer GetLayer(string name)
        {
            return this.TmxTileLayers.First(layer => layer.Name == name);
        }
    }
}
