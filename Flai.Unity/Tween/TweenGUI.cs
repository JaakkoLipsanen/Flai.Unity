using UnityEngine;

namespace Flai.Tween
{
    public class TweenGUI
    {
        public enum ElementType
        {
            Texture,
            Label
        }

        public static int RectLevels = 5;
        public static int RectsPerLevel = 10;
        public static int ButtonsMax = 24;

        private static TweenRectangle[] levels;
        private static int[] levelDepths;
        private static Rect[] buttons;
        private static int[] buttonLevels;
        private static int[] buttonLastFrame;
        private static TweenRectangle r;
        private static Color color = Color.white;
        private static bool isGUIEnabled;
        private static int global_counter;

        public static void Initialize()
        {
            if (levels == null)
            {
                levels = new TweenRectangle[RectLevels * RectsPerLevel];
                levelDepths = new int[RectLevels];
            }
        }

        public static void InitRectCheck()
        {
            if (buttons == null)
            {
                buttons = new Rect[ButtonsMax];
                buttonLevels = new int[ButtonsMax];
                buttonLastFrame = new int[ButtonsMax];
                for (int i = 0; i < buttonLevels.Length; i++)
                {
                    buttonLevels[i] = -1;
                }
            }
        }

        public static void Reset()
        {
            if (isGUIEnabled)
            {
                isGUIEnabled = false;
                for (int i = 0; i < levels.Length; i++)
                {
                    levels[i] = null;
                }

                for (int i = 0; i < levelDepths.Length; i++)
                {
                    levelDepths[i] = 0;
                }
            }
        }

        public static void Update(int updateLevel)
        {
            if (isGUIEnabled)
            {
                Initialize();
                if (levelDepths[updateLevel] > 0)
                {
                    color = GUI.color;
                    int baseI = updateLevel * RectsPerLevel;
                    int maxLoop = baseI + levelDepths[updateLevel]; // RECTS_PER_LEVEL;//;

                    for (int i = baseI; i < maxLoop; i++)
                    {
                        r = levels[i];
                        // Debug.Log("r:"+r+" i:"+i);
                        if (r != null /*&& checkOnScreen(r.rect)*/)
                        {
                            //Debug.Log("label:"+r.labelStr+" textColor:"+r.style.normal.textColor);
                            if (r.UseColor)
                            {
                                GUI.color = r.Color;
                            }
                            if (r.Type == ElementType.Label)
                            {
                                if (r.Style != null)
                                {
                                    GUI.skin.label = r.Style;
                                }
                                if (r.UseSimpleScale)
                                {
                                    GUI.Label(new Rect((r.Rect.x + r.Margin.x + r.RelativeRect.x) * r.RelativeRect.width, (r.Rect.y + r.Margin.y + r.RelativeRect.y) * r.RelativeRect.height, r.Rect.width * r.RelativeRect.width, r.Rect.height * r.RelativeRect.height), r.LabelStr);
                                }
                                else
                                {
                                    GUI.Label(new Rect(r.Rect.x + r.Margin.x, r.Rect.y + r.Margin.y, r.Rect.width, r.Rect.height), r.LabelStr);
                                }
                            }
                            else if (r.Type == ElementType.Texture && r.Texture != null)
                            {
                                Vector2 size = r.UseSimpleScale ? new Vector2(0f, r.Rect.height * r.RelativeRect.height) : new Vector2(r.Rect.width, r.Rect.height);
                                if (r.SizeByHeight)
                                {
                                    size.x = r.Texture.width / (float)r.Texture.height * size.y;
                                }
                                if (r.UseSimpleScale)
                                {
                                    GUI.DrawTexture(new Rect((r.Rect.x + r.Margin.x + r.RelativeRect.x) * r.RelativeRect.width, (r.Rect.y + r.Margin.y + r.RelativeRect.y) * r.RelativeRect.height, size.x, size.y), r.Texture);
                                }
                                else
                                {
                                    GUI.DrawTexture(new Rect(r.Rect.x + r.Margin.x, r.Rect.y + r.Margin.y, size.x, size.y), r.Texture);
                                }
                            }
                        }
                    }
                    GUI.color = color;
                }
            }
        }

        public static bool CheckOnScreen(Rect rect)
        {
            bool offLeft = rect.x + rect.width < 0f;
            bool offRight = rect.x > Screen.width;
            bool offBottom = rect.y > Screen.height;
            bool offTop = rect.y + rect.height < 0f;

            return !(offLeft || offRight || offBottom || offTop);
        }

        public static void Destroy(int id)
        {
            int backId = id & 0xFFFF;
            int backCounter = id >> 16;
            if (id >= 0 && levels[backId] != null && levels[backId].HasInitiliazed && levels[backId].Counter == backCounter)
            {
                levels[backId] = null;
            }
        }

