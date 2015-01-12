
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// From Nuclex originally IIRC
namespace Flai
{
    /// <summary>
    /// Two-dimensional rectangle using floating point coordinates
    /// </summary>
    [Serializable]
    public struct RectangleF : IEquatable<RectangleF>, IEquatable<Rect>
    {
        #region Fields and Properties

        public float X;
        public float Y;

        public float Width; // blahh.. no check if Width/Height > 0
        public float Height;

        public float Left
        {
            get { return this.X; }
            set { this.X += value - this.Left; }
        }

        public float Right
        {
            get { return this.X + this.Width; }
            set { this.X += value - this.Right; }
        }

        public float Top
        {
            get { return this.Y; }
            set { this.Y += value - this.Top; }
        }

        public float Bottom
        {
            get { return this.Y + this.Height; }
            set { this.Y += value - this.Bottom; }
        }

        public Vector2f TopLeft
        {
            get { return new Vector2f(this.X, this.Y); }
        }

        public Vector2f TopRight
        {
            get { return new Vector2f(this.X + this.Width, this.Y); }
        }

        public Vector2f BottomLeft
        {
            get { return new Vector2f(this.X, this.Y + this.Height); }
        }

        public Vector2f BottomRight
        {
            get { return new Vector2f(this.X + this.Width, this.Y + this.Height); }
        }

        public bool IsEmpty
        {
            get { return this.X == 0 && this.Y == 0 && this.Width == 0 && this.Height == 0; }
        }

        public Vector2f Center
        {
            get { return new Vector2f(this.X + this.Width / 2, this.Y + this.Height / 2); }
            set
            {
                this.X = value.X - this.Width / 2f;
                this.Y = value.Y - this.Height / 2;
            }
        }

