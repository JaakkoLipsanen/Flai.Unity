using Flai.Diagnostics;
using Flai.Graphics;
using UnityEngine;

using FadeClass = Flai.Scene.Fade;

namespace Flai.Scene
{
    // TODO maybe: Scene.Load/Scene.ChangeScene/Scene.Switch. "SceneFader" name isn't that good imo :|
    public class SceneFader : Singleton<SceneFader>
    {
        private float? _fadeDelay = null;
        private SceneDescription _newScene;
        private Fade _fadeIn;
        private Fade _fadeOut;
        private float _alpha;
        private int _framesUntilLoad = -1; // this frames until load stuff is because otherwise it wont for some reason render to completely black, and thus there will be a visual "jump" while loading.
        private float _currentFadeTime = 0f;

        public bool IsFadingIn
        {
            get { return _fadeIn != null; }
        }

        public bool IsFadingOut
        {
            get { return !this.IsFadingIn && _fadeOut != null; }
        }

        public bool IsFading
        {
            get { return this.IsFadingIn || this.IsFadingOut; }
        }

        private ColorF CurrentColor
        {
            get
            {
                if (this.IsFadingIn) return _fadeIn.Color;
                if (this.IsFadingOut) return _fadeOut.Color;
                return ColorF.Transparent;
            }
        }

        private Fade CurrentFade
        {
            get
            {
                Ensure.True(this.IsFading);
                return this.IsFadingIn ? _fadeIn : _fadeOut;
            }
        }

        private bool IsFadeDelayRunning
        {
            get { return _fadeDelay.HasValue; }
        }

        protected override void Awake()
        {
            this.DontDestroyOnLoad();
        }

        public static void Fade(SceneDescription newScene, Fade fadeIn = null, Fade fadeOut = null, float delay = 0)
        {
            // for some reason using FadeClass.Default doesn't work :| todo: fix it?
            fadeIn = fadeIn ?? FadeClass.Create();
            fadeOut = fadeOut ?? FadeClass.Create();
            Ensure.NotNull(newScene, fadeIn, fadeOut);
            Ensure.True(delay >= 0f);
            SceneFader.Instance.StartFade(newScene, fadeIn, fadeOut, delay);
        }

        private void StartFade(SceneDescription newScene, Fade fadeIn, Fade fadeOut, float delay = 0)
        {
            if (this.IsFading)
            {
                FlaiDebug.LogWarningWithTypeTag<SceneFader>("Can't start a new fade. There is already another fade in progress", this);
                return;
            }
            else if (this.IsFadeDelayRunning)
            {
                FlaiDebug.LogWarningWithTypeTag<SceneFader>("Can't start a new fade. There is already another fade pending (delay: " + _fadeDelay.Value + "}", this);
                return;
            }

            _fadeIn = fadeIn;
            _fadeOut = fadeOut;
            _newScene = newScene;
            _fadeDelay = (delay == 0) ? default(float?) : delay;

            if (!_fadeDelay.HasValue)
            {
                if (_fadeIn.Time <= 0f)
                {
                    if (_fadeOut.Time <= 0f)
                    {
                        this.LoadLevel();
                    }
                    else
                    {
                        this.StartFade(_fadeOut, 0, 1);
                    }

                    _fadeIn = null;
                    return;
                }

                this.StartFade(_fadeIn, 0, 1);
            }
        }

        protected override void Update()
        {
            if (this.IsFadeDelayRunning)
            {
                _fadeDelay -= Time.deltaTime;
                if (_fadeDelay <= 0)
                {
                    _fadeDelay = null;
                    this.StartFade(_fadeIn, 0, 1);
                }
            }
            else if (this.IsFading)
            {
                _currentFadeTime += Time.deltaTime;
                _alpha = FlaiMath.Min(1, _currentFadeTime / this.CurrentFade.Time);
                _alpha = this.IsFadingIn ? _alpha : (1 - _alpha);
                if (_currentFadeTime >= this.CurrentFade.Time)
                {
                    _currentFadeTime = 0;
                    this.OnFadeCompleted();
                }
            }
        }

        protected override void OnGUI()
        {
            if (this.IsFading)
            {
                GUI.color = this.CurrentColor * _alpha;
                GUI.depth = -int.MaxValue;

                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), TextureHelper.BlankTexture);

                if (_framesUntilLoad == 0)
                {
                    _framesUntilLoad = -1;
                    this.LoadLevel();
                    this.StartFade(_fadeOut, 1, 0);
                }
                else if (_framesUntilLoad > 0)
                {
                    _framesUntilLoad--;
                }
            }
        }

        private void StartFade(Fade fade, float from, float to)
        {
            if (fade.Time <= 0f)
            {
                _alpha = to;
                this.OnFadeCompleted();
                return;
            }

            _currentFadeTime = 0f;
            _alpha = from;
        }

        private void OnFadeCompleted()
        {
            if (this.IsFadingIn)
            {
                Ensure.NotNull(_fadeOut, _newScene);
                _fadeIn = null;
                _alpha = 1;

                _framesUntilLoad = 2;
            }
            else if (this.IsFadingOut)
            {
                _fadeOut = null;
                _alpha = 0;
                _currentFadeTime = 0f;
            }
        }

        private void LoadLevel()
        {
            Ensure.NotNull(_newScene);
            _newScene.Load();
            _newScene = null;
        }
    }
}
