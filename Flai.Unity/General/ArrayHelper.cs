
using System.Collections.Generic;

namespace Flai.General
{
    // should this be at Flai.DataStructures?
    public static class ArrayHelper<T>
    {
        public static readonly T[] Empty = new T[0];

        public static void ToString(T[] array)
        {
            string s = "{";
            for (int i = 0; i < array.Length; i++)
            {
                s += array[i].ToString();
            }
            
        }
    }

    public static class CollectionHelper
    {
        public static string ToString<T>(IList<T> collection)
        {
            string s = "{";
            for (int i = 0; i < collection.Count; i++)
            {
                s += collection[i].ToString();
                if (i != collection.Count - 1)
                {
                    s += ", ";
                }
            }

            s += "}";
            return s;
        }
    }
}