        public Vector2f Location
        {
            get { return new Vector2f(this.X, this.Y); }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public Vector2f Size
        {
            get { return new Vector2f(this.Width, this.Height); }
        }

        public IEnumerable<Vector2f> CornerPoints
        {
            get
            {
                yield return this.TopLeft;
                yield return this.TopRight;
                yield return this.BottomRight;
                yield return this.BottomLeft;
            }
        }

        public IEnumerable<Segment2D> SideSegments
        {
            get
            {
                yield return new Segment2D(this.TopLeft, this.TopRight);
                yield return new Segment2D(this.TopRight, this.BottomRight);
                yield return new Segment2D(this.BottomRight, this.BottomLeft);
                yield return new Segment2D(this.BottomLeft, this.TopLeft);
            }
        }

        #endregion

        public RectangleF(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public RectangleF(Vector2f min, Vector2f max)
        {
            this.X = min.X;
            this.Y = min.Y;

            this.Width = (max.X - min.X);
            this.Height = (max.Y - min.Y);
        }

        public RectangleF(RectangleF rectangle)
        {
            this.X = rectangle.X;
            this.Y = rectangle.Y;
            this.Width = rectangle.Width;
            this.Height = rectangle.Height;
        }

        #region Offset

        public void Offset(Vector2f amount)
        {
            this.Offset(amount.X, amount.Y);
        }

        public void Offset(float offsetX, float offsetY)
        {
            this.X += offsetX;
            this.Y += offsetY;
        }

        public void Offset(float offset)
        {
            this.X += offset;
            this.Y += offset;
        }

        public RectangleF AsOffsetted(float offsetX, float offsetY)
        {
            return new RectangleF(this.X + offsetX, this.Y + offsetY, this.Width, this.Height);
        }

        public RectangleF AsOffsetted(Vector2f amount)
        {
            return new RectangleF(this.X + amount.X, this.Y + amount.Y, this.Width, this.Height);
        }

        public RectangleF AsOffsetted(float offset)
        {
            return new RectangleF(this.X + offset, this.Y + offset, this.Width, this.Height);
        }

        #endregion

        #region Inflate

        public RectangleF Inflate(float amount)
        {
            return this.Inflate(amount, amount);
        }

        /// <summary>
        /// Pushes the edges of the Rectangle out by the horizontal and
        /// vertical values specified
        /// </summary>
        /// <param name="horizontalAmount">Value to push the sides out by</param>
        /// <param name="verticalAmount">Value to push the top and bottom out by</param>
        public RectangleF Inflate(float horizontalAmount, float verticalAmount)
        {
            this.X -= horizontalAmount;
            this.Y -= verticalAmount;
            this.Width += horizontalAmount * 2;
            this.Height += verticalAmount * 2;

            return this;
        }

        // dunno if this is a good name
        public RectangleF AsInflated(float horizontalAmount, float verticalAmount)
        {
            return new RectangleF(this.X - horizontalAmount, this.Y - verticalAmount, this.Width + horizontalAmount * 2, this.Height + verticalAmount * 2);
        }

        // dunno if this is a good name
        public RectangleF AsInflated(float amount)
        {
            return new RectangleF(this.X - amount, this.Y - amount, this.Width + amount * 2, this.Height + amount * 2);
        }

        #endregion

        #region Contains

        public bool Contains(Vector2f point)
        {
            return
                (point.X >= this.X) &&
                (point.Y >= this.Y) &&
                (point.X < this.X + this.Width) &&
                (point.Y < this.Y + this.Height);
        }

        public void Contains(ref Vector2f point, out bool result)
        {
            result =
                (point.X >= this.X) &&
                (point.Y >= this.Y) &&
                (point.X < this.X + this.Width) &&
                (point.Y < this.Y + this.Height);
        }

        /// <summary>
        /// Determines whether this Rectangle contains a specified point represented by
        /// its x- and y-coordinates
        /// </summary>
        /// <param name="x">The x-coordinate of the specified point</param>
        /// <param name="y">The y-coordinate of the specified point</param>
        /// <returns>
        /// True if the specified point is contained within this rectangle; false otherwise
        /// </returns>
        public bool Contains(float x, float y)
        {
            return
              (this.X <= x) &&
              (this.Y <= y) &&
              (this.X + this.Width >= x) &&
              (this.Y + this.Height >= y);
        }

        /// <summary>
        /// Determines whether the rectangle contains another rectangle in its entirety
        /// </summary>
        /// <param name="other">The rectangle to evaluate</param>
        /// <returns>
        /// True if the rectangle entirely contains the specified rectangle; false otherwise
        /// </returns>
        public bool Contains(RectangleF other)
        {
            return
                (other.X >= this.X) &&
                (other.Y >= this.Y) &&
                ((other.X + other.Width) <= (this.X + this.Width)) &&
                ((other.Y + other.Height) <= (this.Y + this.Height));
        }

        /// <summary>
        ///   Determines whether this rectangle entirely contains a specified rectangle
        /// </summary>
        /// <param name="other">The rectangle to evaluate</param>
        /// <param name="result">
        ///   On exit, is true if this rectangle entirely contains the specified rectangle,
        ///   or false if not
        /// </param>
        public void Contains(ref RectangleF other, out bool result)
        {
            result =
              (other.X >= this.X) &&
              (other.Y >= this.Y) &&
              ((other.X + other.Width) <= (this.X + this.Width)) &&
              ((other.Y + other.Height) <= (this.Y + this.Height));
        }

        #endregion

        #region Intersects

        /// <summary>
        /// Determines whether a specified rectangle intersects with this rectangle
        /// </summary>
        /// <param name="rectangle">The rectangle to evaluate</param>
        /// <returns>
        /// True if the specified rectangle intersects with this one; false otherwise
        /// </returns>
        public bool Intersects(RectangleF rectangle)
        {
            return
                (rectangle.X < (this.X + this.Width)) &&
                (rectangle.Y < (this.Y + this.Height)) &&
                ((rectangle.X + rectangle.Width) > this.X) &&
                ((rectangle.Y + rectangle.Height) > this.Y);
        }

        /// <summary>
        /// Determines whether a specified rectangle intersects with this rectangle
        /// </summary>
        /// <param name="rectangle">The rectangle to evaluate</param>
        /// <param name="result">
        /// True if the specified rectangle intersects with this one; false otherwise
        /// </param>
        public void Intersects(ref RectangleF rectangle, out bool result)
        {
            result =
              (rectangle.X < (this.X + this.Width)) &&
              (rectangle.Y < (this.Y + this.Height)) &&
              ((rectangle.X + rectangle.Width) > this.X) &&
              ((rectangle.Y + rectangle.Height) > this.Y);
        }

        public bool Intersects(ref RectangleF rectangle)
        {
            return
              (rectangle.X < (this.X + this.Width)) &&
              (rectangle.Y < (this.Y + this.Height)) &&
              ((rectangle.X + rectangle.Width) > this.X) &&
              ((rectangle.Y + rectangle.Height) > this.Y);
        }

        public bool Overlaps(RectangleF other)
        {
            return this.Intersects(other);
        }

        #endregion

        #region GetIntersectionDepth

        public Vector2f GetIntersectionDepth(RectangleF other)
        {
            // Calculate half sizes.
            float halfWidthA = this.Width / 2.0f;
            float halfHeightA = this.Height / 2.0f;
            float halfWidthB = other.Width / 2.0f;
            float halfHeightB = other.Height / 2.0f;

            // Calculate centers.
            Vector2f centerA = new Vector2f(this.Left + halfWidthA, this.Top + halfHeightA);
            Vector2f centerB = new Vector2f(other.Left + halfWidthB, other.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
            {
                return Vector2f.Zero;
            }

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2f(depthX, depthY);
        }

        public bool GetIntersectionDepth(RectangleF other, Axis axis, out float depth)
        {
            Vector2f depthVector = this.GetIntersectionDepth(other, axis);
            depth = (axis == Axis.Horizontal) ? depthVector.X : depthVector.Y;
            return depth != 0f;
        }

        public bool GetIntersectionDepth(RectangleF other, Axis axis, out Vector2f depth)
        {
            depth = this.GetIntersectionDepth(other, axis);
            return depth.X != 0 || depth.Y != 0;
        }

        public Vector2f GetIntersectionDepth(RectangleF other, Axis axis)
        {
            return axis == Axis.Vertical ?
                new Vector2f(0, this.GetVerticalIntersectionDepth(other)) :
                new Vector2f(this.GetHorizontalIntersectionDepth(other), 0);
        }

        public bool GetHorizontalIntersectionDepth(RectangleF other, out float intersectionDepth)
        {
            intersectionDepth = this.GetHorizontalIntersectionDepth(other);
            return intersectionDepth != 0;
        }

        public float GetHorizontalIntersectionDepth(RectangleF other)
        {
            // Calculate half sizes.
            float halfWidthA = this.Width / 2.0f;
            float halfWidthB = other.Width / 2.0f;

            // Calculate centers.
            float centerA = this.Left + halfWidthA;
            float centerB = other.Left + halfWidthB;

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA - centerB;
            float minDistanceX = halfWidthA + halfWidthB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX)
            {
                return 0f;
            }

            // Calculate and return intersection depths.
            return distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        }

        public bool GetVerticalIntersectionDepth(RectangleF other, out float intersectionDepth)
        {
            intersectionDepth = this.GetVerticalIntersectionDepth(other);
            return intersectionDepth != 0;
        }

        public float GetVerticalIntersectionDepth(RectangleF other)
        {
            // Calculate half sizes.
            float halfHeightA = this.Height / 2.0f;
            float halfHeightB = other.Height / 2.0f;

            // Calculate centers.
            float centerA = this.Top + halfHeightA;
            float centerB = other.Top + halfHeightB;

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceY = centerA - centerB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceY) >= minDistanceY)
            {
                return 0f;
            }

            // Calculate and return intersection depths.
            return distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
        }

