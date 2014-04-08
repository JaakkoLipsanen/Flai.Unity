
using System;

namespace Flai.General
{
    internal static class TypeMaskHelperUInt64<T>
    {
        private static UInt64 NextBit = 1;

        public static UInt64 GetBit<Y>()
            where Y : T
        {
            return TypeMaskHelperInner<Y>.Bit;
        }

        private static class TypeMaskHelperInner<Y>
            where Y : T
        {
            public static readonly UInt64 Bit;
            static TypeMaskHelperInner()
            {
                // hmm.. will it actually be uint.MaxValue? I guess it will..?
                Ensure.True(TypeMaskHelperInner<Y>.Bit != uint.MaxValue, "No more Bits available, use TypeMaskHelperBigInt<T>!");

                TypeMaskHelperInner<Y>.Bit = TypeMaskHelperUInt64<T>.NextBit;
                TypeMaskHelperUInt64<T>.NextBit <<= 1; // move the bits one to the right
            }
        }
    }
}