using Flai.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace Flai.Input
{
    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2,
    }

    public enum PressState
    {
        None,
        Pressed,
        Hold,
        Released
    }

    public class FlaiInput : Singleton<FlaiInput>
    {
        private static Vector2f? _previousMousePosition;
        public static bool IsAnyKeyPressed
        {
            get { return UnityInput.anyKey; }
        }

        public static bool IsNewAnyKeyPress
        {
            get { return UnityInput.anyKeyDown; }
        }

        public static string InputString
        {
            get { return UnityInput.inputString; }
        }

        public static Vector2f MousePosition
        {
            get { return UnityInput.mousePosition; }
        }

        public static Vector2f PreviousMousePosition
        {
            get { return _previousMousePosition ?? FlaiInput.MousePosition; }
        }

        public static Vector2f MouseDelta
        {
            get
            {
                FlaiInput.EnsureInstanceExists();
                return FlaiInput.MousePosition - FlaiInput.PreviousMousePosition;
            }
        }

        public static float ScrollWheelDelta
        {
            get
            {
                const string ScrollWheelInputName = "Mouse ScrollWheel";
                if (!FlaiInput.IsInputSetup(ScrollWheelInputName))
                {
                    FlaiDebug.LogWarningWithTypeTag<FlaiInput>("'{0}' doesn't exist in InputManager!", ScrollWheelInputName);
                    return 0;
                }

                return FlaiInput.GetAxis(ScrollWheelInputName);
            }
        }

        public static Vector2f MousePositionInWorld2D
        {
            get { return Camera.main.ScreenToWorldPoint(FlaiInput.MousePosition); }
        }

        public static Ray MouseToWorldRay
        {
            get { return Camera.main.ScreenPointToRay(FlaiInput.MousePosition); }
        }

        #region Axis

        public static float GetAxis(string name)
        {
            return UnityInput.GetAxis(name);
        }

        public static float GetAxisRaw(string name)
        {
            return UnityInput.GetAxisRaw(name);
        }

        public static void ResetInputAxes()
        {
            UnityInput.ResetInputAxes();
        }

        #endregion

        #region Key

        public static bool IsKeyPressed(KeyCode key)
        {
            return UnityInput.GetKey(key);
        }

        public static bool IsKeyPressed(string name)
        {
            return UnityInput.GetKey(name);
        }

        public static bool IsKeyReleased(KeyCode key)
        {
            return !UnityInput.GetKey(key);
        }

        public static bool IsKeyReleased(string name)
        {
            return !UnityInput.GetKey(name);
        }

        public static bool IsNewKeyPress(KeyCode key)
        {
            return UnityInput.GetKeyDown(key);
        }

        public static bool IsNewKeyPress(string name)
        {
            return UnityInput.GetKeyDown(name);
        }

        public static bool IsNewKeyRelease(KeyCode key)
        {
            return UnityInput.GetKeyUp(key);
        }

        public static bool IsNewKeyRelease(string name)
        {
            return UnityInput.GetKeyUp(name);
        }

        #endregion

        #region Button

        public static bool IsButtonPressed(string name)
        {
            return UnityInput.GetButton(name);
        }

        public static bool IsButtonReleased(string name)
        {
            return !UnityInput.GetButton(name);
        }

        public static bool IsNewButtonPress(string name)
        {
            return UnityInput.GetButtonDown(name);
        }

        public static bool IsNewButtonRelease(string name)
        {
            return UnityInput.GetButtonUp(name);
        }

        #endregion

        #region MouseButton

        public static bool IsMouseButtonPressed(MouseButton mouseButton)
        {
            return UnityInput.GetMouseButton((int)mouseButton);
        }

        public static bool IsMouseButtonReleased(MouseButton mouseButton)
        {
            return !UnityInput.GetMouseButton((int)mouseButton);
        }

        public static bool IsNewMouseButtonPress(MouseButton mouseButton)
        {
            return UnityInput.GetMouseButtonDown((int)mouseButton);
        }

        public static bool IsNewMouseButtonRelease(MouseButton mouseButton)
        {
            return UnityInput.GetMouseButtonUp((int)mouseButton);
        }

        public static bool WasMouseButtonPressed(MouseButton mouseButton)
        {
            return FlaiInput.IsMouseButtonPressed(mouseButton) && !FlaiInput.IsNewMouseButtonPress(mouseButton);
        }

        public static PressState GetMouseButtonState(MouseButton mouseButton)
        {
            if (FlaiInput.IsMouseButtonPressed(mouseButton))
            {
                return FlaiInput.IsNewMouseButtonPress(mouseButton) ? PressState.Pressed : PressState.Hold;
            }

            return FlaiInput.IsNewMouseButtonRelease(mouseButton) ? PressState.Released : PressState.None;
        }

        #endregion

        #region Button & Key Combined ("Safeguard")

        // not a good name. basically, use button 'buttonName' if it exists, otherwise use 'alternativeKey'
        public static bool IsButtonOrKeyPressed(string buttonName, KeyCode alternativeKey)
        {
            if (FlaiInput.IsInputSetup(buttonName))
            {
                return FlaiInput.IsButtonPressed(buttonName);
            }

            return FlaiInput.IsKeyPressed(alternativeKey);
        }

        // not a good name. basically, use button 'buttonName' if it exists, otherwise use 'alternativeKey'
        public static bool IsButtonOrKeyReleased(string buttonName, KeyCode alternativeKey)
        {
            if (FlaiInput.IsInputSetup(buttonName))
            {
                return FlaiInput.IsButtonReleased(buttonName);
            }

            return FlaiInput.IsKeyReleased(alternativeKey);
        }

        // not a good name. basically, use button 'buttonName' if it exists, otherwise use 'alternativeKey'
        public static bool IsNewButtonOrKeyPress(string buttonName, KeyCode alternativeKey)
        {
            if (FlaiInput.IsInputSetup(buttonName))
            {
                return FlaiInput.IsNewButtonPress(buttonName);
            }

            return FlaiInput.IsNewKeyPress(alternativeKey);
        }

        // not a good name. basically, use button 'buttonName' if it exists, otherwise use 'alternativeKey'
        public static bool IsNewButtonOrKeyRelease(string buttonName, KeyCode alternativeKey)
        {
            if (FlaiInput.IsInputSetup(buttonName))
            {
                return FlaiInput.IsNewButtonRelease(buttonName);
            }

            return FlaiInput.IsNewKeyRelease(alternativeKey);
        }

        #endregion

        #region Misc

        private static readonly Dictionary<string, bool> _inputExistsMap = new Dictionary<string, bool>(); 
        // does a button with a name of 'name' exist?
        public static bool IsInputSetup(string name)
        {
            if (_inputExistsMap.ContainsKey(name))
            {
                return _inputExistsMap[name];
            }

            bool exists;
            // this is a really horrible way to do this since throwing exceptions "for no reason" sucks...
            try
            {
                UnityInput.GetButton(name);
                exists = true;
            }
            catch
            {
                exists = false;
            }

            _inputExistsMap.Add(name, exists);
            return exists;
        }

        #endregion

        protected override void LateUpdate()
        {
            _previousMousePosition = FlaiInput.MousePosition;
        }
    }
}
