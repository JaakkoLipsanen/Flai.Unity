using System.Xml.Linq;

namespace Flai.Unity.Tiled
{
    public class TmxTileset
    {
        public readonly int FirstGlobalTileID;
        public readonly string Name;
        public readonly Size TileSize;
        public readonly string ImagePath;
        public readonly Size ImageSize;

        internal TmxTileset(int firstGid, string name, Size tileSize, string imagePath, Size imageSize)
        {
            this.FirstGlobalTileID = firstGid;
            this.Name = name;
            this.TileSize = tileSize;
            this.ImagePath = imagePath;
            this.ImageSize = imageSize;
        }

        internal static TmxTileset  Parse(XElement element)
        {
            int firstGid = int.Parse(element.Attribute("firstgid").Value);
            string name = element.Attribute("name").Value;
            Size tileSize = new Size(int.Parse(element.Attribute("tilewidth").Value), int.Parse(element.Attribute("tileheight").Value));
            string imagePath = element.Element("image").Attribute("source").Value;
            Size imageSize = new Size(
                int.Parse(element.Element("image").Attribute("width").Value), 
                int.Parse(element.Element("image").Attribute("height").Value));

            return new TmxTileset(firstGid, name, tileSize, imagePath, imageSize);
        }
    }
}
