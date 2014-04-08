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

        public static Vector2 MousePosition
        {
            get { return UnityInput.mousePosition; }
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
    }
}
