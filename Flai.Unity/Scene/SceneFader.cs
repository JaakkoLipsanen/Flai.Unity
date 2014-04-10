using Flai.Diagnostics;
using Flai.Graphics;
using System;
using UnityEngine;

namespace Flai.Scene
{
    public class SceneFader : Singleton<SceneFader>
    {
        private float? _fadeDelay = null;
        private SceneDescription _newScene;
        private Fade _fadeIn;
        private Fade _fadeOut;
        private float _alpha;
        private int _framesUntilLoad = -1;

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

        private bool IsFadeDelayRunning
        {
            get { return _fadeDelay.HasValue; }
        }

        protected override void Awake()
        {
             this.DontDestroyOnLoad();
        }

        public static void Fade(SceneDescription newScene, Fade fadeIn, Fade fadeOut, float delay = 0)
        {
            Ensure.NotNull(newScene, fadeIn, fadeOut);
            Ensure.True(delay >= 0f);
            SceneFader.Instance.StartFade(newScene, fadeIn, fadeOut, delay);
        }

        private void StartFade(SceneDescription newScene, Fade fadeIn, Fade fadeOut, float delay = 0)
        {
            if (this.IsFading)
            {
                FlaiDebug.LogErrorWithTypeTag<SceneFader>("Error: Can't start a new fade. There is already another fade in progress", this);
                throw new ArgumentException("Error: Can't start a new fade. There is already another fade in progress");
            }
            else if (this.IsFadeDelayRunning)
            {
                FlaiDebug.LogErrorWithTypeTag<SceneFader>("Error: Can't start a new fade. There is already another fade pending (delay: " + _fadeDelay.Value + "}", this);
                throw new ArgumentException("Error: Can't start a new fade. There is already another fade pending (delay: " + _fadeDelay.Value + "}");
            }

            _fadeIn = fadeIn;
            _fadeOut = fadeOut;
            _newScene = newScene;
            _fadeDelay = (delay == 0) ? default(float?) : delay;

            if (!_fadeDelay.HasValue)
            {
                this.StartFade(_fadeIn, 0, 1);
            }
        }

        private void OnTweenUpdate(float value)
        {
            _alpha = value;
        }

        private void OnTweenCompleted()
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
            Flai.Tween.Tween.Value(this.GameObject, this.OnTweenUpdate, from, to, fade.Time).SetEase(fade.TweenType).SetOnComplete(this.OnTweenCompleted);
            _alpha = from;
        }

        private void LoadLevel()
        {
            Ensure.NotNull(_newScene);
            _newScene.Load();
            _newScene = null;
        }
    }
}
