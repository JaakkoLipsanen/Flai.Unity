using UnityEngine;

namespace Flai.Scripts.Rendering
{
    public class MaterialUvOffsetter : FlaiScript
    {
        public Material Material;
        public Vector2 OffsetPerSecond;

        protected override void Update()
        {
            this.Material.mainTextureOffset += this.OffsetPerSecond * Time.deltaTime;
        }
    }
}
