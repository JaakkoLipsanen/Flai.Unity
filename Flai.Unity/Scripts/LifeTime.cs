
using UnityTime = UnityEngine.Time;

namespace Flai.Scripts
{
    public class LifeTime : ExtendedMonoBehaviour
    {
        private float _currentTimeAlive = 0;

        public float Time;
        public float CurrentTimeAlive
        {
            get { return _currentTimeAlive; }
        }

        protected void LateUpdate()
        {
            _currentTimeAlive += UnityTime.deltaTime;
            if (_currentTimeAlive > this.Time)
            {
                this.DestroyGameObject();
            }
        }
    }
}
