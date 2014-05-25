
using Flai.Graphics;
using System;
using UnityEngine;

namespace Flai
{
    [Serializable]
    public class SerializableColorF : IEquatable<ColorF>, IEquatable<SerializableColorF>
    {
        #region Fields and Properties

        [SerializeField]
        public byte R;
        [SerializeField]
        public byte G;
        [SerializeField]
        public byte B;
        [SerializeField]
        public byte A;

        public float Grayscale
        {
            get
            {
                float r = this.R * 255f * 0.299f;
                float g = this.G * 255f * 0.587f;
                float b = this.B * 0.114f;

                return r + g + b;
            }
        }

        public ColorF Linear
        {
            get
            {
                Color color = this;
                return new ColorF(Mathf.GammaToLinearSpace(color.r), Mathf.GammaToLinearSpace(color.g), Mathf.GammaToLinearSpace(color.b), color.a);
            }
        }

        public ColorF Gamma
        {
            get
            {
                Color color = this;
                return new Color(Mathf.LinearToGammaSpace(color.r), Mathf.LinearToGammaSpace(color.g), Mathf.LinearToGammaSpace(color.b), color.a);
            }
        }

        public uint PackedValue
        {
            get { return ColorHelper.ColorFToUInt(this); }
        }

        #endregion

        #region Constructors

        public SerializableColorF(byte value)
        {
            this.R = value;
            this.G = value;
            this.B = value;
            this.A = 255;
        }

        public SerializableColorF(byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = 255;
        }

