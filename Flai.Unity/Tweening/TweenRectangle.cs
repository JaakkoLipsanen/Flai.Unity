using System;
using UnityEngine;
using Object = System.Object;

namespace Flai.Tweening
{
    /**
     * Animate GUI Elements by creating this object and passing the *.rect variable to the GUI method<br><br>
     * <strong>Example Javascript: </strong><br>var bRect:LTRect = new LTRect( 0, 0, 100, 50 );<br>
     * LeanTween.scale( bRect, Vector2(bRect.rect.width, bRect.rect.height) * 1.3, 0.25 );<br>
     * function OnGUI(){<br>
     * &nbsp; if(GUI.Button(bRect.rect, "Scale")){ }<br>
     * }<br>
     * <br>
     * <strong>Example C#: </strong> <br>
     * LTRect bRect = new LTRect( 0f, 0f, 100f, 50f );<br>
     * LeanTween.scale( bRect, new Vector2(150f,75f), 0.25f );<br>
     * void OnGUI(){<br>
     * &nbsp; if(GUI.Button(bRect.rect, "Scale")){ }<br>
     * }<br>
     *
     * @class LTRect
     * @constructor
     * @param {float} x:float X location
     * @param {float} y:float Y location
     * @param {float} width:float Width
     * @param {float} height:float Height
     * @param {float} alpha:float (Optional) initial alpha amount (0-1)
     * @param {float} rotation:float (Optional) initial rotation in degrees (0-360) 
     */

    [Serializable]
    public class TweenRectangle : Object
    {
        /**
	* Pass this value to the GUI Methods
	* 
	* @property rect
	* @type {Rect} rect:Rect Rect object that controls the positioning and size
	*/
        public static bool ColorTouched;
        private int _id = -1;

        // todo: make private??
        public Rect Rect;
        public float Alpha = 1f;
        public bool AlphaEnabled;
        public Color Color = Color.white;

        [HideInInspector]
        public int Counter;

        [HideInInspector]
        public bool RotateFinished;

        public bool FontScaleToFit;
        public string LabelStr;
        public Vector2 Margin;
        public Vector2 Pivot;
        public Rect RelativeRect = new Rect(0f, 0f, float.PositiveInfinity, float.PositiveInfinity);
        public bool RotateEnabled;
        public float Rotation;
        public bool SizeByHeight;
        public GUIStyle Style;
        public Texture Texture;
        public TweenGUI.ElementType Type;
        public bool UseColor = false;
        public bool UseSimpleScale;

        public TweenRectangle()
        {
            Reset();
            RotateEnabled = AlphaEnabled = true;
            Rect = new Rect(0f, 0f, 1f, 1f);
        }

        public TweenRectangle(Rect rect)
        {
            this.Rect = rect;
            Reset();
        }

        public TweenRectangle(float x, float y, float width, float height)
        {
            Rect = new Rect(x, y, width, height);
            Alpha = 1.0f;
            Rotation = 0.0f;
            RotateEnabled = AlphaEnabled = false;
        }

        public TweenRectangle(float x, float y, float width, float height, float alpha)
        {
            Rect = new Rect(x, y, width, height);
            this.Alpha = alpha;
            Rotation = 0.0f;
            RotateEnabled = AlphaEnabled = false;
        }

        public TweenRectangle(float x, float y, float width, float height, float alpha, float rotation)
        {
            Rect = new Rect(x, y, width, height);
            this.Alpha = alpha;
            this.Rotation = rotation;
            RotateEnabled = AlphaEnabled = false;
            if (rotation != 0.0f)
            {
                RotateEnabled = true;
                ResetForRotation();
            }
        }

        public bool HasInitiliazed
        {
            get { return _id != -1; }
        }

        public int ID
        {
            get
            {
                int toId = _id | Counter << 16;

                /*uint backId = toId & 0xFFFF;
			uint backCounter = toId >> 16;
			if(_id!=backId || backCounter!=counter){
				Debug.LogError("BAD CONVERSION toId:"+_id);
			}*/

                return toId;
            }
        }

        public float X
        {
            get { return Rect.x; }
            set { Rect.x = value; }
        }

        public float Y
        {
            get { return Rect.y; }
            set { Rect.y = value; }
        }

        public float Width
        {
            get { return Rect.width; }
            set { Rect.width = value; }
        }

        public float Height
        {
            get { return Rect.height; }
            set { Rect.height = value; }
        }

        private Rect _ect
        {
            get
            {
                if (ColorTouched)
                {
                    ColorTouched = false;
                    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1.0f);
                }
                if (RotateEnabled)
                {
                    if (RotateFinished)
                    {
                        RotateFinished = false;
                        RotateEnabled = false;
                        //this.rotation = 0.0f;
                        Pivot = Vector2.zero;
                    }
                    else
                    {
                        GUIUtility.RotateAroundPivot(Rotation, Pivot);
                    }
                }
                if (AlphaEnabled)
                {
                    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, Alpha);
                    ColorTouched = true;
                }
                if (FontScaleToFit)
                {
                    if (UseSimpleScale)
                    {
                        Style.fontSize = (int)(Rect.height * RelativeRect.height);
                    }
                    else
                    {
                        Style.fontSize = (int)Rect.height;
                    }
                }
                return Rect;
            }

            set { Rect = value; }
        }

        public void SetId(int id, int counter)
        {
            _id = id;
            this.Counter = counter;
        }

        public void Reset()
        {
            Alpha = 1.0f;
            Rotation = 0.0f;
            RotateEnabled = AlphaEnabled = false;
            Margin = Vector2.zero;
            SizeByHeight = false;
            UseColor = false;
        }

        public void ResetForRotation()
        {
            var scale = new Vector3(GUI.matrix[0, 0], GUI.matrix[1, 1], GUI.matrix[2, 2]);
            if (Pivot == Vector2.zero)
            {
                Pivot = new Vector2((Rect.x + ((Rect.width) * 0.5f)) * scale.x + GUI.matrix[0, 3], (Rect.y + ((Rect.height) * 0.5f)) * scale.y + GUI.matrix[1, 3]);
            }
        }

        public TweenRectangle SetStyle(GUIStyle style)
        {
            this.Style = style;
            return this;
        }

        public TweenRectangle SetFontScaleToFit(bool fontScaleToFit)
        {
            this.FontScaleToFit = fontScaleToFit;
            return this;
        }

        public TweenRectangle SetColor(Color color)
        {
            this.Color = color;
            UseColor = true;
            return this;
        }

        public TweenRectangle SetAlpha(float alpha)
        {
            this.Alpha = alpha;
            return this;
        }

        public TweenRectangle SetLabel(String str)
        {
            LabelStr = str;
            return this;
        }

        public TweenRectangle SetUseSimpleScale(bool useSimpleScale, Rect relativeRect)
        {
            this.UseSimpleScale = useSimpleScale;
            this.RelativeRect = relativeRect;
            return this;
        }

        public TweenRectangle SetUseSimpleScale(bool useSimpleScale)
        {
            this.UseSimpleScale = useSimpleScale;
            RelativeRect = new Rect(0f, 0f, Screen.width, Screen.height);
            return this;
        }

        public TweenRectangle SetSizeByHeight(bool sizeByHeight)
        {
            this.SizeByHeight = sizeByHeight;
            return this;
        }

        public override string ToString()
        {
            return "x:" + Rect.x + " y:" + Rect.y + " width:" + Rect.width + " height:" + Rect.height;
        }
    }
}
