﻿using System;
using System.Collections.Generic;
using System.IO;
 using System.Linq;
 using System.Threading;
 using System.Threading.Tasks;
 using Aspose.Hook;
 using Aspose.PSD;
 using Aspose.PSD.FileFormats.Png;
 using Aspose.PSD.FileFormats.Psd;
using Aspose.PSD.FileFormats.Psd.Layers;
 using Aspose.PSD.ImageLoadOptions;
 using Aspose.PSD.ImageOptions;
 using UnityEngine;

namespace XGame
{
    public class PNGStreamInfo
    {
        public int width;
        public int height;
        public int index;
        public MemoryStream buffer = new MemoryStream();
        public Sprite Sprite 
        {
            get
            {
                if (_sprite == null && Texture != null && _loaded)
                {
                    _sprite = UnityEngine.Sprite.Create(Texture, new Rect(0, 0, _texture.width, _texture.height), Vector2.zero);
                }

                return _sprite;
            }
        }

        public Texture2D Texture
        {
            get
            {
                if (buffer != null && _loaded && _texture == null)
                {
                    _texture = new Texture2D(width, height);
                    _texture.LoadImage(buffer.GetBuffer());
                }

                return _texture;
            }
        }

        public bool loaded
        {
            get => _loaded;
            set => _loaded = value;
        }

        private bool _loaded;
        private Texture2D _texture;
        private Sprite _sprite;
    }
    
    public class PSDInfo : IDisposable
    {
        public Texture2D RootTexture 
        {
            get
            {
                if (!Root.PngStreamInfo.loaded)
                {
                    if (!_loadingPNGs.Contains(Root))
                        _loadingPNGs.Add(Root);
                }

                return Root.PngStreamInfo.Texture;
            }
        }

        public DateTime LastWriteTime
        {
            get { return _lastWriteTime; }
            set { _lastWriteTime = value; }
        }

        public long OriginalSize
        {
            get { return _originalSize; }
            set { _originalSize = value; }
        }

        public PSDLayerGroup Root;
        public PSDParseInfo ParseInfo { private set; get; }
        public PsdImage PsdImage { private set; get; }
        public bool Saving;

        public event Action OnLoaded;
        public bool Loading => _loading;

        private HashSet<string> _layerNames;
        private Dictionary<int, IPSDLayer> _layersMap;

        private Thread _loadThread;
        private List<IPSDLayer> _loadingPNGs = new List<IPSDLayer>();

        private DateTime _lastWriteTime;
        private long _originalSize;
        private bool _loading;
        private string _psdPath;
        public PSDInfo(string path, bool async)
        {
            _psdPath = path;

            _loading = true;
            if (async)
                Task.Run(DoLoad);
            else 
                DoLoad();
            
            _loadThread = new Thread(OnLoadPreview);
            _loadThread.Start();
        }

        private void DoLoad()
        {
            ParseInfo = new PSDParseInfo(_psdPath);
            _layerNames = new HashSet<string>();
            _layersMap = new Dictionary<int, IPSDLayer>();

            var fileInfo = new FileInfo(_psdPath);
            _lastWriteTime = fileInfo.LastWriteTime;
            _originalSize = fileInfo.Length;

            File.Copy(_psdPath, $"{_psdPath}.temp", true);
            
            PsdImage = (PsdImage) Image.Load($"{_psdPath}.temp", new PsdLoadOptions());

            Root = new PSDLayerGroup();
            Root.Size = new Vector2(PsdImage.Width, PsdImage.Height);
            var fileName = Path.GetFileNameWithoutExtension(_psdPath);
            Root.IsRoot = true;
            Root.RealName = fileName;
            Root.CenterPosition = Vector2.zero;
            
            Root.PngStreamInfo = new PNGStreamInfo();
            Root.PngStreamInfo.width = PsdImage.Width;
            Root.PngStreamInfo.height = PsdImage.Height;
            
            _layersMap[Root.UID] = Root;
            var stack = new Stack<PSDLayerGroup>();
            stack.Push(Root);
            for (var index = 0; index < PsdImage.Layers.Length; index++)
            {
                var layer = PsdImage.Layers[index];
                InLayerGroup(layer, index, stack);
            }
            
            FinalVerify(Root);
            _loading = false;
        }
        
