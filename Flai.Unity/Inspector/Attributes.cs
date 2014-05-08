using System;

namespace Flai.Inspector
{
    public class ShowAsObjectFieldAttribute : Attribute
    {
        public bool AllowSceneObjects { get; set; }
    }

    public class ShowAsLayerAttribute : Attribute { }
    public class ShowAsFloatSliderAttribute : Attribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public ShowAsFloatSliderAttribute(float min, float max)
        {
            Ensure.True(max >= min);
            this.Min = min;
            this.Max = max;
        }
    }

    public class ShowAsIntSliderAttribute : Attribute
    {
        public int Min { get; private set; }
        public int Max { get; private set; }

        public ShowAsIntSliderAttribute(int min, int max)
        {
            Ensure.True(max >= min);
            this.Min = min;
            this.Max = max;
        }
    }

    public class ShowInInspectorAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditableWhenNotPlaying { get; set; }

        public ShowInInspectorAttribute()
        {
            this.IsVisible = true;
            this.IsReadOnly = false;
            this.IsEditableWhenNotPlaying = false;
        }
    }
}
