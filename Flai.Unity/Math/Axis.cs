namespace Flai
{
    public enum Axis
    {
        Horizontal = 1,
        Vertical = 2,
    }

    public static class AxisExtensions
    {
        public static Vector2i ToUnitVector(this Axis axis)
        {
            return (axis == Axis.Horizontal) ? Vector2i.UnitX : Vector2i.UnitY;
        }

        public static Axis Inverse(this Axis axis)
        {
            return (axis == Axis.Horizontal) ? Axis.Vertical : Axis.Horizontal;
        }
    }
}
