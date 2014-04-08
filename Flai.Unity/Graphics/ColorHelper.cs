using System.Runtime.InteropServices;
using UnityEngine;

namespace Flai.Graphics
{
    public static class ColorHelper
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct Color32Converter
        {
            [FieldOffset(0)]
            public readonly Color32 Color;

            [FieldOffset(0)]
            public readonly int PackedValue;

            public Color32Converter(Color32 color) 
                : this()
            {
                this.Color = color;
            }

            public Color32Converter(int packedValue)
                : this()
            {
                this.PackedValue = packedValue;
            }
        }

        public static Color32 IntToColor32(int packedValue)
        {
            return new Color32Converter(packedValue).Color;
        }

        public static int Color32ToInt(Color32 color)
        {
            return new Color32Converter(color).PackedValue;
        }

        public static bool Equals(Color32 color, Color32 other)
        {
            return color.r == other.r && color.g == other.g && color.b == other.b && color.a == other.a;
        }

        public static bool Equals(Color color, Color other)
        {
            return color.r == other.r && color.g == other.g && color.b == other.b && color.a == other.a;
        }

        public static Color32 MultiplyAlpha(this Color32 color, float alpha)
        {
            return new Color32(color.r, color.g, color.b, (byte)FlaiMath.Clamp(color.a * alpha, 0, 255));
        }
    }
}
