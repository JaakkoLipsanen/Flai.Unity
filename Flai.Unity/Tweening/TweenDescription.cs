using System;
using System.Collections;
using UnityEngine;

namespace Flai.Tweening
{
    public class TweenDescription
    {
        private static uint _globalCounter;
        private uint _id;

        // todo: make these private.....?
        public bool Toggle;
        public bool UseEstimatedTime;
        public bool UseFrames;
        public bool HasInitiliazed;
        public bool HasPhysics;
        public float Passed;
        public float Delay;
        public float Time;
        public float LastVal;
        public int LoopCount;
        public uint Counter;
        public float Direction;
        public bool DestroyOnComplete;
        public Transform Trans;
        public TweenRectangle TweenRectangle;
        public Vector3 From;
        public Vector3 To;
        public Vector3 Diff;
        public Vector3 Point;
        public Vector3 Axis;
        public Vector3 OrigRotation;
        public TweenBezierPath Path;
        public TweenSpline Spline;
        internal TweenAction Type;
        public TweenType TweenType;
        public AnimationCurve AnimationCurve;
        public TweenType LoopType;
        public Action<float> OnUpdateFloat;
        public Action<float, object> OnUpdateFloatObject;
        public Action<Vector3> OnUpdateVector3;
        public Action<Vector3, object> OnUpdateVector3Object;
        public Action OnComplete;
        public Action<object> OnCompleteObject;
        public object OnCompleteParam;
        public object OnUpdateParam;

#if !UNITY_METRO
        public Hashtable Optional;
#endif


        public override string ToString()
        {
            return (Trans != null ? "gameObject:" + Trans.gameObject : "gameObject:null") + " toggle:" + Toggle + " passed:" + Passed + " time:" + Time + " delay:" + Delay + " from:" + From + " to:" + To + " type:" + Type + " useEstimatedTime:" + UseEstimatedTime + " id:" + ID + " hasInitiliazed:" + HasInitiliazed;
        }

        /**
	* Cancel a tween
	* 
	* @method cancel
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	*/

        public TweenDescription Cancel()
        {
            Tween.RemoveTween((int)_id);
            return this;
        }

        public int UniqueId
        {
            get
            {
                uint toId = _id | Counter << 16;

                /*uint backId = toId & 0xFFFF;
			uint backCounter = toId >> 16;
			if(_id!=backId || backCounter!=counter){
				Debug.LogError("BAD CONVERSION toId:"+_id);
			}*/

                return (int)toId;
            }
        }

        public int ID
        {
            get { return UniqueId; }
        }

        public void Reset()
        {
            Toggle = true;
#if !UNITY_METRO
            Optional = null;
#endif
            DestroyOnComplete = false;
            Passed = Delay = 0.0f;
            UseEstimatedTime = UseFrames = HasInitiliazed = false;
            AnimationCurve = null;
            TweenType = TweenType.Linear;
            LoopType = TweenType.Once;
            LoopCount = 0;
            Direction = LastVal = 1.0f;
            OnUpdateFloat = null;
            OnUpdateVector3 = null;
            OnUpdateFloatObject = null;
            OnUpdateVector3Object = null;
            OnComplete = null;
            OnCompleteObject = null;
            OnCompleteParam = null;
            Point = Vector3.zero;
            _globalCounter++;
        }

        /**
	* Pause a tween
	* 
	* @method pause
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	*/

        public TweenDescription Pause()
        {
            if (Direction != 0.0f)
            {
                // check if tween is already paused
                LastVal = Direction;
                Direction = 0.0f;
            }

            return this;
        }

        /**
	* Resume a paused tween
	* 
	* @method resume
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	*/

        public TweenDescription Resume()
        {
            Direction = LastVal;
            return this;
        }

        public TweenDescription SetAxis(Vector3 axis)
        {
            Axis = axis;
            return this;
        }

        /**
	* Delay the start of a tween
	* 
	* @method setDelay
	* @param {float} float time The time to complete the tween in
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setDelay( 1.5f );
	*/

        public TweenDescription SetDelay(float delay)
        {
            if (UseEstimatedTime)
            {
                Delay = delay;
            }
            else
            {
                Delay = delay * UnityEngine.Time.timeScale;
            }

            return this;
        }

        /**
	* set the type of easing used for the tween. <br>
	* <ul><li><a href="LeanTweenType.html">List of all the ease types</a>.</li>
	* <li><a href="http://www.robertpenner.com/easing/easing_demo.html">This page helps visualize the different easing equations</a></li>
	* </ul>
	* 
	* @method setEase
	* @param {LeanTweenType} easeType:LeanTweenType the easing type to use
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeInBounce );
	*/

        public TweenDescription SetEase(TweenType easeType)
        {
            TweenType = easeType;
            return this;
        }

