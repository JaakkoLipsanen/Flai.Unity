using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Flai.Tween
{
    /**
    * Animate along a set of points that need to be in the format: controlPoint, point1, point2.... pointLast, endControlPoint
    * @class LTSpline
    * @constructor
    * @param {Vector3 Array} pts A set of points that define the points the path will pass through (starting with starting control point, and ending with a control point)
    * @example 
    * LTSpline ltSpline = new LTSpline( new Vector3[] { new Vector3(0f,0f,0f),new Vector3(0f,0f,0f), new Vector3(0f,0.5f,0f), new Vector3(1f,1f,0f), new Vector3(1f,1f,0f)} );<br><br>
    * LeanTween.moveSpline(lt, ltSpline.vec3, 4.0f).setOrientToPath(true).setDelay(1f).setEase(LeanTweenType.easeInOutQuad); // animate <br>
    * Vector3 pt = ltSpline.point( 0.6f ); // retrieve a point along the path
    */

    [Serializable]
    public class TweenSpline
    {
        private readonly float[] lengthRatio;
        private readonly int numSections;
        private int currPt;
        private float[] lengths;
        public bool OrientToPath;
        public Vector3[] Points;
        private float TotalLength;

        public TweenSpline(params Vector3[] pts)
        {
            this.Points = new Vector3[pts.Length];
            Array.Copy(pts, this.Points, pts.Length);

            numSections = pts.Length - 3;
            int precision = 20;
            lengthRatio = new float[precision];
            lengths = new float[precision];

            var lastPoint = new Vector3(Mathf.Infinity, 0, 0);

            TotalLength = 0f;
            for (int i = 0; i < precision; i++)
            {
                //Debug.Log("i:"+i);
                float fract = (i * 1f) / precision;
                Vector3 point = Interp(fract);

                if (i >= 1)
                {
                    lengths[i] = (point - lastPoint).magnitude;
                    // Debug.Log("fract:"+fract+" mag:"+lengths[ i ] + " i:"+i);
                }
                TotalLength += lengths[i];

                lastPoint = point;
            }

            float ratioTotal = 0f;
            for (int i = 0; i < lengths.Length; i++)
            {
                float t = i * 1f / (lengths.Length - 1);
                currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);

                float ratioLength = lengths[i] / TotalLength;
                ratioTotal += ratioLength;
                lengthRatio[i] = ratioTotal;

                //Debug.Log("lengthRatio["+i+"]:"+lengthRatio[i]+" lengths["+i+"]:"+lengths[i] + " t:"+t);
            }
        }

        public float Map(float t)
        {
            //Debug.Log("map t:"+t);
            for (int i = 0; i < lengthRatio.Length; i++)
            {
                if (lengthRatio[i] >= t)
                {
                    // Debug.Log("map lengthRatio["+i+"]:"+lengthRatio[i]);
                    return lengthRatio[i] + (t - lengthRatio[i]);
                }
            }
            return 1f;
        }

        public Vector3 Interp(float t)
        {
            // The adjustments done to numSections, I am not sure why I needed to add them
            /*int numSections = this.numSections+1;
		if(numSections>=3)
			numSections += 1;*/
            currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
            float u = t * numSections - currPt;

            // Debug.Log("currPt:"+currPt+" numSections:"+numSections+" pts.Length :"+pts.Length );
            Vector3 a = Points[currPt];
            Vector3 b = Points[currPt + 1];
            Vector3 c = Points[currPt + 2];
            Vector3 d = Points[currPt + 3];

            return .5f * (
                (-a + 3f * b - 3f * c + d) * (u * u * u)
                + (2f * a - 5f * b + 4f * c - d) * (u * u)
                + (-a + c) * u
                + 2f * b
                );
        }

        /**
	* Retrieve a point along a path
	* 
	* @method point
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @return {Vector3} Vector3 position of the point along the path
	* @example
	* transform.position = ltSpline.point( 0.6f );
	*/

        public Vector3 Point(float ratio)
        {
            float t = Map(ratio);
            //Debug.Log("t:"+t+" ratio:"+ratio);
            //float t = ratio;
            return Interp(t);
        }

        /**
	* Place an object along a certain point on the path (facing the direction perpendicular to the path)
	* 
	* @method place
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @example
	* ltPath.place( transform, 0.6f );
	*/

        public void Place(Transform transform, float ratio)
        {
            Place(transform, ratio, Vector3.up);
        }

        /**
	* Place an object along a certain point on the path, with it facing a certain direction perpendicular to the path
	* 
	* @method place
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @param {Vector3} rotation:Vector3 the direction in which to place the transform ex: Vector3.up
	* @example
	* ltPath.place( transform, 0.6f, Vector3.left );
	*/

        public void Place(Transform transform, float ratio, Vector3 worldUp)
        {
            transform.position = Point(ratio);
            ratio += 0.001f;
            if (ratio <= 1.0f)
            {
                transform.LookAt(Point(ratio), worldUp);
            }
        }

        /**
	* Place an object along a certain point on the path (facing the direction perpendicular to the path) - Local Space, not world-space
	* 
	* @method placeLocal
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @example
	* ltPath.placeLocal( transform, 0.6f );
	*/

        public void PlaceLocal(Transform transform, float ratio)
        {
            PlaceLocal(transform, ratio, Vector3.up);
        }

        /**
	* Place an object along a certain point on the path, with it facing a certain direction perpendicular to the path - Local Space, not world-space
	* 
	* @method placeLocal
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @param {Vector3} rotation:Vector3 the direction in which to place the transform ex: Vector3.up
	* @example
	* ltPath.placeLocal( transform, 0.6f, Vector3.left );
	*/

        public void PlaceLocal(Transform transform, float ratio, Vector3 worldUp)
        {
            transform.localPosition = Point(ratio);
            ratio += 0.001f;
            if (ratio <= 1.0f)
            {
                transform.LookAt(transform.parent.TransformPoint(Point(ratio)), worldUp);
            }
        }

        public void GizmoDraw(float t = -1.0f)
        {
            if (lengthRatio != null && lengthRatio.Length > 0)
            {
                Vector3 prevPt = Point(0);

                for (int i = 1; i <= 120; i++)
                {
                    float pm = i / 120f;
                    Vector3 currPt = Point(pm);
                    Gizmos.DrawLine(currPt, prevPt);
                    prevPt = currPt;
                }
            }
        }

        public Vector3 Velocity(float t)
        {
            t = Map(t);

            int numSections = Points.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
            float u = t * numSections - currPt;

            Vector3 a = Points[currPt];
            Vector3 b = Points[currPt + 1];
            Vector3 c = Points[currPt + 2];
            Vector3 d = Points[currPt + 3];

            return 1.5f * (-a + 3f * b - 3f * c + d) * (u * u)
                   + (2f * a - 5f * b + 4f * c - d) * u
                   + .5f * c - .5f * a;
        }
    }
}
