
using System;
using UnityEngine;

namespace Flai
{
    public struct Vector3i : IEquatable<Vector3i>
    {  
        public int X;
        public int Y;
        public int Z;

        public float LengthSquared
        {
            get {  return this.X * this.X + this.Y * this.Y + this.Z * this.Z; }
        }

        public float Length
        {
            get { return FlaiMath.Sqrt(this.LengthSquared); }
        }

        public Vector3i(int value)
        {
            this.X = value;
            this.Y = value;
            this.Z = value;
        }

        public Vector3i(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3i(Vector3 vector3)
        {
            this.X = (int)vector3.x;
            this.Y = (int)vector3.y;
            this.Z = (int)vector3.z;
        }

        public override int GetHashCode()
        {
            return (int)(this.X ^ this.Y ^ this.Z);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3i)
            {
                Vector3i other = (Vector3i)obj;
                return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
            }
            return base.Equals(obj);
        }

        public bool Equals(Vector3i other)
        {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
        }

        public override string ToString()
        {
            return ("Vector3i (" + this.X + "," + this.Y + "," + this.Z + ")");
        }

        #region Operators

        public static bool operator ==(Vector3i a, Vector3i b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Vector3i a, Vector3i b)
        {
            return !(a.X == b.X && a.Y == b.Y && a.Z == b.Z);
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
        {
            return new Vector3i { X = a.X + b.X, Y = a.Y + b.Y, Z = a.Z + b.Z };
        }

        public static Vector3i operator +(Vector3i a, Vector3b b)
        {
            return new Vector3i { X = a.X + b.X, Y = a.Y + b.Y, Z = a.Z + b.Z };
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return new Vector3i { X = a.X - b.X, Y = a.Y - b.Y, Z = a.Z - b.Z };
        }

        public static Vector3i operator -(Vector3i a, Vector3b b)
        {
            return new Vector3i { X = a.X - b.X, Y = a.Y - b.Y, Z = a.Z - b.Z };
        }

        public static Vector3i operator *(Vector3i a, Vector3i b)
        {
            return new Vector3i { X = a.X * b.X, Y = a.Y * b.Y, Z = a.Z * b.Z };
        }

        public static Vector3i operator *(Vector3i a, Vector3b b)
        {
            return new Vector3i { X = a.X * b.X, Y = a.Y * b.Y, Z = a.Z * b.Z };
        }

        public static Vector3i operator *(Vector3i a, int multiplier)
        {
            return new Vector3i { X = a.X * multiplier, Y = a.Y * multiplier, Z = a.Z * multiplier };
        }

        public static Vector3 operator *(Vector3i a, float multiplier)
        {
            return new Vector3 { x = a.X * multiplier, y = a.Y * multiplier, z = a.Z * multiplier };
        }

        public static Vector3 operator *(Vector3i a, Vector3 multiplier)
        {
            return new Vector3 { x = a.X * multiplier.x, y = a.Y * multiplier.y, z = a.Z * multiplier.z };
        }

        public static Vector3i operator /(Vector3i a, int divider)
        {
            return new Vector3i { X = a.X / divider, Y = a.Y / divider, Z = a.Z / divider };
        }

        public static Vector3 operator /(Vector3i a, float divider)
        {
            return new Vector3 { x = a.X / divider, y = a.Y / divider, z = a.Z / divider };
        }

        public static Vector3i operator -(Vector3i v)
        {
            return new Vector3i { X = -v.X, Y = -v.Y, Z = -v.Z };
        }

        public static implicit operator Vector3(Vector3i v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        #endregion

        #region Static Methods

        public static Vector3i Round(Vector3 vec)
        {
            return new Vector3i((int)FlaiMath.Round(vec.x), (int)FlaiMath.Round(vec.y), (int)FlaiMath.Round(vec.z));
        }

        public static Vector3i Floor(Vector3 vec)
        {
            return new Vector3i((int) FlaiMath.Floor(vec.x), (int) FlaiMath.Floor(vec.y), (int) FlaiMath.Floor(vec.z));
        }

        public static Vector3i Ceiling(Vector3 vec)
        {
            return new Vector3i((int)FlaiMath.Ceiling(vec.x), (int)FlaiMath.Ceiling(vec.y), (int)FlaiMath.Ceiling(vec.z));
        }

        public static int DistanceSquared(Vector3i value1, Vector3i value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;
            int z = value1.Z - value2.Z;

            return (x * x) + (y * y) + (z * z);
        }

        public static double Distance(Vector3i value1, Vector3i value2)
        {
            return Math.Sqrt(Vector3i.DistanceSquared(value1, value2));
        }

        public static Vector3i Abs(Vector3i value)
        {
            return new Vector3i { X = FlaiMath.Abs(value.X), Y = FlaiMath.Abs(value.Y), Z = FlaiMath.Abs(value.Z) };
        }

        #endregion

        public static Vector3i Zero
        {
            get { return Vector3i.ZeroVector; }
        }

        public static Vector3i UnitX
        {
            get { return Vector3i.UnitXVector; }
        }

        public static Vector3i UnitY
        {
            get { return Vector3i.UnitYVector; }
        }

        public static Vector3i UnitZ
        {
            get { return Vector3i.UnitZVector; }
        }

        public static Vector3i One
        {
            get { return Vector3i.OneVector; }
        }

        private static readonly Vector3i ZeroVector = new Vector3i(0, 0, 0);
        private static readonly Vector3i UnitXVector = new Vector3i(1, 0, 0);
        private static readonly Vector3i UnitYVector = new Vector3i(0, 1, 0);
        private static readonly Vector3i UnitZVector = new Vector3i(0, 1, 0);
        private static readonly Vector3i OneVector = new Vector3i(1, 1, 1);

        public Vector3 ToVector3()
        {
            return new Vector3 { x = this.X, y = this.Y, z = this.Z };
        }
    }
}
