using UnityEngine;

namespace Flai.Networking
{
    public interface INetworkState<T>
    {
        void Serialize(BitStream stream, NetworkMessageInfo info);
        T Deserialize(BitStream stream, NetworkMessageInfo info);
    }

    public interface IInterpolatableNetworkState<T> : INetworkState<T>
    {
        T Interpolate(T other, float amount);
        // separate Extrapolate method..?
    }
}
