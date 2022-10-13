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
    public class PSDLayerTextPro : IPSDLayer
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
                    ShowText = ShowText.ToUpper();
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
                        OutlineWidth = 1;
                    }
                }
            }
        }
        
        public override void SetVariableValue(ref RectTransform rect)
        {
            var text = rect.GetComponent<TextMeshProUGUI>();
            text.color = Color;
            text.fontSize = (int)FontSize;
            text.text = ShowText;
            if (!string.IsNullOrEmpty(Font))
            {
                var fonts = AssetDatabase.FindAssets($"{Font}Pro");
                foreach (var f in fonts)
                {
                    var p = AssetDatabase.GUIDToAssetPath(f);
                    if (Path.GetFileNameWithoutExtension(p) == $"{Font}Pro")
                    {
                        text.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(p);
                        break;
                    }
                }
            }
            else
            {
                var setting = AssetDatabase.LoadAssetAtPath<PSDSetting>("Assets/PsdSetting.asset");
                var fonts = AssetDatabase.FindAssets($"{setting.DefaultFont.name}Pro");
                foreach (var f in fonts)
                {
                    var p = AssetDatabase.GUIDToAssetPath(f);
                    if (Path.GetFileNameWithoutExtension(p) == $"{setting.DefaultFont.name}Pro")
                    {
                        text.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(p);
                        break;
                    }
                }
            }

            if (Regex.IsMatch(ShowText, "^[a-z ]$"))
            {
                text.rectTransform.anchoredPosition += new Vector2(0, 5);
            }
            else
                text.rectTransform.anchoredPosition += new Vector2(0, 2);
            

            if (Bold && Italic)
            {
                text.fontStyle = FontStyles.Bold | FontStyles.Italic;
            }
            else if (Bold)
            {
                text.fontStyle = FontStyles.Bold;
            }
            else if (Italic)
            {
                text.fontStyle = FontStyles.Italic;
            }
            else
            {
                text.fontStyle = FontStyles.Normal;
            }
        }

        public override void SetDefaultValue(RectTransform obj)
        {
            var normal = obj.GetComponent<XText>();
            if (normal != null)
            {
                Object.Destroy(normal);
            }
            var t = obj.GetComponent<TextMeshProUGUI>();
            if (t == null)
                t = obj.gameObject.AddComponent<TextMeshProUGUI>();

            t.overflowMode = TextOverflowModes.Overflow;
            t.alignment = TextAlignmentOptions.Midline;
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