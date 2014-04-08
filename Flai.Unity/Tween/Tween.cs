#region License

// Copyright (c) 2013 Russell Savage - Dented Pixel
// 
// LeanTween version 2.14 - http://dentedpixel.com/developer-diary/
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

/*
TERMS OF USE - EASING EQUATIONS#
Open source under the BSD License.
Copyright (c)2001 Robert Penner
All rights reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#endregion

using System;
using System.Collections;
using UnityEngine;
using Object = System.Object;

namespace Flai.Tween
{
    /**
    * LeanTween is an efficient tweening engine for Unity3d<br><br>
    * <a href="#index">Index of All Methods</a> | <a href="LTDescr.html">Optional Paramaters that can be passed</a><br><br>
    * <strong id='optional'>Optional Parameters</strong> are passed at the end of every method<br> 
    * <br>
    * <i>Example:</i><br>
    * LeanTween.moveX( gameObject, 1f, 1f).setEase( <a href="LeanTweenType.html">LeanTweenType</a>.easeInQuad ).setDelay(1f);<br>
    * <br>
    * You can pass the optional parameters in any order, and chain on as many as you wish.<br>
    * You can also pass parameters at a later time by saving a reference to what is returned.<br>
    * <br>
    * <i>Example:</i><br>
    * <a href="LTDescr.html">LTDescr</a> d = LeanTween.moveX(gameObject, 1f, 1f);<br>
    *  &nbsp; ...later set some parameters<br>
    * d.setOnComplete( onCompleteFunc ).setEase( <a href="LeanTweenType.html">LeanTweenType</a>.easeInOutBack );<br>
    *
    * @class LeanTween
    */

    public class Tween : MonoBehaviour
    {
        public static bool LogErrors = true;

        private static TweenDescription[] tweens;
        private static int tweenMaxSearch;
        private static int maxTweens = 400;
        private static int frameRendered = -1;
        private static GameObject _tweenEmpty;
        private static float dtEstimated;
        private static float previousRealTime;
        private static float dt;
        private static float dtActual;
        private static TweenDescription tween;
        private static int i;
        private static int j;
        private static readonly AnimationCurve punch = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(0.112586f, 0.9976035f), new Keyframe(0.3120486f, -0.1720615f), new Keyframe(0.4316337f, 0.07030682f), new Keyframe(0.5524869f, -0.03141804f), new Keyframe(0.6549395f, 0.003909959f), new Keyframe(0.770987f, -0.009817753f), new Keyframe(0.8838775f, 0.001939224f), new Keyframe(1.0f, 0.0f));
        private static readonly AnimationCurve shake = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 1f), new Keyframe(0.75f, -1f), new Keyframe(1f, 0f));

        public static void Initialize()
        {
            Initialize(maxTweens);
        }

        /**
        * This line is optional. Here you can specify the maximum number of tweens you will use (the default is 400).  This must be called before any use of LeanTween is made for it to be effective.
        * 
        * @method LeanTween.init
        * @param {integer} maxSimultaneousTweens:int The maximum number of tweens you will use, make sure you don't go over this limit, otherwise the code will throw an error
        * @example
        *   LeanTween.init( 800 );
        */

        public static void Initialize(int maxSimultaneousTweens)
        {
            if (tweens == null)
            {
                maxTweens = maxSimultaneousTweens;
                tweens = new TweenDescription[maxTweens];
                _tweenEmpty = new GameObject();
                _tweenEmpty.name = "~LeanTween";
                _tweenEmpty.AddComponent(typeof(Tween));
                _tweenEmpty.isStatic = true;
#if !UNITY_EDITOR
                _tweenEmpty.hideFlags = HideFlags.HideAndDontSave;
#endif
                DontDestroyOnLoad(_tweenEmpty);
                for (int i = 0; i < maxTweens; i++)
                {
                    tweens[i] = new TweenDescription();
                }
            }
        }

        public static void Reset()
        {
            tweens = null;
        }

        public void Update()
        {
            UpdateInner();
        }

        public void OnLevelWasLoaded(int lvl)
        {
            // Debug.Log("reseting gui");
            TweenGUI.Reset();
        }

        private static Transform trans;
        private static float timeTotal;
        private static TweenAction tweenAction;
        private static float ratioPassed;
        private static float from;
        private static float to;
        private static float val;
        private static Vector3 newVect;
        private static bool isTweenFinished;
        private static GameObject target;
        private static GameObject customTarget;

        /* CAN THIS BE CHANGED TO PRIVATE */
        public static void UpdateInner()
        {
            if (frameRendered != Time.frameCount)
            {
                // make sure update is only called once per frame
                Initialize();

                dtEstimated = Time.realtimeSinceStartup - previousRealTime;
                if (dtEstimated > 0.2f) // a catch put in, when at the start sometimes this number can grow unrealistically large
                {
                    dtEstimated = 0.2f;
                }
                previousRealTime = Time.realtimeSinceStartup;
                dtActual = Time.deltaTime * Time.timeScale;
                // if(tweenMaxSearch>1500)
                // 	Debug.Log("tweenMaxSearch:"+tweenMaxSearch +" maxTweens:"+maxTweens);
                for (int i = 0; i < tweenMaxSearch && i < maxTweens; i++)
                {
                    //Debug.Log("tweens["+i+"].toggle:"+tweens[i].toggle);
                    if (tweens[i].Toggle)
                    {
                        tween = tweens[i];
                        trans = tween.Trans;
                        timeTotal = tween.Time;
                        tweenAction = tween.Type;

                        dt = dtActual;
                        if (tween.UseEstimatedTime)
                        {
                            dt = dtEstimated;
                            timeTotal = tween.Time;
                        }
                        else if (tween.UseFrames)
                        {
                            dt = 1;
                        }
                        else if (tween.Direction == 0f)
                        {
                            dt = 0f;
                        }

                        if (trans == null)
                        {
                            RemoveTween(i);
                            continue;
                        }
                        //Debug.Log("i:"+i+" tween:"+tween+" dt:"+dt);

                        // Check for tween finished
                        isTweenFinished = false;
                        if (tween.Delay <= 0)
                        {
                            if ((tween.Passed + dt > timeTotal && tween.Direction > 0.0f))
                            {
                                isTweenFinished = true;
                                tween.Passed = tween.Time; // Set to the exact end time so that it can finish tween exactly on the end value
                            }
                            else if (tween.Direction < 0.0f && tween.Passed - dt < 0.0f)
                            {
                                isTweenFinished = true;
                                tween.Passed = Mathf.Epsilon;
                            }
                        }

                        if (!tween.HasInitiliazed && ((tween.Passed == 0.0 && tween.Delay == 0.0) || tween.Passed > 0.0))
                        {
                            tween.HasInitiliazed = true;

                            // Set time based on current timeScale
                            if (!tween.UseEstimatedTime)
                            {
                                tween.Time = tween.Time * Time.timeScale;
                            }

                            // Initialize From Values
                            switch (tweenAction)
                            {
                                case TweenAction.MOVE:
                                    tween.From = trans.position;
                                    break;
                                case TweenAction.MOVE_X:
                                    tween.From.x = trans.position.x;
                                    break;
                                case TweenAction.MOVE_Y:
                                    tween.From.x = trans.position.y;
                                    break;
                                case TweenAction.MOVE_Z:
                                    tween.From.x = trans.position.z;
                                    break;
                                case TweenAction.MOVE_LOCAL_X:
                                    tweens[i].From.x = trans.localPosition.x;
                                    break;
                                case TweenAction.MOVE_LOCAL_Y:
                                    tweens[i].From.x = trans.localPosition.y;
                                    break;
                                case TweenAction.MOVE_LOCAL_Z:
                                    tweens[i].From.x = trans.localPosition.z;
                                    break;
                                case TweenAction.SCALE_X:
                                    tween.From.x = trans.localScale.x;
                                    break;
                                case TweenAction.SCALE_Y:
                                    tween.From.x = trans.localScale.y;
                                    break;
                                case TweenAction.SCALE_Z:
                                    tween.From.x = trans.localScale.z;
                                    break;
                                case TweenAction.ALPHA:
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
								tween.from.x = trans.gameObject.renderer.material.color.a; 
								break;	
#else
                                    var ren = trans.gameObject.GetComponent<SpriteRenderer>();
                                    tween.From.x = (ren != null) ? ren.color.a : trans.gameObject.renderer.material.color.a;
                                    break;
#endif
                                case TweenAction.MOVE_LOCAL:
                                    tween.From = trans.localPosition;
                                    break;
                                case TweenAction.MOVE_CURVED:
                                case TweenAction.MOVE_CURVED_LOCAL:
                                case TweenAction.MOVE_SPLINE:
                                case TweenAction.MOVE_SPLINE_LOCAL:
                                    tween.From.x = 0;
                                    break;
                                case TweenAction.ROTATE:
                                    tween.From = trans.eulerAngles;
                                    tween.To = new Vector3(ClosestRotation(tween.From.x, tween.To.x), ClosestRotation(tween.From.y, tween.To.y), ClosestRotation(tween.From.z, tween.To.z));
                                    break;
                                case TweenAction.ROTATE_X:
                                    tween.From.x = trans.eulerAngles.x;
                                    tween.To.x = ClosestRotation(tween.From.x, tween.To.x);
                                    break;
                                case TweenAction.ROTATE_Y:
                                    tween.From.x = trans.eulerAngles.y;
                                    tween.To.x = ClosestRotation(tween.From.x, tween.To.x);
                                    break;
                                case TweenAction.ROTATE_Z:
                                    tween.From.x = trans.eulerAngles.z;
                                    tween.To.x = ClosestRotation(tween.From.x, tween.To.x);
                                    break;
                                case TweenAction.ROTATE_AROUND:
                                    tween.LastVal = 0.0f; // optional["last"]
                                    tween.OrigRotation = trans.eulerAngles; // optional["origRotation"
                                    break;
                                case TweenAction.ROTATE_LOCAL:
                                    tween.From = trans.localEulerAngles;
                                    tween.To = new Vector3(ClosestRotation(tween.From.x, tween.To.x), ClosestRotation(tween.From.y, tween.To.y), ClosestRotation(tween.From.z, tween.To.z));
                                    break;
                                case TweenAction.SCALE:
                                    tween.From = trans.localScale;
                                    break;
                                case TweenAction.GUI_MOVE:
                                    tween.From = new Vector3(tween.TweenRectangle.Rect.x, tween.TweenRectangle.Rect.y, 0);
                                    break;
                                case TweenAction.GUI_MOVE_MARGIN:
                                    tween.From = new Vector2(tween.TweenRectangle.Margin.x, tween.TweenRectangle.Margin.y);
                                    break;
                                case TweenAction.GUI_SCALE:
                                    tween.From = new Vector3(tween.TweenRectangle.Rect.width, tween.TweenRectangle.Rect.height, 0);
                                    break;
                                case TweenAction.GUI_ALPHA:
                                    tween.From.x = tween.TweenRectangle.Alpha;
                                    break;
                                case TweenAction.GUI_ROTATE:
                                    if (tween.TweenRectangle.RotateEnabled == false)
                                    {
                                        tween.TweenRectangle.RotateEnabled = true;
                                        tween.TweenRectangle.ResetForRotation();
                                    }

                                    tween.From.x = tween.TweenRectangle.Rotation;
                                    break;
                                case TweenAction.ALPHA_VERTEX:
                                    tween.From.x = trans.GetComponent<MeshFilter>().mesh.colors32[0].a;
                                    break;
                            }
                            tween.Diff = tween.To - tween.From;
                        }
                        if (tween.Delay <= 0)
                        {
                            // Move Values
                            if (timeTotal <= 0f)
                            {
                                //Debug.LogError("time total is zero Time.timeScale:"+Time.timeScale+" useEstimatedTime:"+tween.useEstimatedTime);
                                ratioPassed = 0f;
                            }
                            else
                            {
                                ratioPassed = tween.Passed / timeTotal;
                            }

                            if (ratioPassed > 1.0f)
                            {
                                ratioPassed = 1.0f;
                            }
                            else if (ratioPassed < 0f)
                            {
                                ratioPassed = 0f;
                            }
                            // Debug.Log("action:"+tweenAction+" ratioPassed:"+ratioPassed + " timeTotal:" + timeTotal + " tween.passed:"+ tween.passed +" dt:"+dt);

                            if (tweenAction >= TweenAction.MOVE_X && tweenAction <= TweenAction.CALLBACK)
                            {
                                if (tween.AnimationCurve != null)
                                {
                                    val = tweenOnCurve(tween, ratioPassed);
                                }
                                else
                                {
                                    switch (tween.TweenType)
                                    {
                                        case TweenType.Linear:
                                            val = tween.From.x + tween.Diff.x * ratioPassed;
                                            break;
                                        case TweenType.EaseOutQuad:
                                            val = easeOutQuadOpt(tween.From.x, tween.Diff.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInQuad:
                                            val = easeInQuadOpt(tween.From.x, tween.Diff.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInOutQuad:
                                            val = easeInOutQuadOpt(tween.From.x, tween.Diff.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInCubic:
                                            val = easeInCubic(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseOutCubic:
                                            val = easeOutCubic(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInOutCubic:
                                            val = easeInOutCubic(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInQuart:
                                            val = easeInQuart(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseOutQuart:
                                            val = easeOutQuart(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInOutQuart:
                                            val = easeInOutQuart(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInQuint:
                                            val = easeInQuint(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseOutQuint:
                                            val = easeOutQuint(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInOutQuint:
                                            val = easeInOutQuint(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInSine:
                                            val = easeInSine(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseOutSine:
                                            val = easeOutSine(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInOutSine:
                                            val = easeInOutSine(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInExpo:
                                            val = easeInExpo(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseOutExpo:
                                            val = easeOutExpo(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInOutExpo:
                                            val = easeInOutExpo(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInCirc:
                                            val = easeInCirc(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseOutCirc:
                                            val = easeOutCirc(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInOutCirc:
                                            val = easeInOutCirc(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInBounce:
                                            val = easeInBounce(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseOutBounce:
                                            val = easeOutBounce(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInOutBounce:
                                            val = easeInOutBounce(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInBack:
                                            val = easeInBack(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseOutBack:
                                            val = easeOutBack(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInOutBack:
                                            val = easeInOutElastic(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInElastic:
                                            val = easeInElastic(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseOutElastic:
                                            val = easeOutElastic(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.EaseInOutElastic:
                                            val = easeInOutElastic(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        case TweenType.Punch:
                                        case TweenType.EaseShake:
                                            if (tween.TweenType == TweenType.Punch)
                                            {
                                                tween.AnimationCurve = punch;
                                            }
                                            else if (tween.TweenType == TweenType.EaseShake)
                                            {
                                                tween.AnimationCurve = shake;
                                            }
                                            tween.To.x = tween.From.x + tween.To.x;
                                            tween.Diff.x = tween.To.x - tween.From.x;
                                            val = tweenOnCurve(tween, ratioPassed);
                                            break;
                                        case TweenType.EaseSpring:
                                            val = spring(tween.From.x, tween.To.x, ratioPassed);
                                            break;
                                        default:
                                            {
                                                val = tween.From.x + tween.Diff.x * ratioPassed;
                                                break;
                                            }
                                    }
                                }

                                //Debug.Log("from:"+from+" to:"+to+" val:"+val+" ratioPassed:"+ratioPassed);
                                if (tweenAction == TweenAction.MOVE_X)
                                {
                                    trans.position = new Vector3(val, trans.position.y, trans.position.z);
                                }
                                else if (tweenAction == TweenAction.MOVE_Y)
                                {
                                    trans.position = new Vector3(trans.position.x, val, trans.position.z);
                                }
                                else if (tweenAction == TweenAction.MOVE_Z)
                                {
                                    trans.position = new Vector3(trans.position.x, trans.position.y, val);
                                }
                                if (tweenAction == TweenAction.MOVE_LOCAL_X)
                                {
                                    trans.localPosition = new Vector3(val, trans.localPosition.y, trans.localPosition.z);
                                }
                                else if (tweenAction == TweenAction.MOVE_LOCAL_Y)
                                {
                                    trans.localPosition = new Vector3(trans.localPosition.x, val, trans.localPosition.z);
                                }
                                else if (tweenAction == TweenAction.MOVE_LOCAL_Z)
                                {
                                    trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, val);
                                }
                                else if (tweenAction == TweenAction.MOVE_CURVED)
                                {
                                    if (tween.Path.OrientToPath)
                                    {
                                        tween.Path.Place(trans, val);
                                    }
                                    else
                                    {
                                        trans.position = tween.Path.Point(val);
                                    }
                                    // Debug.Log("val:"+val+" trans.position:"+trans.position + " 0:"+ tween.curves[0] +" 1:"+tween.curves[1] +" 2:"+tween.curves[2] +" 3:"+tween.curves[3]);
                                }
                                else if (tweenAction == TweenAction.MOVE_CURVED_LOCAL)
                                {
                                    if (tween.Path.OrientToPath)
                                    {
                                        tween.Path.PlaceLocal(trans, val);
                                    }
                                    else
                                    {
                                        trans.localPosition = tween.Path.Point(val);
                                    }
                                    // Debug.Log("val:"+val+" trans.position:"+trans.position);
                                }
                                else if (tweenAction == TweenAction.MOVE_SPLINE)
                                {
                                    if (tween.Spline.OrientToPath)
                                    {
                                        tween.Spline.Place(trans, val);
                                    }
                                    else
                                    {
                                        trans.position = tween.Spline.Point(val);
                                    }
                                }
                                else if (tweenAction == TweenAction.MOVE_SPLINE_LOCAL)
                                {
                                    if (tween.Spline.OrientToPath)
                                    {
                                        tween.Spline.PlaceLocal(trans, val);
                                    }
                                    else
                                    {
                                        trans.localPosition = tween.Spline.Point(val);
                                    }
                                }
                                else if (tweenAction == TweenAction.SCALE_X)
                                {
                                    trans.localScale = new Vector3(val, trans.localScale.y, trans.localScale.z);
                                }
                                else if (tweenAction == TweenAction.SCALE_Y)
                                {
                                    trans.localScale = new Vector3(trans.localScale.x, val, trans.localScale.z);
                                }
                                else if (tweenAction == TweenAction.SCALE_Z)
                                {
                                    trans.localScale = new Vector3(trans.localScale.x, trans.localScale.y, val);
                                }
                                else if (tweenAction == TweenAction.ROTATE_X)
                                {
                                    trans.eulerAngles = new Vector3(val, trans.eulerAngles.y, trans.eulerAngles.z);
                                }
                                else if (tweenAction == TweenAction.ROTATE_Y)
                                {
                                    trans.eulerAngles = new Vector3(trans.eulerAngles.x, val, trans.eulerAngles.z);
                                }
                                else if (tweenAction == TweenAction.ROTATE_Z)
                                {
                                    trans.eulerAngles = new Vector3(trans.eulerAngles.x, trans.eulerAngles.y, val);
                                }
                                else if (tweenAction == TweenAction.ROTATE_AROUND)
                                {
                                    float move = val - tween.LastVal;
                                    // Debug.Log("move:"+move+" val:"+val + " timeTotal:"+timeTotal + " from:"+tween.from+ " diff:"+tween.diff);
                                    if (isTweenFinished)
                                    {
                                        trans.eulerAngles = tween.OrigRotation;
                                        trans.RotateAround(trans.TransformPoint(tween.Point), tween.Axis, tween.To.x);
                                    }
                                    else
                                    {
                                        /*trans.rotation = tween.origRotation;
					    		trans.RotateAround((Vector3)trans.TransformPoint( tween.point ), tween.axis, val);
								tween.lastVal = val;*/

                                        trans.RotateAround(trans.TransformPoint(tween.Point), tween.Axis, move);
                                        tween.LastVal = val;

                                        //trans.rotation =  * Quaternion.AngleAxis(val, tween.axis);
                                    }
                                }
                                else if (tweenAction == TweenAction.ALPHA)
                                {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2

							foreach(Material mat in trans.gameObject.renderer.materials){
        						mat.color = new Color( mat.color.r, mat.color.g, mat.color.b, val);
    						}

#else

                                    var ren = trans.gameObject.GetComponent<SpriteRenderer>();
                                    if (ren != null)
                                    {
                                        ren.color = new Color(ren.color.r, ren.color.g, ren.color.b, val);
                                    }
                                    else
                                    {
                                        foreach (Material mat in trans.gameObject.renderer.materials)
                                        {
                                            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, val);
                                        }
                                    }

#endif
                                }
                                else if (tweenAction == TweenAction.ALPHA_VERTEX)
                                {
                                    Mesh mesh = trans.GetComponent<MeshFilter>().mesh;
                                    Vector3[] vertices = mesh.vertices;
                                    var colors = new Color32[vertices.Length];
                                    Color32 c = mesh.colors32[0];
                                    c = new Color(c.r, c.g, c.b, val);
                                    for (int k = 0; k < vertices.Length; k++)
                                    {
                                        colors[k] = c;
                                    }
                                    mesh.colors32 = colors;
                                }
                            }
                            else if (tweenAction >= TweenAction.MOVE)
                            {
                                //

                                if (tween.AnimationCurve != null)
                                {
                                    newVect = tweenOnCurveVector(tween, ratioPassed);
                                }
                                else
                                {
                                    if (tween.TweenType == TweenType.Linear)
                                    {
                                        newVect = new Vector3(tween.From.x + tween.Diff.x * ratioPassed, tween.From.y + tween.Diff.y * ratioPassed, tween.From.z + tween.Diff.z * ratioPassed);
                                    }
                                    else if (tween.TweenType >= TweenType.Linear)
                                    {
                                        switch (tween.TweenType)
                                        {
                                            case TweenType.EaseOutQuad:
                                                newVect = new Vector3(easeOutQuadOpt(tween.From.x, tween.Diff.x, ratioPassed), easeOutQuadOpt(tween.From.y, tween.Diff.y, ratioPassed), easeOutQuadOpt(tween.From.z, tween.Diff.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInQuad:
                                                newVect = new Vector3(easeInQuadOpt(tween.From.x, tween.Diff.x, ratioPassed), easeInQuadOpt(tween.From.y, tween.Diff.y, ratioPassed), easeInQuadOpt(tween.From.z, tween.Diff.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInOutQuad:
                                                newVect = new Vector3(easeInOutQuadOpt(tween.From.x, tween.Diff.x, ratioPassed), easeInOutQuadOpt(tween.From.y, tween.Diff.y, ratioPassed), easeInOutQuadOpt(tween.From.z, tween.Diff.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInCubic:
                                                newVect = new Vector3(easeInCubic(tween.From.x, tween.To.x, ratioPassed), easeInCubic(tween.From.y, tween.To.y, ratioPassed), easeInCubic(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseOutCubic:
                                                newVect = new Vector3(easeOutCubic(tween.From.x, tween.To.x, ratioPassed), easeOutCubic(tween.From.y, tween.To.y, ratioPassed), easeOutCubic(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInOutCubic:
                                                newVect = new Vector3(easeInOutCubic(tween.From.x, tween.To.x, ratioPassed), easeInOutCubic(tween.From.y, tween.To.y, ratioPassed), easeInOutCubic(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInQuart:
                                                newVect = new Vector3(easeInQuart(tween.From.x, tween.To.x, ratioPassed), easeInQuart(tween.From.y, tween.To.y, ratioPassed), easeInQuart(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseOutQuart:
                                                newVect = new Vector3(easeOutQuart(tween.From.x, tween.To.x, ratioPassed), easeOutQuart(tween.From.y, tween.To.y, ratioPassed), easeOutQuart(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInOutQuart:
                                                newVect = new Vector3(easeInOutQuart(tween.From.x, tween.To.x, ratioPassed), easeInOutQuart(tween.From.y, tween.To.y, ratioPassed), easeInOutQuart(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInQuint:
                                                newVect = new Vector3(easeInQuint(tween.From.x, tween.To.x, ratioPassed), easeInQuint(tween.From.y, tween.To.y, ratioPassed), easeInQuint(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseOutQuint:
                                                newVect = new Vector3(easeOutQuint(tween.From.x, tween.To.x, ratioPassed), easeOutQuint(tween.From.y, tween.To.y, ratioPassed), easeOutQuint(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInOutQuint:
                                                newVect = new Vector3(easeInOutQuint(tween.From.x, tween.To.x, ratioPassed), easeInOutQuint(tween.From.y, tween.To.y, ratioPassed), easeInOutQuint(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInSine:
                                                newVect = new Vector3(easeInSine(tween.From.x, tween.To.x, ratioPassed), easeInSine(tween.From.y, tween.To.y, ratioPassed), easeInSine(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseOutSine:
                                                newVect = new Vector3(easeOutSine(tween.From.x, tween.To.x, ratioPassed), easeOutSine(tween.From.y, tween.To.y, ratioPassed), easeOutSine(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInOutSine:
                                                newVect = new Vector3(easeInOutSine(tween.From.x, tween.To.x, ratioPassed), easeInOutSine(tween.From.y, tween.To.y, ratioPassed), easeInOutSine(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInExpo:
                                                newVect = new Vector3(easeInExpo(tween.From.x, tween.To.x, ratioPassed), easeInExpo(tween.From.y, tween.To.y, ratioPassed), easeInExpo(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseOutExpo:
                                                newVect = new Vector3(easeOutExpo(tween.From.x, tween.To.x, ratioPassed), easeOutExpo(tween.From.y, tween.To.y, ratioPassed), easeOutExpo(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInOutExpo:
                                                newVect = new Vector3(easeInOutExpo(tween.From.x, tween.To.x, ratioPassed), easeInOutExpo(tween.From.y, tween.To.y, ratioPassed), easeInOutExpo(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInCirc:
                                                newVect = new Vector3(easeInCirc(tween.From.x, tween.To.x, ratioPassed), easeInCirc(tween.From.y, tween.To.y, ratioPassed), easeInCirc(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseOutCirc:
                                                newVect = new Vector3(easeOutCirc(tween.From.x, tween.To.x, ratioPassed), easeOutCirc(tween.From.y, tween.To.y, ratioPassed), easeOutCirc(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInOutCirc:
                                                newVect = new Vector3(easeInOutCirc(tween.From.x, tween.To.x, ratioPassed), easeInOutCirc(tween.From.y, tween.To.y, ratioPassed), easeInOutCirc(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInBounce:
                                                newVect = new Vector3(easeInBounce(tween.From.x, tween.To.x, ratioPassed), easeInBounce(tween.From.y, tween.To.y, ratioPassed), easeInBounce(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseOutBounce:
                                                newVect = new Vector3(easeOutBounce(tween.From.x, tween.To.x, ratioPassed), easeOutBounce(tween.From.y, tween.To.y, ratioPassed), easeOutBounce(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInOutBounce:
                                                newVect = new Vector3(easeInOutBounce(tween.From.x, tween.To.x, ratioPassed), easeInOutBounce(tween.From.y, tween.To.y, ratioPassed), easeInOutBounce(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInBack:
                                                newVect = new Vector3(easeInBack(tween.From.x, tween.To.x, ratioPassed), easeInBack(tween.From.y, tween.To.y, ratioPassed), easeInBack(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseOutBack:
                                                newVect = new Vector3(easeOutBack(tween.From.x, tween.To.x, ratioPassed), easeOutBack(tween.From.y, tween.To.y, ratioPassed), easeOutBack(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInOutBack:
                                                newVect = new Vector3(easeInOutBack(tween.From.x, tween.To.x, ratioPassed), easeInOutBack(tween.From.y, tween.To.y, ratioPassed), easeInOutBack(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInElastic:
                                                newVect = new Vector3(easeInElastic(tween.From.x, tween.To.x, ratioPassed), easeInElastic(tween.From.y, tween.To.y, ratioPassed), easeInElastic(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseOutElastic:
                                                newVect = new Vector3(easeOutElastic(tween.From.x, tween.To.x, ratioPassed), easeOutElastic(tween.From.y, tween.To.y, ratioPassed), easeOutElastic(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.EaseInOutElastic:
                                                newVect = new Vector3(easeInOutElastic(tween.From.x, tween.To.x, ratioPassed), easeInOutElastic(tween.From.y, tween.To.y, ratioPassed), easeInOutElastic(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                            case TweenType.Punch:
                                            case TweenType.EaseShake:
                                                if (tween.TweenType == TweenType.Punch)
                                                {
                                                    tween.AnimationCurve = punch;
                                                }
                                                else if (tween.TweenType == TweenType.EaseShake)
                                                {
                                                    tween.AnimationCurve = shake;
                                                }
                                                tween.To = tween.From + tween.To;
                                                tween.Diff = tween.To - tween.From;
                                                if (tweenAction == TweenAction.ROTATE || tweenAction == TweenAction.ROTATE_LOCAL)
                                                {
                                                    tween.To = new Vector3(ClosestRotation(tween.From.x, tween.To.x), ClosestRotation(tween.From.y, tween.To.y), ClosestRotation(tween.From.z, tween.To.z));
                                                }
                                                newVect = tweenOnCurveVector(tween, ratioPassed);
                                                break;
                                            case TweenType.EaseSpring:
                                                newVect = new Vector3(spring(tween.From.x, tween.To.x, ratioPassed), spring(tween.From.y, tween.To.y, ratioPassed), spring(tween.From.z, tween.To.z, ratioPassed));
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        newVect = new Vector3(tween.From.x + tween.Diff.x * ratioPassed, tween.From.y + tween.Diff.y * ratioPassed, tween.From.z + tween.Diff.z * ratioPassed);
                                    }
                                }

                                if (tweenAction == TweenAction.MOVE)
                                {
                                    trans.position = newVect;
                                }
                                else if (tweenAction == TweenAction.MOVE_LOCAL)
                                {
                                    trans.localPosition = newVect;
                                }
                                else if (tweenAction == TweenAction.ROTATE)
                                {
                                    /*if(tween.hasPhysics){
					    		trans.gameObject.rigidbody.MoveRotation(Quaternion.Euler( newVect ));
				    		}else{*/
                                    trans.eulerAngles = newVect;
                                    // }
                                }
                                else if (tweenAction == TweenAction.ROTATE_LOCAL)
                                {
                                    trans.localEulerAngles = newVect;
                                }
                                else if (tweenAction == TweenAction.SCALE)
                                {
                                    trans.localScale = newVect;
                                }
                                else if (tweenAction == TweenAction.GUI_MOVE)
                                {
                                    tween.TweenRectangle.Rect = new Rect(newVect.x, newVect.y, tween.TweenRectangle.Rect.width, tween.TweenRectangle.Rect.height);
                                }
                                else if (tweenAction == TweenAction.GUI_MOVE_MARGIN)
                                {
                                    tween.TweenRectangle.Margin = new Vector2(newVect.x, newVect.y);
                                }
                                else if (tweenAction == TweenAction.GUI_SCALE)
                                {
                                    tween.TweenRectangle.Rect = new Rect(tween.TweenRectangle.Rect.x, tween.TweenRectangle.Rect.y, newVect.x, newVect.y);
                                }
                                else if (tweenAction == TweenAction.GUI_ALPHA)
                                {
                                    tween.TweenRectangle.Alpha = newVect.x;
                                }
                                else if (tweenAction == TweenAction.GUI_ROTATE)
                                {
                                    tween.TweenRectangle.Rotation = newVect.x;
                                }
                            }
                            //Debug.Log("tween.delay:"+tween.delay + " tween.passed:"+tween.passed + " tweenAction:"+tweenAction + " to:"+newVect+" axis:"+tween.axis);

                            if (tween.OnUpdateFloat != null)
                            {
                                tween.OnUpdateFloat(val);
                            }
                            else if (tween.OnUpdateFloatObject != null)
                            {
                                tween.OnUpdateFloatObject(val, tween.OnUpdateParam);
                            }
                            else if (tween.OnUpdateVector3Object != null)
                            {
                                tween.OnUpdateVector3Object(newVect, tween.OnUpdateParam);
                            }
                            else if (tween.OnUpdateVector3 != null)
                            {
                                tween.OnUpdateVector3(newVect);
                            }
#if !UNITY_METRO
                            else if (tween.Optional != null)
                            {
                                // LeanTween 1.x legacy stuff

                                object onUpdate = tween.Optional["onUpdate"];
                                if (onUpdate != null)
                                {
                                    var updateParam = (Hashtable)tween.Optional["onUpdateParam"];
                                    if (tweenAction == TweenAction.VALUE3)
                                    {
                                        if (onUpdate.GetType() == typeof(string))
                                        {
                                            var onUpdateS = onUpdate as string;
                                            customTarget = tween.Optional["onUpdateTarget"] != null ? tween.Optional["onUpdateTarget"] as GameObject : trans.gameObject;
                                            customTarget.BroadcastMessage(onUpdateS, newVect);
                                        }
                                        else if (onUpdate.GetType() == typeof(Action<Vector3, Hashtable>))
                                        {
                                            var onUpdateA = (Action<Vector3, Hashtable>)onUpdate;
                                            onUpdateA(newVect, updateParam);
                                        }
                                        else
                                        {
                                            var onUpdateA = (Action<Vector3>)onUpdate;
                                            onUpdateA(newVect);
                                        }
                                    }
                                    else
                                    {
                                        if (onUpdate.GetType() == typeof(string))
                                        {
                                            var onUpdateS = onUpdate as string;
                                            if (tween.Optional["onUpdateTarget"] != null)
                                            {
                                                customTarget = tween.Optional["onUpdateTarget"] as GameObject;
                                                customTarget.BroadcastMessage(onUpdateS, val);
                                            }
                                            else
                                            {
                                                trans.gameObject.BroadcastMessage(onUpdateS, val);
                                            }
                                        }
                                        else if (onUpdate.GetType() == typeof(Action<float, Hashtable>))
                                        {
                                            var onUpdateA = (Action<float, Hashtable>)onUpdate;
                                            onUpdateA(val, updateParam);
                                        }
                                        else if (onUpdate.GetType() == typeof(Action<Vector3>))
                                        {
                                            var onUpdateA = (Action<Vector3>)onUpdate;
                                            onUpdateA(newVect);
                                        }
                                        else
                                        {
                                            var onUpdateA = (Action<float>)onUpdate;
                                            onUpdateA(val);
                                        }
                                    }
                                }
                            }
#endif
                        }

                        if (isTweenFinished)
                        {
                            // Debug.Log("finished tween:"+i+" tween:"+tween);
                            if (tweenAction == TweenAction.GUI_ROTATE)
                            {
                                tween.TweenRectangle.RotateFinished = true;
                            }

                            if (tween.LoopType == TweenType.Once || tween.LoopCount == 1)
                            {
                                if (tweenAction == TweenAction.DELAYED_SOUND)
                                {
                                    AudioSource.PlayClipAtPoint((AudioClip)tween.OnCompleteParam, tween.To, tween.From.x);
                                }
                                if (tween.OnComplete != null)
                                {
                                    RemoveTween(i);
                                    tween.OnComplete();
                                }
                                else if (tween.OnCompleteObject != null)
                                {
                                    RemoveTween(i);
                                    tween.OnCompleteObject(tween.OnCompleteParam);
                                }

#if !UNITY_METRO
                                else if (tween.Optional != null)
                                {
                                    Action callback = null;
                                    Action<object> callbackWithParam = null;
                                    string callbackS = string.Empty;
                                    object callbackParam = null;
                                    if (tween.Optional != null && tween.Trans)
                                    {
                                        if (tween.Optional["onComplete"] != null)
                                        {
                                            callbackParam = tween.Optional["onCompleteParam"];
                                            if (tween.Optional["onComplete"].GetType() == typeof(string))
                                            {
                                                callbackS = tween.Optional["onComplete"] as string;
                                            }
                                            else
                                            {
                                                if (callbackParam != null)
                                                {
                                                    callbackWithParam = (Action<object>)tween.Optional["onComplete"];
                                                }
                                                else
                                                {
                                                    callback = (Action)tween.Optional["onComplete"];
                                                    if (callback == null)
                                                    {
                                                        Debug.LogWarning("callback was not converted");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    RemoveTween(i);
                                    if (callbackWithParam != null)
                                    {
                                        callbackWithParam(callbackParam);
                                    }
                                    else if (callback != null)
                                    {
                                        callback();
                                    }
                                    else if (callbackS != string.Empty)
                                    {
                                        if (tween.Optional["onCompleteTarget"] != null)
                                        {
                                            customTarget = tween.Optional["onCompleteTarget"] as GameObject;
                                            if (callbackParam != null)
                                            {
                                                customTarget.BroadcastMessage(callbackS, callbackParam);
                                            }
                                            else
                                            {
                                                customTarget.BroadcastMessage(callbackS);
                                            }
                                        }
                                        else
                                        {
                                            if (callbackParam != null)
                                            {
                                                trans.gameObject.BroadcastMessage(callbackS, callbackParam);
                                            }
                                            else
                                            {
                                                trans.gameObject.BroadcastMessage(callbackS);
                                            }
                                        }
                                    }
                                }
#endif
                                else
                                {
                                    RemoveTween(i);
                                }
                            }
                            else
                            {
                                if (tween.LoopCount < 0 && tween.Type == TweenAction.CALLBACK)
                                {
                                    if (tween.OnComplete != null)
                                    {
                                        tween.OnComplete();
                                    }
                                    else if (tween.OnCompleteObject != null)
                                    {
                                        tween.OnCompleteObject(tween.OnCompleteParam);
                                    }
                                }
                                if (tween.LoopCount >= 1)
                                {
                                    tween.LoopCount--;
                                }
                                if (tween.LoopType == TweenType.Clamp)
                                {
                                    tween.Passed = Mathf.Epsilon;
                                    // tween.delay = 0.0;
                                }
                                else if (tween.LoopType == TweenType.PingPong)
                                {
                                    tween.Direction = 0.0f - (tween.Direction);
                                }
                            }
                        }
                        else if (tween.Delay <= 0)
                        {
                            tween.Passed += dt * tween.Direction;
                        }
                        else
                        {
                            tween.Delay -= dt;
                            // Debug.Log("dt:"+dt+" tween:"+i+" tween:"+tween);
                            if (tween.Delay < 0)
                            {
                                tween.Passed = 0.0f; //-tween.delay
                                tween.Delay = 0.0f;
                            }
                        }
                    }
                }

                frameRendered = Time.frameCount;
            }
        }

        // This method is only used internally! Do not call this from your scripts. To cancel a tween use LeanTween.cancel
        public static void RemoveTween(int index)
        {
            if (tweens[index].Toggle)
            {
                tweens[index].Toggle = false;
                if (tweens[index].DestroyOnComplete)
                {
                    //Debug.Log("destroying tween.type:"+tween.type);
                    if (tweens[index].TweenRectangle != null)
                    {
                        //	Debug.Log("destroy i:"+i+" id:"+tweens[i].ltRect.id);
                        TweenGUI.Destroy(tweens[index].TweenRectangle.ID);
                    }
                }
                //tweens[i].optional = null;
                StartSearch = index;
                //Debug.Log("start search reset:"+startSearch + " i:"+i+" tweenMaxSearch:"+tweenMaxSearch);
                if (index + 1 >= tweenMaxSearch)
                {
                    //Debug.Log("reset to zero");
                    StartSearch = 0;
                    tweenMaxSearch--;
                }
            }
        }

        public static Vector3[] Add(Vector3[] a, Vector3 b)
        {
            var c = new Vector3[a.Length];
            for (i = 0; i < a.Length; i++)
            {
                c[i] = a[i] + b;
            }

            return c;
        }

        public static float ClosestRotation(float from, float to)
        {
            float minusWhole = 0 - (360 - to);
            float plusWhole = 360 + to;
            float toDiffAbs = Mathf.Abs(to - from);
            float minusDiff = Mathf.Abs(minusWhole - from);
            float plusDiff = Mathf.Abs(plusWhole - from);
            if (toDiffAbs < minusDiff && toDiffAbs < plusDiff)
            {
                return to;
            }
            if (minusDiff < plusDiff)
            {
                return minusWhole;
            }
            return plusWhole;
        }

        /**
        * Cancel all tweens that are currently targeting the gameObject
        * 
        * @method LeanTween.cancel
        * @param {GameObject} gameObject:GameObject gameObject whose tweens you wish to cancel
        * @example LeanTween.move( gameObject, new Vector3(0f,1f,2f), 1f); <br>
        * LeanTween.cancel( gameObject );
        */

        public static void Cancel(GameObject gameObject)
        {
            Initialize();
            Transform trans = gameObject.transform;
            for (int i = 0; i < tweenMaxSearch; i++)
            {
                if (tweens[i].Trans == trans)
                {
                    RemoveTween(i);
                }
            }
        }

        /**
        * Cancel a specific tween with the provided id
        * 
        * @method LeanTween.cancel
        * @param {GameObject} gameObject:GameObject gameObject whose tweens you want to cancel
        * @param {float} id:float unique id that represents that tween
        */

        public static void Cancel(GameObject gameObject, int uniqueId)
        {
            if (uniqueId >= 0)
            {
                Initialize();
                int backId = uniqueId & 0xFFFF;
                int backCounter = uniqueId >> 16;
                // Debug.Log("uniqueId:"+uniqueId+ " id:"+backId +" counter:"+backCounter + " setCounter:"+ tweens[backId].counter + " tweens[id].type:"+tweens[backId].type);
                if (tweens[backId].Trans == null || (tweens[backId].Trans.gameObject == gameObject && tweens[backId].Counter == backCounter))
                {
                    RemoveTween(backId);
                }
            }
        }

        /**
        * Cancel a specific tween with the provided id
        * 
        * @method LeanTween.cancel
        * @param {LTRect} ltRect:LTRect LTRect object whose tweens you want to cancel
        * @param {float} id:float unique id that represents that tween
        */

        public static void Cancel(TweenRectangle tweenRectangle, int uniqueId)
        {
            if (uniqueId >= 0)
            {
                Initialize();
                int backId = uniqueId & 0xFFFF;
                int backCounter = uniqueId >> 16;
                // Debug.Log("uniqueId:"+uniqueId+ " id:"+backId +" action:"+(TweenAction)backType + " tweens[id].type:"+tweens[backId].type);
                if (tweens[backId].TweenRectangle == tweenRectangle && tweens[backId].Counter == backCounter)
                {
                    RemoveTween(backId);
                }
            }
        }

        private static void Cancel(int uniqueId)
        {
            if (uniqueId >= 0)
            {
                Initialize();
                int backId = uniqueId & 0xFFFF;
                int backCounter = uniqueId >> 16;
                // Debug.Log("uniqueId:"+uniqueId+ " id:"+backId +" action:"+(TweenAction)backType + " tweens[id].type:"+tweens[backId].type);
                if (tweens[backId].HasInitiliazed && tweens[backId].Counter == backCounter)
                {
                    RemoveTween(backId);
                }
            }
        }

        // Deprecated
        public static TweenDescription Description(int uniqueId)
        {
            int backId = uniqueId & 0xFFFF;
            int backCounter = uniqueId >> 16;

            if (tweens[backId] != null && tweens[backId].UniqueId == uniqueId && tweens[backId].Counter == backCounter)
            {
                return tweens[backId];
            }
            for (int i = 0; i < tweenMaxSearch; i++)
            {
                if (tweens[i].UniqueId == uniqueId && tweens[i].Counter == backCounter)
                {
                    return tweens[i];
                }
            }
            return null;
        }

        // Deprecated use pause( id )
        public static void Pause(GameObject gameObject, int uniqueId)
        {
            Pause(uniqueId);
        }

        public static void Pause(int uniqueId)
        {
            int backId = uniqueId & 0xFFFF;
            int backCounter = uniqueId >> 16;
            if (tweens[backId].Counter == backCounter)
            {
                tweens[backId].Pause();
            }
        }

        /**
        * Pause all tweens for a GameObject
        * 
        * @method LeanTween.pause
        * @param {GameObject} gameObject:GameObject GameObject whose tweens you want to pause
        */

        public static void Pause(GameObject gameObject)
        {
            Transform trans = gameObject.transform;
            for (int i = 0; i < tweenMaxSearch; i++)
            {
                if (tweens[i].Trans == trans)
                {
                    tweens[i].Pause();
                }
            }
        }

        // Deprecated
        public static void Resume(GameObject gameObject, int uniqueId)
        {
            Resume(uniqueId);
        }

        /**
        * Resume a specific tween
        * 
        * @method LeanTween.resume
        * @param {int} id:int Id of the tween you want to resume ex: int id = LeanTween.MoveX(gameObject, 5, 1.0).id;
        */

        public static void Resume(int uniqueId)
        {
            int backId = uniqueId & 0xFFFF;
            int backCounter = uniqueId >> 16;
            if (tweens[backId].Counter == backCounter)
            {
                tweens[backId].Resume();
            }
        }

        /**
        * Resume all the tweens on a GameObject
        * 
        * @method LeanTween.resume
        * @param {GameObject} gameObject:GameObject GameObject whose tweens you want to resume
        */

        public static void Resume(GameObject gameObject)
        {
            Transform trans = gameObject.transform;
            for (int i = 0; i < tweenMaxSearch; i++)
            {
                if (tweens[i].Trans == trans)
                {
                    tweens[i].Resume();
                }
            }
        }

        /**
        * Test whether or not a tween is active on a GameObject
        * 
        * @method LeanTween.isTweening
        * @param {GameObject} gameObject:GameObject GameObject that you want to test if it is tweening
        */

        public static bool IsTweening(GameObject gameObject)
        {
            Transform trans = gameObject.transform;
            for (int i = 0; i < tweenMaxSearch; i++)
            {
                if (tweens[i].Toggle && tweens[i].Trans == trans)
                {
                    return true;
                }
            }
            return false;
        }

        /**
        * Test whether or not a tween is active or not
        * 
        * @method LeanTween.isTweening
        * @param {GameObject} id:int id of the tween that you want to test if it is tweening
        * &nbsp;&nbsp;<i>Example:</i><br>
        * &nbsp;&nbsp;int id = LeanTween.moveX(gameObject, 1f, 3f).id;<br>
        * &nbsp;&nbsp;if(LeanTween.isTweening( id ))<br>
        * &nbsp;&nbsp; &nbsp;&nbsp;Debug.Log("I am tweening!");<br>
        */

        public static bool IsTweening(int uniqueId)
        {
            int backId = uniqueId & 0xFFFF;
            int backCounter = uniqueId >> 16;
            if (tweens[backId].Counter == backCounter && tweens[backId].Toggle)
            {
                return true;
            }
            return false;
        }

        /**
        * Test whether or not a tween is active on a LTRect
        * 
        * @method LeanTween.isTweening
        * @param {LTRect} ltRect:LTRect LTRect that you want to test if it is tweening
        */

        public static bool IsTweening(TweenRectangle tweenRectangle)
        {
            for (int i = 0; i < tweenMaxSearch; i++)
            {
                if (tweens[i].Toggle && tweens[i].TweenRectangle == tweenRectangle)
                {
                    return true;
                }
            }
            return false;
        }

        public static void DrawBezierPath(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 last = a;
            Vector3 p;
            Vector3 aa = (-a + 3 * (b - c) + d);
            Vector3 bb = 3 * (a + c) - 6 * b;
            Vector3 cc = 3 * (b - a);
            float t;
            for (float k = 1.0f; k <= 30.0f; k++)
            {
                t = k / 30.0f;
                p = ((aa * t + (bb)) * t + cc) * t + a;
                Gizmos.DrawLine(last, p);
                last = p;
            }
        }

        public static object LogError(string error)
        {
            if (LogErrors)
            {
                Debug.LogError(error);
            }
            else
            {
                Debug.Log(error);
            }
            return null;
        }

        // LeanTween 2.0 Methods

        public static TweenDescription Options(TweenDescription seed)
        {
            Debug.LogError("error this function is no longer used");
            return null;
        }

        public static TweenDescription Options()
        {
            Initialize();

            for (j = 0, i = StartSearch; j < maxTweens; i++)
            {
                if (i >= maxTweens - 1)
                {
                    i = 0;
                }
                if (tweens[i].Toggle == false)
                {
                    if (i + 1 > tweenMaxSearch)
                    {
                        tweenMaxSearch = i + 1;
                    }
                    StartSearch = i + 1;
                    break;
                }

                j++;
                if (j >= maxTweens)
                {
                    return LogError("LeanTween - You have run out of available spaces for tweening. To avoid this error increase the number of spaces to available for tweening when you initialize the LeanTween class ex: LeanTween.init( " + (maxTweens * 2) + " );") as TweenDescription;
                }
            }
            tween = tweens[i];
            tween.Reset();
            tween.SetId((uint)i);

            return tween;
        }

        public static GameObject TweenEmpty
        {
            get
            {
                Initialize(maxTweens);
                return _tweenEmpty;
            }
        }

        public static int StartSearch = 0;
        public static TweenDescription Descr;

        private static TweenDescription PushNewTween(GameObject gameObject, Vector3 to, float time, TweenAction tweenAction, TweenDescription tween)
        {
            Initialize(maxTweens);
            if (gameObject == null)
            {
                return null;
            }
            tween.Trans = gameObject.transform;
            tween.To = to;
            tween.Time = time;
            tween.Type = tweenAction;
            //tween.hasPhysics = gameObject.rigidbody!=null;

            return tween;
        }

        /**
        * Fade a gameobject's material to a certain alpha value. The material's shader needs to support alpha. <a href="http://owlchemylabs.com/content/">Owl labs has some excellent efficient shaders</a>.
        * 
        * @method LeanTween.alpha
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to fade
        * @param {float} to:float the final alpha value (0-1)
        * @param {float} time:float The time with which to fade the object
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example
        * LeanTween.alpha(gameObject, 1f, 1f) .setDelay(1f);
        */

        public static TweenDescription Alpha(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.ALPHA, Options());
        }

        /**
        * Fade a GUI Object
        * 
        * @method LeanTween.alpha
        * @param {LTRect} ltRect:LTRect LTRect that you wish to fade
        * @param {float} to:float the final alpha value (0-1)
        * @param {float} time:float The time with which to fade the object
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example
        * LeanTween.alpha(ltRect, 1f, 1f) .setEase(LeanTweenType.easeInCirc);
        */

        public static TweenDescription Alpha(TweenRectangle tweenRectangle, float to, float time)
        {
            tweenRectangle.AlphaEnabled = true;
            return PushNewTween(TweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ALPHA, Options().SetRect(tweenRectangle));
        }

        /**
        * This works by tweening the vertex colors directly.<br>
        <br>
        Vertex-based coloring is useful because you avoid making a copy of your
        object's material for each instance that needs a different color.<br>
        <br>
        A shader that supports vertex colors is required for it to work
        (for example the shaders in Mobile/Particles/)
        * 
        * @method LeanTween.alphaVertex
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to alpha
        * @param {float} to:float The alpha value you wish to tween to
        * @param {float} time:float The time with which to delay before calling the function
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription AlphaVertex(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ALPHA_VERTEX, Options());
        }

        public static TweenDescription DelayedCall(float delayTime, Action callback)
        {
            return PushNewTween(TweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, Options().SetOnComplete(callback));
        }

        public static TweenDescription DelayedCall(float delayTime, Action<object> callback)
        {
            return PushNewTween(TweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, Options().SetOnComplete(callback));
        }

        public static TweenDescription DelayedCall(GameObject gameObject, float delayTime, Action callback)
        {
            return PushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, Options().SetOnComplete(callback));
        }

        public static TweenDescription DelayedCall(GameObject gameObject, float delayTime, Action<object> callback)
        {
            return PushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, Options().SetOnComplete(callback));
        }

        public static TweenDescription DestroyAfter(TweenRectangle rect, float delayTime)
        {
            return PushNewTween(TweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, Options().SetRect(rect).SetDestroyOnComplete(true));
        }

        /*public static LTDescr delayedCall(GameObject gameObject, float delayTime, string callback){
            return pushNewTween( gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete( callback ) );
        }*/

        /**
        * Move a GameObject to a certain location
        * 
        * @method LeanTween.move
        * @param {GameObject} GameObject gameObject Gameobject that you wish to move
        * @param {Vector3} vec:Vector3 to The final positin with which to move to
        * @param {float} time:float time The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example LeanTween.move(gameObject, new Vector3(0f,-3f,5f), 2.0f) .setEase( LeanTween.easeOutQuad );
        */

        public static TweenDescription Move(GameObject gameObject, Vector3 to, float time)
        {
            return PushNewTween(gameObject, to, time, TweenAction.MOVE, Options());
        }

        public static TweenDescription Move(GameObject gameObject, Vector2 to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to.x, to.y, gameObject.transform.position.z), time, TweenAction.MOVE, Options());
        }


        /**
        * Move a GameObject along a set of bezier curves
        * 
        * @method LeanTween.move
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to move
        * @param {Vector3[]} path:Vector3[] A set of points that define the curve(s) ex: Point1,Handle1,Handle2,Point2,...
        * @param {float} time:float The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example
        * <i>Javascript:</i><br>
        * LeanTween.move(gameObject, [Vector3(0,0,0),Vector3(1,0,0),Vector3(1,0,0),Vector3(1,0,1)], 2.0) .setEase(LeanTween.easeOutQuad).setOrientToPath(true);<br><br>
        * <i>C#:</i><br>
        * LeanTween.move(gameObject, new Vector3{Vector3(0f,0f,0f),Vector3(1f,0f,0f),Vector3(1f,0f,0f),Vector3(1f,0f,1f)}, 1.5f) .setEase(LeanTween.easeOutQuad).setOrientToPath(true);;<br>
        */

        public static TweenDescription Move(GameObject gameObject, Vector3[] to, float time)
        {
            Descr = Options();
            if (Descr.Path == null)
            {
                Descr.Path = new TweenBezierPath(to);
            }
            else
            {
                Descr.Path.SetPoints(to);
            }

            return PushNewTween(gameObject, new Vector3(1.0f, 0.0f, 0.0f), time, TweenAction.MOVE_CURVED, Descr);
        }

        /**
        * Move a GameObject through a set of points
        * 
        * @method LeanTween.moveSpline
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to move
        * @param {Vector3[]} path:Vector3[] A set of points that define the curve(s) ex: ControlStart,Pt1,Pt2,Pt3,.. ..ControlEnd
        * @param {float} time:float The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example
        * <i>Javascript:</i><br>
        * LeanTween.moveSpline(gameObject, [Vector3(0,0,0),Vector3(1,0,0),Vector3(1,0,0),Vector3(1,0,1)], 2.0) .setEase(LeanTween.easeOutQuad).setOrientToPath(true);<br><br>
        * <i>C#:</i><br>
        * LeanTween.moveSpline(gameObject, new Vector3{Vector3(0f,0f,0f),Vector3(1f,0f,0f),Vector3(1f,0f,0f),Vector3(1f,0f,1f)}, 1.5f).setEase(LeanTween.easeOutQuad).setOrientToPath(true);<br>
        */

        public static TweenDescription MoveSpline(GameObject gameObject, Vector3[] to, float time)
        {
            Descr = Options();
            Descr.Spline = new TweenSpline(to);

            return PushNewTween(gameObject, new Vector3(1.0f, 0.0f, 0.0f), time, TweenAction.MOVE_SPLINE, Descr);
        }

        /**
        * Move a GameObject through a set of points, in local space
        * 
        * @method LeanTween.moveSplineLocal
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to move
        * @param {Vector3[]} path:Vector3[] A set of points that define the curve(s) ex: ControlStart,Pt1,Pt2,Pt3,.. ..ControlEnd
        * @param {float} time:float The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example
        * <i>Javascript:</i><br>
        * LeanTween.moveSpline(gameObject, [Vector3(0,0,0),Vector3(1,0,0),Vector3(1,0,0),Vector3(1,0,1)], 2.0) .setEase(LeanTween.easeOutQuad).setOrientToPath(true);<br><br>
        * <i>C#:</i><br>
        * LeanTween.moveSpline(gameObject, new Vector3{Vector3(0f,0f,0f),Vector3(1f,0f,0f),Vector3(1f,0f,0f),Vector3(1f,0f,1f)}, 1.5f). setEase(LeanTween.easeOutQuad).setOrientToPath(true);<br>
        */

        public static TweenDescription MoveSplineLocal(GameObject gameObject, Vector3[] to, float time)
        {
            Descr = Options();
            Descr.Spline = new TweenSpline(to);

            return PushNewTween(gameObject, new Vector3(1.0f, 0.0f, 0.0f), time, TweenAction.MOVE_SPLINE_LOCAL, Descr);
        }

        /**
        * Move a GUI Element to a certain location
        * 
        * @method LeanTween.move (GUI)
        * @param {LTRect} ltRect:LTRect ltRect LTRect object that you wish to move
        * @param {Vector2} vec:Vector2 to The final position with which to move to (pixel coordinates)
        * @param {float} time:float time The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription Move(TweenRectangle tweenRectangle, Vector2 to, float time)
        {
            return PushNewTween(TweenEmpty, to, time, TweenAction.GUI_MOVE, Options().SetRect(tweenRectangle));
        }

        public static TweenDescription MoveMargin(TweenRectangle tweenRectangle, Vector2 to, float time)
        {
            return PushNewTween(TweenEmpty, to, time, TweenAction.GUI_MOVE_MARGIN, Options().SetRect(tweenRectangle));
        }

        /**
        * Move a GameObject along the x-axis
        * 
        * @method LeanTween.moveX
        * @param {GameObject} gameObject:GameObject gameObject Gameobject that you wish to move
        * @param {float} to:float to The final position with which to move to
        * @param {float} time:float time The time to complete the move in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription MoveX(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_X, Options());
        }

        /**
        * Move a GameObject along the y-axis
        * 
        * @method LeanTween.moveY
        * @param {GameObject} GameObject gameObject Gameobject that you wish to move
        * @param {float} float to The final position with which to move to
        * @param {float} float time The time to complete the move in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription MoveY(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_Y, Options());
        }

        /**
        * Move a GameObject along the z-axis
        * 
        * @method LeanTween.moveZ
        * @param {GameObject} GameObject gameObject Gameobject that you wish to move
        * @param {float} float to The final position with which to move to
        * @param {float} float time The time to complete the move in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription MoveZ(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_Z, Options());
        }

        /**
        * Move a GameObject to a certain location relative to the parent transform. 
        * 
        * @method LeanTween.moveLocal
        * @param {GameObject} GameObject gameObject Gameobject that you wish to rotate
        * @param {Vector3} Vector3 to The final positin with which to move to
        * @param {float} float time The time to complete the tween in
        * @param {Hashtable} Hashtable optional Hashtable where you can pass <a href="#optional">optional items</a>.
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription MoveLocal(GameObject gameObject, Vector3 to, float time)
        {
            return PushNewTween(gameObject, to, time, TweenAction.MOVE_LOCAL, Options());
        }

        /**
        * Move a GameObject along a set of bezier curves, in local space
        * 
        * @method LeanTween.moveLocal
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to move
        * @param {Vector3[]} path:Vector3[] A set of points that define the curve(s) ex: Point1,Handle1,Handle2,Point2,...
        * @param {float} time:float The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example
        * <i>Javascript:</i><br>
        * LeanTween.move(gameObject, [Vector3(0,0,0),Vector3(1,0,0),Vector3(1,0,0),Vector3(1,0,1)], 2.0).setEase(LeanTween.easeOutQuad).setOrientToPath(true);<br><br>
        * <i>C#:</i><br>
        * LeanTween.move(gameObject, new Vector3{Vector3(0f,0f,0f),Vector3(1f,0f,0f),Vector3(1f,0f,0f),Vector3(1f,0f,1f)}).setEase(LeanTween.easeOutQuad).setOrientToPath(true);<br>
        */

        public static TweenDescription MoveLocal(GameObject gameObject, Vector3[] to, float time)
        {
            Descr = Options();
            if (Descr.Path == null)
            {
                Descr.Path = new TweenBezierPath(to);
            }
            else
            {
                Descr.Path.SetPoints(to);
            }

            return PushNewTween(gameObject, new Vector3(1.0f, 0.0f, 0.0f), time, TweenAction.MOVE_CURVED_LOCAL, Descr);
        }

        public static TweenDescription MoveLocalX(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_LOCAL_X, Options());
        }

        public static TweenDescription MoveLocalY(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_LOCAL_Y, Options());
        }

        public static TweenDescription MoveLocalZ(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_LOCAL_Z, Options());
        }

        /**
        * Rotate a GameObject, to values are in passed in degrees
        * 
        * @method LeanTween.rotate
        * @param {GameObject} GameObject gameObject Gameobject that you wish to rotate
        * @param {Vector3} Vector3 to The final rotation with which to rotate to
        * @param {float} float time The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example LeanTween.rotate(cube, new Vector3(180f,30f,0f), 1.5f);
        */

        public static TweenDescription Rotate(GameObject gameObject, Vector3 to, float time)
        {
            return PushNewTween(gameObject, to, time, TweenAction.ROTATE, Options());
        }

        /**
        * Rotate a GUI element (using an LTRect object), to a value that is in degrees
        * 
        * @method LeanTween.rotate
        * @param {LTRect} ltRect:LTRect LTRect that you wish to rotate
        * @param {float} to:float The final rotation with which to rotate to
        * @param {float} time:float The time to complete the tween in
        * @param {Array} optional:Array Object Array where you can pass <a href="#optional">optional items</a>.
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example 
        * if(GUI.Button(buttonRect.rect, "Rotate"))<br>
        *	LeanTween.rotate( buttonRect4, 150.0f, 1.0f).setEase(LeanTween.easeOutElastic);<br>
        * GUI.matrix = Matrix4x4.identity;<br>
        */

        public static TweenDescription Rotate(TweenRectangle tweenRectangle, float to, float time)
        {
            return PushNewTween(TweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ROTATE, Options().SetRect(tweenRectangle));
        }

        /**
        * Rotate a GameObject in the objects local space (on the transforms localEulerAngles object)
        * 
        * @method LeanTween.rotateLocal
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to rotate
        * @param {Vector3} to:Vector3 The final rotation with which to rotate to
        * @param {float} time:float The time to complete the rotation in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription RotateLocal(GameObject gameObject, Vector3 to, float time)
        {
            return PushNewTween(gameObject, to, time, TweenAction.ROTATE_LOCAL, Options());
        }

        /**
        * Rotate a GameObject only on the X axis
        * 
        * @method LeanTween.rotateX
        * @param {GameObject} GameObject Gameobject that you wish to rotate
        * @param {float} to:float The final x-axis rotation with which to rotate
        * @param {float} time:float The time to complete the rotation in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription RotateX(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.ROTATE_X, Options());
        }

        /**
        * Rotate a GameObject only on the Y axis
        * 
        * @method LeanTween.rotateY
        * @param {GameObject} GameObject Gameobject that you wish to rotate
        * @param {float} to:float The final y-axis rotation with which to rotate
        * @param {float} time:float The time to complete the rotation in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription RotateY(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.ROTATE_Y, Options());
        }

        /**
        * Rotate a GameObject only on the Z axis
        * 
        * @method LeanTween.rotateZ
        * @param {GameObject} GameObject Gameobject that you wish to rotate
        * @param {float} to:float The final z-axis rotation with which to rotate
        * @param {float} time:float The time to complete the rotation in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription RotateZ(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.ROTATE_Z, Options());
        }

        /**
        * Rotate a GameObject around a certain Axis (the best method to use when you want to rotate beyond 180 degrees)
        * 
        * @method LeanTween.rotateAround
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to rotate
        * @param {Vector3} vec:Vector3 axis in which to rotate around ex: Vector3.up
        * @param {float} degrees:float the degrees in which to rotate
        * @param {float} time:float time The time to complete the rotation in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example
        * <i>Example:</i><br>
        * LeanTween.rotateAround ( gameObject, Vector3.left, 90f,  1f );
        */

        public static TweenDescription RotateAround(GameObject gameObject, Vector3 axis, float add, float time)
        {
            return PushNewTween(gameObject, new Vector3(add, 0f, 0f), time, TweenAction.ROTATE_AROUND, Options().SetAxis(axis));
        }

        /**
        * Scale a GameObject to a certain size
        * 
        * @method LeanTween.scale
        * @param {GameObject} gameObject:GameObject gameObject Gameobject that you wish to scale
        * @param {Vector3} vec:Vector3 to The size with which to tween to
        * @param {float} time:float time The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription Scale(GameObject gameObject, Vector3 to, float time)
        {
            return PushNewTween(gameObject, to, time, TweenAction.SCALE, Options());
        }

        /**
        * Scale a GUI Element to a certain width and height
        * 
        * @method LeanTween.scale (GUI)
        * @param {LTRect} LTRect ltRect LTRect object that you wish to move
        * @param {Vector2} Vector2 to The final width and height to scale to (pixel based)
        * @param {float} float time The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example
        * <i>Example Javascript: </i><br>
        * var bRect:LTRect = new LTRect( 0, 0, 100, 50 );<br>
        * LeanTween.scale( bRect, Vector2(bRect.rect.width, bRect.rect.height) * 1.3, 0.25 ).setEase(LeanTweenType.easeOutBounce);<br>
        * function OnGUI(){<br>
        * &nbsp; if(GUI.Button(bRect.rect, "Scale")){ }<br>
        * }<br>
        * <br>
        * <i>Example C#: </i> <br>
        * LTRect bRect = new LTRect( 0f, 0f, 100f, 50f );<br>
        * LeanTween.scale( bRect, new Vector2(150f,75f), 0.25f ).setEase(LeanTweenType.easeOutBounce);<br>
        * void OnGUI(){<br>
        * &nbsp; if(GUI.Button(bRect.rect, "Scale")){ }<br>
        * }<br>
        */

        public static TweenDescription Scale(TweenRectangle tweenRectangle, Vector2 to, float time)
        {
            return PushNewTween(TweenEmpty, to, time, TweenAction.GUI_SCALE, Options().SetRect(tweenRectangle));
        }

        /**
        * Scale a GameObject to a certain size along the x-axis only
        * 
        * @method LeanTween.scaleX
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to scale
        * @param {float} scaleTo:float the size with which to scale to
        * @param {float} time:float the time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription ScaleX(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.SCALE_X, Options());
        }

        /**
        * Scale a GameObject to a certain size along the y-axis only
        * 
        * @method LeanTween.scaleY
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to scale
        * @param {float} scaleTo:float the size with which to scale to
        * @param {float} time:float the time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription ScaleY(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.SCALE_Y, Options());
        }

        /**
        * Scale a GameObject to a certain size along the z-axis only
        * 
        * @method LeanTween.scaleZ
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to scale
        * @param {float} scaleTo:float the size with which to scale to
        * @param {float} time:float the time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription ScaleZ(GameObject gameObject, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.SCALE_Z, Options());
        }

        /**
        * Tween any particular value, it does not need to be tied to any particular type or GameObject
        * 
        * @method LeanTween.value (float)
        * @param {GameObject} GameObject gameObject GameObject with which to tie the tweening with. This is only used when you need to cancel this tween, it does not actually perform any operations on this gameObject
        * @param {Action<float>} callOnUpdate:Action<float> The function that is called on every Update frame, this function needs to accept a float value ex: function updateValue( float val ){ }
        * @param {float} float from The original value to start the tween from
        * @param {float} float to The value to end the tween on
        * @param {float} float time The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        * @example
        * <i>Example Javascript: </i><br>
        * LeanTween.value( gameObject, updateValueExampleCallback, 180f, 270f, 1f).setEase(LeanTweenType.easeOutElastic);<br>
        * function updateValueExampleCallback( val:float ){<br>
        * &nbsp; Debug.Log("tweened value:"+val+" set this to whatever variable you are tweening...");<br>
        * }<br>
        * <br>
        * <i>Example C#: </i> <br>
        * LeanTween.value( gameObject, updateValueExampleCallback, 180f, 270f, 1f).setEase(LeanTweenType.easeOutElastic);<br>
        * void updateValueExampleCallback( float val ){<br>
        * &nbsp; Debug.Log("tweened value:"+val+" set this to whatever variable you are tweening...");<br>
        * }<br>
        */

        public static TweenDescription Value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.CALLBACK, Options().SetTo(new Vector3(to, 0, 0)).SetFrom(new Vector3(from, 0, 0)).SetOnUpdate(callOnUpdate));
        }

        /**
        * Tween any particular value (Vector3), this could be used to tween an arbitrary value like a material color
        * 
        * @method LeanTween.value (Vector3)
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to attach the tween to
        * @param {Action<Vector3>} callOnUpdate:Action<Vector3> The function that is called on every Update frame, this function needs to accept a float value ex: function updateValue( Vector3 val ){ }
        * @param {float} from:Vector3 The original value to start the tween from
        * @param {Vector3} to:Vector3 The final Vector3 with which to tween to
        * @param {float} time:float The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription Value(GameObject gameObject, Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time)
        {
            return PushNewTween(gameObject, to, time, TweenAction.VALUE3, Options().SetTo(to).SetFrom(from).SetOnUpdateVector3(callOnUpdate));
        }

        /**
        * Tween any particular value (float)
        * 
        * @method LeanTween.value (float,object)
        * @param {GameObject} gameObject:GameObject Gameobject that you wish to attach the tween to
        * @param {Action<float,object>} callOnUpdate:Action<float,object> The function that is called on every Update frame, this function needs to accept a float value ex: function updateValue( Vector3 val, object obj ){ }
        * @param {float} from:Vector3 The original value to start the tween from
        * @param {Vector3} to:Vector3 The final Vector3 with which to tween to
        * @param {float} time:float The time to complete the tween in
        * @return {LTDescr} LTDescr an object that distinguishes the tween
        */

        public static TweenDescription Value(GameObject gameObject, Action<float, object> callOnUpdate, float from, float to, float time)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.CALLBACK, Options().SetTo(new Vector3(to, 0, 0)).SetFrom(new Vector3(from, 0, 0)).SetOnUpdateObject(callOnUpdate));
        }

        public static TweenDescription DelayedSound(AudioClip audio, Vector3 pos, float volume)
        {
            return PushNewTween(TweenEmpty, pos, 0f, TweenAction.DELAYED_SOUND, Options().SetTo(pos).SetFrom(new Vector3(volume, 0, 0)).SetAudio(audio));
        }

#if !UNITY_METRO
        // LeanTween 1.x Methods

        public static Hashtable Hash(object[] arr)
        {
            if (arr.Length % 2 == 1)
            {
                LogError("LeanTween - You have attempted to create a Hashtable with an odd number of values.");
                return null;
            }
            var hash = new Hashtable();
            for (i = 0; i < arr.Length; i += 2)
            {
                hash.Add(arr[i] as string, arr[i + 1]);
            }

            return hash;
        }

        private static int idFromUnique(int uniqueId)
        {
            return uniqueId & 0xFFFF;
        }

        private static int PushNewTween(GameObject gameObject, Vector3 to, float time, TweenAction tweenAction, Hashtable optional)
        {
            Initialize(maxTweens);
            if (gameObject == null)
            {
                return -1;
            }

            j = 0;
            for (i = StartSearch; j < maxTweens; i++)
            {
                if (i >= maxTweens - 1)
                {
                    i = 0;
                }
                if (tweens[i].Toggle == false)
                {
                    if (i + 1 > tweenMaxSearch)
                    {
                        tweenMaxSearch = i + 1;
                    }
                    StartSearch = i + 1;
                    break;
                }

                j++;
                if (j >= maxTweens)
                {
                    LogError("LeanTween - You have run out of available spaces for tweening. To avoid this error increase the number of spaces to available for tweening when you initialize the LeanTween class ex: LeanTween.init( " + (maxTweens * 2) + " );");
                    return -1;
                }
            }
            tween = tweens[i];
            tween.Toggle = true;
            tween.Reset();
            tween.Trans = gameObject.transform;
            tween.To = to;
            tween.Time = time;
            tween.Type = tweenAction;
            tween.Optional = optional;
            tween.SetId((uint)i);
            //tween.hasPhysics = gameObject.rigidbody!=null;

            if (optional != null)
            {
                object ease = optional["ease"];
                //LeanTweenType ease;
                int optionsNotUsed = 0;
                if (ease != null)
                {
                    tween.TweenType = TweenType.Linear;
                    if (ease.GetType() == typeof(TweenType))
                    {
                        tween.TweenType = (TweenType)ease; // Enum.Parse(typeof(LeanTweenType), optional["ease"].ToString());
                    }
                    else if (ease.GetType() == typeof(AnimationCurve))
                    {
                        tween.AnimationCurve = optional["ease"] as AnimationCurve;
                    }
                    else
                    {
                        string func = optional["ease"].ToString();
                        if (func.Equals("easeOutQuad"))
                        {
                            tween.TweenType = TweenType.EaseOutQuad;
                        }
                        else if (func.Equals("easeInQuad"))
                        {
                            tween.TweenType = TweenType.EaseInQuad;
                        }
                        else if (func.Equals("easeInOutQuad"))
                        {
                            tween.TweenType = TweenType.EaseInOutQuad;
                        }
                    }
                    optionsNotUsed++;
                }
                if (optional["rect"] != null)
                {
                    tween.TweenRectangle = (TweenRectangle)optional["rect"];
                    optionsNotUsed++;
                }
                if (optional["path"] != null)
                {
                    tween.Path = (TweenBezierPath)optional["path"];
                    optionsNotUsed++;
                }
                if (optional["delay"] != null)
                {
                    tween.Delay = (float)optional["delay"];
                    optionsNotUsed++;
                }
                if (optional["useEstimatedTime"] != null)
                {
                    tween.UseEstimatedTime = (bool)optional["useEstimatedTime"];
                    optionsNotUsed++;
                }
                if (optional["useFrames"] != null)
                {
                    tween.UseFrames = (bool)optional["useFrames"];
                    optionsNotUsed++;
                }
                if (optional["loopType"] != null)
                {
                    tween.LoopType = (TweenType)optional["loopType"];
                    optionsNotUsed++;
                }
                if (optional["repeat"] != null)
                {
                    tween.LoopCount = (int)optional["repeat"];
                    if (tween.LoopType == TweenType.Once)
                    {
                        tween.LoopType = TweenType.Clamp;
                    }
                    optionsNotUsed++;
                }
                if (optional["point"] != null)
                {
                    tween.Point = (Vector3)optional["point"];
                    optionsNotUsed++;
                }
                if (optional["axis"] != null)
                {
                    tween.Axis = (Vector3)optional["axis"];
                    optionsNotUsed++;
                }
                if (optional.Count <= optionsNotUsed)
                {
                    tween.Optional = null; // nothing else is used with the extra piece, so set to null
                }
            }
            else
            {
                tween.Optional = null;
            }
            //Debug.Log("pushing new tween["+i+"]:"+tweens[i]);

            return tweens[i].UniqueId;
        }

        public static int Value(string callOnUpdate, float from, float to, float time, Hashtable optional)
        {
            return Value(TweenEmpty, callOnUpdate, from, to, time, optional);
        }

        public static int Value(GameObject gameObject, string callOnUpdate, float from, float to, float time)
        {
            return Value(gameObject, callOnUpdate, from, to, time, new Hashtable());
        }

        public static int Value(GameObject gameObject, string callOnUpdate, float from, float to, float time, object[] optional)
        {
            return Value(gameObject, callOnUpdate, from, to, time, Hash(optional));
        }

        public static int Value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time, object[] optional)
        {
            return Value(gameObject, callOnUpdate, from, to, time, Hash(optional));
        }

        public static int Value(GameObject gameObject, Action<float, Hashtable> callOnUpdate, float from, float to, float time, object[] optional)
        {
            return Value(gameObject, callOnUpdate, from, to, time, Hash(optional));
        }

        public static int Value(GameObject gameObject, string callOnUpdate, float from, float to, float time, Hashtable optional)
        {
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            optional["onUpdate"] = callOnUpdate;
            int id = idFromUnique(PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.CALLBACK, optional));
            tweens[id].From = new Vector3(from, 0, 0);
            return id;
        }

        public static int Value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time, Hashtable optional)
        {
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            optional["onUpdate"] = callOnUpdate;
            int id = idFromUnique(PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.CALLBACK, optional));
            tweens[id].From = new Vector3(from, 0, 0);
            return id;
        }

        public static int Value(GameObject gameObject, Action<float, Hashtable> callOnUpdate, float from, float to, float time, Hashtable optional)
        {
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            optional["onUpdate"] = callOnUpdate;
            int id = idFromUnique(PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.CALLBACK, optional));
            tweens[id].From = new Vector3(from, 0, 0);
            return id;
        }

        public static int Value(GameObject gameObject, String callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional)
        {
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            optional["onUpdate"] = callOnUpdate;
            int id = idFromUnique(PushNewTween(gameObject, to, time, TweenAction.VALUE3, optional));
            tweens[id].From = from;
            return id;
        }

        public static int Value(GameObject gameObject, String callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional)
        {
            return Value(gameObject, callOnUpdate, from, to, time, Hash(optional));
        }

        public static int Value(GameObject gameObject, Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional)
        {
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            optional["onUpdate"] = callOnUpdate;
            int id = idFromUnique(PushNewTween(gameObject, to, time, TweenAction.VALUE3, optional));
            tweens[id].From = from;
            return id;
        }

        public static int Value(GameObject gameObject, Action<Vector3, Hashtable> callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional)
        {
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            optional["onUpdate"] = callOnUpdate;
            int id = idFromUnique(PushNewTween(gameObject, to, time, TweenAction.VALUE3, optional));
            tweens[id].From = from;
            return id;
        }

        public static int Value(GameObject gameObject, Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional)
        {
            return Value(gameObject, callOnUpdate, from, to, time, Hash(optional));
        }

        public static int Value(GameObject gameObject, Action<Vector3, Hashtable> callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional)
        {
            return Value(gameObject, callOnUpdate, from, to, time, Hash(optional));
        }

        public static int Rotate(GameObject gameObject, Vector3 to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, to, time, TweenAction.ROTATE, optional);
        }

        public static int Rotate(GameObject gameObject, Vector3 to, float time, object[] optional)
        {
            return Rotate(gameObject, to, time, Hash(optional));
        }


        public static int Rotate(TweenRectangle tweenRectangle, float to, float time, Hashtable optional)
        {
            Initialize();
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            optional["rect"] = tweenRectangle;
            return PushNewTween(TweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ROTATE, optional);
        }

        public static int Rotate(TweenRectangle tweenRectangle, float to, float time, object[] optional)
        {
            return Rotate(tweenRectangle, to, time, Hash(optional));
        }

        public static int RotateX(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.ROTATE_X, optional);
        }

        public static int RotateX(GameObject gameObject, float to, float time, object[] optional)
        {
            return RotateX(gameObject, to, time, Hash(optional));
        }

        public static int RotateY(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.ROTATE_Y, optional);
        }

        public static int RotateY(GameObject gameObject, float to, float time, object[] optional)
        {
            return RotateY(gameObject, to, time, Hash(optional));
        }

        public static int RotateZ(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.ROTATE_Z, optional);
        }

        public static int RotateZ(GameObject gameObject, float to, float time, object[] optional)
        {
            return RotateZ(gameObject, to, time, Hash(optional));
        }

        public static int RotateLocal(GameObject gameObject, Vector3 to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, to, time, TweenAction.ROTATE_LOCAL, optional);
        }

        public static int RotateLocal(GameObject gameObject, Vector3 to, float time, object[] optional)
        {
            return RotateLocal(gameObject, to, time, Hash(optional));
        }

        /*public static int rotateAround(GameObject gameObject, Vector3 point, Vector3 axis, float add, float time, Hashtable optional){
            if(optional==null || optional.Count==0)
                optional = new Hashtable();
		
            optional["axis"] = axis;
            if(optional["point"]!=null)
                optional["point"] = Vector3.zero;
            return pushNewTween( gameObject, new Vector3(add,0f,0f), time, TweenAction.ROTATE_AROUND, optional );
        }*/

        public static int RotateAround(GameObject gameObject, Vector3 axis, float add, float time, Hashtable optional)
        {
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            optional["axis"] = axis;
            if (optional["point"] == null)
            {
                optional["point"] = Vector3.zero;
            }

            return PushNewTween(gameObject, new Vector3(add, 0f, 0f), time, TweenAction.ROTATE_AROUND, optional);
        }

        public static int RotateAround(GameObject gameObject, Vector3 axis, float add, float time, object[] optional)
        {
            return RotateAround(gameObject, axis, add, time, Hash(optional));
        }

        public static int MoveX(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_X, optional);
        }

        public static int MoveX(GameObject gameObject, float to, float time, object[] optional)
        {
            return MoveX(gameObject, to, time, Hash(optional));
        }

        public static int MoveY(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_Y, optional);
        }

        public static int MoveY(GameObject gameObject, float to, float time, object[] optional)
        {
            return MoveY(gameObject, to, time, Hash(optional));
        }

        public static int MoveZ(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_Z, optional);
        }

        public static int MoveZ(GameObject gameObject, float to, float time, object[] optional)
        {
            return MoveZ(gameObject, to, time, Hash(optional));
        }

        public static int Move(GameObject gameObject, Vector3 to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, to, time, TweenAction.MOVE, optional);
        }

        public static int Move(GameObject gameObject, Vector3 to, float time, object[] optional)
        {
            return Move(gameObject, to, time, Hash(optional));
        }

        public static int Move(GameObject gameObject, Vector3[] to, float time, Hashtable optional)
        {
            if (to.Length < 4)
            {
                string errorMsg = "LeanTween - When passing values for a vector path, you must pass four or more values!";
                if (LogErrors)
                {
                    Debug.LogError(errorMsg);
                }
                else
                {
                    Debug.Log(errorMsg);
                }
                return -1;
            }
            if (to.Length % 4 != 0)
            {
                string errorMsg2 = "LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...";
                if (LogErrors)
                {
                    Debug.LogError(errorMsg2);
                }
                else
                {
                    Debug.Log(errorMsg2);
                }
                return -1;
            }

            Initialize();
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            var ltPath = new TweenBezierPath(to);
            if (optional["orientToPath"] != null)
            {
                ltPath.OrientToPath = true;
            }
            optional["path"] = ltPath;

            return PushNewTween(gameObject, new Vector3(1.0f, 0.0f, 0.0f), time, TweenAction.MOVE_CURVED, optional);
        }

        public static int Move(GameObject gameObject, Vector3[] to, float time, object[] optional)
        {
            return Move(gameObject, to, time, Hash(optional));
        }

        public static int Move(TweenRectangle tweenRectangle, Vector2 to, float time, Hashtable optional)
        {
            Initialize();
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            optional["rect"] = tweenRectangle;
            return PushNewTween(TweenEmpty, to, time, TweenAction.GUI_MOVE, optional);
        }

        public static int Move(TweenRectangle tweenRectangle, Vector3 to, float time, object[] optional)
        {
            return Move(tweenRectangle, to, time, Hash(optional));
        }

        public static int MoveLocal(GameObject gameObject, Vector3 to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, to, time, TweenAction.MOVE_LOCAL, optional);
        }

        public static int MoveLocal(GameObject gameObject, Vector3 to, float time, object[] optional)
        {
            return MoveLocal(gameObject, to, time, Hash(optional));
        }

        public static int MoveLocal(GameObject gameObject, Vector3[] to, float time, Hashtable optional)
        {
            if (to.Length < 4)
            {
                string errorMsg = "LeanTween - When passing values for a vector path, you must pass four or more values!";
                if (LogErrors)
                {
                    Debug.LogError(errorMsg);
                }
                else
                {
                    Debug.Log(errorMsg);
                }
                return -1;
            }
            if (to.Length % 4 != 0)
            {
                string errorMsg2 = "LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...";
                if (LogErrors)
                {
                    Debug.LogError(errorMsg2);
                }
                else
                {
                    Debug.Log(errorMsg2);
                }
                return -1;
            }

            Initialize();
            if (optional == null)
            {
                optional = new Hashtable();
            }

            var ltPath = new TweenBezierPath(to);
            if (optional["orientToPath"] != null)
            {
                ltPath.OrientToPath = true;
            }
            optional["path"] = ltPath;

            return PushNewTween(gameObject, new Vector3(1.0f, 0.0f, 0.0f), time, TweenAction.MOVE_CURVED_LOCAL, optional);
        }

        public static int MoveLocal(GameObject gameObject, Vector3[] to, float time, object[] optional)
        {
            return MoveLocal(gameObject, to, time, Hash(optional));
        }

        public static int MoveLocalX(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_LOCAL_X, optional);
        }

        public static int MoveLocalX(GameObject gameObject, float to, float time, object[] optional)
        {
            return MoveLocalX(gameObject, to, time, Hash(optional));
        }

        public static int MoveLocalY(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_LOCAL_Y, optional);
        }

        public static int MoveLocalY(GameObject gameObject, float to, float time, object[] optional)
        {
            return MoveLocalY(gameObject, to, time, Hash(optional));
        }

        public static int MoveLocalZ(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.MOVE_LOCAL_Z, optional);
        }

        public static int MoveLocalZ(GameObject gameObject, float to, float time, object[] optional)
        {
            return MoveLocalZ(gameObject, to, time, Hash(optional));
        }

        public static int Scale(GameObject gameObject, Vector3 to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, to, time, TweenAction.SCALE, optional);
        }

        public static int Scale(GameObject gameObject, Vector3 to, float time, object[] optional)
        {
            return Scale(gameObject, to, time, Hash(optional));
        }

        public static int Scale(TweenRectangle tweenRectangle, Vector2 to, float time, Hashtable optional)
        {
            Initialize();
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            optional["rect"] = tweenRectangle;
            return PushNewTween(TweenEmpty, to, time, TweenAction.GUI_SCALE, optional);
        }

        public static int Scale(TweenRectangle tweenRectangle, Vector2 to, float time, object[] optional)
        {
            return Scale(tweenRectangle, to, time, Hash(optional));
        }

        public static int Alpha(TweenRectangle tweenRectangle, float to, float time, Hashtable optional)
        {
            Initialize();
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }

            tweenRectangle.AlphaEnabled = true;
            optional["rect"] = tweenRectangle;
            return PushNewTween(TweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ALPHA, optional);
        }

        public static int Alpha(TweenRectangle tweenRectangle, float to, float time, object[] optional)
        {
            return Alpha(tweenRectangle, to, time, Hash(optional));
        }

        public static int ScaleX(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.SCALE_X, optional);
        }

        public static int ScaleX(GameObject gameObject, float to, float time, object[] optional)
        {
            return ScaleX(gameObject, to, time, Hash(optional));
        }

        public static int ScaleY(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.SCALE_Y, optional);
        }

        public static int ScaleY(GameObject gameObject, float to, float time, object[] optional)
        {
            return ScaleY(gameObject, to, time, Hash(optional));
        }

        public static int ScaleZ(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.SCALE_Z, optional);
        }

        public static int ScaleZ(GameObject gameObject, float to, float time, object[] optional)
        {
            return ScaleZ(gameObject, to, time, Hash(optional));
        }

        public static int DelayedCall(float delayTime, string callback, Hashtable optional)
        {
            Initialize();
            return DelayedCall(TweenEmpty, delayTime, callback, optional);
        }

        public static int DelayedCall(float delayTime, Action callback, object[] optional)
        {
            Initialize();
            return DelayedCall(TweenEmpty, delayTime, callback, Hash(optional));
        }

        public static int DelayedCall(GameObject gameObject, float delayTime, string callback, object[] optional)
        {
            return DelayedCall(gameObject, delayTime, callback, Hash(optional));
        }

        public static int DelayedCall(GameObject gameObject, float delayTime, Action callback, object[] optional)
        {
            return DelayedCall(gameObject, delayTime, callback, Hash(optional));
        }

        public static int DelayedCall(GameObject gameObject, float delayTime, string callback, Hashtable optional)
        {
            if (optional == null || optional.Count == 0)
            {
                optional = new Hashtable();
            }
            optional["onComplete"] = callback;

            return PushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional);
        }

        public static int DelayedCall(GameObject gameObject, float delayTime, Action callback, Hashtable optional)
        {
            if (optional == null)
            {
                optional = new Hashtable();
            }
            optional["onComplete"] = callback;

            return PushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional);
        }

        public static int DelayedCall(GameObject gameObject, float delayTime, Action<object> callback, Hashtable optional)
        {
            if (optional == null)
            {
                optional = new Hashtable();
            }
            optional["onComplete"] = callback;

            return PushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional);
        }

        public static int Alpha(GameObject gameObject, float to, float time, Hashtable optional)
        {
            return PushNewTween(gameObject, new Vector3(to, 0, 0), time, TweenAction.ALPHA, optional);
        }

        public static int Alpha(GameObject gameObject, float to, float time, object[] optional)
        {
            return Alpha(gameObject, to, time, Hash(optional));
        }
#endif

        // Tweening Functions - Thanks to Robert Penner and GFX47

        private static float tweenOnCurve(TweenDescription tweenDescr, float ratioPassed)
        {
            // Debug.Log("single ratio:"+ratioPassed+" tweenDescr.animationCurve.Evaluate(ratioPassed):"+tweenDescr.animationCurve.Evaluate(ratioPassed));
            return tweenDescr.From.x + (tweenDescr.Diff.x) * tweenDescr.AnimationCurve.Evaluate(ratioPassed);
        }

        private static Vector3 tweenOnCurveVector(TweenDescription tweenDescr, float ratioPassed)
        {
            return new Vector3(tweenDescr.From.x + (tweenDescr.Diff.x) * tweenDescr.AnimationCurve.Evaluate(ratioPassed),
                tweenDescr.From.y + (tweenDescr.Diff.y) * tweenDescr.AnimationCurve.Evaluate(ratioPassed),
                tweenDescr.From.z + (tweenDescr.Diff.z) * tweenDescr.AnimationCurve.Evaluate(ratioPassed));
        }

        private static float easeOutQuadOpt(float start, float diff, float ratioPassed)
        {
            return -diff * ratioPassed * (ratioPassed - 2) + start;
        }

        private static float easeInQuadOpt(float start, float diff, float ratioPassed)
        {
            return diff * ratioPassed * ratioPassed + start;
        }

        private static float easeInOutQuadOpt(float start, float diff, float ratioPassed)
        {
            ratioPassed /= .5f;
            if (ratioPassed < 1)
            {
                return diff / 2 * ratioPassed * ratioPassed + start;
            }
            ratioPassed--;
            return -diff / 2 * (ratioPassed * (ratioPassed - 2) - 1) + start;
        }

        private static float linear(float start, float end, float val)
        {
            return Mathf.Lerp(start, end, val);
        }

        private static float clerp(float start, float end, float val)
        {
            float min = 0.0f;
            float max = 360.0f;
            float half = Mathf.Abs((max - min) / 2.0f);
            float retval = 0.0f;
            float diff = 0.0f;
            if ((end - start) < -half)
            {
                diff = ((max - start) + end) * val;
                retval = start + diff;
            }
            else if ((end - start) > half)
            {
                diff = -((max - end) + start) * val;
                retval = start + diff;
            }
            else
            {
                retval = start + (end - start) * val;
            }
            return retval;
        }

        private static float spring(float start, float end, float val)
        {
            val = Mathf.Clamp01(val);
            val = (Mathf.Sin(val * Mathf.PI * (0.2f + 2.5f * val * val * val)) * Mathf.Pow(1f - val, 2.2f) + val) * (1f + (1.2f * (1f - val)));
            return start + (end - start) * val;
        }

        private static float easeInQuad(float start, float end, float val)
        {
            end -= start;
            return end * val * val + start;
        }

        private static float easeOutQuad(float start, float end, float val)
        {
            end -= start;
            return -end * val * (val - 2) + start;
        }

        private static float easeInOutQuad(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1)
            {
                return end / 2 * val * val + start;
            }
            val--;
            return -end / 2 * (val * (val - 2) - 1) + start;
        }

        private static float easeInCubic(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val + start;
        }

        private static float easeOutCubic(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val + 1) + start;
        }

        private static float easeInOutCubic(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1)
            {
                return end / 2 * val * val * val + start;
            }
            val -= 2;
            return end / 2 * (val * val * val + 2) + start;
        }

        private static float easeInQuart(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val + start;
        }

        private static float easeOutQuart(float start, float end, float val)
        {
            val--;
            end -= start;
            return -end * (val * val * val * val - 1) + start;
        }

        private static float easeInOutQuart(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1)
            {
                return end / 2 * val * val * val * val + start;
            }
            val -= 2;
            return -end / 2 * (val * val * val * val - 2) + start;
        }

        private static float easeInQuint(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val * val + start;
        }

        private static float easeOutQuint(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val * val * val + 1) + start;
        }

        private static float easeInOutQuint(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1)
            {
                return end / 2 * val * val * val * val * val + start;
            }
            val -= 2;
            return end / 2 * (val * val * val * val * val + 2) + start;
        }

        private static float easeInSine(float start, float end, float val)
        {
            end -= start;
            return -end * Mathf.Cos(val / 1 * (Mathf.PI / 2)) + end + start;
        }

        private static float easeOutSine(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Sin(val / 1 * (Mathf.PI / 2)) + start;
        }

        private static float easeInOutSine(float start, float end, float val)
        {
            end -= start;
            return -end / 2 * (Mathf.Cos(Mathf.PI * val / 1) - 1) + start;
        }

        private static float easeInExpo(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (val / 1 - 1)) + start;
        }

        private static float easeOutExpo(float start, float end, float val)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * val / 1) + 1) + start;
        }

        private static float easeInOutExpo(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1)
            {
                return end / 2 * Mathf.Pow(2, 10 * (val - 1)) + start;
            }
            val--;
            return end / 2 * (-Mathf.Pow(2, -10 * val) + 2) + start;
        }

        private static float easeInCirc(float start, float end, float val)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - val * val) - 1) + start;
        }

        private static float easeOutCirc(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * Mathf.Sqrt(1 - val * val) + start;
        }

        private static float easeInOutCirc(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1)
            {
                return -end / 2 * (Mathf.Sqrt(1 - val * val) - 1) + start;
            }
            val -= 2;
            return end / 2 * (Mathf.Sqrt(1 - val * val) + 1) + start;
        }

        /* GFX47 MOD START */

        private static float easeInBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            return end - easeOutBounce(0, end, d - val) + start;
        }

        /* GFX47 MOD END */

        /* GFX47 MOD START */
        //public static function bounce(float start, float end, float val){
        private static float easeOutBounce(float start, float end, float val)
        {
            val /= 1f;
            end -= start;
            if (val < (1 / 2.75f))
            {
                return end * (7.5625f * val * val) + start;
            }
            if (val < (2 / 2.75f))
            {
                val -= (1.5f / 2.75f);
                return end * (7.5625f * (val) * val + .75f) + start;
            }
            if (val < (2.5 / 2.75))
            {
                val -= (2.25f / 2.75f);
                return end * (7.5625f * (val) * val + .9375f) + start;
            }
            val -= (2.625f / 2.75f);
            return end * (7.5625f * (val) * val + .984375f) + start;
        }

        /* GFX47 MOD END */

        /* GFX47 MOD START */

        private static float easeInOutBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            if (val < d / 2)
            {
                return easeInBounce(0, end, val * 2) * 0.5f + start;
            }
            return easeOutBounce(0, end, val * 2 - d) * 0.5f + end * 0.5f + start;
        }

        /* GFX47 MOD END */

        private static float easeInBack(float start, float end, float val)
        {
            end -= start;
            val /= 1;
            float s = 1.70158f;
            return end * (val) * val * ((s + 1) * val - s) + start;
        }

        private static float easeOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val = (val / 1) - 1;
            return end * ((val) * val * ((s + 1) * val + s) + 1) + start;
        }

        private static float easeInOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val /= .5f;
            if ((val) < 1)
            {
                s *= (1.525f);
                return end / 2 * (val * val * (((s) + 1) * val - s)) + start;
            }
            val -= 2;
            s *= (1.525f);
            return end / 2 * ((val) * val * (((s) + 1) * val + s) + 2) + start;
        }

        /* GFX47 MOD START */

        private static float easeInElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0)
            {
                return start;
            }
            val = val / d;
            if (val == 1)
            {
                return start + end;
            }

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }
            val = val - 1;
            return -(a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
        }

        /* GFX47 MOD END */

        /* GFX47 MOD START */
        //public static function elastic(float start, float end, float val){
        private static float easeOutElastic(float start, float end, float val)
        {
            /* GFX47 MOD END */
            //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0)
            {
                return start;
            }

            val = val / d;
            if (val == 1)
            {
                return start + end;
            }

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        /* GFX47 MOD START */

        private static float easeInOutElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0)
            {
                return start;
            }

            val = val / (d / 2);
            if (val == 2)
            {
                return start + end;
            }

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (val < 1)
            {
                val = val - 1;
                return -0.5f * (a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
            }
            val = val - 1;
            return a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }

        // LeanTween Listening/Dispatch

        private static Action<TweenEvent>[] eventListeners;
        private static GameObject[] goListeners;
        private static int eventsMaxSearch;
        public static int MaxEvents = 10;
        public static int MaxListeners = 10;

        public static void AddListener(int eventId, Action<TweenEvent> callback)
        {
            AddListener(TweenEmpty, eventId, callback);
        }

        /**
        * Add a listener method to be called when the appropriate LeanTween.dispatchEvent is called
        * @method LeanTween.addListener
        * @param {GameObject} caller:GameObject the gameObject the listener is attached to
        * @param {int} eventId:int a unique int that describes the event (best to use an enum)
        * @param {System.Action<LTEvent>} callback:System.Action<LTEvent> the method to call when the event has been dispatched
        * @example
        * LeanTween.addListener(gameObject, (int)MyEvents.JUMP, jumpUp);<br>
        * <br>
        * void jumpUp( LTEvent e ){ Debug.Log("jump!"); }<br>
        */

        public static void AddListener(GameObject caller, int eventId, Action<TweenEvent> callback)
        {
            if (eventListeners == null)
            {
                eventListeners = new Action<TweenEvent>[MaxEvents * MaxListeners];
                goListeners = new GameObject[MaxEvents * MaxListeners];
            }
            // Debug.Log("searching for an empty space for:"+caller + " eventid:"+event);
            for (i = 0; i < MaxListeners; i++)
            {
                int point = eventId * MaxListeners + i;
                if (goListeners[point] == null || eventListeners[point] == null)
                {
                    eventListeners[point] = callback;
                    goListeners[point] = caller;
                    if (i >= eventsMaxSearch)
                    {
                        eventsMaxSearch = i + 1;
                    }
                    // Debug.Log("adding event for:"+caller.name);

                    return;
                }
                if (goListeners[point] == caller && ReferenceEquals(eventListeners[point], callback))
                {
                    // Debug.Log("This event is already being listened for.");
                    return;
                }
            }
            Debug.LogError("You ran out of areas to add listeners, consider increasing LISTENERS_MAX, ex: LeanTween.LISTENERS_MAX = " + (MaxListeners * 2));
        }

        public static bool RemoveListener(int eventId, Action<TweenEvent> callback)
        {
            return RemoveListener(TweenEmpty, eventId, callback);
        }

        /**
        * Remove an event listener you have added
        * @method LeanTween.removeListener
        * @param {GameObject} caller:GameObject the gameObject the listener is attached to
        * @param {int} eventId:int a unique int that describes the event (best to use an enum)
        * @param {System.Action<LTEvent>} callback:System.Action<LTEvent> the method that was specified to call when the event has been dispatched
        * @example
        * LeanTween.removeListener(gameObject, (int)MyEvents.JUMP, jumpUp);<br>
        * <br>
        * void jumpUp( LTEvent e ){ }<br>
        */

        public static bool RemoveListener(GameObject caller, int eventId, Action<TweenEvent> callback)
        {
            for (i = 0; i < eventsMaxSearch; i++)
            {
                int point = eventId * MaxListeners + i;
                if (goListeners[point] == caller && ReferenceEquals(eventListeners[point], callback))
                {
                    eventListeners[point] = null;
                    goListeners[point] = null;
                    return true;
                }
            }
            return false;
        }

        /**
        * Tell the added listeners that you are dispatching the event
        * @method LeanTween.dispatchEvent
        * @param {int} eventId:int a unique int that describes the event (best to use an enum)
        * @example
        * LeanTween.dispatchEvent( (int)MyEvents.JUMP );<br>
        */

        public static void DispatchEvent(int eventId)
        {
            DispatchEvent(eventId, null);
        }

        /**
        * Tell the added listeners that you are dispatching the event
        * @method LeanTween.dispatchEvent
        * @param {int} eventId:int a unique int that describes the event (best to use an enum)
        * @param {object} data:object Pass data to the listener, access it from the listener with *.data on the LTEvent object
        * @example
        * LeanTween.dispatchEvent( (int)MyEvents.JUMP, transform );<br>
        * <br>
        * void jumpUp( LTEvent e ){<br>
        * &nbsp; Transform tran = (Transform)e.data;<br>
        * }<br>
        */

        public static void DispatchEvent(int eventId, object data)
        {
            for (int k = 0; k < eventsMaxSearch; k++)
            {
                int point = eventId * MaxListeners + k;
                if (eventListeners[point] != null)
                {
                    if (goListeners[point])
                    {
                        eventListeners[point](new TweenEvent(eventId, data));
                    }
                    else
                    {
                        eventListeners[point] = null;
                    }
                }
            }
        }
    }
}