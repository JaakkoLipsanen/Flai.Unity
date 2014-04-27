using Flai.Diagnostics;
using UnityEngine;

namespace Flai.Scripts
{
    public class DestroyOnTrigger : FlaiScript
    {
        public enum DestroyOnTriggerStyle
        {
            Enter,
            Exit,
        }

        public DestroyOnTriggerStyle Style = DestroyOnTriggerStyle.Enter;

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (this.Style == DestroyOnTriggerStyle.Enter)
            {
                other.gameObject.DestroyIfNotNull();
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (this.Style == DestroyOnTriggerStyle.Exit)
            {
                other.gameObject.DestroyIfNotNull();
            }
        }
    }
}
