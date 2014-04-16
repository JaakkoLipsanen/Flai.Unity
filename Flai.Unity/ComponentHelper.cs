using Flai.General;
using UnityEngine;

namespace Flai
{
    internal static class ComponentHelper<T>
        where T : Component
    {
        public static readonly uint ID = TypeID<Component>.GetID<T>();
    }
}
