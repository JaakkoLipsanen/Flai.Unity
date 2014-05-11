using UnityEngine;

namespace Flai.Scripts.Responses
{
    public class ExecuteResponseOnTrigger2D : FlaiScript
    {
        private bool _hasTriggered = false;
        public Response Response;
        public bool CanExecuteMultipleTimes = false;
        public TriggerType TriggerType;

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (this.TriggerType != TriggerType.Enter || this.TriggerType != TriggerType.Stay)
            {
                return;
            }

            this.TryExecute(other);
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (this.TriggerType != TriggerType.Enter || this.TriggerType != TriggerType.Stay)
            {
                return;
            }

            this.TryExecute(other);
        }

        private void TryExecute(Collider2D other)
        {
            if ((!_hasTriggered || this.CanExecuteMultipleTimes) && this.AllowExecute(other))
            {
                if (this.Response != null)
                {
                    this.Response.ExecuteOn();
                }

                _hasTriggered = true;
            }
        }

        protected virtual bool AllowExecute(Collider2D other)
        {
            return true;
        }
    }
}
