using UnityEngine;

namespace Flai.Tween
{
    /**
    * Manually animate along a bezier path with this class
    * @class LTBezierPath
    * @constructor
    * @param {Vector3 Array} pts A set of points that define one or many bezier paths (the paths should be passed in multiples of 4, which correspond to each individual bezier curve)
    * @example 
    * LTBezierPath ltPath = new LTBezierPath( new Vector3[] { new Vector3(0f,0f,0f),new Vector3(1f,0f,0f), new Vector3(1f,0f,0f), new Vector3(1f,1f,0f)} );<br><br>
    * LeanTween.move(lt, ltPath.vec3, 4.0f).setOrientToPath(true).setDelay(1f).setEase(LeanTweenType.easeInOutQuad); // animate <br>
    * Vector3 pt = ltPath.point( 0.6f ); // retrieve a point along the path
    */

    public class TweenBezierPath
    {
        private TweenBezier[] beziers;
        private float[] lengthRatio;

        public bool OrientToPath;
        public Vector3[] Points;
        public float Length;

        public TweenBezierPath()
        {
        }

        public TweenBezierPath(Vector3[] pts_)
        {
            SetPoints(pts_);
        }

        public void SetPoints(Vector3[] pts_)
        {
            if (pts_.Length < 4)
            {
                Tween.LogError("LeanTween - When passing values for a vector path, you must pass four or more values!");
            }
            if (pts_.Length % 4 != 0)
            {
                Tween.LogError("LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...");
            }

            Points = pts_;

            int k = 0;
            beziers = new TweenBezier[Points.Length / 4];
            lengthRatio = new float[beziers.Length];
            int i;
            Length = 0;
            for (i = 0; i < Points.Length; i += 4)
            {
                beziers[k] = new TweenBezier(Points[i + 0], Points[i + 2], Points[i + 1], Points[i + 3], 0.05f);
                Length += beziers[k].Length;
                k++;
            }
            // Debug.Log("beziers.Length:"+beziers.Length + " beziers:"+beziers);
            for (i = 0; i < beziers.Length; i++)
            {
                lengthRatio[i] = beziers[i].Length / Length;
            }
        }

        /**
	* Retrieve a point along a path
	* 
	* @method point
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @return {Vector3} Vector3 position of the point along the path
	* @example
	* transform.position = ltPath.point( 0.6f );
	*/

        public Vector3 Point(float ratio)
        {
            float added = 0.0f;
            for (int i = 0; i < lengthRatio.Length; i++)
            {
                added += lengthRatio[i];
                if (added >= ratio)
                {
                    return beziers[i].PointAt((ratio - (added - lengthRatio[i])) / lengthRatio[i]);
                }
            }
            return beziers[lengthRatio.Length - 1].PointAt(1.0f);
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
    }

}