        #endregion

        #region GetSideArea

        public RectangleF GetSideArea(Direction2D side)
        {
            float x = (side != Direction2D.Right) ? this.Left : this.Right - 1;
            float y = (side != Direction2D.Down) ? this.Top : this.Bottom - 1;
            float width = (side == Direction2D.Left || side == Direction2D.Right) ? 1 : this.Width;
            float height = (side == Direction2D.Up || side == Direction2D.Down) ? 1 : this.Height;

            return new RectangleF(x, y, width, height);
        }

        // TODO: "GetSideAreaPlus(Direction2D side, float amount)" ?
        public RectangleF GetSideAreaPlusOne(Direction2D side)
        {
            if (side == Direction2D.Left)
            {
                return new RectangleF(this.Left - 1, this.Top, 1, this.Height);
            }
            else if (side == Direction2D.Right)
            {
                return new RectangleF(this.Right, this.Top, 1, this.Height);
            }
            else if (side == Direction2D.Up)
            {
                return new RectangleF(this.Left, this.Top - 1, this.Width, 1);
            }
            else // if(side == Direction2D.Down)
            {
                return new RectangleF(this.Left, this.Bottom, this.Width, 1);
            }
        }

        public RectangleF GetSideAreaPlus(Direction2D side, float value)
        {
            if (side == Direction2D.Left)
            {
                return new RectangleF(this.Left - value, this.Top, value, this.Height);
            }
            else if (side == Direction2D.Right)
            {
                return new RectangleF(this.Right, this.Top, value, this.Height);
            }
            else if (side == Direction2D.Up)
            {
                return new RectangleF(this.Left, this.Top - value, this.Width, value);
            }
            else // if(side == Direction2D.Down)
            {
                return new RectangleF(this.Left, this.Bottom, this.Width, value);
            }
        }

