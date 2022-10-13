using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

namespace XGame
{
    public class PSDLayerGroup : IPSDLayer
    {
        public List<IPSDLayer> PsdLayers = new List<IPSDLayer>();

        public int LayerDividerIndex;

        public override void TagToVariableValue()
        {
            
        }

        public override void SetVariableValue(ref RectTransform transform)
        {
            if (IsRoot)
            {
                return;
            }
            foreach (var tag in Tags)
            {
                if (tag.StartsWith("grid"))
                {
                    var gridLayoutGroup = transform.GetComponent<GridLayoutGroup>();
                    if (gridLayoutGroup == null)
                        gridLayoutGroup = transform.gameObject.AddComponent<GridLayoutGroup>();
                    if (tag.StartsWith("grid="))
                    {
                        var ss = tag.Replace("grid=", "").Split('x');
                        var x = Convert.ToSingle(ss[0]);
                        var y = Convert.ToSingle(ss[1]);
                        gridLayoutGroup.spacing = new Vector2(x, y);
                    }
                    
                }else if (tag.StartsWith("hor"))
                {
                    var horizontalLayoutGroup = transform.GetComponent<HorizontalLayoutGroup>();
                    if (horizontalLayoutGroup == null)
                        horizontalLayoutGroup = transform.gameObject.AddComponent<HorizontalLayoutGroup>();
                    if (tag.StartsWith("hor="))
                    {
                        var ss = tag.Replace("hor=", "").Split('x');
                        var x = Convert.ToSingle(ss[0]);
                        horizontalLayoutGroup.spacing = x;
                    }
                }
                else if (tag.StartsWith("ver"))
                {
                    var verticalLayoutGroup = transform.GetComponent<VerticalLayoutGroup>();
                    if (verticalLayoutGroup == null)
                        verticalLayoutGroup = transform.gameObject.AddComponent<VerticalLayoutGroup>();
                    if (tag.StartsWith("ver="))
                    {
                        var ss = tag.Replace("ver=", "").Split('x');
                        var y = Convert.ToSingle(ss[0]);
                        verticalLayoutGroup.spacing = y;
                    }
                }else if (tag.StartsWith("list="))
                {
                    var list = transform.GetComponent<UIList>();
                    if (list == null)
                        list = transform.gameObject.AddComponent<UIList>();
                }
                else if (tag.StartsWith("img="))
                {
                    var img = transform.GetComponent<Image>();
                    if (img == null)
                        img = transform.gameObject.AddComponent<Image>();
                    var ImageName = tag.Replace("img=", "").Trim();
                    var sprites = AssetDatabase.FindAssets($"t:Sprite {ImageName}");
                    foreach (var s in sprites)
                    {
                        var p = AssetDatabase.GUIDToAssetPath(s);
                        if (Path.GetFileNameWithoutExtension(p) == ImageName)
                        {
                            img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(p);
                            if (img.sprite.border != Vector4.zero)
                            {
                                img.type = Image.Type.Sliced;
                            }

                            break;
                        }
                    }

                }
            }

        }

        public override bool IsOldTag(string tag)
        {
            if (tag == "list")
                return true;
            if (tag.StartsWith("ys="))
                return true;
            if (tag.StartsWith("spacing="))
                return true;
            return base.IsOldTag(tag);
        }

        public override void ReplaceOldTags()
        {
            bool isList = false;
            var list = Tags.ToList();
            for (var i = Tags.Length - 1; i >= 0; i--)
            {
                var tag = Tags[i];
                if (tag == "list")
                {
                    isList = true;
                    list.RemoveAt(i);
                }

                if (tag.StartsWith("spacing="))
                {
                    var symbol = "hor";
                    if (tag.Contains('x'))
                        symbol = "grid";
                    list[i] = tag.Replace("spacing", symbol);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (isList)
                {
                    if (list[i].StartsWith("ys="))
                        list[i] = list[i].Replace("ys=", "list=");
                }
            }

            Tags = list.ToArray();

            base.ReplaceOldTags();
        }

        public override void SetDefaultValue(RectTransform obj)
        {
            
        }

        public override bool ValidTag(string tag)
        {
            if (tag.StartsWith("grid") 
                || tag.StartsWith("ver") 
                || tag.StartsWith("hor") 
                || tag.StartsWith("list=")
                )
                return true;
            return base.ValidTag(tag);
        }
    }

    public enum PSDGroupFlag
    {
        None,
        GridLayout,
        HorizontalLayout,
        VerticalLayout,
    }
}