        public SerializableColorF(byte r, byte g, byte b, byte a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public SerializableColorF(float r, float g, float b)
        {
            this.R = FlaiMath.Clamp((byte)(r * 255), byte.MinValue, byte.MaxValue);
            this.G = FlaiMath.Clamp((byte)(g * 255), byte.MinValue, byte.MaxValue);
            this.B = FlaiMath.Clamp((byte)(b * 255), byte.MinValue, byte.MaxValue);
            this.A = 255;
        }

        public SerializableColorF(float r, float g, float b, float a)
        {
            this.R = FlaiMath.Clamp((byte)(r * 255), byte.MinValue, byte.MaxValue);
            this.G = FlaiMath.Clamp((byte)(g * 255), byte.MinValue, byte.MaxValue);
            this.B = FlaiMath.Clamp((byte)(b * 255), byte.MinValue, byte.MaxValue);
            this.A = FlaiMath.Clamp((byte)(a * 255), byte.MinValue, byte.MaxValue);
        }

        #endregion

        #region Implementations of IEquatable

        public bool Equals(ColorF other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public bool Equals(Color32 other)
        {
            return this.R == other.r && this.G == other.g && this.B == other.b && this.A == other.a;
        }

        #endregion

        #region Methods

        public Vector3 ToVector3()
        {
            return this;
        }

        public Vector4 ToVector4()
        {
            return this;
        }

        public Color ToColor()
        {
            return this;
        }

        public Color32 ToColor32()
        {
            return this;
        }

        public ColorF ToColorF()
        {
            return this;
        }

        #endregion

        #region Static Methods

        public static ColorF Lerp(ColorF from, ColorF to, float amount)
        {
            // from UnityEngine.Color32
            amount = FlaiMath.Clamp(amount, 0, 1);
            return new Color32((byte)(from.R + (to.R - from.R) * amount), (byte)(from.G + (to.G - from.G) * amount), (byte)(from.B + (to.B - from.B) * amount), (byte)(from.A + (to.A - from.A) * amount));
        }

        // smoothstep
        public static ColorF Slerp(Color from, Color to, float amount)
        {
            amount = FlaiMath.Clamp(amount, 0, 1);
            return ColorF.Lerp(from, to, amount * amount * (3f - 2f * amount));
        }

        #endregion

        #region Operators

        public static ColorF operator *(SerializableColorF color, float multiplier)
        {
            return new ColorF(color.R, color.G, color.B, (byte)FlaiMath.Clamp(color.A * multiplier, 0, 255));
        }

        public static ColorF operator +(SerializableColorF a, SerializableColorF b)
        {
            return new ColorF
            {
                R = (byte)FlaiMath.Min(255, a.R + b.R),
                G = (byte)FlaiMath.Min(255, a.G + b.G),
                B = (byte)FlaiMath.Min(255, a.B + b.B),
                A = (byte)FlaiMath.Min(255, a.A + b.A),
            };
        }

        public static ColorF operator -(SerializableColorF a, SerializableColorF b)
        {
            return new ColorF
            {
                R = (byte)FlaiMath.Max(0, a.R - b.R),
                G = (byte)FlaiMath.Max(0, a.G - b.G),
                B = (byte)FlaiMath.Max(0, a.B - b.B),
                A = (byte)FlaiMath.Max(0, a.A - b.A),
            };
        }

        public static ColorF operator *(SerializableColorF a, SerializableColorF b)
        {
            return a.ToColor() * b.ToColor(); // meh
        }

        public static bool operator ==(SerializableColorF a, SerializableColorF b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
        }

        public static bool operator !=(SerializableColorF a, SerializableColorF b)
        {
            return !(a == b);
        }

        public static bool operator ==(SerializableColorF a, Color32 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SerializableColorF a, Color32 b)
        {
            return !(a == b);
        }

        public static bool operator ==(SerializableColorF a, ColorF b)
        {
            return b.Equals(a.ToColorF());
        }

        public static bool operator !=(SerializableColorF a, ColorF b)
        {
            return !(a == b);
        }

        public static bool operator ==(SerializableColorF a, Color b)
        {
            return b.Equals(a.ToColor());
        }

        public static bool operator !=(SerializableColorF a, Color b)
        {
            return !(a == b);
        }

        public static implicit operator SerializableColorF(Color32 color)
        {
            return new SerializableColorF(color.r, color.g, color.b, color.a);
        }

        public static implicit operator SerializableColorF(ColorF color)
        {
            return new SerializableColorF(color.R, color.G, color.B, color.A);
        }

        public static implicit operator ColorF(SerializableColorF color)
        {
            return new ColorF(color.R, color.G, color.B, color.A);
        }

        public static implicit operator Color32(SerializableColorF color)
        {
            return new Color32(color.R, color.G, color.B, color.A);
        }

        public static implicit operator SerializableColorF(Color color)
        {
            return new SerializableColorF(color.r, color.g, color.b, color.a);
        }

        public static implicit operator Color(SerializableColorF color)
        {
            return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public static implicit operator Vector3(SerializableColorF color)
        {
            return new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
        }

        public static implicit operator Vector4(SerializableColorF color)
        {
            return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        #endregion

        #region Object Overrides

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is ColorF && Equals((ColorF)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.R.GetHashCode();
                hashCode = (hashCode * 397) ^ this.G.GetHashCode();
                hashCode = (hashCode * 397) ^ this.B.GetHashCode();
                hashCode = (hashCode * 397) ^ this.A.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(SerializableColorF other)
        {
            return false;
        }

        public override string ToString()
        {
            return string.Format("RGBA({0}, {1}, {2}, {3})",
                this.R,
                this.G,
                this.B,
                this.A);
        }

        #endregion
    }

    // fuck... byte vs float..
    public struct ColorF : IEquatable<ColorF>, IEquatable<Color32>
    {
        #region Fields and Properties

        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public float Grayscale
        {
            get
            {
                float r = this.R * 255f * 0.299f;
                float g = this.G * 255f * 0.587f;
                float b = this.B * 0.114f;

                return r + g + b;
            }
        }

        public ColorF Linear
        {
            get
            {
                Color color = this;
                return new ColorF(Mathf.GammaToLinearSpace(color.r), Mathf.GammaToLinearSpace(color.g), Mathf.GammaToLinearSpace(color.b), color.a);
            }
        }

        public ColorF Gamma
        {
            get
            {
                Color color = this;
                return new Color(Mathf.LinearToGammaSpace(color.r), Mathf.LinearToGammaSpace(color.g), Mathf.LinearToGammaSpace(color.b), color.a);
            }
        }

        public uint PackedValue
        {
            get { return ColorHelper.ColorFToUInt(this); }
        }

        #endregion

        #region Constructors

        public ColorF(uint packedValue)
        {
            this = ColorHelper.UIntToColorF(packedValue);
        }

        public ColorF(byte value)
        {
            this.R = value;
            this.G = value;
            this.B = value;
            this.A = 255;
        }

        public ColorF(byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = 255;
        }

        public ColorF(byte r, byte g, byte b, byte a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public ColorF(float r, float g, float b)
        {
            this.R = FlaiMath.Clamp((byte)(r * 255), byte.MinValue, byte.MaxValue);
            this.G = FlaiMath.Clamp((byte)(g * 255), byte.MinValue, byte.MaxValue);
            this.B = FlaiMath.Clamp((byte)(b * 255), byte.MinValue, byte.MaxValue);
            this.A = 255;
        }

        public ColorF(float r, float g, float b, float a)
        {
            this.R = FlaiMath.Clamp((byte)(r * 255), byte.MinValue, byte.MaxValue);
            this.G = FlaiMath.Clamp((byte)(g * 255), byte.MinValue, byte.MaxValue);
            this.B = FlaiMath.Clamp((byte)(b * 255), byte.MinValue, byte.MaxValue);
            this.A = FlaiMath.Clamp((byte)(a * 255), byte.MinValue, byte.MaxValue);
        }

        #endregion

        #region Implementations of IEquatable

        public bool Equals(ColorF other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public bool Equals(Color32 other)
        {
            return this.R == other.r && this.G == other.g && this.B == other.b && this.A == other.a;
        }

        #endregion

        #region Methods

        public Vector3 ToVector3()
        {
            return this;
        }

        public Vector4 ToVector4()
        {
            return this;
        }

        public Color ToColor()
        {
            return this;
        }

        public Color32 ToColor32()
        {
            return this;
        }

        #endregion

        #region Static Methods

        public static ColorF Lerp(ColorF from, ColorF to, float amount)
        {
            // from UnityEngine.Color32
            amount = FlaiMath.Clamp(amount, 0, 1);
            return new Color32((byte)(from.R + (to.R - from.R) * amount), (byte)(from.G + (to.G - from.G) * amount), (byte)(from.B + (to.B - from.B) * amount), (byte)(from.A + (to.A - from.A) * amount));
        }

        // smoothstep
        public static ColorF Slerp(Color from, Color to, float amount)
        {
            amount = FlaiMath.Clamp(amount, 0, 1);
            return ColorF.Lerp(from, to, amount * amount * (3f - 2f * amount));
        }

        #endregion

        #region Operators

        public static ColorF operator *(ColorF color, float multiplier)
        {
            return new ColorF(color.R, color.G, color.B, (byte)FlaiMath.Clamp(color.A * multiplier, 0, 255));
        }

        public static ColorF operator +(ColorF a, ColorF b)
        {
            return new ColorF
            {
                R = (byte)FlaiMath.Min(255, a.R + b.R),
                G = (byte)FlaiMath.Min(255, a.G + b.G),
                B = (byte)FlaiMath.Min(255, a.B + b.B),
                A = (byte)FlaiMath.Min(255, a.A + b.A),
            };
        }

        public static ColorF operator -(ColorF a, ColorF b)
        {
            return new ColorF
            {
                R = (byte)FlaiMath.Max(0, a.R - b.R),
                G = (byte)FlaiMath.Max(0, a.G - b.G),
                B = (byte)FlaiMath.Max(0, a.B - b.B),
                A = (byte)FlaiMath.Max(0, a.A - b.A),
            };
        }

        public static ColorF operator *(ColorF a, ColorF b)
        {
            return a.ToColor() * b.ToColor(); // meh
        }

        public static bool operator ==(ColorF a, ColorF b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ColorF a, ColorF b)
        {
            return !(a == b);
        }

        public static bool operator ==(ColorF a, Color32 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ColorF a, Color32 b)
        {
            return !(a == b);
        }

        public static bool operator ==(Color32 a, ColorF b)
        {
            return b.Equals(a);
        }

        public static bool operator !=(Color32 a, ColorF b)
        {
            return !(a == b);
        }

        public static implicit operator ColorF(Color32 color)
        {
            return new ColorF(color.r, color.g, color.b, color.a);
        }

        public static implicit operator Color32(ColorF color)
        {
            return new Color32(color.R, color.G, color.B, color.A);
        }

        public static implicit operator ColorF(Color color)
        {
            return new ColorF(color.r, color.g, color.b, color.a);
        }

        public static implicit operator Color(ColorF color)
        {
            return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public static implicit operator Vector3(ColorF color)
        {
            return new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
        }

        public static implicit operator Vector4(ColorF color)
        {
            return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        #endregion

        #region Object Overrides

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is ColorF && Equals((ColorF)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.R.GetHashCode();
                hashCode = (hashCode * 397) ^ this.G.GetHashCode();
                hashCode = (hashCode * 397) ^ this.B.GetHashCode();
                hashCode = (hashCode * 397) ^ this.A.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("RGBA({0}, {1}, {2}, {3})",
                this.R,
                this.G,
                this.B,
                this.A);
        }

        #endregion

        // From XNA
        #region Colors

        public static ColorF Transparent
        {
            get
            {
                return new ColorF(0u);
            }
        }
        public static ColorF AliceBlue
        {
            get
            {
                return new ColorF(4294965488u);
            }
        }
        public static ColorF AntiqueWhite
        {
            get
            {
                return new ColorF(4292340730u);
            }
        }
        public static ColorF Aqua
        {
            get
            {
                return new ColorF(4294967040u);
            }
        }
        public static ColorF Aquamarine
        {
            get
            {
                return new ColorF(4292149119u);
            }
        }
        public static ColorF Azure
        {
            get
            {
                return new ColorF(4294967280u);
            }
        }
        public static ColorF Beige
        {
            get
            {
                return new ColorF(4292670965u);
            }
        }
        public static ColorF Bisque
        {
            get
            {
                return new ColorF(4291093759u);
            }
        }
        public static ColorF Black
        {
            get
            {
                return new ColorF(4278190080u);
            }
        }
        public static ColorF BlanchedAlmond
        {
            get
            {
                return new ColorF(4291685375u);
            }
        }
        public static ColorF Blue
        {
            get
            {
                return new ColorF(4294901760u);
            }
        }
        public static ColorF BlueViolet
        {
            get
            {
                return new ColorF(4293012362u);
            }
        }
        public static ColorF Brown
        {
            get
            {
                return new ColorF(4280953509u);
            }
        }
        public static ColorF BurlyWood
        {
            get
            {
                return new ColorF(4287084766u);
            }
        }
        public static ColorF CadetBlue
        {
            get
            {
                return new ColorF(4288716383u);
            }
        }
        public static ColorF Chartreuse
        {
            get
            {
                return new ColorF(4278255487u);
            }
        }
        public static ColorF Chocolate
        {
            get
            {
                return new ColorF(4280183250u);
            }
        }
        public static ColorF Coral
        {
            get
            {
                return new ColorF(4283465727u);
            }
        }
        public static ColorF CornflowerBlue
        {
            get
            {
                return new ColorF(4293760356u);
            }
        }
        public static ColorF Cornsilk
        {
            get
            {
                return new ColorF(4292671743u);
            }
        }
        public static ColorF Crimson
        {
            get
            {
                return new ColorF(4282127580u);
            }
        }
        public static ColorF Cyan
        {
            get
            {
                return new ColorF(4294967040u);
            }
        }
        public static ColorF DarkBlue
        {
            get
            {
                return new ColorF(4287299584u);
            }
        }
        public static ColorF DarkCyan
        {
            get
            {
                return new ColorF(4287335168u);
            }
        }
        public static ColorF DarkGoldenrod
        {
            get
            {
                return new ColorF(4278945464u);
            }
        }
        public static ColorF DarkGray
        {
            get
            {
                return new ColorF(4289309097u);
            }
        }
        public static ColorF DarkGreen
        {
            get
            {
                return new ColorF(4278215680u);
            }
        }
        public static ColorF DarkKhaki
        {
            get
            {
                return new ColorF(4285249469u);
            }
        }
        public static ColorF DarkMagenta
        {
            get
            {
                return new ColorF(4287299723u);
            }
        }
        public static ColorF DarkOliveGreen
        {
            get
            {
                return new ColorF(4281297749u);
            }
        }
        public static ColorF DarkOrange
        {
            get
            {
                return new ColorF(4278226175u);
            }
        }
        public static ColorF DarkOrchid
        {
            get
            {
                return new ColorF(4291572377u);
            }
        }
        public static ColorF DarkRed
        {
            get
            {
                return new ColorF(4278190219u);
            }
        }
        public static ColorF DarkSalmon
        {
            get
            {
                return new ColorF(4286224105u);
            }
        }
        public static ColorF DarkSeaGreen
        {
            get
            {
                return new ColorF(4287347855u);
            }
        }
        public static ColorF DarkSlateBlue
        {
            get
            {
                return new ColorF(4287315272u);
            }
        }
        public static ColorF DarkSlateGray
        {
            get
            {
                return new ColorF(4283387695u);
            }
        }
        public static ColorF DarkTurquoise
        {
            get
            {
                return new ColorF(4291939840u);
            }
        }
        public static ColorF DarkViolet
        {
            get
            {
                return new ColorF(4292018324u);
            }
        }
        public static ColorF DeepPink
        {
            get
            {
                return new ColorF(4287829247u);
            }
        }
        public static ColorF DeepSkyBlue
        {
            get
            {
                return new ColorF(4294950656u);
            }
        }
        public static ColorF DimGray
        {
            get
            {
                return new ColorF(4285098345u);
            }
        }
        public static ColorF DodgerBlue
        {
            get
            {
                return new ColorF(4294938654u);
            }
        }
        public static ColorF Firebrick
        {
            get
            {
                return new ColorF(4280427186u);
            }
        }
        public static ColorF FloralWhite
        {
            get
            {
                return new ColorF(4293982975u);
            }
        }
        public static ColorF ForestGreen
        {
            get
            {
                return new ColorF(4280453922u);
            }
        }
        public static ColorF Fuchsia
        {
            get
            {
                return new ColorF(4294902015u);
            }
        }
        public static ColorF Gainsboro
        {
            get
            {
                return new ColorF(4292664540u);
            }
        }
        public static ColorF GhostWhite
        {
            get
            {
                return new ColorF(4294965496u);
            }
        }
        public static ColorF Gold
        {
            get
            {
                return new ColorF(4278245375u);
            }
        }
        public static ColorF Goldenrod
        {
            get
            {
                return new ColorF(4280329690u);
            }
        }
        public static ColorF Gray
        {
            get
            {
                return new ColorF(4286611584u);
            }
        }
        public static ColorF Green
        {
            get
            {
                return new ColorF(4278222848u);
            }
        }
        public static ColorF GreenYellow
        {
            get
            {
                return new ColorF(4281335725u);
            }
        }
        public static ColorF Honeydew
        {
            get
            {
                return new ColorF(4293984240u);
            }
        }
        public static ColorF HotPink
        {
            get
            {
                return new ColorF(4290013695u);
            }
        }
        public static ColorF IndianRed
        {
            get
            {
                return new ColorF(4284243149u);
            }
        }
        public static ColorF Indigo
        {
            get
            {
                return new ColorF(4286709835u);
            }
        }
        public static ColorF Ivory
        {
            get
            {
                return new ColorF(4293984255u);
            }
        }
        public static ColorF Khaki
        {
            get
            {
                return new ColorF(4287424240u);
            }
        }
        public static ColorF Lavender
        {
            get
            {
                return new ColorF(4294633190u);
            }
        }
        public static ColorF LavenderBlush
        {
            get
            {
                return new ColorF(4294308095u);
            }
        }
        public static ColorF LawnGreen
        {
            get
            {
                return new ColorF(4278254716u);
            }
        }
        public static ColorF LemonChiffon
        {
            get
            {
                return new ColorF(4291689215u);
            }
        }
        public static ColorF LightBlue
        {
            get
            {
                return new ColorF(4293318829u);
            }
        }
        public static ColorF LightCoral
        {
            get
            {
                return new ColorF(4286611696u);
            }
        }
        public static ColorF LightCyan
        {
            get
            {
                return new ColorF(4294967264u);
            }
        }
        public static ColorF LightGoldenrodYellow
        {
            get
            {
                return new ColorF(4292016890u);
            }
        }
        public static ColorF LightGreen
        {
            get
            {
                return new ColorF(4287688336u);
            }
        }
        public static ColorF LightGray
        {
            get
            {
                return new ColorF(4292072403u);
            }
        }
        public static ColorF LightPink
        {
            get
            {
                return new ColorF(4290885375u);
            }
        }
        public static ColorF LightSalmon
        {
            get
            {
                return new ColorF(4286226687u);
            }
        }
        public static ColorF LightSeaGreen
        {
            get
            {
                return new ColorF(4289376800u);
            }
        }
        public static ColorF LightSkyBlue
        {
            get
            {
                return new ColorF(4294626951u);
            }
        }
        public static ColorF LightSlateGray
        {
            get
            {
                return new ColorF(4288252023u);
            }
        }
        public static ColorF LightSteelBlue
        {
            get
            {
                return new ColorF(4292789424u);
            }
        }
        public static ColorF LightYellow
        {
            get
            {
                return new ColorF(4292935679u);
            }
        }
        public static ColorF Lime
        {
            get
            {
                return new ColorF(4278255360u);
            }
        }
        public static ColorF LimeGreen
        {
            get
            {
                return new ColorF(4281519410u);
            }
        }
        public static ColorF Linen
        {
            get
            {
                return new ColorF(4293325050u);
            }
        }
        public static ColorF Magenta
        {
            get
            {
                return new ColorF(4294902015u);
            }
        }
        public static ColorF Maroon
        {
            get
            {
                return new ColorF(4278190208u);
            }
        }
        public static ColorF MediumAquamarine
        {
            get
            {
                return new ColorF(4289383782u);
            }
        }
        public static ColorF MediumBlue
        {
            get
            {
                return new ColorF(4291624960u);
            }
        }
        public static ColorF MediumOrchid
        {
            get
            {
                return new ColorF(4292040122u);
            }
        }
        public static ColorF MediumPurple
        {
            get
            {
                return new ColorF(4292571283u);
            }
        }
        public static ColorF MediumSeaGreen
        {
            get
            {
                return new ColorF(4285641532u);
            }
        }
        public static ColorF MediumSlateBlue
        {
            get
            {
                return new ColorF(4293814395u);
            }
        }
        public static ColorF MediumSpringGreen
        {
            get
            {
                return new ColorF(4288346624u);
            }
        }
        public static ColorF MediumTurquoise
        {
            get
            {
                return new ColorF(4291613000u);
            }
        }
        public static ColorF MediumVioletRed
        {
            get
            {
                return new ColorF(4286911943u);
            }
        }
        public static ColorF MidnightBlue
        {
            get
            {
                return new ColorF(4285536537u);
            }
        }
        public static ColorF MintCream
        {
            get
            {
                return new ColorF(4294639605u);
            }
        }
        public static ColorF MistyRose
        {
            get
            {
                return new ColorF(4292994303u);
            }
        }
        public static ColorF Moccasin
        {
            get
            {
                return new ColorF(4290110719u);
            }
        }
        public static ColorF NavajoWhite
        {
            get
            {
                return new ColorF(4289584895u);
            }
        }
        public static ColorF Navy
        {
            get
            {
                return new ColorF(4286578688u);
            }
        }
        public static ColorF OldLace
        {
            get
            {
                return new ColorF(4293326333u);
            }
        }
        public static ColorF Olive
        {
            get
            {
                return new ColorF(4278222976u);
            }
        }
        public static ColorF OliveDrab
        {
            get
            {
                return new ColorF(4280520299u);
            }
        }
        public static ColorF Orange
        {
            get
            {
                return new ColorF(4278232575u);
            }
        }
        public static ColorF OrangeRed
        {
            get
            {
                return new ColorF(4278207999u);
            }
        }
        public static ColorF Orchid
        {
            get
            {
                return new ColorF(4292243674u);
            }
        }
        public static ColorF PaleGoldenrod
        {
            get
            {
                return new ColorF(4289390830u);
            }
        }
        public static ColorF PaleGreen
        {
            get
            {
                return new ColorF(4288215960u);
            }
        }
        public static ColorF PaleTurquoise
        {
            get
            {
                return new ColorF(4293848751u);
            }
        }
        public static ColorF PaleVioletRed
        {
            get
            {
                return new ColorF(4287852763u);
            }
        }
        public static ColorF PapayaWhip
        {
            get
            {
                return new ColorF(4292210687u);
            }
        }
        public static ColorF PeachPuff
        {
            get
            {
                return new ColorF(4290370303u);
            }
        }
        public static ColorF Peru
        {
            get
            {
                return new ColorF(4282353101u);
            }
        }
        public static ColorF Pink
        {
            get
            {
                return new ColorF(4291543295u);
            }
        }
        public static ColorF Plum
        {
            get
            {
                return new ColorF(4292714717u);
            }
        }
        public static ColorF PowderBlue
        {
            get
            {
                return new ColorF(4293320880u);
            }
        }
        public static ColorF Purple
        {
            get
            {
                return new ColorF(4286578816u);
            }
        }
        public static ColorF Red
        {
            get
            {
                return new ColorF(4278190335u);
            }
        }
        public static ColorF RosyBrown
        {
            get
            {
                return new ColorF(4287598524u);
            }
        }
        public static ColorF RoyalBlue
        {
            get
            {
                return new ColorF(4292962625u);
            }
        }
        public static ColorF SaddleBrown
        {
            get
            {
                return new ColorF(4279453067u);
            }
        }
        public static ColorF Salmon
        {
            get
            {
                return new ColorF(4285694202u);
            }
        }
        public static ColorF SandyBrown
        {
            get
            {
                return new ColorF(4284523764u);
            }
        }
        public static ColorF SeaGreen
        {
            get
            {
                return new ColorF(4283927342u);
            }
        }
        public static ColorF SeaShell
        {
            get
            {
                return new ColorF(4293850623u);
            }
        }
        public static ColorF Sienna
        {
            get
            {
                return new ColorF(4281160352u);
            }
        }
        public static ColorF Silver
        {
            get
            {
                return new ColorF(4290822336u);
            }
        }
        public static ColorF SkyBlue
        {
            get
            {
                return new ColorF(4293643911u);
            }
        }
        public static ColorF SlateBlue
        {
            get
            {
                return new ColorF(4291648106u);
            }
        }
        public static ColorF SlateGray
        {
            get
            {
                return new ColorF(4287660144u);
            }
        }
        public static ColorF Snow
        {
            get
            {
                return new ColorF(4294638335u);
            }
        }
        public static ColorF SpringGreen
        {
            get
            {
                return new ColorF(4286578432u);
            }
        }
        public static ColorF SteelBlue
        {
            get
            {
                return new ColorF(4290019910u);
            }
        }
        public static ColorF Tan
        {
            get
            {
                return new ColorF(4287411410u);
            }
        }
        public static ColorF Teal
        {
            get
            {
                return new ColorF(4286611456u);
            }
        }
        public static ColorF Thistle
        {
            get
            {
                return new ColorF(4292394968u);
            }
        }
        public static ColorF Tomato
        {
            get
            {
                return new ColorF(4282868735u);
            }
        }
        public static ColorF Turquoise
        {
            get
            {
                return new ColorF(4291878976u);
            }
        }
        public static ColorF Violet
        {
            get
            {
                return new ColorF(4293821166u);
            }
        }
        public static ColorF Wheat
        {
            get
            {
                return new ColorF(4289978101u);
            }
        }
        public static ColorF White
        {
            get
            {
                return new ColorF(4294967295u);
            }
        }
        public static ColorF WhiteSmoke
        {
            get
            {
                return new ColorF(4294309365u);
            }
        }
        public static ColorF Yellow
        {
            get
            {
                return new ColorF(4278255615u);
            }
        }
        public static ColorF YellowGreen
        {
            get
            {
                return new ColorF(4281519514u);
            }
        }


        #endregion
    }
}