        #endregion

        #region Equals

        /// <summary>
        /// Determines whether the specified rectangle is equal to this rectangle
        /// </summary>
        /// <param name="other">The rectangle to compare with this rectangle</param>
        /// <returns>
        /// True if the specified rectangle is equal to the this rectangle; false otherwise
        /// </returns>
        public bool Equals(RectangleF other)
        {
            return
              (this.X == other.X) &&
              (this.Y == other.Y) &&
              (this.Width == other.Width) &&
              (this.Height == other.Height);
        }

        public bool Equals(Rect other)
        {
            return
              (this.X == other.x) &&
              (this.Y == other.y) &&
              (this.Width == other.width) &&
              (this.Height == other.height);
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance is equal to a
        /// specified object
        /// </summary>
        /// <param name="other">Object to make the comparison with</param>
        /// <returns>
        /// True if the current instance is equal to the specified object; false otherwise
        /// </returns>
        public override bool Equals(object otherObj)
        {
            if (otherObj is RectangleF)
            {
                RectangleF other = (RectangleF)otherObj;
                return
                    (this.X == other.X) &&
                    (this.Y == other.Y) &&
                    (this.Width == other.Width) &&
                    (this.Height == other.Height);
            }
            else if (otherObj is Rect)
            {

                Rect other = (Rect)otherObj;
                return
                    (this.X == other.x) &&
                    (this.Y == other.y) &&
                    (this.Width == other.width) &&
                    (this.Height == other.height);
            }

            return false;
        }

        #endregion

        #region GetHashCode

        public override int GetHashCode()
        {
            return
                this.X.GetHashCode() ^
                this.Y.GetHashCode() ^
                this.Width.GetHashCode() ^
                this.Height.GetHashCode();
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(
                currentCulture, "{{X:{0} Y:{1} Width:{2} Height:{3}}}",
                this.X.ToString(currentCulture),
                this.Y.ToString(currentCulture),
                this.Width.ToString(currentCulture),
                this.Height.ToString(currentCulture)
            );
        }

        #endregion

        #region Operators

        public static bool operator ==(RectangleF first, RectangleF second)
        {
            return
                (first.X == second.X) &&
                (first.Y == second.Y) &&
                (first.Width == second.Width) &&
                (first.Height == second.Height);
        }

        public static bool operator !=(RectangleF first, RectangleF second)
        {
            return
                (first.X != second.X) ||
                (first.Y != second.Y) ||
                (first.Width != second.Width) ||
                (first.Height != second.Height);
        }

        public static bool operator ==(RectangleF first, Rect second)
        {
            return
                (first.X == second.x) &&
                (first.Y == second.y) &&
                (first.Width == second.width) &&
                (first.Height == second.height);
        }

        public static bool operator !=(RectangleF first, Rect second)
        {
            return !(first == second);
        }

        public static implicit operator Rect(RectangleF rectangle)
        {
            return new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static implicit operator RectangleF(Rect rect)
        {
            return new RectangleF(rect.x, rect.y, rect.width, rect.height);
        }

        public static RectangleF operator *(RectangleF rectangle, float multiplier)
        {
            Ensure.True(multiplier >= 0);
            return new RectangleF(rectangle.X * multiplier, rectangle.Y * multiplier, rectangle.Width * multiplier, rectangle.Height * multiplier);
        }

        public static RectangleF operator *(RectangleF rectangle, Vector2f multiplier)
        {
            Ensure.True(multiplier.X >= 0 && multiplier.Y >= 0);
            return new RectangleF(rectangle.X * multiplier.X, rectangle.Y * multiplier.Y, rectangle.Width * multiplier.X, rectangle.Height * multiplier.Y);
        }

        public static RectangleF operator /(RectangleF rectangle, float divider)
        {
            Ensure.True(divider > 0);
            return new RectangleF(rectangle.X / divider, rectangle.Y / divider, rectangle.Width / divider, rectangle.Height / divider);
        }


        public static RectangleF operator /(RectangleF rectangle, Vector2f divider)
        {
            Ensure.True(divider.X > 0 && divider.Y > 0);
            return new RectangleF(rectangle.X / divider.X, rectangle.Y / divider.Y, rectangle.Width / divider.X, rectangle.Height / divider.Y);
        }

        #endregion

        #region Static Methods

        public static RectangleF Intersection(RectangleF value1, RectangleF value2)
        {
            float num = value1.X + value1.Width;
            float num2 = value2.X + value2.Width;
            float num3 = value1.Y + value1.Height;
            float num4 = value2.Y + value2.Height;
            float num5 = (value1.X > value2.X) ? value1.X : value2.X;
            float num6 = (value1.Y > value2.Y) ? value1.Y : value2.Y;
            float num7 = (num < num2) ? num : num2;
            float num8 = (num3 < num4) ? num3 : num4;
            RectangleF result;
            if (num7 > num5 && num8 > num6)
            {
                result.X = num5;
                result.Y = num6;
                result.Width = num7 - num5;
                result.Height = num8 - num6;
            }
            else
            {
                result.X = 0;
                result.Y = 0;
                result.Width = 0;
                result.Height = 0;
            }
            return result;
        }

        public static void Intersection(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            float num = value1.X + value1.Width;
            float num2 = value2.X + value2.Width;
            float num3 = value1.Y + value1.Height;
            float num4 = value2.Y + value2.Height;
            float num5 = (value1.X > value2.X) ? value1.X : value2.X;
            float num6 = (value1.Y > value2.Y) ? value1.Y : value2.Y;
            float num7 = (num < num2) ? num : num2;
            float num8 = (num3 < num4) ? num3 : num4;
            if (num7 > num5 && num8 > num6)
            {
                result.X = num5;
                result.Y = num6;
                result.Width = num7 - num5;
                result.Height = num8 - num6;
                return;
            }
            result.X = 0;
            result.Y = 0;
            result.Width = 0;
            result.Height = 0;
        }

        public static RectangleF Union(RectangleF value1, RectangleF value2)
        {
            float num = value1.X + value1.Width;
            float num2 = value2.X + value2.Width;
            float num3 = value1.Y + value1.Height;
            float num4 = value2.Y + value2.Height;
            float num5 = (value1.X < value2.X) ? value1.X : value2.X;
            float num6 = (value1.Y < value2.Y) ? value1.Y : value2.Y;
            float num7 = (num > num2) ? num : num2;
            float num8 = (num3 > num4) ? num3 : num4;
            RectangleF result;
            result.X = num5;
            result.Y = num6;
            result.Width = num7 - num5;
            result.Height = num8 - num6;
            return result;
        }

        public static void Union(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            float num = value1.X + value1.Width;
            float num2 = value2.X + value2.Width;
            float num3 = value1.Y + value1.Height;
            float num4 = value2.Y + value2.Height;
            float num5 = (value1.X < value2.X) ? value1.X : value2.X;
            float num6 = (value1.Y < value2.Y) ? value1.Y : value2.Y;
            float num7 = (num > num2) ? num : num2;
            float num8 = (num3 > num4) ? num3 : num4;
            result.X = num5;
            result.Y = num6;
            result.Width = num7 - num5;
            result.Height = num8 - num6;
        }

        public static RectangleF GetRounded(RectangleF rectangle) // should this be non-static?
        {
            return new RectangleF(FlaiMath.Round(rectangle.X), FlaiMath.Round(rectangle.Y), rectangle.Width, rectangle.Height);
        }

        public static RectangleF FromPoints(Vector2f p1, Vector2f p2)
        {
            return new RectangleF(Vector2f.Min(p1, p2), Vector2f.Max(p1, p2));
        }

        #endregion

        #region Static Properties and Variables

        public static RectangleF Empty
        {
            get { return RectangleF.EmptyRectangle; }
        }

        public static RectangleF MinMax
        {
            get { return RectangleF.MinMaxRectangle; }
        }

        private static readonly RectangleF EmptyRectangle = new RectangleF();
        private static readonly RectangleF MinMaxRectangle = new RectangleF(float.MinValue / 2f + 1, float.MinValue / 2f + 1, float.MaxValue - 1, float.MaxValue - 1);

        #endregion

        public static RectangleF CreateCentered(Vector2f center, float size)
        {
            return new RectangleF(center.X - size / 2f, center.Y - size / 2f, size, size);
        }

        public static RectangleF CreateCentered(Vector2f center, SizeF size)
        {
            return new RectangleF(center.X - size.Width / 2f, center.Y - size.Height / 2f, size.Width, size.Height);
        }
    }
}