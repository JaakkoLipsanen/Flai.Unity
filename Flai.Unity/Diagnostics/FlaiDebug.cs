using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Flai.Diagnostics
{
    public static class FlaiDebug
    {
        public static bool IsDeveloperConsoleVisible
        {
            get { return Debug.developerConsoleVisible; }
        }

        public static bool IsDebugBuild
        {
            get { return Debug.isDebugBuild; }
        }

        #region Draw Line

        public static void DrawLine(Vector3 start, Vector3 end)
        {
            Debug.DrawLine(start, end);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            Debug.DrawLine(start, end, color);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            Debug.DrawLine(start, end, color, duration);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
        {
            Debug.DrawLine(start, end, color, duration, depthTest);
        }

        #endregion

        #region Draw Ray

        public static void DrawRay(Vector3 start, Vector3 direction)
        {
            Debug.DrawRay(start, direction);
        }

        public static void DrawRay(Vector3 start, Vector3 direction, Color color)
        {
            Debug.DrawRay(start, direction, color);
        }

        public static void DrawRay(Vector3 start, Vector3 direction, Color color, float duration)
        {
            Debug.DrawRay(start, direction, color, duration);
        }

        public static void DrawRay(Vector3 start, Vector3 direction, Color color, float duration, bool depthTest)
        {
            Debug.DrawRay(start, direction, color, duration, depthTest);
        }

        #endregion

        #region Draw Rect Outlines

        public static void DrawRectangleOutlines(RectangleF rectangle)
        {
            Debug.DrawLine(rectangle.TopLeft, rectangle.TopRight);
            Debug.DrawLine(rectangle.TopRight, rectangle.BottomRight);
            Debug.DrawLine(rectangle.BottomRight, rectangle.BottomLeft);
            Debug.DrawLine(rectangle.BottomLeft, rectangle.TopLeft);
        }

        public static void DrawRectangleOutlines(RectangleF rectangle, Color color)
        {
            Debug.DrawLine(rectangle.TopLeft, rectangle.TopRight, color);
            Debug.DrawLine(rectangle.TopRight, rectangle.BottomRight, color);
            Debug.DrawLine(rectangle.BottomRight, rectangle.BottomLeft, color);
            Debug.DrawLine(rectangle.BottomLeft, rectangle.TopLeft, color);
        }

        public static void DrawRectangleOutlines(RectangleF rectangle, Color color, float time)
        {
            Ensure.True(time >= 0);
            Debug.DrawLine(rectangle.TopLeft, rectangle.TopRight, color, time);
            Debug.DrawLine(rectangle.TopRight, rectangle.BottomRight, color, time);
            Debug.DrawLine(rectangle.BottomRight, rectangle.BottomLeft, color, time);
            Debug.DrawLine(rectangle.BottomLeft, rectangle.TopLeft, color, time);
        }

        public static void DrawTransformedRectangleOutlines(TransformedRectangleF rectangle)
        {
            Debug.DrawLine(rectangle.TopLeft, rectangle.TopRight);
            Debug.DrawLine(rectangle.TopRight, rectangle.BottomRight);
            Debug.DrawLine(rectangle.BottomRight, rectangle.BottomLeft);
            Debug.DrawLine(rectangle.BottomLeft, rectangle.TopLeft);
        }

        public static void DrawTransformedRectangleOutlines(TransformedRectangleF rectangle, Color color)
        {
            Debug.DrawLine(rectangle.TopLeft, rectangle.TopRight, color);
            Debug.DrawLine(rectangle.TopRight, rectangle.BottomRight, color);
            Debug.DrawLine(rectangle.BottomRight, rectangle.BottomLeft, color);
            Debug.DrawLine(rectangle.BottomLeft, rectangle.TopLeft, color);
        }

        public static void DrawTransformedRectangleOutlines(TransformedRectangleF rectangle, Color color, float time)
        {
            Ensure.True(time >= 0);
            Debug.DrawLine(rectangle.TopLeft, rectangle.TopRight, color, time);
            Debug.DrawLine(rectangle.TopRight, rectangle.BottomRight, color, time);
            Debug.DrawLine(rectangle.BottomRight, rectangle.BottomLeft, color, time);
            Debug.DrawLine(rectangle.BottomLeft, rectangle.TopLeft, color, time);
        }

        #endregion

        #region Misc

        public static void Break()
        {
            Debug.Break();
        }

        public static void ClearDeveloperConsole()
        {
            Debug.ClearDeveloperConsole();
        }

        #endregion

        #region Log

        public static void Log(object message)
        {
            Debug.Log(message);
        }

        public static void LogIf(bool shouldLog, object message)
        {
            if (shouldLog)
            {
                FlaiDebug.Log(message);
            }
        }

        public static void Log(string format, params object[] parameters)
        {
            Debug.Log(string.Format(format, parameters));
        }

        public static void Log(object message, UnityObject context)
        {
            Debug.Log(message, context);
        }

        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        public static void LogWarning(string format, params object[] parameters)
        {
            Debug.LogWarning(string.Format(format, parameters));
        }

        public static void LogWarning(object message, UnityObject context)
        {
            Debug.LogWarning(message, context);
        }

        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        public static void LogError(string format, params object[] parameters)
        {
            Debug.LogError(string.Format(format, parameters));
        }

        public static void LogError(object message, UnityObject context)
        {
            Debug.LogError(message, context);
        }

        public static void LogEnter(string message)
        {
            Debug.Log("ENTER: " + message);
        }

        public static void LogExit(string message)
        {
            Debug.Log("EXIT: " + message);
        }

        public static void LogWithTypeTag<T>(string message)
        {
            FlaiDebug.Log("[" + typeof(T).Name + "] " + message);
        }

        public static void LogWithTypeTag<T>(string format, params object[] parameters)
        {
            FlaiDebug.Log("[" + typeof(T).Name + "] " + string.Format(format, parameters));
        }

        public static void LogWithTypeTag<T>(string format, UnityObject context, params object[] parameters)
        {
            Debug.Log("[" + typeof(T).Name + "] " + string.Format(format, parameters), context);
        }

        public static void LogWarningWithTypeTag<T>(string message)
        {
            FlaiDebug.LogWarning("[" + typeof(T).Name + "] " + message);
        }

        public static void LogWarningWithTypeTag<T>(string format, params object[] parameters)
        {
            FlaiDebug.LogWarning("[" + typeof(T).Name + "] " + string.Format(format, parameters));
        }

        public static void LogWarningWithTypeTag<T>(string format, UnityObject context, params object[] parameters)
        {
            Debug.LogWarning("[" + typeof(T).Name + "] " + string.Format(format, parameters), context);
        }

        public static void LogErrorWithTypeTag<T>(string message)
        {
            FlaiDebug.LogError("[" + typeof(T).Name + "] " + message);
        }

        public static void LogErrorWithTypeTag<T>(string format, params object[] parameters)
        {
            FlaiDebug.LogError("[" + typeof(T).Name + "] " + string.Format(format, parameters));
        }

        public static void LogErrorWithTypeTag<T>(string format, UnityObject context, params object[] parameters)
        {
            Debug.LogError("[" + typeof(T).Name + "] " + string.Format(format, parameters), context);
        }

        #endregion
    }
}