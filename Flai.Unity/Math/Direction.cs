
using System;

namespace Flai
{
    public enum HorizontalDirection
    {
        Left = -1,
        Right = 1,
    }

    public enum VerticalDirection
    {
        Up = 1, // different than in XNA
        Down = -1,
    }

    public enum Direction2D // : byte // maybe I shouldn't use byte's after all.. int's are a bit more efficient and the memory difference
    {                                 // in level files or in runtime is so small
        Right = 0,
        Up = 1,
        Left = 2,
        Down = 3,
    }

    public enum Direction3D
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
        Forward = 4,
        Backward = 5,
    }

    public static class DirectionHelper
    {
        public static Direction2D FromRotation(float degrees, Direction2D startDirection = Direction2D.Right) // 
        {
            degrees = FlaiMath.RealModulus(degrees, 360);

            int step = (int)FlaiMath.Round(degrees / 90);
            return (Direction2D)FlaiMath.RealModulus((int)(step + startDirection), 4);
        }
    }

    public static class DirectionExtensions
    {
        #region Direction2D Extensions

        public static Vector2i ToUnitVector(this Direction2D direction)
        {
            switch (direction)
            {
                case Direction2D.Right:
                    return Vector2i.UnitX;

                case Direction2D.Left:
                    return -Vector2i.UnitX;

                case Direction2D.Up:
                    return Vector2i.UnitY;

                case Direction2D.Down:
                    return -Vector2i.UnitY;

                default:
                    throw new ArgumentException("Value \"" + direction + "\" not recognized");
            }
        }

        public static Direction2D Inverse(this Direction2D direction)
        {
            switch (direction)
            {
                case Direction2D.Right:
                    return Direction2D.Left;
                case Direction2D.Left:
                    return Direction2D.Right;
                case Direction2D.Up:
                    return Direction2D.Down;
                case Direction2D.Down:
                    return Direction2D.Up;

                default:
                    throw new ArgumentException("Value \"" + direction + "\" not recognized");
            }
        }

        public static Direction2D RotateRight(this Direction2D direction)
        {
            switch (direction)
            {
                case Direction2D.Right:
                    return Direction2D.Down;
                case Direction2D.Down:
                    return Direction2D.Left;
                case Direction2D.Left:
                    return Direction2D.Up;
                case Direction2D.Up:
                    return Direction2D.Right;

                default:
                    throw new ArgumentException("Value \"" + direction + "\" not recognized");
            }
        }

        public static Direction2D RotateLeft(this Direction2D direction)
        {
            switch (direction)
            {
                case Direction2D.Right:
                    return Direction2D.Up;
                case Direction2D.Up:
                    return Direction2D.Left;
                case Direction2D.Left:
                    return Direction2D.Down;
                case Direction2D.Down:
                    return Direction2D.Right;

                default:
                    throw new ArgumentException("Value \"" + direction + "\" not recognized");
            }
        }

        public static int ToDegrees(this Direction2D direction)
        {
            switch (direction)
            {
                case Direction2D.Right:
                    return 0;

                case Direction2D.Up:
                    return 90;

                case Direction2D.Left:
                    return 180;

                case Direction2D.Down:
                    return 270;

                default:
                    throw new ArgumentException("Direction is invalid");
            }
        }

        public static float ToRadians(this Direction2D direction)
        {
            return FlaiMath.ToRadians(direction.ToDegrees());
        }

        public static Axis ToAxis(this Direction2D direction)
        {
            if (direction == Direction2D.Left || direction == Direction2D.Right)
            {
                return Axis.Horizontal;
            }

            return Axis.Vertical;
        }

        public static int Sign(this Direction2D direction)
        {
            if (direction == Direction2D.Left || direction == Direction2D.Up)
            {
                return -1;
            }

            return 1;
        }

        public static Direction2D Opposite(this Direction2D direction)
        {
            switch (direction)
            {
                case Direction2D.Left:
                    return Direction2D.Right;

                case Direction2D.Right:
                    return Direction2D.Left;

                case Direction2D.Up:
                    return Direction2D.Down;

                case Direction2D.Down:
                    return Direction2D.Up;

                default:
                    throw new ArgumentException("Invalid direction!");
            }
        }

        #endregion

        #region VerticalDirection/HorizontalDirection Extensions

        public static Vector2f ToUnitVector(this VerticalDirection verticalDirection)
        {
            return (verticalDirection == VerticalDirection.Down) ? Vector2f.Down : Vector2f.Up;
        }

        public static Vector2f ToUnitVector(this HorizontalDirection horizontalDirection)
        {
            return (horizontalDirection == HorizontalDirection.Left) ? Vector2f.Left : Vector2f.Right;
        }

        public static int ToInt(this VerticalDirection verticalDirection)
        {
            return (verticalDirection == VerticalDirection.Down) ? -1 : 1;
        }

        public static int ToInt(this HorizontalDirection horizontalDirection)
        {
            return (horizontalDirection == HorizontalDirection.Left) ? -1 : 1;
        }

        public static VerticalDirection Opposite(this VerticalDirection verticalDirection)
        {
            return (verticalDirection == VerticalDirection.Down) ? VerticalDirection.Up : VerticalDirection.Down;
        }

        public static HorizontalDirection Opposite(this HorizontalDirection horizontalDirection)
        {
            return (horizontalDirection == HorizontalDirection.Left) ? HorizontalDirection.Right : HorizontalDirection.Left;
        }

        #endregion

        #region Direction3D Extensions

        public static Vector3i ToVector3i(this Direction3D direction)
        {
            switch (direction)
            {
                case Direction3D.Right:
                    return Vector3i.UnitX;

                case Direction3D.Left:
                    return -Vector3i.UnitX;

                case Direction3D.Up:
                    return Vector3i.UnitY;

                case Direction3D.Down:
                    return -Vector3i.UnitY;

                case Direction3D.Forward:
                    return Vector3i.UnitZ;

                case Direction3D.Backward:
                    return -Vector3i.UnitZ;

                default:
                    throw new ArgumentException("Value \"" + direction + "\" not recognized");
            }
        }

        public static Direction3D Inverse(this Direction3D direction)
        {
            switch (direction)
            {
                case Direction3D.Right:
                    return Direction3D.Left;
                case Direction3D.Left:
                    return Direction3D.Right;

                case Direction3D.Up:
                    return Direction3D.Down;
                case Direction3D.Down:
                    return Direction3D.Up;

                case Direction3D.Forward:
                    return Direction3D.Backward;
                case Direction3D.Backward:
                    return Direction3D.Forward;

                default:
                    throw new ArgumentException("Value \"" + direction + "\" not recognized");
            }
        }

        #endregion
    }
}