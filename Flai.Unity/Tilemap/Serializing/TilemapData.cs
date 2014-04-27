using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flai.Tilemap.Serializing
{
    [Serializable]
    public class TilemapData
    {
        [SerializeField]
        private int[] _tiles;
        [SerializeField]
        private int _width;
        [SerializeField]
        private int _height;

        [SerializeField] 
        private string _name = "";

        [SerializeField]
        private List<string> _propertyKeys = new List<string>();

        [SerializeField]
        private List<string> _propertyValues = new List<string>(); 

        public Size Size
        {
            get { return new Size(_width, _height); }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public string Name
        {
            get { return _name ?? ""; }
        }

        public int PropertyCount
        {
            get { return _propertyKeys.Count; }
        }

        public TilemapData(int[] tiles, Size size, Dictionary<string, string> properties = null, string name = null)
        {
            _tiles = tiles;
            _width = size.Width;
            _height = size.Height;
            _name = name ?? "";

            var kvps = properties.ToArray();
            _propertyKeys = kvps.Select(kvp => kvp.Key).ToList();
            _propertyValues = kvps.Select(kvp => kvp.Value).ToList();
        }

        public int this[Vector2i v]
        {
            get { return this[v.X, v.Y]; }
        }

        public int this[int x, int y]
        {
            get { return _tiles[x + _width * y]; }
        }

        public bool HasTileAt(Vector2i v)
        {
            return this.HasTileAt(v.X, v.Y);
        }

        public bool HasTileAt(int x, int y)
        {
            return this[x, y] != 0;
        }

        public bool HasProperty(string propertyName)
        {
            return _propertyKeys.Contains(propertyName);
        }

        public string GetProperty(string propertyName)
        {
            for (int i = 0; i < _propertyKeys.Count; i++)
            {
                if (_propertyKeys[i] == propertyName)
                {
                    return _propertyValues[i];
                }
            }

            return null;
        }

        public static bool AreEqual(TilemapData tm1, TilemapData tm2)
        {
            return tm1.Size == tm2.Size &&  tm1._name == tm2._name && tm1._tiles.SequenceEqual(tm2._tiles); // todo: for loop would be faster
        }
    }
}
