using System.Collections.Generic;
using Flai.IO;
using System;
using System.IO;
using UnityEngine;

namespace Flai
{
    [Serializable]
    public struct Vector2d : IEquatable<Vector2d>, IBinarySerializable
    {
        public const double Epsilon = double.Epsilon; 

        public double X;
        public double Y;

        public double Length
        {
            get { return Math.Sqrt(this.X * this.X + this.Y * this.Y); }
        }

        public double LengthSquared
        {
            get { return this.X * this.X + this.Y * this.Y; }
        }

        public Vector2d FlippedAxis
        {
            get { return new Vector2d(this.Y, this.X); }
        }

        public Vector2d(double value)
        {
            this.X = value;
            this.Y = value;
        }

        public Vector2d(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2d(Vector2 vector2)
        {
            this.X = vector2.x;
            this.Y = vector2.y;
        }

        public Vector2d(Vector2f vector2)
        {
            this.X = vector2.X;
            this.Y = vector2.Y;
        }

        public Vector2d(Vector2i vector2)
        {
            this.X = vector2.X;
            this.Y = vector2.Y;
        }

        #region IEquatable Members

        public bool Equals(Vector2d other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        #endregion

        #region Object Overrides

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Vector2d && Equals((Vector2d)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.X.GetHashCode() * 397) ^ this.Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return this.ToString("0.00");
        }

        public string ToString(string floatFormat)
        {
            return this.X.ToString(floatFormat) + ", " + this.Y.ToString(floatFormat);
        }

        #endregion

        #region Non-Static Methods

        public void Normalize()
        {
            double length = this.Length;
            this.X /= length;
            this.Y /= length;
        }

        public void NormalizeOrZero()
        {
            double length = this.Length;
            this.X /= length;
            this.Y /= length;
            if (!this.X.IsValidNumber() || !this.Y.IsValidNumber())
            {
                this.X = 0;
                this.Y = 0;
            }
        }

        public Vector2d AsNormalized()
        {
            return Vector2d.Normalize(this);
        }

        public Vector2f ToVector2f()
        {
            return (Vector2f)this;
        }

        public double GetAxis(Axis axis)
        {
            return (axis == Axis.Horizontal) ? this.X : this.Y;
        }

        public void SetAxis(Axis axis, double value)
        {
            if (axis == Axis.Horizontal)
            {
                this.X = value;
            }
            else
            {
                this.Y = value;
            }
        }

        #endregion

        #region Static Methods

        public static double DistanceSquared(Vector2d value1, Vector2d value2)
        {
            double x = value1.X - value2.X;
            double y = value1.Y - value2.Y;

            return (x * x) + (y * y);
        }

        public static double Distance(Vector2d value1, Vector2d value2)
        {
            return Math.Sqrt(Vector2d.DistanceSquared(value1, value2));
        }

        public static Vector2d Abs(Vector2d value)
        {
            return new Vector2d(Math.Abs(value.X), Math.Abs(value.Y));
        }

        public static Vector2d Min(Vector2d value1, Vector2d value2)
        {
            return new Vector2d(Math.Min(value1.X, value2.X), Math.Min(value1.Y, value2.Y));
        }

        public static Vector2d Max(Vector2d value1, Vector2d value2)
        {
            return new Vector2d(Math.Max(value1.X, value2.X), Math.Max(value1.Y, value2.Y));
        }

        public static Vector2d Normalize(Vector2d v)
        {
            double length = v.Length;
            v.X /= length;
            v.Y /= length;
            return v;
        }

        public static Vector2d NormalizeOrZero(Vector2d v)
        {
            double length = v.Length;
            v.X /= length;
            v.Y /= length;
            if (!v.X.IsValidNumber() || !v.Y.IsValidNumber())
            {
                v.X = 0;
                v.Y = 0;
            }

            return v;
        }

        public static Vector2d ClampLength(Vector2d vec, double max)
        {
            Ensure.True(max >= 0);
            if (vec.LengthSquared > max * max)
            {
                vec.Normalize();
                vec *= max;
            }

            return vec;
        }

        public static Vector2d ClampX(Vector2d vec, double min, double max)
        {
            Ensure.True(min <= max);
            vec.X = FlaiMath.Clamp(vec.X, min, max);
            return vec;
        }

        public static Vector2d ClampY(Vector2d vec, double min, double max)
        {
            Ensure.True(min <= max);
            vec.Y = FlaiMath.Clamp(vec.Y, min, max);
            return vec;
        }

        public static Vector2d ClampLength(Vector2d vec, double min, double max)
        {
            Ensure.True(max >= 0 && min >= 0 && min <= max);
            double lengthSquared = vec.LengthSquared;
            if (lengthSquared > max * max)
            {
                vec.Normalize();
                vec *= max;
            }
            else if (lengthSquared < min * min)
            {
                vec.Normalize();
                vec *= min;
            }

            return vec;
        }

        public static Vector2d Lerp(Vector2d current, Vector2d target, double amount)
        {
            return new Vector2d { X = FlaiMath.Lerp(current.X, target.X, amount), Y = FlaiMath.Lerp(current.Y, target.Y, amount) };
        }

        public static Vector2d SmoothStep(Vector2d current, Vector2d target, double amount)
        {
            return new Vector2d { X = FlaiMath.SmoothStep(current.X, target.X, amount), Y = FlaiMath.SmoothStep(current.Y, target.Y, amount) };
        }

        public static Vector2d Lerp(LerpType lerpType, Vector2d current, Vector2d target, double amount)
        {
            if (lerpType == LerpType.Lerp)
            {
                return Vector2d.Lerp(current, target, amount);
            }

            return Vector2d.SmoothStep(current, target, amount);
        }

        public static Vector2d Rotate(Vector2d point, double radians)
        {
            double cosRadians = FlaiMath.Cos(radians);
            double sinRadians = FlaiMath.Sin(radians);
            return new Vector2d(
                point.X * cosRadians - point.Y * sinRadians,
                point.X * sinRadians + point.Y * cosRadians);
        }

        public static Vector2d Rotate(Vector2d point, double radians, Vector2d origin)
        {
            double cosRadians = Math.Cos(radians);
            double sinRadians = Math.Sin(radians);

            Vector2d translatedPoint = new Vector2d
            {
                X = point.X - origin.X,
                Y = point.Y - origin.Y,
            };

            return new Vector2d
            {
                X = translatedPoint.X * cosRadians - translatedPoint.Y * sinRadians + origin.X,
                Y = translatedPoint.X * sinRadians + translatedPoint.Y * cosRadians + origin.Y,
            };
        }

        #endregion

        #region Operators

        public static bool operator ==(Vector2d a, Vector2d b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2d a, Vector2d b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static bool operator ==(Vector2d a, Vector2f b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2d a, Vector2f b)
        {
            return !(a == b);
        }

        public static bool operator ==(Vector2f a, Vector2d b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2f a, Vector2d b)
        {
            return !(a == b);
        }

        public static Vector2d operator +(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2d operator -(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2d operator *(Vector2d a, Vector2d multiplier)
        {
            return new Vector2d(a.X * multiplier.X, a.Y * multiplier.Y);
        }

        public static Vector2d operator *(Vector2d a, int multiplier)
        {
            return new Vector2d(a.X * multiplier, a.Y * multiplier);
        }

        public static Vector2d operator *(Vector2d a, float multiplier)
        {
            return new Vector2d(a.X * multiplier, a.Y * multiplier);
        }

        public static Vector2d operator *(Vector2d a, double multiplier)
        {
            return new Vector2d(a.X * multiplier, a.Y * multiplier);
        }

        public static Vector2d operator *(float multiplier, Vector2d a)
        {
            return new Vector2d(a.X * multiplier, a.Y * multiplier);
        }

        public static Vector2d operator /(Vector2d a, int divider)
        {
            return new Vector2d(a.X / divider, a.Y / divider);
        }

        public static Vector2d operator /(Vector2d a, float divider)
        {
            return new Vector2d(a.X / divider, a.Y / divider);
        }

        public static Vector2d operator /(Vector2d a, Vector2d divider)
        {
            return new Vector2d(a.X / divider.X, a.Y / divider.Y);
        }

        public static Vector2d operator -(Vector2d a)
        {
            return new Vector2d(-a.X, -a.Y);
        }

        #endregion

        #region Implicit/Explicit Operators

        public static implicit operator Vector2d(Vector2f v)
        {
            return new Vector2d { X = v.X, Y = v.Y };
        }

        public static implicit operator Vector2d(Vector2 v)
        {
            return new Vector2d { X = v.x, Y = v.y };
        }

        public static implicit operator Vector2d(Vector2i v)
        {
            return new Vector2d { X = v.X, Y = v.Y };
        }

        public static explicit operator Vector2i(Vector2d v)
        {
            return new Vector2i { X = (int)v.X, Y = (int)v.Y };
        }

        public static explicit operator Vector2f(Vector2d v)
        {
            return new Vector2f { X = (float)v.X, Y = (float)v.Y };
        }

        #endregion

        #region "Constants"

        public static Vector2d Zero
        {
            get { return Vector2d.ZeroVector; }
        }

        public static Vector2d UnitX
        {
            get { return Vector2d.UnitXVector; }
        }

        public static Vector2d UnitY
        {
            get { return Vector2d.UnitYVector; }
        }

        public static Vector2d One
        {
            get { return Vector2d.OneVector; }
        }

        public static Vector2d MinValue
        {
            get { return Vector2d.MinVector; }
        }

        public static Vector2d MaxValue
        {
            get { return Vector2d.MaxVector; }
        }

        public static Vector2d Left
        {
            get { return Vector2d.LeftVector; }
        }

        public static Vector2d Right
        {
            get { return Vector2d.RightVector; }
        }

        public static Vector2d Up
        {
            get { return Vector2d.UpVector; }
        }

        public static Vector2d Down
        {
            get { return Vector2d.DownVector; }
        }

        private static readonly Vector2d ZeroVector = new Vector2d(0);
        private static readonly Vector2d UnitXVector = new Vector2d(1, 0);
        private static readonly Vector2d UnitYVector = new Vector2d(0, 1);
        private static readonly Vector2d OneVector = new Vector2d(1, 1);
        private static readonly Vector2d MinVector = new Vector2d(double.MinValue, double.MinValue);
        private static readonly Vector2d MaxVector = new Vector2d(double.MaxValue, double.MaxValue);
        private static readonly Vector2d LeftVector = new Vector2d(-1, 0);
        private static readonly Vector2d RightVector = new Vector2d(1, 0);
        private static readonly Vector2d UpVector = new Vector2d(0, 1);
        private static readonly Vector2d DownVector = new Vector2d(0, -1);

        #endregion

        public static bool Approximately(Vector2d v1, Vector2d v2)
        {
            return Mathf.Approximately((float)v1.X, (float)v2.X) && Mathf.Approximately((float)v1.Y, (float)v2.Y);
        }

        #region IBinarySerializable Members

        void IBinarySerializable.Write(BinaryWriter writer)
        {
            writer.Write(this.X);
            writer.Write(this.Y);
        }

        void IBinarySerializable.Read(BinaryReader reader)
        {
            this.X = reader.ReadInt32();
            this.Y = reader.ReadInt32();
        }

        #endregion

        private static readonly ApproximateComparerVector2d _approximateComparer = new ApproximateComparerVector2d();
        public static IEqualityComparer<Vector2d> ApproximateComparer
        {
            get { return _approximateComparer; }
        }

        #region ApproximateComparerVector2f

        private class ApproximateComparerVector2d : IEqualityComparer<Vector2d>
        {
            public bool Equals(Vector2d x, Vector2d y)
            {
                return Vector2d.Approximately(x, y);
            }

            public int GetHashCode(Vector2d obj)
            {
                // !!! This can give different value for very close values. for example 2.9999999999 vs 3.0000000001 etc
                return Vector2i.Round(obj.ToVector2f()).GetHashCode();
            }
        }

        #endregion
    }
}
