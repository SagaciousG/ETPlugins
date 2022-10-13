using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.PSD.FileFormats.Psd.Layers;
using UnityEngine;

namespace XGame
{
    public abstract class IPSDLayer
    {
        public PSDLayerGroup Parent { set; get; }

        public virtual string RealName
        {
            get => _realName;
            set
            {
                if (_realName == value)
                    return;
                _realName = value;

                var nameMatches = value.Split('@');
                if (IsRoot)
                {
                    Name = nameMatches[1];
                }
                else
                {
                    Name = nameMatches.TryMatchOne(a => a.StartsWith("name="), out var res)
                        ? res.Replace("name=", "")
                        : nameMatches[0];
                    ReloadTags();
                }
            }
        }

        public string UName
        {
            get
            {
                return $"{Parent?.UName}-{Name}{LayerIndex}".Trim('-');
            }
        }

        public int UID => UName.GetHashCode();
        public string Name { get; private set; }
        public Vector2 RootSize { get; set; }
        public Vector2 Size { set; get; }
        public Vector2 CenterPosition { set; get; }

        public Vector2 Position =>
            new Vector2(RootSize.x / 2 + CenterPosition.x,
                RootSize.y / 2 - CenterPosition.y) + Size / 2;

        public Rect Rect => new Rect(CenterPosition - Size / 2, Size);
        public bool Ignore
        {
            get
            {
                if (IsRoot)
                    return false;
                if (_tags == null)
                    return false;
                //因为在文件节点下，当子节点被解析完成时，父节点并没有被拿到全部数据，所以该字段需要动态获取
                var res = _tags.Contains("ignore") || (Parent?.Ignore ?? false);
                return res;
            }
        }

        public bool Visible = true;
        public bool Reference
        {
            get
            {
                if (IsRoot)
                    return false;
                if (_tags == null)
                    return false;
                foreach (var tag in _tags)
                {
                    if (tag.StartsWith("ref"))
                        return true;
                }

                return false;
            }
        }

        public bool IsRoot;
        public int LayerIndex;

        public string[] Tags
        {
            get => _tags;
            protected set => _tags = value;
        }

        public PNGStreamInfo PngStreamInfo;
        private string[] _tags;
        private string _realName;
        
        public void SetTransform(RectTransform transform, Vector2 screenSize)
        {
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size.x);
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size.y);
            var centerPos = CenterPosition + new Vector2(screenSize.x / 2, screenSize.y / 2);
            transform.position = centerPos + new Vector2(Size.x * (transform.pivot.x - 0.5f), Size.y * (transform.pivot.y - 0.5f));
            transform.gameObject.SetActive(Visible);
        }

        public abstract void TagToVariableValue();
        public abstract void SetVariableValue(ref RectTransform rect);
        public abstract void SetDefaultValue(RectTransform obj);

        public bool ContainTag(string tag)
        {
            foreach (var t in _tags)
            {
                if (t.StartsWith(tag))
                    return true;
            }

            return false;
        }
        public virtual void SetName(string name)
        {
            foreach (var tag in _tags)
            {
                if (tag.StartsWith("name="))
                {
                    RealName = _realName.Replace(tag, $"name={name}");
                    return;
                }
            }

            RealName = $"{_realName}@name={name}";
        }

        public void AddTag(string tag)
        {
            var kv = tag.Split('=');
            foreach (var t in _tags)
            {
                var kv0 = t.Split('=');
                if (kv0[0] == kv[0])
                    return;
                if (tag == "ref" && t.StartsWith("ref"))
                    return;
            }

            RealName = $"{_realName}@{tag}";
        }
        
        public virtual bool ValidTag(string tag)
        {
            if (tag == "ignore" 
                || tag.StartsWith("ref") 
                || tag.StartsWith("name=") 
                || tag.StartsWith("isPro") 
                )
                return true;
            return false;
        }

        public virtual bool IsOldTag(string tag)
        {
            if (tag.StartsWith("f="))
            {
                return true;
            }

            return false;
        }

        public virtual void ReplaceOldTags()
        {
            var list = _tags.ToList();
            for (int i = _tags.Length - 1; i >= 0; i--)
            {
                var t = _tags[i];
                if (t.StartsWith("f="))
                {
                    var refVal = "ref";
                    list.RemoveAt(i);
                    for (int j = list.Count - 1; j >= 0; j--)
                    {
                        if (list[j].StartsWith("name="))
                            list.RemoveAt(j);
                        if (list[j].StartsWith("ref"))
                        {
                            refVal = list[j];
                            list.RemoveAt(j);
                        }
                    }   
                    list.Add(t.Replace("f=", "name="));
                    list.Add(refVal);
                }
            }
            
            _tags = list.ToArray();
            RefreshRealName();
        }

        public virtual void RefreshRealName()
        {
            var nameMatches = _realName.Split('@');
            var res = nameMatches[0];
            foreach (var tag in _tags)
            {
                res += $"@{tag}";
            }

            _realName = res;
        }

        private void ReloadTags()
        {
            var nameMatches = RealName.Split('@');
            _tags = new string[nameMatches.Length - 1];
            if (_tags.Length > 0)
                Array.Copy(nameMatches, 1, _tags, 0, _tags.Length);
            TagToVariableValue();
        }
    }
}