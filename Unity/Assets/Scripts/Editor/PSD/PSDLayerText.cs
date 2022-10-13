using System;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace XGame
{
    public class PSDLayerText : IPSDLayer
    {
        public string ShowText;
        public string Text;
        public string Font;
        
        public float FontSize;
        
        public Color Color;
        public bool Bold;
        public bool Italic;
        
        public int TextSpacing;
        public int ColorAdapter = -1;
        public int OutlineWidth;

        public string OutlineColor;

        public bool RaycastTarget
        {
            set
            {
                if(_raycastTarget == value)
                    return;
                _raycastTarget = value;
                _raySet = true;
            }
            get => _raycastTarget;
        }
        public TextAnchor Align
        {
            set
            {
                if (_align == value)
                    return;
                _align = value;
                _alignSet = true;
            }
            get => _align;
        }

        public HorizontalWrapMode HorizontalWrapMode
        {
            set
            {
                if (_horizontalWrap == value)
                    return;
                _horizontalWrap = value;
                _horSet = true;
            }
            get => _horizontalWrap;
        }

        public VerticalWrapMode VerticalWrapMode
        {
            set
            {
                if (_verticalWrap == value)
                    return;
                _verSet = true;
                _verticalWrap = value;
            }
            get => _verticalWrap;
        }

        protected bool _raySet;
        protected bool _alignSet;
        protected bool _horSet;
        protected bool _verSet;
        protected bool _raycastTarget = false;
        protected TextAnchor _align = TextAnchor.MiddleCenter;
        protected HorizontalWrapMode _horizontalWrap = HorizontalWrapMode.Overflow;
        protected VerticalWrapMode _verticalWrap = VerticalWrapMode.Overflow;
        
        public override void TagToVariableValue()
        {
            foreach (var tag in Tags)
            {
                if (tag.StartsWith("font="))
                {
                    Font = tag.Replace("font=", "").Trim();
                }
                else if (tag.StartsWith("size="))
                {
                    FontSize = Convert.ToInt32(tag.Replace("size=", ""));
                }
                else if (tag.StartsWith("sz="))
                {
                    FontSize = Convert.ToInt32(tag.Replace("sz=", ""));
                }
                else if (tag.StartsWith("up"))
                {
                    ShowText = ShowText?.ToUpper();
                }
                else if (tag == "ref")
                {
                    ShowText = "";
                }
               
                else if (tag.StartsWith("cidx="))
                {
                    ColorAdapter = Convert.ToInt32(tag.Replace("cidx=", ""));
                }else if (tag.StartsWith("outline=") || tag.StartsWith("ol="))
                {
                    var ss = tag.Replace("ol=", "").Replace("outline=", "").Split('x');
                    OutlineColor = ss[0];
                    if (ss.Length == 2)
                    {
                        OutlineWidth = Convert.ToInt32(ss[1]);
                    }
                    else
                    {
                        OutlineWidth = 2;
                    }
                }
            }
        }
        
        public override void SetVariableValue(ref RectTransform rect)
        {
            var text = rect.GetComponent<Text>();
            text.color = Color;
            text.fontSize = (int)FontSize;
            text.text = ShowText;
            if (!string.IsNullOrEmpty(Font))
            {
                var fonts = AssetDatabase.FindAssets($"t:Font {Font}");
                foreach (var f in fonts)
                {
                    var p = AssetDatabase.GUIDToAssetPath(f);
                    if (Path.GetFileNameWithoutExtension(p) == Font)
                    {
                        text.font = AssetDatabase.LoadAssetAtPath<Font>(p);
                        break;
                    }
                }
            }
            else
            {
                var setting = AssetDatabase.LoadAssetAtPath<PSDSetting>("Assets/PsdSetting.asset");
                text.font = setting.DefaultFont;
            }

            if (Regex.IsMatch(ShowText, "^[a-z ]$"))
            {
                text.rectTransform.anchoredPosition += new Vector2(0, 5);
            }
            else
                text.rectTransform.anchoredPosition += new Vector2(0, 2);
            
            if (Bold && Italic)
            {
                text.fontStyle = FontStyle.BoldAndItalic;
            }
            else if (Bold)
            {
                text.fontStyle = FontStyle.Bold;
            }
            else if (Italic)
            {
                text.fontStyle = FontStyle.Italic;
            }
            else
            {
                text.fontStyle = FontStyle.Normal;
            }

            if (_alignSet)
                text.alignment = Align;
            if (_horSet)
                text.horizontalOverflow = HorizontalWrapMode;
            if (_verSet)
                text.verticalOverflow = VerticalWrapMode;
            if (_raySet)
                text.raycastTarget = _raycastTarget;
            
            if (string.IsNullOrEmpty(OutlineColor))
            {
                var outline = text.gameObject.GetComponent<Outline>();
                if (outline != null)
                    Object.DestroyImmediate(outline);
            }
            else
            {
                var outline = text.gameObject.GetComponent<Outline>();
                if (outline == null)
                    outline = text.gameObject.AddComponent<Outline>();
                ColorUtility.TryParseHtmlString($"#{OutlineColor}", out var color);
                outline.effectColor = color;
                outline.effectDistance = new Vector2(OutlineWidth, OutlineWidth * -1);
            }
            
        }

        public override void SetDefaultValue(RectTransform obj)
        {
            var pro = obj.GetComponent<TextMeshProUGUI>();
            if (pro != null)
                Object.Destroy(pro);
            var t = obj.GetComponent<XText>();
            if (t == null)
                t = obj.gameObject.AddComponent<XText>();
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.alignment = TextAnchor.MiddleCenter;
            t.raycastTarget = false;
            
            _horizontalWrap = HorizontalWrapMode.Overflow;
            _verticalWrap = VerticalWrapMode.Overflow;
            _align = TextAnchor.MiddleCenter;
            _raycastTarget = false;
        }

        public override bool IsOldTag(string tag)
        {
            if (tag.StartsWith("sz="))
                return true;
            
            return base.IsOldTag(tag);
        }

        public override void ReplaceOldTags()
        {
            for (var i = 0; i < Tags.Length; i++)
            {
                var t = Tags[i];
                if (t.StartsWith("sz="))
                {
                    Tags[i] = t.Replace("sz=", "size=");
                }
            }

            base.ReplaceOldTags();
        }

        public override bool ValidTag(string tag)
        {
            if (tag.StartsWith("font=") 
                || tag.StartsWith("size=")
                || tag.StartsWith("sz=")
                || tag.StartsWith("up")
                || tag.StartsWith("cidx=")
                
                )
                return true;
            return base.ValidTag(tag);
        }
    }
}