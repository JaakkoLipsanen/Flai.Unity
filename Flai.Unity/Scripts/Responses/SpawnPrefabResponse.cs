using UnityEngine;

namespace Flai.Scripts.Responses
{
    public class SpawnPrefabResponse : Response
    {
        private bool _hasExecuted = false;
        public GameObject Prefab;
        public bool DestroyOnExecute = true;
        public bool AllowMultipleExecutes = true;
        protected override bool ExecuteInner()
        {
            if (!this.AllowMultipleExecutes && _hasExecuted)
            {
                return false;
            }

            this.Prefab.Instantiate(this.Position, this.RotationQuaternion);
            if (this.DestroyOnExecute)
            {
                this.DestroyGameObject();
            }

            _hasExecuted = true;
            return true;
        }
    }
}
