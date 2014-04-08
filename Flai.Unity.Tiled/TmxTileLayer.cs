using System.Linq;
using System.Xml.Linq;

namespace Flai.Unity.Tiled
{
    public class TmxTileLayer
    {
        public readonly string Name;
        public readonly int[] TileData;

        internal TmxTileLayer(string name, int[] tileData)
        {
            this.Name = name;
            this.TileData = tileData;
        }

        internal static TmxTileLayer Parse(XElement element)
        {
            string name = element.Attribute("name").Value;
            int[] tiles = element.Element("data").Value.Split(',').Select(int.Parse).ToArray();

            return new TmxTileLayer(name, tiles);
        }
    }
}
