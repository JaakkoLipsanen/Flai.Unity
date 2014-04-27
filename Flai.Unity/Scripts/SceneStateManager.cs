using Flai.Input;
using Flai.Scene;
using UnityEngine;

namespace Flai.Scripts
{
    public class SceneStateManager : FlaiScript
    {
        public KeyCode LoadFirstSceneKey = KeyCode.Escape;
        public KeyCode ReloadSceneKey = KeyCode.Backspace;
        public KeyCode NextSceneKey = KeyCode.PageUp;
        public KeyCode PreviousSceneKey = KeyCode.PageDown;

        protected override void Update()
        {
            if (FlaiInput.IsNewKeyPress(this.LoadFirstSceneKey))
            {
                SceneFader.Fade(SceneDescription.FromIndex(0), Fade.Create(0.75f), Fade.Create(0.75f));
            }
            else if (FlaiInput.IsNewKeyPress(this.ReloadSceneKey))
            {
                SceneFader.Fade(SceneDescription.FromIndex(Application.loadedLevel), Fade.Create(0.15f), Fade.Create(0.15f));
            }
            else if (FlaiInput.IsNewKeyPress(this.PreviousSceneKey))
            {
                SceneFader.Fade(SceneDescription.FromIndex(Application.loadedLevel - 1), Fade.Create(0.15f), Fade.Create(0.15f));
            }
            else if (FlaiInput.IsNewKeyPress(this.NextSceneKey))
            {
                SceneFader.Fade(SceneDescription.FromIndex((Application.loadedLevel == Application.levelCount - 1) ? 0 : Application.loadedLevel + 1), Fade.Create(0.15f), Fade.Create(0.15f));
            }
        }
    }
}
