using UnityEditor;
using UnityEngine;

namespace Flai.UI
{
    public static class FlaiHandles
    {
        private static readonly Vector3[] _solidRectangleVertices = new Vector3[4];
        public static void DrawSolidRectangleWithOutline(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 vertex4, Color faceColor, Color outlineColor)
        {
            _solidRectangleVertices[0] = vertex1;
            _solidRectangleVertices[1] = vertex2;
            _solidRectangleVertices[2] = vertex3;
            _solidRectangleVertices[3] = vertex4;

            Handles.DrawSolidRectangleWithOutline(_solidRectangleVertices, faceColor, outlineColor);
        }
    }
}
