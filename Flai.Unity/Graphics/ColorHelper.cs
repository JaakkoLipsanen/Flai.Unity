using System.Runtime.InteropServices;
using UnityEngine;

namespace Flai.Graphics
{
    public static class ColorHelper
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct ColorFToIntConverter
        {
            [FieldOffset(0)]
            public readonly ColorF Color;

            [FieldOffset(0)]
            public readonly int PackedValue;

            public ColorFToIntConverter(ColorF color)
                : this()
            {
                this.Color = color;
            }

            public ColorFToIntConverter(int packedValue)
                : this()
            {
                this.PackedValue = packedValue;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ColorFToUintConverter
        {
            [FieldOffset(0)]
            public readonly ColorF Color;

            [FieldOffset(0)]
            public readonly uint PackedValue;

            public ColorFToUintConverter(ColorF color)
                : this()
            {
                this.Color = color;
            }

            public ColorFToUintConverter(uint packedValue)
                : this()
            {
                this.PackedValue = packedValue;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ColorFArrayToColor32ArrayConverter
        {
            [FieldOffset(0)]
            public readonly ColorF[] ColorF;

            [FieldOffset(0)]
            public readonly Color32[] Color32;

            public ColorFArrayToColor32ArrayConverter(Color32[] colors)
                : this()
            {
                this.Color32 = colors;
            }

            public ColorFArrayToColor32ArrayConverter(ColorF[] colors)
                : this()
            {
                this.ColorF = colors;
            }
        }

        public static ColorF IntToColorF(int packedValue)
        {
            return new ColorFToIntConverter(packedValue).Color;
        }

        public static int ColorFToInt(ColorF color)
        {
            return new ColorFToIntConverter(color).PackedValue;
        }

        public static ColorF UIntToColorF(uint packedValue)
        {
            return new ColorFToUintConverter(packedValue).Color;
        }

        public static uint ColorFToUInt(ColorF color)
        {
            return new ColorFToUintConverter(color).PackedValue;
        }

        public static Color32[] ConvertToColor32(ColorF[] colors)
        {
            Ensure.NotNull(colors);
            return new ColorFArrayToColor32ArrayConverter(colors).Color32;
        }

        public static ColorF[] ConvertToColorF(Color32[] colors)
        {
            Ensure.NotNull(colors);
            return new ColorFArrayToColor32ArrayConverter(colors).ColorF;
        }
    }
}
