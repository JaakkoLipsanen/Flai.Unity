using UnityEngine;

namespace Flai
{
    public static class Layer
    {
        public static string NameFromIndex(int index)
        {
            return LayerMask.LayerToName(index);
        }

        public static int IndexFromName(string name)
        {
            return LayerMask.NameToLayer(name);
        }

        public static LayerMaskF MaskFromIndex(int index)
        {
            return new LayerMaskF(index);
        }

        public static LayerMaskF MaskFromName(string name)
        {
            return LayerMaskF.FromName(name);
        }

        public static LayerMaskF MaskFromNames(string name1, string name2)
        {
            return LayerMaskF.FromNames(name1, name2);
        }

        public static LayerMaskF MaskFromNames(string name1, string name2, string name3)
        {
            return LayerMaskF.FromNames(name1, name2, name3);
        }
    }

    public struct LayerMaskF
    {
        public readonly int Mask;
        public LayerMaskF Inverse
        {
            get { return ~this.Mask; }
        }

        public string Name
        {
            get { return LayerMask.LayerToName(this.Mask); }
        }

        public LayerMaskF(int layerMask)
        {
            this.Mask = layerMask;
        }

        public static LayerMaskF FromName(string name)
        {
            return new LayerMaskF(1 << LayerMask.NameToLayer(name));
        }

        public static LayerMaskF FromNames(string name1)
        {
            return LayerMaskF.FromName(name1);
        }

        public static LayerMaskF FromNames(string name1, string name2)
        {
            return LayerMaskF.FromName(name1) | LayerMaskF.FromName(name2);
        }

        public static LayerMaskF FromNames(string name1, string name2, string name3)
        {
            return LayerMaskF.FromName(name1) | LayerMaskF.FromName(name2) | LayerMaskF.FromName(name3);
        }

        public static LayerMaskF FromNames(string name1, string name2, string name3, string name4)
        {
            return LayerMaskF.FromName(name1) | LayerMaskF.FromName(name2) | LayerMaskF.FromName(name3) | LayerMaskF.FromName(name4);
        }

        public static LayerMaskF FromNames(string name1, string name2, string name3, string name4, string name5)
        {
            return LayerMaskF.FromName(name1) | LayerMaskF.FromName(name2) | LayerMaskF.FromName(name3) | LayerMaskF.FromName(name4) | LayerMaskF.FromName(name5);
        }

        public static LayerMaskF FromNames(string name1, string name2, string name3, string name4, string name5, string name6)
        {
            return LayerMaskF.FromName(name1) | LayerMaskF.FromName(name2) | LayerMaskF.FromName(name3) | LayerMaskF.FromName(name4) | LayerMaskF.FromName(name5) | LayerMaskF.FromName(name6);
        }

        public static LayerMaskF Combine(LayerMaskF a, LayerMaskF b)
        {
            return a | b;
        }

        public LayerMaskF Combine(LayerMaskF other)
        {
            return LayerMaskF.Combine(this, other);
        }

        public static LayerMaskF operator |(LayerMaskF a, LayerMaskF b)
        {
            return new LayerMaskF(a.Mask | b.Mask);
        }

        public static implicit operator LayerMaskF(int mask)
        {
            return new LayerMaskF(mask);
        }

        public static implicit operator int(LayerMaskF mask)
        {
            return mask.Mask;
        }

        public static implicit operator LayerMask(LayerMaskF mask)
        {
            return new LayerMask { value = mask.Mask };
        }

        public static implicit operator LayerMaskF(LayerMask mask)
        {
            return new LayerMaskF(mask.value);
        }
    }
}
