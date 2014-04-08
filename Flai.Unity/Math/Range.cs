
using Flai.Diagnostics;
using System;
using UnityEngine;

namespace Flai
{
    public interface IRange<T>
    {
        T Min { get; }
        T Max { get; }

        bool Contains(T value);
        bool Intersects<TRange>(TRange other) where TRange : IRange<T>;
    }

    #region Range (float)

    public struct Range : IRange<float>, IEquatable<Range>
    {
        public static Range Zero = new Range(0, 0);
        public static Range FullRotation = new Range(-FlaiMath.Pi, FlaiMath.Pi); // meh name

        private readonly float _min;
        private readonly float _max;

        public float Min
        {
            get { return _min; }
        }

        public float Max
        {
            get { return _max; }
        }

        public float Average
        {
            get { return (_max + _min) / 2f; }
        }

        // Naming?
        public float Length
        {
            get { return _max - _min; }
        }

        public Range(float min, float max)
        {
            Ensure.IsValid(min);
            Ensure.IsValid(max);
            Ensure.True(max >= min, "Maximum can't be smaller than minimium");

            _min = min;
            _max = max;
        }

        public Range AsInflated(float amount)
        {
            return new Range(this.Min - amount, this.Max + amount * 2);
        }

        public bool Contains(float value)
        {
            return value >= _min && value <= _max;
        }

        public bool Intersects<TRange>(TRange other)
            where TRange : IRange<float>
        {
            return (other.Min < _max && _max < other.Max) || (other.Min < _min && _min < other.Max) || (_min < other.Min && other.Min < _max) || (_min == other.Min && _max == other.Max);
        }

        public bool Intersects(Range other)
        {
            return (other.Min < _max && _max < other.Max) || (other.Min < _min && _min < other.Max) || (_min < other.Min && other.Min < _max) || (_min == other.Min && _max == other.Max);
        }

        public bool Intersects(RangeInt other)
        {
            return (other.Min < _max && _max < other.Max) || (other.Min < _min && _min < other.Max) || (_min < other.Min && other.Min < _max) || (_min == other.Min && _max == other.Max);
        }

        public static implicit operator Range(float value)
        {
            return new Range(value, value);
        }

        public static Range CreateCentered(float center, float length)
        {
            Ensure.IsValid(center);
            Ensure.IsValid(length);
            Ensure.True(length > 0);

            return new Range(center - length * 0.5f, center + length * 0.5f);
        }

        #region IEquatable<Range> Members

        public bool Equals(Range other)
        {
            return _min == other.Min && _max == other.Max;
        }

        #endregion
    }

    #endregion

    #region Range<T>

    public struct Range<T> : IRange<T>, IEquatable<Range<T>>
         where T : IComparable<T>
    {
        private readonly T _min;
        private readonly T _max;

        public T Min
        {
            get { return _min; }
        }

        public T Max
        {
            get { return _max; }
        }

        public Range(T min, T max)
        {
            Ensure.True(max.IsGreaterThanOrEqual(min), "Maximum can't be smaller than minimium");

            _min = min;
            _max = max;
        }

        public bool Contains(T value)
        {
            return value.IsGreaterThanOrEqual(_min) && value.IsLessThanOrEqual(_max);
        }

        public bool Intersects<TRange>(TRange other)
            where TRange : IRange<T>
        {
            return (other.Min.IsLessThan(_max) && _max.IsLessThan(other.Max)) || (other.Min.IsLessThan(_min) && _min.IsLessThan(other.Max)) || (_min.IsLessThan(other.Min) && other.Min.IsLessThan(_max)) || (_min.IsEqual(other.Min) && _max.IsEqual(other.Max));
        }

        public bool Intersects(Range<T> other)
        {
            return (other.Min.IsLessThan(_max) && _max.IsLessThan(other.Max)) || (other.Min.IsLessThan(_min) && _min.IsLessThan(other.Max)) || (_min.IsLessThan(other.Min) && other.Min.IsLessThan(_max)) || (_min.IsEqual(other.Min) && _max.IsEqual(other.Max));
        }

        public static implicit operator Range<T>(T value)
        {
            return new Range<T>(value, value);
        }

        #region IEquatable<Range> Members

        public bool Equals(Range<T> other)
        {
            return _min.IsEqual(other.Min) && _max.IsEqual(other.Max); // don't use Equals.. or should it?
        }

        #endregion
    }

    #endregion
}
