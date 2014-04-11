using UnityEngine;

namespace Flai
{
    public struct LayerMaskF
    {
        public readonly int Mask;
        public LayerMaskF Inverse
        {
            get { return ~this.Mask; }
        }

        public LayerMaskF(int layerMask)
        {
            this.Mask = layerMask;
        }

        public static LayerMaskF FromName(string name)
        {
            return new LayerMaskF(1 << LayerMask.NameToLayer(name));
        }

        public static LayerMaskF FromNames(string name1, string name2)
        {
            return LayerMaskF.FromName(name1) | LayerMaskF.FromName(name2);
        }

        public static LayerMaskF FromNames(string name1, string name2, string name3)
        {
            return LayerMaskF.FromName(name1) | LayerMaskF.FromName(name2) | LayerMaskF.FromName(name3);
        }

        public static LayerMaskF Combine(LayerMaskF a, LayerMaskF b)
        {
            return a | b;
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
