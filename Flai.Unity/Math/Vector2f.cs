using System.Collections.Generic;
using Flai.IO;
using System;
using System.IO;
using UnityEngine;

namespace Flai
{
    [Serializable]
    public struct Vector2f : IEquatable<Vector2f>, IEquatable<Vector2>, IBinarySerializable
    {
        public const float Epsilon = Vector2.kEpsilon; // just drop out "k" prefix

        public float X;
        public float Y;

        public float Length
        {
            get { return FlaiMath.Sqrt(this.X * this.X + this.Y * this.Y); }
        }

        public float LengthSquared
        {
            get { return this.X * this.X + this.Y * this.Y; }
        }

        public Vector2f FlippedAxis
        {
            get { return new Vector2f(this.Y, this.X); }
        }

        public Vector2f(float value)
        {
            this.X = value;
            this.Y = value;
        }

        public Vector2f(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2f(Vector2 vector2)
        {
            this.X = vector2.x;
            this.Y = vector2.y;
        }

        public Vector2f(Vector2i vector2)
        {
            this.X = vector2.X;
            this.Y = vector2.Y;
        }

        #region IEquatable Members

        public bool Equals(Vector2f other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public bool Equals(Vector2 other)
        {
            return this.X == other.x && this.Y == other.y;
        }

        #endregion

        #region Object Overrides

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Vector2f && Equals((Vector2f)obj);
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
            float length = this.Length;
            this.X /= length;
            this.Y /= length;
        }

        public void NormalizeOrZero()
        {
            float length = this.Length;
            this.X /= length;
            this.Y /= length;
            if (!this.X.IsValidNumber() || !this.Y.IsValidNumber())
            {
                this.X = 0;
                this.Y = 0;
            }
        }

        public Vector2f AsNormalized()
        {
            return Vector2f.Normalize(this);
        }

        public Vector2 ToVector2()
        {
            return this;
        }

        public Vector3 ToVector3()
        {
            return this;
        }

        public float GetAxis(Axis axis)
        {
            return (axis == Axis.Horizontal) ? this.X : this.Y;
        }

        public void SetAxis(Axis axis, float value)
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

        public static float DistanceSquared(Vector2f value1, Vector2f value2)
        {
            float x = value1.X - value2.X;
            float y = value1.Y - value2.Y;

            return (x * x) + (y * y);
        }

        public static float Distance(Vector2f value1, Vector2f value2)
        {
            return FlaiMath.Sqrt(Vector2f.DistanceSquared(value1, value2));
        }

        public static Vector2f Abs(Vector2f value)
        {
            return new Vector2f(FlaiMath.Abs(value.X), FlaiMath.Abs(value.Y));
        }

        public static Vector2f Min(Vector2f value1, Vector2f value2)
        {
            return new Vector2f(FlaiMath.Min(value1.X, value2.X), FlaiMath.Min(value1.Y, value2.Y));
        }

        public static Vector2f Min(Vector2f value1, Vector2f value2, Vector2f value3)
        {
            return new Vector2f(FlaiMath.Min(value1.X, value2.X, value3.X), FlaiMath.Min(value1.Y, value2.Y, value3.Y));
        }

        public static Vector2f Min(Vector2f value1, Vector2f value2, Vector2f value3, Vector2f value4)
        {
            return new Vector2f(FlaiMath.Min(value1.X, value2.X, value3.X, value4.X), FlaiMath.Min(value1.Y, value2.Y, value3.Y, value4.Y));
        }

        public static Vector2f Max(Vector2f value1, Vector2f value2)
        {
            return new Vector2f(FlaiMath.Max(value1.X, value2.X), FlaiMath.Max(value1.Y, value2.Y));
        }

        public static Vector2f Max(Vector2f value1, Vector2f value2, Vector2f value3)
        {
            return new Vector2f(FlaiMath.Max(value1.X, value2.X, value3.X), FlaiMath.Max(value1.Y, value2.Y, value3.Y));
        }

        public static Vector2f Max(Vector2f value1, Vector2f value2, Vector2f value3, Vector2f value4)
        {
            return new Vector2f(FlaiMath.Max(value1.X, value2.X, value3.X, value4.X), FlaiMath.Max(value1.Y, value2.Y, value3.Y, value4.Y));
        }

        public static Vector2f Clamp(Vector2f value, Vector2f min, Vector2f max)
        {
            return new Vector2f(FlaiMath.Clamp(value.X, min.X, max.X), FlaiMath.Clamp(value.Y, min.Y, max.Y));
        }

        public static Vector2f Normalize(Vector2f v)
        {
            float length = v.Length;
            v.X /= length;
            v.Y /= length;
            return v;
        }

        public static Vector2f NormalizeOrZero(Vector2f v)
        {
            float length = v.Length;
            v.X /= length;
            v.Y /= length;
            if (!v.X.IsValidNumber() || !v.Y.IsValidNumber())
            {
                v.X = 0;
                v.Y = 0;
            }

            return v;
        }

        public static Vector2f ClampLength(Vector2f vec, float max)
        {
            Ensure.True(max >= 0);
            if (vec.LengthSquared > max * max)
            {
                vec.Normalize();
                vec *= max;
            }

            return vec;
        }

        public static Vector2f ClampX(Vector2f vec, float min, float max)
        {
            Ensure.True(min <= max);
            vec.X = FlaiMath.Clamp(vec.X, min, max);
            return vec;
        }

        public static Vector2f ClampY(Vector2f vec, float min, float max)
        {
            Ensure.True(min <= max);
            vec.Y = FlaiMath.Clamp(vec.Y, min, max);
            return vec;
        }

        public static Vector2f ClampLength(Vector2f vec, float min, float max)
        {
            Ensure.True(max >= 0 && min >= 0 && min <= max);
            float lengthSquared = vec.LengthSquared;
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

        public static float Angle(Vector2f left, Vector2f right)
        {
            return Vector2.Angle(left, right); // meh
        }

        public static float Dot(Vector2f left, Vector2f right)
        {
            return Vector2.Dot(left, right); // meh
        }

        public static Vector2f MoveTowards(Vector2f current, Vector2f target, float maxDistanceDelta)
        {
            return Vector2.MoveTowards(current, target, maxDistanceDelta);
        }

        public static Vector2f Lerp(Vector2f current, Vector2f target, float amount)
        {
            return new Vector2f { X = FlaiMath.Lerp(current.X, target.X, amount), Y = FlaiMath.Lerp(current.Y, target.Y, amount) };
        }

        public static Vector2f SmoothStep(Vector2f current, Vector2f target, float amount)
        {
            return new Vector2f { X = FlaiMath.SmoothStep(current.X, target.X, amount), Y = FlaiMath.SmoothStep(current.Y, target.Y, amount) };
        }

        public static Vector2f Lerp(LerpType lerpType, Vector2f current, Vector2f target, float amount)
        {
            if (lerpType == LerpType.Lerp)
            {
                return Vector2f.Lerp(current, target, amount);
            }

            return Vector2f.SmoothStep(current, target, amount);
        }

        public static Vector2f Rotate(Vector2f point, float radians)
        {
            float cosRadians = FlaiMath.Cos(radians);
            float sinRadians = FlaiMath.Sin(radians);
            return new Vector2(
                point.X * cosRadians - point.Y * sinRadians,
                point.X * sinRadians + point.Y * cosRadians);
        }

        public static Vector2f Rotate(Vector2f point, float radians, Vector2f origin)
        {
            float cosRadians = (float)Math.Cos(radians);
            float sinRadians = (float)Math.Sin(radians);

            Vector2f translatedPoint = new Vector2f
            {
                X = point.X - origin.X,
                Y = point.Y - origin.Y,
            };

            return new Vector2f
            {
                X = translatedPoint.X * cosRadians - translatedPoint.Y * sinRadians + origin.X,
                Y = translatedPoint.X * sinRadians + translatedPoint.Y * cosRadians + origin.Y,
            };
        }

        #endregion

        #region Operators

        public static bool operator ==(Vector2f a, Vector2f b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2f a, Vector2f b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static bool operator ==(Vector2 a, Vector2f b)
        {
            return a.x == b.X && a.y == b.Y;
        }

        public static bool operator !=(Vector2 a, Vector2f b)
        {
            return !(a == b);
        }

        public static bool operator ==(Vector2f a, Vector2 b)
        {
            return a.X == b.x && a.Y == b.y;
        }

        public static bool operator !=(Vector2f a, Vector2 b)
        {
            return !(a == b);
        }

        public static Vector2f operator +(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2f operator -(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2f operator *(Vector2f a, Vector2f multiplier)
        {
            return new Vector2f(a.X * multiplier.X, a.Y * multiplier.Y);
        }

        public static Vector2f operator *(Vector2f a, int multiplier)
        {
            return new Vector2f(a.X * multiplier, a.Y * multiplier);
        }

        public static Vector2f operator *(Vector2f a, float multiplier)
        {
            return new Vector2f(a.X * multiplier, a.Y * multiplier);
        }

        public static Vector2f operator *(float multiplier, Vector2f a)
        {
            return new Vector2f(a.X * multiplier, a.Y * multiplier);
        }

        public static Vector2f operator /(Vector2f a, int divider)
        {
            return new Vector2f(a.X / divider, a.Y / divider);
        }

        public static Vector2f operator /(Vector2f a, float divider)
        {
            return new Vector2f(a.X / divider, a.Y / divider);
        }

        public static Vector2f operator /(Vector2f a, Vector2f divider)
        {
            return new Vector2f(a.X / divider.X, a.Y / divider.Y);
        }

        public static Vector2f operator -(Vector2f a)
        {
            return new Vector2f(-a.X, -a.Y);
        }

        #endregion

        #region Implicit/Explicit Operators

        public static implicit operator Vector2f(Vector2 v)
        {
            return new Vector2f { X = v.x, Y = v.y };
        }

        public static implicit operator Vector2(Vector2f v)
        {
            return new Vector2 { x = v.X, y = v.Y };
        }

        public static implicit operator Vector3(Vector2f v)
        {
            return new Vector3 { x = v.X, y = v.Y };
        }

        public static implicit operator Vector2f(Vector3 v)
        {
            return new Vector2f { X = v.x, Y = v.y };
        }

        public static implicit operator Vector2f(Vector2i v)
        {
            return new Vector2f { X = v.X, Y = v.Y };
        }

        public static explicit operator Vector2i(Vector2f v)
        {
            return new Vector2i { X = (int)v.X, Y = (int)v.Y };
        }

        #endregion

        #region "Constants"

        public static Vector2f Zero
        {
            get { return Vector2f.ZeroVector; }
        }

        public static Vector2f UnitX
        {
            get { return Vector2f.UnitXVector; }
        }

        public static Vector2f UnitY
        {
            get { return Vector2f.UnitYVector; }
        }

        public static Vector2f One
        {
            get { return Vector2f.OneVector; }
        }

        public static Vector2f MinValue
        {
            get { return Vector2f.MinVector; }
        }

        public static Vector2f MaxValue
        {
            get { return Vector2f.MaxVector; }
        }

        public static Vector2f Left
        {
            get { return Vector2f.LeftVector; }
        }

        public static Vector2f Right
        {
            get { return Vector2f.RightVector; }
        }

        public static Vector2f Up
        {
            get { return Vector2f.UpVector; }
        }

        public static Vector2f Down
        {
            get { return Vector2f.DownVector; }
        }

        private static readonly Vector2f ZeroVector = new Vector2f(0);
        private static readonly Vector2f UnitXVector = new Vector2f(1, 0);
        private static readonly Vector2f UnitYVector = new Vector2f(0, 1);
        private static readonly Vector2f OneVector = new Vector2f(1, 1);
        private static readonly Vector2f MinVector = new Vector2f(int.MinValue, int.MinValue);
        private static readonly Vector2f MaxVector = new Vector2f(int.MaxValue, int.MaxValue);
        private static readonly Vector2f LeftVector = new Vector2f(-1, 0);
        private static readonly Vector2f RightVector = new Vector2f(1, 0);
        private static readonly Vector2f UpVector = new Vector2f(0, 1);
        private static readonly Vector2f DownVector = new Vector2f(0, -1);

        #endregion

        public static bool Approximately(Vector2f v1, Vector2f v2)
        {
            return Mathf.Approximately(v1.X, v2.X) && Mathf.Approximately(v1.Y, v2.Y);
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

        private static readonly ApproximateComparerVector2f _approximateComparer = new ApproximateComparerVector2f();
        public static IEqualityComparer<Vector2f> ApproximateComparer
        {
            get { return _approximateComparer; }
        }

        #region ApproximateComparerVector2f

        private class ApproximateComparerVector2f : IEqualityComparer<Vector2f>
        {
            public bool Equals(Vector2f x, Vector2f y)
            {
                return Vector2f.Approximately(x, y);
            }

            public int GetHashCode(Vector2f obj)
            {
                // !!! This can give different value for very close values. for example 2.9999999999 vs 3.0000000001 etc
                return Vector2i.Round(obj).GetHashCode();
            }
        }

        #endregion
    }
}
