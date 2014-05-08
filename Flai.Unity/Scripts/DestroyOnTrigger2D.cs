using UnityEngine;

namespace Flai.Scripts
{
    public class DestroyOnTrigger2D : FlaiScript
    {
        public TriggerType Type = TriggerType.Enter;

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (this.Type == TriggerType.Enter || this.Type == TriggerType.Stay)
            {
                other.gameObject.DestroyIfNotNull();
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (this.Type == TriggerType.Exit || this.Type == TriggerType.Stay)
            {
                other.gameObject.DestroyIfNotNull();
            }
        }
    }
}