        public static TweenRectangle Label(Rect rect, string label, int depth)
        {
            return TweenGUI.Label(new TweenRectangle(rect), label, depth);
        }

        public static TweenRectangle Label(TweenRectangle rect, string label, int depth)
        {
            rect.Type = ElementType.Label;
            rect.LabelStr = label;
            return Element(rect, depth);
        }

        public static TweenRectangle Texture(Rect rect, Texture texture, int depth)
        {
            return TweenGUI.Texture(new TweenRectangle(rect), texture, depth);
        }

        public static TweenRectangle Texture(TweenRectangle rect, Texture texture, int depth)
        {
            rect.Type = ElementType.Texture;
            rect.Texture = texture;
            return Element(rect, depth);
        }

        public static TweenRectangle Element(TweenRectangle rect, int depth)
        {
            isGUIEnabled = true;
            Initialize();
            int maxLoop = depth * RectsPerLevel + RectsPerLevel;
            int k = 0;
            if (rect != null)
            {
                Destroy(rect.ID);
            }
            if (rect.Type == ElementType.Label && rect.Style != null)
            {
                if (rect.Style.normal.textColor.a <= 0f)
                {
                    Debug.LogWarning("Your GUI normal color has an alpha of zero, and will not be rendered.");
                }
            }
            if (rect.RelativeRect.width == float.PositiveInfinity)
            {
                rect.RelativeRect = new Rect(0f, 0f, Screen.width, Screen.height);
            }
            for (int i = depth * RectsPerLevel; i < maxLoop; i++)
            {
                r = levels[i];
                if (r == null)
                {
                    r = rect;
                    r.RotateEnabled = true;
                    r.AlphaEnabled = true;
                    r.SetId(i, global_counter);
                    levels[i] = r;
                    // Debug.Log("k:"+k+ " maxDepth:"+levelDepths[depth]);
                    if (k >= levelDepths[depth])
                    {
                        levelDepths[depth] = k + 1;
                    }
                    global_counter++;
                    return r;
                }
                k++;
            }

            Debug.LogError("You ran out of GUI Element spaces");

            return null;
        }

        public static bool HasNoOverlap(Rect rect, int depth)
        {
            InitRectCheck();
            bool hasNoOverlap = true;
            bool wasAddedToList = false;
            for (int i = 0; i < buttonLevels.Length; i++)
            {
                // Debug.Log("buttonLastFrame["+i+"]:"+buttonLastFrame[i]);
                //Debug.Log("buttonLevels["+i+"]:"+buttonLevels[i]);
                if (buttonLevels[i] >= 0)
                {
                    //Debug.Log("buttonLastFrame["+i+"]:"+buttonLastFrame[i]+" Time.frameCount:"+Time.frameCount);
                    if (buttonLastFrame[i] + 1 < Time.frameCount)
                    {
                        // It has to have been visible within the current, or
                        buttonLevels[i] = -1;
                        // Debug.Log("resetting i:"+i);
                    }
                    else
                    {
                        //if(buttonLevels[i]>=0)
                        //	 Debug.Log("buttonLevels["+i+"]:"+buttonLevels[i]);
                        if (buttonLevels[i] > depth)
                        {
                            /*if(firstTouch().x > 0){
							Debug.Log("buttons["+i+"]:"+buttons[i] + " firstTouch:");
							Debug.Log(firstTouch());
							Debug.Log(buttonLevels[i]);
						}*/
                            if (PressedWithinRect(buttons[i]))
                            {
                                hasNoOverlap = false; // there is an overlapping button that is higher
                            }
                        }
                    }
                }

                if (wasAddedToList == false && buttonLevels[i] < 0)
                {
                    wasAddedToList = true;
                    buttonLevels[i] = depth;
                    buttons[i] = rect;
                    buttonLastFrame[i] = Time.frameCount;
                }
            }

            return hasNoOverlap;
        }

        public static bool PressedWithinRect(Rect rect)
        {
            Vector2 vec2 = FirstTouch();
            if (vec2.x < 0f)
            {
                return false;
            }
            float vecY = Screen.height - vec2.y;
            return (vec2.x > rect.x && vec2.x < rect.x + rect.width && vecY > rect.y && vecY < rect.y + rect.height);
        }

        public static bool CheckWithinRect(Vector2 vec2, Rect rect)
        {
            vec2.y = Screen.height - vec2.y;
            return (vec2.x > rect.x && vec2.x < rect.x + rect.width && vec2.y > rect.y && vec2.y < rect.y + rect.height);
        }

        public static Vector2 FirstTouch()
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                return UnityEngine.Input.touches[0].position;
            }
            if (UnityEngine.Input.GetMouseButton(0))
            {
                return UnityEngine.Input.mousePosition;
            }

            return new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
        }
    }
}
