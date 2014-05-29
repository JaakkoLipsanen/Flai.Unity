using Flai.Tweening;

namespace Flai.Scene
{
    // this isn't really "Scene"-spefic, so this could be in Flai.General maybe..
    public class Fade
    {
        public static readonly Fade Default = Fade.Create();
        public static readonly Fade None = Fade.Create(0f);
        public static readonly ColorF DefaultColor = ColorF.Black;
        public static readonly TweenType DefaulTween = TweenType.Linear;

        public float Time { get; private set; }
        public ColorF Color { get; private set; }
        public TweenType TweenType { get; private set; }

        public static Fade Create(float time = 0.5f)
        {
            return Fade.Create(time, Fade.DefaulTween, Fade.DefaultColor);
        }

        public static Fade Create(float time, TweenType tweenType)
        {
            return Fade.Create(time, tweenType, Fade.DefaultColor);
        }

        public static Fade Create(float time, TweenType tweenType, ColorF color)
        {
            Ensure.True(time >= 0f);
            return new Fade { Time = time, TweenType = tweenType, Color = color };
        }
    }
}