        private void OnLoadPreview()
        {
            while (true)
            {
                for (var i = 0; i < _loadingPNGs.Count; i++)
                {
                    var layer = _loadingPNGs[i];
                    if (layer.IsRoot)
                    {
                        PsdImage.Save(layer.PngStreamInfo.buffer, new PngOptions()
                        {
                            ColorType = PngColorType.TruecolorWithAlpha
                        });
                    }
                    else
                    {
                        var psdLayer = PsdImage.Layers[layer.LayerIndex];
                        psdLayer.Save(layer.PngStreamInfo.buffer,
                            new PngOptions() {ColorType = PngColorType.TruecolorWithAlpha});
                    }

                    layer.PngStreamInfo.loaded = true;
                }

                _loadingPNGs.Clear();
                Thread.Sleep(1);
            }
        }
        
        public PNGStreamInfo GetStreamInfo(int index)
        {
            foreach (var layer in _layersMap.Values)
            {
                if (layer.LayerIndex == index)
                {
                    if (!layer.PngStreamInfo.loaded)
                    {
                        if (!_loadingPNGs.Contains(layer))
                            _loadingPNGs.Add(layer);
                    }
                    return layer.PngStreamInfo;
                }
            }
            return null;
        }
        public IPSDLayer GetLayer(int uid)
        {
            _layersMap.TryGetValue(uid, out var layer);
            return layer;
        }
        
        private void InLayerGroup(Layer l, int index, Stack<PSDLayerGroup> stack)
        {
            if (l is SectionDividerLayer || l.DisplayName == "</Layer group>")
            {
                var psdGroup = new PSDLayerGroup();
                psdGroup.Parent = stack.Peek();
                psdGroup.Parent.PsdLayers.Add(psdGroup);
                psdGroup.LayerDividerIndex = index;
                stack.Push(psdGroup);
                return;
            }

            var isPro = !l.DisplayName.Contains("notPro");
            IPSDLayer psdLayer = null;
            var group = stack.Peek();
            switch (l)
            {
                case LayerGroup p1:
                    psdLayer = group;
                    break;
                case TextLayer p2:
                    if (isPro)
                        psdLayer = new PSDLayerTextPro();
                    else
                        psdLayer = new PSDLayerText();
                    psdLayer.Parent = @group;
                    break;
                default:
                    psdLayer = new PSDLayerImage();
                    psdLayer.Parent = @group;
                    break;
            }
            
            psdLayer.RealName = l.DisplayName.Replace("Layer group: ", "");
            psdLayer.LayerIndex = index;
            
            if (l is LayerGroup layerGroup)
            {
                (group.CenterPosition, group.Size) = GetPosSize(layerGroup);
                group.RootSize = Root.Size;
                stack.Pop();
                group.PngStreamInfo = new PNGStreamInfo(){
                    width = (int) group.Size.x,
                    height = (int) group.Size.y,
                    index = index
                };
                return;
            } 
           
            if (l is TextLayer textLayer)
            {
                if (psdLayer.ContainTag("notPro"))
                {
                    var text = psdLayer as PSDLayerText;
                    var font = textLayer.TextData.Items[0];
                    text.Text = textLayer.InnerText;
                    text.ShowText = textLayer.InnerText;
                    text.FontSize = (float) font.Style.FontSize;
                    text.Bold = font.Style.FauxBold;
                    text.Italic = font.Style.FauxItalic;
                    text.Color = new UnityEngine.Color(textLayer.TextColor.R / 255f, textLayer.TextColor.G / 255f, textLayer.TextColor.B / 255f, textLayer.TextColor.A);
                    text.TextSpacing = font.Style.Tracking;
                    text.TagToVariableValue();
                }
                else
                {
                    var text = psdLayer as PSDLayerTextPro;
                    var font = textLayer.TextData.Items[0];
                    text.Text = textLayer.InnerText;
                    text.ShowText = textLayer.InnerText;
                    text.FontSize = (float) font.Style.FontSize;
                    text.Bold = font.Style.FauxBold;
                    text.Italic = font.Style.FauxItalic;
                    text.Color = new UnityEngine.Color(textLayer.TextColor.R / 255f, textLayer.TextColor.G / 255f, textLayer.TextColor.B / 255f, textLayer.TextColor.A);
                    text.TextSpacing = font.Style.Tracking;
                    text.TagToVariableValue();
                }
            }
            else
            {
                var image = psdLayer as PSDLayerImage;
                image.ImageLayer = l;
                image.Alpha = l.FillOpacity;
            }

 
            psdLayer.RootSize = Root.Size;
            psdLayer.Size = new Vector2(l.Width, l.Height);
            psdLayer.CenterPosition = GetCenterPos(l);
            psdLayer.Visible = l.IsVisible;
            @group.PsdLayers.Add(psdLayer);
            psdLayer.PngStreamInfo = new PNGStreamInfo(){
                width = (int) psdLayer.Size.x, 
                height = (int) psdLayer.Size.y,
                index = index
            };
        }

