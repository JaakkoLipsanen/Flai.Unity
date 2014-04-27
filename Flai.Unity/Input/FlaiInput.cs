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

    public static class FlaiInput
    {
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

        #endregion

        #region Button & Key Combined ("Safeguard")

        // not a good name. basically, use button 'buttonName' if it exists, otherwise use 'alternativeKey'
        public static bool IsButtonOrKeyPressed(string buttonName, KeyCode alternativeKey)
        {
            if (FlaiInput.IsButtonSetUp(buttonName))
            {
                return FlaiInput.IsButtonPressed(buttonName);
            }

            return FlaiInput.IsKeyPressed(alternativeKey);
        }

        // not a good name. basically, use button 'buttonName' if it exists, otherwise use 'alternativeKey'
        public static bool IsButtonOrKeyReleased(string buttonName, KeyCode alternativeKey)
        {
            if (FlaiInput.IsButtonSetUp(buttonName))
            {
                return FlaiInput.IsButtonReleased(buttonName);
            }

            return FlaiInput.IsKeyReleased(alternativeKey);
        }

        // not a good name. basically, use button 'buttonName' if it exists, otherwise use 'alternativeKey'
        public static bool IsNewButtonOrKeyPress(string buttonName, KeyCode alternativeKey)
        {
            if (FlaiInput.IsButtonSetUp(buttonName))
            {
                return FlaiInput.IsNewButtonPress(buttonName);
            }

            return FlaiInput.IsNewKeyPress(alternativeKey);
        }

        // not a good name. basically, use button 'buttonName' if it exists, otherwise use 'alternativeKey'
        public static bool IsNewButtonOrKeyRelease(string buttonName, KeyCode alternativeKey)
        {
            if (FlaiInput.IsButtonSetUp(buttonName))
            {
                return FlaiInput.IsNewButtonRelease(buttonName);
            }

            return FlaiInput.IsNewKeyRelease(alternativeKey);
        }

        #endregion

        #region Misc

        private static readonly Dictionary<string, bool> _buttonExistsMap = new Dictionary<string, bool>(); 
        // does a button with a name of 'name' exist?
        public static bool IsButtonSetUp(string name)
        {
            if (_buttonExistsMap.ContainsKey(name))
            {
                return _buttonExistsMap[name];
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

            _buttonExistsMap.Add(name, exists);
            return exists;
        }

        #endregion
    }
}