        /**
	* set the type of easing used for the tween with a custom curve. <br>
	* @method setEase (AnimationCurve)
	* @param {AnimationCurve} easeDefinition:AnimationCurve an <a href="http://docs.unity3d.com/Documentation/ScriptReference/AnimationCurve.html" target="_blank">AnimationCure</a> that describes the type of easing you want, this is great for when you want a unique type of movement
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeInBounce );
	*/

        public TweenDescription SetEase(AnimationCurve easeCurve)
        {
            AnimationCurve = easeCurve;
            return this;
        }

        public TweenDescription SetTo(Vector3 to)
        {
            To = to;
            return this;
        }

        public TweenDescription SetFrom(Vector3 from)
        {
            From = from;
            HasInitiliazed = true; // this is set, so that the "from" value isn't overwritten later on when the tween starts
            Diff = To - From;
            return this;
        }

        public TweenDescription SetId(uint id)
        {
            _id = id;
            Counter = _globalCounter;
            return this;
        }

        /**
	* set the tween to repeat a number of times.
	* @method setRepeat
	* @param {int} repeatNum:int the number of times to repeat the tween. -1 to repeat infinite times
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 10 ).setLoopPingPong();
	*/

        public TweenDescription SetRepeat(int repeat)
        {
            LoopCount = repeat;
            if ((repeat > 1 && LoopType == TweenType.Once) || (repeat < 0 && LoopType == TweenType.Once))
            {
                LoopType = TweenType.Clamp;
            }
            return this;
        }

        public TweenDescription SetLoopType(TweenType loopType)
        {
            LoopType = loopType;
            return this;
        }

        /**
	* Use estimated time when tweening an object. Great for pause screens, when you want all other action to be stopped (or slowed down)
	* @method setUseEstimatedTime
	* @param {bool} useEstimatedTime:bool whether to use estimated time or not
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 2 ).setUseEstimatedTime( true );
	*/

        public TweenDescription SetUseEstimatedTime(bool useEstimatedTime)
        {
            UseEstimatedTime = useEstimatedTime;
            return this;
        }

        /**
	* Use frames when tweening an object, when you don't want the animation to be time-frame independent...
	* @method setUseFrames
	* @param {bool} useFrames:bool whether to use estimated time or not
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 2 ).setUseFrames( true );
	*/

        public TweenDescription SetUseFrames(bool useFrames)
        {
            UseFrames = useFrames;
            return this;
        }

        public TweenDescription SetLoopCount(int loopCount)
        {
            LoopCount = loopCount;
            return this;
        }

        /**
	* No looping involved, just run once (the default)
	* @method setLoopOnce
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setLoopOnce();
	*/

        public TweenDescription SetLoopOnce()
        {
            LoopType = TweenType.Once;
            return this;
        }

        /**
	* When the animation gets to the end it starts back at where it began
	* @method setLoopClamp
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat(2).setLoopClamp();
	*/

        public TweenDescription SetLoopClamp()
        {
            LoopType = TweenType.Clamp;
            if (LoopCount == 0)
            {
                LoopCount = -1;
            }
            return this;
        }

        /**
	* When the animation gets to the end it then tweens back to where it started (and on, and on)
	* @method setLoopPingPong
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat(2).setLoopPingPong();
	*/

        public TweenDescription SetLoopPingPong()
        {
            LoopType = TweenType.PingPong;
            if (LoopCount == 0)
            {
                LoopCount = -1;
            }
            return this;
        }

        /**
	* Have a method called when the tween finishes
	* @method setOnComplete
	* @param {Action} onComplete:Action the method that should be called when the tween is finished ex: tweenFinished(){ }
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnComplete( tweenFinished );
	*/

        public TweenDescription SetOnComplete(Action onComplete)
        {
            OnComplete = onComplete;
            return this;
        }

        /**
	* Have a method called when the tween finishes
	* @method setOnComplete (object)
	* @param {Action<object>} onComplete:Action<object> the method that should be called when the tween is finished ex: tweenFinished( object myObj ){ }
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnComplete( tweenFinished );
	*/

        public TweenDescription SetOnComplete(Action<object> onComplete)
        {
            OnCompleteObject = onComplete;
            return this;
        }

        public TweenDescription SetOnComplete(Action<object> onComplete, object onCompleteParam)
        {
            OnCompleteObject = onComplete;
            if (onCompleteParam != null)
            {
                OnCompleteParam = onCompleteParam;
            }
            return this;
        }

        /**
	* Pass an object to along with the onComplete Function
	* @method setOnCompleteParam
	* @param {object} onComplete:object an object that 
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnComplete( tweenFinished );
	*/

        public TweenDescription SetOnCompleteParam(object onCompleteParam)
        {
            OnCompleteParam = onCompleteParam;
            return this;
        }


