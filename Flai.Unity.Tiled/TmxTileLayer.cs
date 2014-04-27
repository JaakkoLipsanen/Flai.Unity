using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Flai.Unity.Tiled
{
    public class TmxTileLayer
    {
        public readonly string Name;
        public readonly int[] TileData;
        public readonly Dictionary<string, string> Properties;

        internal TmxTileLayer(string name, int[] tileData, Dictionary<string, string> properties)
        {
            this.Name = name;
            this.TileData = tileData;
            this.Properties = properties;
        }

        internal static TmxTileLayer Parse(XElement element)
        {
            string name = element.Attribute("name").Value;
            int[] tiles = element.Element("data").Value.Split(',').Select(int.Parse).ToArray();
            Dictionary<string, string> properties = new Dictionary<string, string>();

            var propertiesElement = element.Element("properties");
            if (propertiesElement != null && propertiesElement.Elements("property").Any())
            {
                foreach (XElement property in propertiesElement.Elements("property"))
                {
                    if (property == null)
                    {
                        continue;
                    }

                    properties.Add(property.Attribute("name").Value, property.Attribute("value").Value);
                }
            }

            return new TmxTileLayer(name, tiles, properties);
        }
    }
}