        private void FindBound(LayerGroup layerGroup, ref int l, ref int r, ref int t, ref int b)
        {
            foreach (var g in layerGroup.Layers)
            {
                if (g is LayerGroup lg)
                {
                    FindBound(lg, ref l, ref r, ref t, ref b);
                }
                else if (! (g is SectionDividerLayer))
                {
                    l = Mathf.Min(l, g.Left);
                    r = Mathf.Max(r, g.Right);
                    t = Mathf.Min(t, g.Top);
                    b = Mathf.Max(b, g.Bottom);
                }
            }
        }
        
        private Vector2 GetCenterPos(Layer layer)
        {
            var t = layer.Top;
            var l = layer.Left;
            var r = layer.Right;
            var b = layer.Bottom;
            var center = new Vector2(Root.Size.x / 2, Root.Size.y / 2);
            var pos = new Vector2((l + r) / 2f, (t + b) / 2f);
            return new Vector2(pos.x - center.x, center.y - pos.y);
        }
        
        
        private (Vector2, Vector2) GetPosSize(LayerGroup layerGroup)
        {
            var l = 9999;
            var r = 0;
            var t = 9999;
            var b = 0;
            FindBound(layerGroup, ref l, ref r, ref t, ref b);
            var center = new Vector2(Root.Size.x / 2, Root.Size.y / 2);
            var pos = new Vector2((l + r) / 2f, (t + b) / 2f);
            return (new Vector2(pos.x - center.x, center.y - pos.y),
                new Vector2(r - l, b - t));
        }
        
        private bool VerifyNames(string name)
        {
            if (string.IsNullOrEmpty(name))
                return true;
            if (!_layerNames.Contains(name))
            {
                _layerNames.Add(name);
                return true;
            }
            else
            {
                return false;
                throw new Exception($"{Root.Name}存在重复名称{name}, 可能会导致重新生成时数据错乱或丢失");
            }
            
        }
        
        
        private void FinalVerify(IPSDLayer group)
        {
            if (group.Parent != null)
            {
                _layersMap[group.UID] = group;
                if (!group.Ignore)
                {
                    if (!VerifyNames(group.UName))
                    {
                        group.SetName($"{group.Name}{group.LayerIndex}");
                    }
                }
            }
            if (group is PSDLayerGroup layerGroup)
            {
                foreach (var psdLayer in layerGroup.PsdLayers)
                {
                    FinalVerify(psdLayer);
                }
            }
        }

        public void Dispose()
        {
            _loadThread?.Abort();
            PsdImage?.Dispose();
            File.Delete($"{ParseInfo.FullName}.temp");
        }
    }
}