        /**
	* Have a method called on each frame that the tween is being animated (passes a float value)
	* @method setOnUpdate
	* @param {Action<float>} onUpdate:Action<float> a method that will be called on every frame with the float value of the tweened object
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved );<br>
	* <br>
	* void tweenMoved( float val ){ }<br>
	*/

        public TweenDescription SetOnUpdate(Action<float> onUpdate)
        {
            OnUpdateFloat = onUpdate;
            return this;
        }

        public TweenDescription SetOnUpdateObject(Action<float, object> onUpdate)
        {
            OnUpdateFloatObject = onUpdate;
            return this;
        }

        public TweenDescription SetOnUpdateVector3(Action<Vector3> onUpdate)
        {
            OnUpdateVector3 = onUpdate;
            return this;
        }

#if !UNITY_FLASH
        /**
	* Have a method called on each frame that the tween is being animated (passes a float value and a object)
	* @method setOnUpdate (object)
	* @param {Action<float,object>} onUpdate:Action<float,object> a method that will be called on every frame with the float value of the tweened object, and an object of the person's choosing
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved ).setOnUpdateParam( myObject );<br>
	* <br>
	* void tweenMoved( float val, object obj ){ }<br>
	*/

        public TweenDescription SetOnUpdate(Action<float, object> onUpdate, object onUpdateParam = null)
        {
            OnUpdateFloatObject = onUpdate;
            if (onUpdateParam != null)
            {
                OnUpdateParam = onUpdateParam;
            }
            return this;
        }

        public TweenDescription SetOnUpdate(Action<Vector3, object> onUpdate, object onUpdateParam = null)
        {
            OnUpdateVector3Object = onUpdate;
            if (onUpdateParam != null)
            {
                OnUpdateParam = onUpdateParam;
            }
            return this;
        }

        /**
	* Have a method called on each frame that the tween is being animated (passes a float value)
	* @method setOnUpdate (Vector3)
	* @param {Action<Vector3>} onUpdate:Action<Vector3> a method that will be called on every frame with the float value of the tweened object
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved );<br>
	* <br>
	* void tweenMoved( Vector3 val ){ }<br>
	*/

        public TweenDescription SetOnUpdate(Action<Vector3> onUpdate, object onUpdateParam = null)
        {
            OnUpdateVector3 = onUpdate;
            if (onUpdateParam != null)
            {
                OnUpdateParam = onUpdateParam;
            }
            return this;
        }
#endif


        /**
	* Have an object passed along with the onUpdate method
	* @method setOnUpdateParam
	* @param {object} onUpdateParam:object an object that will be passed along with the onUpdate method
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved ).setOnUpdateParam( myObject );<br>
	* <br>
	* void tweenMoved( float val, object obj ){ }<br>
	*/

        public TweenDescription SetOnUpdateParam(object onUpdateParam)
        {
            OnUpdateParam = onUpdateParam;
            return this;
        }

        /**
	* While tweening along a curve, set this property to true, to be perpendicalur to the path it is moving upon
	* @method setOrientToPath
	* @param {bool} doesOrient:bool whether the gameobject will orient to the path it is animating along
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.move( ltLogo, path, 1.0f ).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true).setAxis(Vector3.forward);<br>
	*/

        public TweenDescription SetOrientToPath(bool doesOrient)
        {
            if (Type == TweenAction.MOVE_CURVED || Type == TweenAction.MOVE_CURVED_LOCAL)
            {
                if (Path == null)
                {
                    Path = new TweenBezierPath();
                }
                Path.OrientToPath = doesOrient;
            }
            else
            {
                Spline.OrientToPath = doesOrient;
            }
            return this;
        }

        public TweenDescription SetRect(TweenRectangle rect)
        {
            TweenRectangle = rect;
            return this;
        }

        public TweenDescription SetRect(Rect rect)
        {
            TweenRectangle = new TweenRectangle(rect);
            return this;
        }

        public TweenDescription SetPath(TweenBezierPath path)
        {
            Path = path;
            return this;
        }

        /**
	* set the point at which the GameObject will be rotated around
	* @method setPoint
	* @param {Vector3} point:Vector3 point at which you want the object to rotate around (local space)
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.rotateAround( cube, Vector3.up, 360.0f, 1.0f ) .setPoint( new Vector3(1f,0f,0f) ) .setEase( LeanTweenType.easeInOutBounce );<br>
	*/

        public TweenDescription SetPoint(Vector3 point)
        {
            Point = point;
            return this;
        }

        public TweenDescription SetDestroyOnComplete(bool doesDestroy)
        {
            DestroyOnComplete = doesDestroy;
            return this;
        }

        public TweenDescription SetAudio(object audio)
        {
            OnCompleteParam = audio;
            return this;
        }
    }
}