using UnityEngine;

namespace Flai.Scripts
{
    public class FpsCounter
    {
        private const float AvgFpsPeriod = 0.25f;

        private int _ticks = 0;
        private float _timeSinceAvgFpsStep = 0;

        public float AvgFps { get; private set; }
        public float Fps { get; private set; }

        public void Update()
        {
            _ticks++;
            _timeSinceAvgFpsStep += Time.unscaledDeltaTime;
            if (_timeSinceAvgFpsStep >= AvgFpsPeriod)
            {
                _timeSinceAvgFpsStep -= AvgFpsPeriod;
                this.AvgFps = _ticks / AvgFpsPeriod;
                _ticks = 0;
            }

            this.Fps = 1f / Time.unscaledDeltaTime;
        }

    }

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
            GUI.Label(new Rect(this.Position.X, this.Position.Y + 28, 90, 20), _avgFpsString);
        }
    }
}
