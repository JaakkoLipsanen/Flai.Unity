
using System.Collections.Generic;

namespace Flai
{
    // todo: atm supports only rotation. implement scale etc. also atm class, make it struct later
    public class TransformedRectangleF
    {
        private readonly RectangleF _rectangle;
        private readonly Vector2f _origin;
        private readonly float _rotation;

        public RectangleF Bounds
        {
            get
            {
                Vector2f tl = this.TopLeft;
                Vector2f tr = this.TopRight;
                Vector2f bl = this.BottomLeft;
                Vector2f br = this.BottomRight;

                return new RectangleF(Vector2f.Min(tl, tr, bl, br), Vector2f.Max(tl, tr, bl, br));
            }
        }

        public Vector2f TopLeft
        {
            get { return Vector2f.Rotate(_rectangle.TopLeft, _rotation, _origin); }
        }

        public Vector2f TopRight
        {
            get { return Vector2f.Rotate(_rectangle.TopRight, _rotation, _origin); }
        }

        public Vector2f BottomRight
        {
            get { return Vector2f.Rotate(_rectangle.BottomRight, _rotation, _origin); }
        }

        public Vector2f BottomLeft
        {
            get { return Vector2f.Rotate(_rectangle.BottomLeft, _rotation, _origin); }
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
                yield return this.TopSegment;
                yield return this.RightSegment;
                yield return this.BottomSegment;
                yield return this.LeftSegment;
            }
        }


        public Segment2D TopSegment
        {
            get { return new Segment2D(this.TopLeft, this.TopRight); }
        }

        public Segment2D RightSegment
        {
            get { return new Segment2D(this.TopRight, this.BottomRight); }
        }

        public Segment2D BottomSegment
        {
            get { return new Segment2D(this.BottomRight, this.BottomLeft); }
        }

        public Segment2D LeftSegment
        {
            get { return new Segment2D(this.BottomLeft, this.TopLeft); }
        }


        private TransformedRectangleF(RectangleF rectangle, Vector2f origin, float rotation)
        {
            _rectangle = rectangle;
            _origin = origin;
            _rotation = FlaiMath.ToRadians(rotation);
        }

        public static TransformedRectangleF CreateRotated(RectangleF rectangle, Vector2f origin, float rotation)
        {
            return new TransformedRectangleF(rectangle, origin, rotation);
        }
    }
}
