using UnityEngine;

namespace Flai.Tween
{
    public class TweenBezier
    {
        private readonly Vector3 a;
        private readonly Vector3 aa;
        private readonly float[] arcLengths;
        private readonly Vector3 bb;
        private readonly Vector3 cc;
        private readonly float len;
        public float Length;

        public TweenBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float precision)
        {
            this.a = a;
            aa = (-a + 3 * (b - c) + d);
            bb = 3 * (a + c) - 6 * b;
            cc = 3 * (b - a);

            len = 1.0f / precision;
            arcLengths = new float[(int)len + 1];
            arcLengths[0] = 0;

            Vector3 ov = a;
            Vector3 v;
            float clen = 0.0f;
            for (int i = 1; i <= len; i++)
            {
                v = BezierPoint(i * precision);
                clen += (ov - v).magnitude;
                arcLengths[i] = clen;
                ov = v;
            }
            Length = clen;
        }

        private float Map(float u)
        {
            float targetLength = u * arcLengths[(int)len];
            int low = 0;
            var high = (int)len;
            int index = 0;
            while (low < high)
            {
                index = low + ((int)((high - low) / 2.0f) | 0);
                if (arcLengths[index] < targetLength)
                {
                    low = index + 1;
                }
                else
                {
                    high = index;
                }
            }
            if (arcLengths[index] > targetLength)
            {
                index--;
            }
            if (index < 0)
            {
                index = 0;
            }

            return (index + (targetLength - arcLengths[index]) / (arcLengths[index + 1] - arcLengths[index])) / len;
        }

        private Vector3 BezierPoint(float t)
        {
            return ((aa * t + (bb)) * t + cc) * t + a;
        }

        public Vector3 PointAt(float t)
        {
            return BezierPoint(Map(t));
        }
    }
}
