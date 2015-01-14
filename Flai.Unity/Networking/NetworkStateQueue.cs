using System.Collections.Generic;
using UnityEngine;

namespace Flai.Networking
{
    // !! I'm not 100% sure if "extrapolate rules" are correct. Also it'd be nice to remove _currentSnapshot and _extrapolateSnapshot. Maybe use SizedQueue(capacity 2) or something
    // TODO: The timestamp should probably be from NetworkMessageInfo.timestamp ?? That's the time that the server sent the message, instead of currently which is when I received the message!!
    // todo: make some kind of wiser extrapolating...? maybe something like extrapolating between multiple states (so that the "acceleration" is being taken into account?)
    public class NetworkStateQueue<T> // better name?
        where T : IInterpolatableNetworkState<T>
    {
        private readonly float _delay;
        private readonly Queue<Snapshot<T>> _snapshots = new Queue<Snapshot<T>>();
        private Snapshot<T> _currentSnapshot;
        private Snapshot<T> _extrapolateSnapshot;

        public bool IsExtrapolating
        {
            get { return _snapshots.Count == 0; }
        }

        // todo: at the moment the timestamp is local time. that means that delay is "network ping + delay".
        // > if I wont change this, then I could make the delay be "automatic" (ie, calculate the average time between AddState's and use that)
        public NetworkStateQueue(T initialState, float delay = 0.1f)
        {
            _currentSnapshot = new Snapshot<T>(initialState, this.GetCurrentTimestamp());
            _extrapolateSnapshot = _currentSnapshot;

            _delay = delay;
        }

        public void AddState(T state) // todo: add the "timestamp" parameter..!
        {
            _snapshots.Enqueue(new Snapshot<T>(state, this.GetCurrentTimestamp()));
        }

        public T GetCurrentState()
        {
            double delayedTimestamp = this.GetCurrentTimestamp() - _delay;
            while (_snapshots.Count > 0 && _snapshots.Peek().Timestamp < delayedTimestamp) // dequeue passed states
            {
                _extrapolateSnapshot = _currentSnapshot;
                _currentSnapshot = _snapshots.Dequeue();
            }

            if (_snapshots.Count == 0)
            {
                return _extrapolateSnapshot.Interpolate(_currentSnapshot, delayedTimestamp);
            }

            return _currentSnapshot.Interpolate(_snapshots.Peek(), delayedTimestamp);
        }

        public void Reset(T state)
        {
            _snapshots.Clear();
            _currentSnapshot = new Snapshot<T>(state, this.GetCurrentTimestamp());
            _extrapolateSnapshot = _currentSnapshot;
        }

        private double GetCurrentTimestamp()
        {
            return Network.time; // ?
        }

        #region Snapshot

        private struct Snapshot<T>
            where T : IInterpolatableNetworkState<T>
        {
            public readonly T Value;
            public readonly double Timestamp;

            public Snapshot(T value, double timestamp)
            {
                this.Value = value;
                this.Timestamp = timestamp;
            }

            public T Interpolate(Snapshot<T> other, double currentTimeStamp)
            {
                Ensure.True(this.Timestamp <= other.Timestamp);
                if (FlaiMath.EqualsApproximately(this.Timestamp, other.Timestamp))
                {
                    return this.Value;
                }

                return this.Value.Interpolate(other.Value,
                    (float)FlaiMath.Scale(currentTimeStamp, this.Timestamp, other.Timestamp, 0, 1));
            }
        }

        #endregion
    }
}
