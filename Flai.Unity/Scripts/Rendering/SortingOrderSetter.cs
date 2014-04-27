using Flai.Diagnostics;
using UnityEngine;

namespace Flai.Scripts.Rendering
{
    [ExecuteInEditMode]
    public class SortingOrderSetter : FlaiScript
    {
        public int SortingOrder = 0;
        public bool SetEveryFrame = false;
        protected override void Awake()
        {
            var renderer = this.renderer;
            if (renderer == null)
            {
                FlaiDebug.LogWarningWithTypeTag<SortingOrderSetter>("Renderer is null");
            }
            else
            {
                renderer.sortingOrder = this.SortingOrder;
            }
        }

        protected override void Update()
        {
            if (this.SetEveryFrame)
            {
                var r = this.renderer;
                if (r != null)
                {
                    r.sortingOrder = this.SortingOrder;
                }
            }
        }
    }
}
