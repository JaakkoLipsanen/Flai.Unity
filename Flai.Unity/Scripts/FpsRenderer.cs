using UnityEngine;

namespace Flai.Scripts
{
    public class FpsRenderer : Singleton<FpsRenderer>
    {
        private int _ticks = 0;
        private float _timeSinceAvgFpsStep = 0;
        private string _avgFpsString = "0 FPS";

        public Vector2f Position = new Vector2f(4, 4);

        protected override void Update()
        {
            _ticks++;
            _timeSinceAvgFpsStep += Time.unscaledDeltaTime;
            if (_timeSinceAvgFpsStep >= 1)
            {
                _timeSinceAvgFpsStep -= 1;
                _avgFpsString = _ticks + " FPS";
                _ticks = 0;
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(this.Position.X, this.Position.Y, 90, 20), "Current: " + FlaiMath.Round(1f / Time.unscaledDeltaTime).ToString());
            GUI.Label(new Rect(this.Position.X, this.Position.Y + 28, 90, 20),  _avgFpsString);
        }
    }
}
