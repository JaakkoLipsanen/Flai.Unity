using System;
using UnityEngine;

namespace Flai
{
    #region Size

    public struct Size : IEquatable<Size>
    {
        private int _width;
        private int _height;

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public float AspectRatio
        {
            get { return (float)this.Width / this.Height; }
        }

        public Size(Vector2i size)
            : this(size.X, size.Y)
        {
        }

        public Size(int width, int height)
            : this()
        {
            Ensure.True(width >= 0 && height >= 0);

            _width = width;
            _height = height;
        }

        public static Vector2f operator /(Size size, float value)
        {
            return new Vector2f(size.Width / value, size.Height / value);
        }

        public static bool operator ==(Size size1, Size size2)
        {
            return size1.Width == size2.Width && size1.Height == size2.Height;
        }

        public static bool operator !=(Size size1, Size size2)
        {
            return size1.Width != size2.Width || size1.Height != size2.Height;
        }

        public static implicit operator Vector2i(Size size)
        {
            return new Vector2i(size.Width, size.Height);
        }

        public static implicit operator Vector2f(Size size)
        {
            return new Vector2f(size.Width, size.Height);
        }

        public static implicit operator Vector2(Size size)
        {
            return new Vector2(size.Width, size.Height);
        }

        public Vector2i ToVector2i()
        {
            return new Vector2i(this.Width, this.Height);
        }

        public Vector2f ToVector2f()
        {
            return new Vector2f(this.Width, this.Height);
        }

        public bool Equals(Size other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj is Size)
            {
                return (Size)obj == this;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Width ^ this.Height;
        }

        public override string ToString()
        {
            return this.Width + ", " + this.Height;
        }

        public static readonly Size Empty = new Size(0, 0);
        public static readonly Size Invalid = new Size { _width = -1, _height = -1 };
    }

    #endregion

    #region SizeF

    // TODO: This is currently not used anywhere. Make it used :P! (or delete this class)

    [Serializable]
    public struct SizeF : IEquatable<SizeF>
    {
        [SerializeField]
        private float _width;
        [SerializeField]
        private float _height;

        public float Width
        {
            get { return _width; }
        }

        public float Height
        {
            get { return _height; }
        }

        public float AspectRatio
        {
            get { return (float)this.Width / this.Height; }
        }

        public SizeF(Vector2 size)
            : this(size.x, size.y)
        {
        }

        public SizeF(float width, float height)
            : this()
        {
            Ensure.True(width >= 0 && height >= 0);

            _width = width;
            _height = height;
        }

        public static bool operator ==(SizeF size1, SizeF size2)
        {
            return size1.Width == size2.Width && size1.Height == size2.Height;
        }

        public static bool operator !=(SizeF size1, SizeF size2)
        {
            return size1.Width != size2.Width || size1.Height != size2.Height;
        }

        public static SizeF operator +(SizeF size1, SizeF size2)
        {
            return new SizeF(size1.Width + size2.Width, size1.Height + size2.Height);
        }

        public static implicit operator Vector2f(SizeF size)
        {
            return new Vector2f(size.Width, size.Height);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(this.Width, this.Height);
        }

        public Vector2f ToVector2f()
        {
            return new Vector2f(this.Width, this.Height);
        }

        public bool Equals(SizeF other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj is SizeF)
            {
                return (SizeF)obj == this;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Width.GetHashCode() ^ this.Height.GetHashCode();
        }

        public override string ToString()
        {
            return this.Width + ", " + this.Height;
        }

        public static readonly SizeF Empty = new SizeF(0, 0);
        public static readonly SizeF Invalid = new SizeF { _width = -1, _height = -1 };
    }

    #endregion

    #region SerializableSize

    [Serializable]
    public class SerializableSize : IEquatable<SerializableSize>, IEquatable<Size>
    {
        [SerializeField]
        private int _width;

        [SerializeField]
        private int _height;

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public float AspectRatio
        {
            get { return (float)this.Width / this.Height; }
        }

        public SerializableSize(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public override bool Equals(object obj)
        {
            if (obj is Size)
            {
                return this.Equals((Size)obj);
            }
            else if (obj is SerializableSize)
            {
                return this.Equals((SerializableSize)obj);
            }

            return base.Equals(obj);
        }

        public bool Equals(SerializableSize other)
        {
            if (other == null) return false;
            return _width == other.Width && _height == other.Height;
        }

        public bool Equals(Size other)
        {
            return _width == other.Width && _height == other.Height;
        }

        public Vector2i ToVector2i()
        {
            return new Vector2i(this.Width, this.Height);
        }

        public override int GetHashCode()
        {
            return this.Width ^ this.Height;
        }

        public override string ToString()
        {
            return this.Width + ", " + this.Height;
        }

        public static implicit operator Size(SerializableSize size)
        {
            return new Size(size.Width, size.Height);
        }

        public static implicit operator SerializableSize(Size size)
        {
            return new SerializableSize(size.Width, size.Height);
        }

        public static readonly SerializableSize Empty = new SerializableSize(0, 0);
        public static readonly SerializableSize Invalid = new SerializableSize(0, 0) { _width = -1, _height = -1 };
    }

    #endregion

    #region SerializableSize

    [Serializable]
    public class SerializableSizeF : IEquatable<SerializableSizeF>, IEquatable<Size>
    {
        [SerializeField]
        private float _width;

        [SerializeField]
        private float _height;

        public float Width
        {
            get { return _width; }
        }

        public float Height
        {
            get { return _height; }
        }

        public float AspectRatio
        {
            get { return (float)this.Width / this.Height; }
        }

        public SerializableSizeF(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public override bool Equals(object obj)
        {
            if (obj is Size)
            {
                return this.Equals((Size)obj);
            }
            else if (obj is SerializableSizeF)
            {
                return this.Equals((SerializableSizeF)obj);
            }

            return base.Equals(obj);
        }

        public bool Equals(SerializableSizeF other)
        {
            if (other == null) return false;
            return _width == other.Width && _height == other.Height;
        }

        public bool Equals(Size other)
        {
            return _width == other.Width && _height == other.Height;
        }

        public Vector2 ToVector2i()
        {
            return new Vector2(this.Width, this.Height);
        }

        public override int GetHashCode()
        {
            return this.Width.GetHashCode() ^ this.Height.GetHashCode();
        }

        public override string ToString()
        {
            return this.Width + ", " + this.Height;
        }

        public static implicit operator SizeF(SerializableSizeF size)
        {
            return new SizeF(size.Width, size.Height);
        }

        public static implicit operator SerializableSizeF(Size size)
        {
            return new SerializableSizeF(size.Width, size.Height);
        }

        public static readonly SerializableSizeF Empty = new SerializableSizeF(0, 0);
        public static readonly SerializableSizeF Invalid = new SerializableSizeF(0, 0) { _width = -1, _height = -1 };
    }

    #endregion
}
