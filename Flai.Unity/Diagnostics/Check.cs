
using System;
using UnityEngine;

namespace Flai.Diagnostics
{
    public static class Check
    {
        public static bool IsPlaying
        {
            get { return Application.isPlaying; }
        }

        public static bool IsEditor
        {
            get { return Application.isEditor; }
        }

        public static bool IsValid(float value)
        {
            return value.IsValidNumber();
        }

        public static bool IsValid(Vector2 value)
        {
            return value.x.IsValidNumber() && value.y.IsValidNumber();
        }

        public static bool IsValid(double value)
        {
            return value.IsValidNumber();
        }

        public static bool IsValid(Ray2D ray)
        {
            return Check.IsValid(ray.origin) && Check.IsValid(ray.direction);
        }

        public static bool IsValid(Range value)
        {
            return Check.IsValid(value.Min) && Check.IsValid(value.Max);
        }

        public static bool IsValidPath(string path)
        {
            return !(path == null || string.IsNullOrEmpty(path.Trim())) && Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute);
        }

        public static bool WithinRange<T>(T value, T min, T max)
            where T : IComparable<T>
        {
            return value.IsGreaterThanOrEqual(min) && value.IsLessThanOrEqual(max);
        }

        public static bool WithinRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        public static bool WithinRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        public static bool WithinRange<T, TRange>(T value, TRange range)
            where TRange : IRange<T>
        {
            return range.Contains(value);
        }

        public static bool Is<T>(object value)
            where T : class
        {
            return (value as T) != null;
        }
    }
}
