using System;
using System.IO;
using System.Linq;
using Aspose.PSD.FileFormats.Psd.Layers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace XGame
{
    public class PSDLayerImage : IPSDLayer
    {
        public int Alpha;
        public string ImageName;
        public Layer ImageLayer;
        //仅用于创建RectTransform，不带Image组件
        public bool Empty
        {
            get
            {
                if (Tags == null)
                    return false;
                foreach (var tag in Tags)
                {
                    if (tag.StartsWith("ys="))
                        return true;
                    if (tag == "empty")
                        return true;
                }

                return false;
            }
        }

        public override string RealName
        {
            get => base.RealName;
            set
            {
                base.RealName = value;
                var nameMatches = value.Split('@');
                ImageName = nameMatches[0];
            }
        }

        public override void TagToVariableValue()
        {
            
        }

        public override void SetVariableValue(ref RectTransform rect)
        {
            foreach (var tag in Tags)
            {
                if (tag.StartsWith("prefab="))
                {
                    var prefab = tag.Replace("prefab=", "").Trim();
                    if (!string.IsNullOrEmpty(prefab))
                    {
                        var path = $"Assets/UI/module/{prefab.Insert(prefab.LastIndexOf('/'), "/prefab")}.prefab";
                        var p = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        if (p == null)
                        {
                            throw new Exception($"{path}为空");
                        }
                        var obj = (GameObject) PrefabUtility.InstantiateAttachedAsset(p);
                        obj.transform.SetParent(rect.parent, false);
                        obj.name = rect.name;
                        Object.DestroyImmediate(rect.gameObject);
                        rect = obj.GetComponent<RectTransform>();
                    }
                }
            }
      
            if (!Empty)
            {
                var image = rect.GetComponent<XImage>();
                if (image == null)
                {
                    Debug.LogError($"{rect.gameObject.name}不存在组件Image，请检查是否需要被标记为Empty");
                    return;
                }
                var c = image.color;
                c.a = Alpha / 100f;
                image.color = c;
                var sprites = AssetDatabase.FindAssets($"t:Sprite {ImageName}");
                foreach (var s in sprites)
                {
                    var p = AssetDatabase.GUIDToAssetPath(s);
                    if (Path.GetFileNameWithoutExtension(p) == ImageName)
                    {
                        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(p);
                        if (image.sprite.border != Vector4.zero && image.type == Image.Type.Simple)
                        {
                            image.type = Image.Type.Sliced;
                        }
                        break;
                    }
                }
                
            }

     
        }

        public override void SetDefaultValue(RectTransform obj)
        {
            var img = obj.GetComponent<XImage>();
            if (img == null && !Empty)
                img = obj.gameObject.AddComponent<XImage>();
        }

        public override bool ValidTag(string tag)
        {
            if (tag.StartsWith("ys=") || tag == "empty" || tag.StartsWith("prefab="))
                return true;
            return base.ValidTag(tag);
        }
    }
}