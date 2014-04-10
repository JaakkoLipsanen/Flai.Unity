using UnityEngine;

namespace Flai.Scene
{
    public class SceneDescription
    {
        private int _levelIndex;
        private string _levelName;

        public void Load()
        {
            if (_levelName != null)
            {
                Application.LoadLevel(_levelName);
            }
            else
            {
                Application.LoadLevel(_levelIndex);
            }
        }

        public static SceneDescription FromIndex(int index)
        {
            Ensure.WithinRange(index, 0, Application.levelCount - 1);
            return new SceneDescription { _levelIndex = index };
        }

        public static SceneDescription FromName(string name)
        {
            Ensure.NotNull(name);
            return new SceneDescription { _levelName = name };
        }
    }
}
