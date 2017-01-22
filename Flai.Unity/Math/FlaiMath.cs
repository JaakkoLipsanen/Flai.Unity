
using System;
using System.Linq;
using Flai.Diagnostics;
using UnityEngine;

namespace Flai
{
    public static class FlaiMath
    {
        public const float E = 2.71828175f;
        public const float Log2E = 1.442695f;
        public const float Log10E = 0.4342945f ;
        public const float Pi = 3.14159274f;
        public const float TwoPi = 6.28318548f;
        public const float PiOver2 = 1.57079637f;
        public const float PiOver4 = 0.7853982f;

        #region Scale

        public static float Scale(float input, Range oldRange, Range newRange)
        {
            return newRange.Min + ((newRange.Max - newRange.Min) * (input - oldRange.Min)) /
                (oldRange.Max - oldRange.Min);
        }

        public static float Scale(float input, float oldMin, float oldMax, float newMin, float newMax)
        {
            return newMin + ((newMax - newMin) * (input - oldMin)) /
                (oldMax - oldMin);
        }

        public static void Scale(float[] input, Range oldRange, Range newRange)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = newRange.Min + ((newRange.Max - newRange.Min) * (input[i] - oldRange.Min)) /
                    (oldRange.Max - oldRange.Min);
            }
        }

        public static void Scale(float[] input, float oldMin, float oldMax, float newMin, float newMax)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = newMin + ((newMax - newMin) * (input[i] - oldMin)) /
                    (oldMax - oldMin);
            }
        }


        public static double Scale(double input, double oldMin, double oldMax, double newMin, double newMax)
        {
            return newMin + ((newMax - newMin) * (input - oldMin)) /
                (oldMax - oldMin);
        }

        #endregion

        #region Vector to angle and angle to unit-vector

        public static float GetAngle(Vector2f vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static float GetAngleDeg(Vector2f vector)
        {
            return FlaiMath.ToDegrees(FlaiMath.GetAngle(vector));
        }

        public static Vector2f GetAngleVector(float radians)
        {
            Vector2f result = new Vector2f((float) Math.Cos(radians), (float) Math.Sin(radians));
            result.Normalize();

            return result;
        }

        public static Vector2d GetAngleVector(double radians)
        {
            Vector2d result = new Vector2d(Math.Cos(radians), Math.Sin(radians));
            result.Normalize();

            return result;
        }

        public static Vector2f GetAngleVectorDeg(float degrees)
        {
            return FlaiMath.GetAngleVector(FlaiMath.ToRadians(degrees));
        }

        public static Vector2d GetAngleVectorDeg(double degrees)
        {
            return FlaiMath.GetAngleVector(FlaiMath.ToRadians(degrees));
        }

        #endregion

        #region GetSign

        public static int Sign(int x)
        {
            if (x > 0)
            {
                return 1;
            }
            else if (x < 0)
            {
                return -1;
            }

            return 0;
        }

        public static int Sign(float x)
        {
            if (x > 0)
            {
                return 1;
            }
            else if (x < 0)
            {
                return -1;
            }

            return 0;
        }

        public static Sign GetSign(sbyte x)
        {
            if (x > 0)
            {
                return Flai.Sign.Positive;
            }
            else if (x < 0)
            {
                return Flai.Sign.Negative;
            }

            return Flai.Sign.None;
        }

        public static Sign GetSign(short x)
        {
            if (x > 0)
            {
                return Flai.Sign.Positive;
            }
            else if (x < 0)
            {
                return Flai.Sign.Negative;
            }

            return Flai.Sign.None;
        }

        public static Sign GetSign(int x)
        {
            if (x > 0)
            {
                return Flai.Sign.Positive;
            }
            else if (x < 0)
            {
                return Flai.Sign.Negative;
            }

            return Flai.Sign.None;
        }

        public static Sign GetSign(long x)
        {
            if (x > 0)
            {
                return Flai.Sign.Positive;
            }
            else if (x < 0)
            {
                return Flai.Sign.Negative;
            }

            return Flai.Sign.None;
        }

        public static Sign GetSign(float x)
        {
            if (x > 0)
            {
                return Flai.Sign.Positive;
            }
            else if (x < 0)
            {
                return Flai.Sign.Negative;
            }

            return Flai.Sign.None;
        }

        public static Sign GetSign(double x)
        {
            if (x > 0)
            {
                return Flai.Sign.Positive;
            }
            else if (x < 0)
            {
                return Flai.Sign.Negative;
            }

            return Flai.Sign.None;
        }

        public static Sign GetSign(decimal x)
        {
            if (x > 0)
            {
                return Flai.Sign.Positive;
            }
            else if (x < 0)
            {
                return Flai.Sign.Negative;
            }

            return Flai.Sign.None;
        }

        #endregion

        #region Distance

        public static float Distance(float x, float y)
        {
            return Math.Abs(x - y);
        }

        public static int Distance(int x, int y)
        {
            return Math.Abs(x - y);
        }

        public static double Distance(double x, double y)
        {
            return Math.Abs(x - y);
        }

        // radians
        public static float AbsAngleDistance(float angle1, float angle2)
        {
            return FlaiMath.Min((2 * FlaiMath.Pi) - FlaiMath.Abs(angle1 - angle2), FlaiMath.Abs(angle1 - angle2));
        }

        // radians
        public static float ShortestAngleDistance(float angle1, float angle2)
        {
            angle1 = FlaiMath.WrapAngle(angle1);
            angle2 = FlaiMath.WrapAngle(angle2);

            if (angle1 <= angle2)
            {
                return (angle2 - angle1) <= FlaiMath.Pi ? (angle2 - angle1) : (-angle1 - (FlaiMath.TwoPi - angle2));
            }
            else // angle1 > angle2
            {
                return (angle1 - angle2) <= FlaiMath.Pi ? -(angle1 - angle2) : (angle2 + (FlaiMath.TwoPi - angle1));
            }
        }

        // deg
        public static float ShortestAngleDistanceDeg(float angle1, float angle2)
        {
            return FlaiMath.ToDegrees(FlaiMath.ShortestAngleDistance(FlaiMath.ToRadians(angle1), FlaiMath.ToRadians(angle2)));
        }

        #endregion

        #region Clamp

        public static T Clamp<T>(T value, T min, T max)
            where T : IComparable<T>
        {
            if (value.IsLessThan(min))
            {
                return min;
            }
            else if (value.IsGreaterThan(max))
            {
                return max;
            }

            return value;
        }

        public static T Clamp<T>(T value, IRange<T> range)
            where T : IComparable<T>
        {
            if (value.IsLessThan(range.Min))
            {
                return range.Min;
            }
            else if (value.IsGreaterThan(range.Max))
            {
                return range.Max;
            }

            return value;
        }

        #endregion

        #region Real Modulus

        /// <summary>
        /// This is real modulus function. In C#, % isn't actually modulus function but remainder function
        /// </summary>
        public static int RealModulus(int a, int b)
        {
            return (int)(a - b * Math.Floor((float)a / b));
        }

        /// <summary>
        /// This is real modulus function. In C#, % isn't actually modulus function but remainder function
        /// </summary>
        public static float RealModulus(float a, float b)
        {
            return (float)(a - b * Math.Floor(a / b));
        }

        /// <summary>
        /// This is real modulus function. In C#, % isn't actually modulus function but remainder function
        /// </summary>
        public static double RealModulus(double a, double b)
        {
            return a - b * Math.Floor(a / b);
        }

        #endregion

        #region ToRadians and ToDegrees

        public static float ToRadians(float degrees)
        {
            return degrees * 0.0174532924f;
        }

        public static double ToRadians(double degrees)
        {
            return degrees * 0.0174532925199432957692369076848861271344287188854172d;
        }

        public static float ToDegrees(float radians)
        {
            return radians * 57.2957764f; // MathHelper.ToDegrees
        }

        public static double ToDegrees(double radians)
        {
            return radians * 57.295779513082320876798154814105170332405472466564321d;
        }

        #endregion

        #region Lerp and SmoothStep

        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static float SmoothStep(float value1, float value2, float amount)
        {
            float num = FlaiMath.Clamp(amount, 0f, 1f);
            return FlaiMath.Lerp(value1, value2, num * num * (3f - 2f * num));
        }

        public static double SmoothStep(double value1, double value2, double amount)
        {
            double num = FlaiMath.Clamp(amount, 0f, 1f);
            return FlaiMath.Lerp(value1, value2, num * num * (3f - 2f * num));
        }

        public static float Lerp(LerpType lerpType, float value1, float value2, float amount)
        {
            if (lerpType == LerpType.Lerp)
            {
                return FlaiMath.Lerp(value1, value2, amount);
            }
            else if (lerpType == LerpType.Instant)
            {
                return value2;
            }

            return FlaiMath.SmoothStep(value1, value2, amount);
        }

        // deg
        public static float AngleLerp(float angle1, float angle2, float amount)
        {
            return angle1 + FlaiMath.ShortestAngleDistanceDeg(angle1, angle2) * amount;
        }

        // deg
        public static float AngleSmoothstep(float angle1, float angle2, float amount)
        {
            amount = amount * amount * (3f - 2f * amount);
            return angle1 + FlaiMath.ShortestAngleDistanceDeg(angle1, angle2) * amount;
        }

        #endregion

        #region WrapAngle

        public static float WrapAngle(float angle)
        {
            angle = (float)Math.IEEERemainder((double)angle, 6.2831854820251465);
            if (angle <= -3.14159274f)
            {
                angle += 6.28318548f;
            }
            else
            {
                if (angle > 3.14159274f)
                {
                    angle -= 6.28318548f;
                }
            }
            return angle;
        }

        public static float WrapAngleDeg(float angle)
        {
            return FlaiMath.ToDegrees(FlaiMath.WrapAngle(FlaiMath.ToRadians(angle)));
        }

        public static double WrapAngle(double angle)
        {
            angle = Math.IEEERemainder(angle, 6.2831854820251465);
            if (angle <= -3.14159274f)
            {
                angle += 6.28318548f;
            }
            else
            {
                if (angle > 3.14159274f)
                {
                    angle -= 6.28318548f;
                }
            }
            return angle;
        }

        #endregion

        #region Min & Max functions

        /* Not implemented for all types */

        // Byte

        // This method only exists for consistency, so you can *always* call
        // FlaiMath.Max instead of alternating between MoreMath.Max and Math.Max
        // depending on your argument count.
        public static byte Max(byte x, byte y)
        {
            return Math.Max(x, y);
        }

        public static byte Max(byte x, byte y, byte z)
        {
            // Or inline it as x < y ? (y < z ? z : y) : (x < z ? z : x);
            // Time it before micro-optimizing though!
            return Math.Max(x, Math.Max(y, z));
        }

        public static byte Max(byte x, byte y, byte z, byte w)
        {
            // Or inline it as x < y ? (y < z ? z : y) : (x < z ? z : x);
            // Time it before micro-optimizing though!
            return Math.Max(w, Math.Max(x, Math.Max(y, z)));
        }

        public static byte Max(params byte[] values)
        {
            return Enumerable.Max(values);
        }

        // This method only exists for consistency, so you can *always* call
        // FlaiMath.Min instead of alternating between MoreMath.Min and Math.Min
        // depending on your argument count.
        public static byte Min(byte x, byte y)
        {
            return Math.Min(x, y);
        }

        public static byte Min(byte x, byte y, byte z)
        {
            return Math.Min(x, Math.Min(y, z));
        }

        public static byte Min(byte x, byte y, byte z, byte w)
        {
            return Math.Min(w, Math.Min(x, Math.Min(y, z)));
        }

        public static byte Min(params byte[] values)
        {
            return Enumerable.Min(values);
        }

        // Int

        // This method only exists for consistency, so you can *always* call
        // FlaiMath.Max instead of alternating between MoreMath.Max and Math.Max
        // depending on your argument count.
        public static int Max(int x, int y)
        {
            return Math.Max(x, y);
        }

        public static int Max(int x, int y, int z)
        {
            // Or inline it as x < y ? (y < z ? z : y) : (x < z ? z : x);
            // Time it before micro-optimizing though!
            return Math.Max(x, Math.Max(y, z));
        }

        public static int Max(int x, int y, int z, int w)
        {
            // Or inline it as x < y ? (y < z ? z : y) : (x < z ? z : x);
            // Time it before micro-optimizing though!
            return Math.Max(w, Math.Max(x, Math.Max(y, z)));
        }

        public static int Max(params int[] values)
        {
            return Enumerable.Max(values);
        }

        // This method only exists for consistency, so you can *always* call
        // FlaiMath.Min instead of alternating between MoreMath.Min and Math.Min
        // depending on your argument count.
        public static int Min(int x, int y)
        {
            return Math.Min(x, y);
        }

        public static int Min(int x, int y, int z)
        {
            return Math.Min(x, Math.Min(y, z));
        }

        public static int Min(int x, int y, int z, int w)
        {
            return Math.Min(w, Math.Min(x, Math.Min(y, z)));
        }

        public static int Min(params int[] values)
        {
            return Enumerable.Min(values);
        }

        // Float

        // This method only exists for consistency, so you can *always* call
        // FlaiMath.Max instead of alternating between MoreMath.Max and Math.Max
        // depending on your argument count.
        public static float Max(float x, float y)
        {
            return Math.Max(x, y);
        }

        public static float Max(float x, float y, float z)
        {
            // Or inline it as x < y ? (y < z ? z : y) : (x < z ? z : x);
            // Time it before micro-optimizing though!
            return Math.Max(x, Math.Max(y, z));
        }

        public static float Max(float x, float y, float z, float w)
        {
            // Or inline it as x < y ? (y < z ? z : y) : (x < z ? z : x);
            // Time it before micro-optimizing though!
            return Math.Max(w, Math.Max(x, Math.Max(y, z)));
        }

        public static float Max(params float[] values)
        {
            return Enumerable.Max(values);
        }

        // This method only exists for consistency, so you can *always* call
        // FlaiMath.Min instead of alternating between MoreMath.Min and Math.Min
        // depending on your argument count.
        public static float Min(float x, float y)
        {
            return Math.Min(x, y);
        }

        public static float Min(float x, float y, float z)
        {
            return Math.Min(x, Math.Min(y, z));
        }

        public static float Min(float x, float y, float z, float w)
        {
            return Math.Min(w, Math.Min(x, Math.Min(y, z)));
        }

        public static float Min(params float[] values)
        {
            return Enumerable.Min(values);
        }

        // Double

        // This method only exists for consistency, so you can *always* call
        // FlaiMath.Max instead of alternating between MoreMath.Max and Math.Max
        // depending on your argument count.
        public static double Max(double x, double y)
        {
            return Math.Max(x, y);
        }

        public static double Max(double x, double y, double z)
        {
            // Or inline it as x < y ? (y < z ? z : y) : (x < z ? z : x);
            // Time it before micro-optimizing though!
            return Math.Max(x, Math.Max(y, z));
        }

        public static double Max(double x, double y, double z, double w)
        {
            // Or inline it as x < y ? (y < z ? z : y) : (x < z ? z : x);
            // Time it before micro-optimizing though!
            return Math.Max(w, Math.Max(x, Math.Max(y, z)));
        }

        public static double Max(params double[] values)
        {
            return Enumerable.Max(values);
        }

        // This method only exists for consistency, so you can *always* call
        // FlaiMath.Min instead of alternating between MoreMath.Min and Math.Min
        // depending on your argument count.
        public static double Min(double x, double y)
        {
            return Math.Min(x, y);
        }

        public static double Min(double x, double y, double z)
        {
            return Math.Min(x, Math.Min(y, z));
        }

        public static double Min(double x, double y, double z, double w)
        {
            return Math.Min(w, Math.Min(x, Math.Min(y, z)));
        }

        public static double Min(params double[] values)
        {
            return Enumerable.Min(values);
        }

        #endregion

        #region Trigonometry

        #region Cos/Sin/Tan

        public static float Cos(float input)
        {
            return (float)Math.Cos(input);
        }

        public static double Cos(double input)
        {
            return Math.Cos(input);
        }

        public static float Sin(float input)
        {
            return (float)Math.Sin(input);
        }

        public static double Sin(double input)
        {
            return Math.Sin(input);
        }

        public static float Tan(float input)
        {
            return (float)Math.Tan(input);
        }

        public static double Tan(double input)
        {
            return Math.Tan(input);
        }

        #endregion

        #region Cosh/Sinh/Tanh

        public static float Cosh(float input)
        {
            return (float)Math.Cosh(input);
        }

        public static double Cosh(double input)
        {
            return Math.Cosh(input);
        }

        public static float Sinh(float input)
        {
            return (float)Math.Sinh(input);
        }

        public static double Sinh(double input)
        {
            return Math.Sinh(input);
        }

        public static float Tanh(float input)
        {
            return (float)Math.Tanh(input);
        }

        public static double Tanh(double input)
        {
            return Math.Tanh(input);
        }

        #endregion

        #region Acos/Asin/Atan/Atan2

        public static float Acos(float input)
        {
            return (float)Math.Acos(input);
        }

        public static double Acos(double input)
        {
            return Math.Acos(input);
        }

        public static float Asin(float input)
        {
            return (float)Math.Asin(input);
        }

        public static double Asin(double input)
        {
            return Math.Asin(input);
        }

        public static float Atan(float input)
        {
            return (float)Math.Atan(input);
        }

        public static double Atan(double input)
        {
            return Math.Atan(input);
        }

        public static float Atan2(float input1, float input2)
        {
            return (float)Math.Atan2(input1, input2);
        }

        public static float Atan2(Vector2 v)
        {
            return (float)Math.Atan2(v.y, v.x);
        }

        public static double Atan2(double input1, double input2)
        {
            return Math.Atan2(input1, input2);
        }

        #endregion

        #endregion

        #region Round

        public static float Round(float value)
        {
            return (float)Math.Round(value);
        }

        public static float Round(float value, int digits)
        {
            return (float)Math.Round(value, digits);
        }

#if WINDOWS
        public static float Round(float value, MidpointRounding mode)
        {
            return (float)Math.Round(value, 0, mode);
        }

        public static float Round(float value, int digits, MidpointRounding mode)
        {
            return (float)Math.Round(value, digits, mode);
        }

      
#endif
        public static double Round(double value)
        {
            return Math.Round(value);
        }

        public static double Round(double value, int digits)
        {
            return Math.Round(value, digits);
        }

#if WINDOWS
        public static double Round(double value, MidpointRounding mode)
        {
            return Math.Round(value, 0, mode);
        }

        public static double Round(double value, int digits, MidpointRounding mode)
        {
            return Math.Round(value, digits, mode);
        }

#endif

        public static decimal Round(decimal d)
        {
            return decimal.Round(d, 0);
        }

        public static decimal Round(decimal d, int decimals)
        {
            return decimal.Round(d, decimals);
        }


#if WINDOWS
        public static decimal Round(decimal d, MidpointRounding mode)
        {
            return decimal.Round(d, 0, mode);
        }

        public static decimal Round(decimal d, int decimals, MidpointRounding mode)
        {
            return decimal.Round(d, decimals, mode);
        }

#endif

        #endregion

        #region Truncate

#if WINDOWS
        public static float Truncate(float d)
        {
            return (float)Math.Truncate(d);
        }

        public static double Truncate(double d)
        {
            return Math.Truncate(d);
        }
#endif

        public static decimal Truncate(decimal d)
        {
            return decimal.Truncate(d);
        }

        #endregion

        #region Sqrt

        public static float Sqrt(float d)
        {
            return (float)Math.Sqrt(d);
        }

        public static double Sqrt(double d)
        {
            return Math.Sqrt(d);
        }

        #endregion

        #region Pow

        public static int Pow(int x, int y)
        {
            // Not sure if this is precise due to floating point errors? could z.99999 be converted to z?
            return (int)Math.Pow(x, y);
        }

        public static float Pow(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }

        public static double Pow(double x, double y)
        {
            return Math.Pow(x, y);
        }

        #endregion

        #region Log

        public static float Log(float d)
        {
            return (float)Math.Log(d);
        }

        public static double Log(double d)
        {
            return Math.Log(d);
        }

        public static float Log10(float d)
        {
            return (float)Math.Log10(d);
        }

        public static double Log10(double d)
        {
            return Math.Log10(d);
        }

        public static float Log(float a, float newBase)
        {
            return (float)Math.Log(a, newBase);
        }

        public static double Log(double a, double newBase)
        {
            if (double.IsNaN(a))
            {
                return a;
            }
            if (double.IsNaN(newBase))
            {
                return newBase;
            }
            if (newBase == 1.0)
            {
                return double.NaN;
            }
            if (a != 1.0 && (newBase == 0.0 || double.IsPositiveInfinity(newBase)))
            {
                return double.NaN;
            }
            return Math.Log(a) / Math.Log(newBase);
        }

        #endregion

        #region Exp

        public static float Exp(float d)
        {
            return (float)Math.Exp(d);
        }

        public static double Exp(double d)
        {
            return Math.Exp(d);
        }

        #endregion

        #region IEERemainder

        public static float IEEERemainder(float x, float y)
        {
            return (float)Math.IEEERemainder(x, y);
        }

        public static double IEEERemainder(double x, double y)
        {
            return Math.IEEERemainder(x, y);
        }

        #endregion

        #region Abs

        public static sbyte Abs(sbyte input)
        {
            return Math.Abs(input);
        }

        public static short Abs(short input)
        {
            return Math.Abs(input);
        }

        public static int Abs(int input)
        {
            return Math.Abs(input);
        }

        public static long Abs(long input)
        {
            return Math.Abs(input);
        }

        public static float Abs(float input)
        {
            return Math.Abs(input);
        }

        public static double Abs(double input)
        {
            return Math.Abs(input);
        }

        public static decimal Abs(decimal input)
        {
            return Math.Abs(input);
        }

        #endregion

        #region Floor and Ceiling

        public static float Ceiling(float input)
        {
            return (float)Math.Ceiling(input);
        }

        public static double Ceiling(double input)
        {
            return Math.Ceiling(input);
        }

        public static float Floor(float input)
        {
            return (float)Math.Floor(input);
        }

        public static double Floor(double input)
        {
            return Math.Floor(input);
        }

        #endregion

        #region Float/Double Increment/Decrement

        // http://stackoverflow.com/a/14278361/925777
        // float
        public static float Increment(float f)
        {
            if (float.IsNaN(f) || float.IsInfinity(f))
            {
                return f;
            }
            else if (f == 0)
            {
                return float.Epsilon;
            }

            int floatBytesAsInt = BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
            if (f > 0)
            {
                floatBytesAsInt++;
            }
            else if (f < 0)
            {
                floatBytesAsInt--;
            }

            return BitConverter.ToSingle(BitConverter.GetBytes(floatBytesAsInt), 0);
        }

        public static float Decrement(float f)
        {
            if (float.IsNaN(f) || float.IsInfinity(f))
            {
                return f;
            }
            else if (f == 0)
            {
                return -float.Epsilon;
            }

            int floatBytesAsInt = BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
            if (f > 0)
            {
                floatBytesAsInt--;
            }
            else if (f < 0)
            {
                floatBytesAsInt++;
            }

            return BitConverter.ToSingle(BitConverter.GetBytes(floatBytesAsInt), 0);
        }

        // double
        // Not tested but should work (changed int -> long)
        public static double Increment(double d)
        {
            if (double.IsNaN(d) || double.IsInfinity(d))
            {
                return d;
            }
            else if (d == 0)
            {
                return double.Epsilon;
            }

            long doubleBytesAsLong = BitConverter.DoubleToInt64Bits(d);
            if (d > 0)
            {
                doubleBytesAsLong++;
            }
            else if (d < 0)
            {
                doubleBytesAsLong--;
            }

            return BitConverter.Int64BitsToDouble(doubleBytesAsLong);
        }

        public static double Decrement(double d)
        {
            if (double.IsNaN(d) || double.IsInfinity(d))
            {
                return d;
            }
            else if (d == 0)
            {
                return -double.Epsilon;
            }

            long doubleBytesAsLong = BitConverter.DoubleToInt64Bits(d);
            if (d > 0)
            {
                doubleBytesAsLong--;
            }
            else
            {
                doubleBytesAsLong++;
            }

            return BitConverter.Int64BitsToDouble(doubleBytesAsLong);
        }

        #endregion

        #region Misc

        public static int DigitCount(int x)
        {
            if (x == 0)
            {
                return 1;
            }

            int count = 0;
            while (x != 0)
            {
                x /= 10;
                count++;
            }

            return count;
        }

        public static int DigitCount(long x)
        {
            if (x == 0)
            {
                return 1;
            }

            int count = 0;
            while (x != 0)
            {
                x /= 10;
                count++;
            }

            return count;
        }

        public static int DigitCount(ulong x)
        {
            if (x == 0)
            {
                return 1;
            }

            int count = 0;
            while (x != 0)
            {
                x /= 10;
                count++;
            }

            return count;
        }

        public static long BigMul(int a, int b)
        {
            return (long)a * (long)b;
        }

        public static int DivRem(int a, int b, out int result)
        {
            result = a % b;
            return a / b;
        }

        public static long DivRem(long a, long b, out long result)
        {
            result = a % b;
            return a / b;
        }

        #endregion

        public static bool EqualsApproximately(float value1, float value2)
        {
            return Mathf.Approximately(value1, value2);
        }

        public static bool EqualsApproximately(double value1, double value2)
        {
            return FlaiMath.Abs(value1 - value2) <= Mathf.Epsilon;
        }

        public static float PingPong(float value, float step)
        {
            HorizontalDirection direction;
            return FlaiMath.PingPong(value, step, out direction);
        }

        public static float PingPong(float value, float step, out HorizontalDirection currentDirection)
        {
            Ensure.False(step == 0);
            int count = (int)(value / step);

            float mod = value % step;
            if (count%2 == 0)
            {
                currentDirection = HorizontalDirection.Right;
                return mod;
            }
            currentDirection = HorizontalDirection.Left;
            return step - mod;
        }

        public static Vector2 Normalize(Vector2 value)
        {
            value.Normalize();
            return value;
        }

        public static Vector2 NormalizeOrZero(Vector2 value)
        {
            value.Normalize();
            if (!Check.IsValid(value))
            {
                return Vector2.zero;
            }

            return value;
        }

        public static Vector2 Normalize(Vector2i value)
        {
            int sum = value.X * value.X + value.Y * value.Y;
            float divider = 1f / FlaiMath.Sqrt(sum);
            return new Vector2 { x = value.X * divider, y = value.Y * divider };
        }

        public static Vector2 NormalizeOrZero(Vector2i value)
        {
            if (value.X == 0 && value.Y == 0)
            {
                return Vector2.zero;
            }

            return FlaiMath.Normalize(value);
        }

        public static Vector2 ClampLength(Vector2 value, float maxLength)
        {
            if (value.LengthSquared() > maxLength * maxLength)
            {
                return value.NormalizeOrZero() * maxLength;
            }

            return value;
        }

        public static float AngleBetweenVectors(Vector2 a, Vector2 b)
        {
            return FlaiMath.Atan2(a.x * b.y - a.y * b.x, a.x * b.x + a.y * b.y);
        }
    }
}
