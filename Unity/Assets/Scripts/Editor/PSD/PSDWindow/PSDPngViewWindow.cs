
using System;
using UnityEditor;
using UnityEngine;
using XGame;

public class PSDPngViewWindow : EditorWindow
{
    public static PSDPngViewWindow Instance;
    public static bool isShow;
    private static int _selectedLayer;
    private void OnEnable()
    {
        Instance = this;
        titleContent = new GUIContent("预览");
        isShow = true;
    }

    private void OnDisable()
    {
        Instance = null;
        isShow = false;
    }

    private void Update()
    {
        if (_selectedLayer != PSDUtility.RuntimeInfo.LayerIndex)
        {
            Repaint();
            _selectedLayer = PSDUtility.RuntimeInfo.LayerIndex;
        }
    }

    private void OnGUI()
    {
        var center = new Vector2(position.width / 2, position.height / 2);
        var runtimeInfo = PSDUtility.RuntimeInfo;
        if (runtimeInfo.PSD?.RootTexture != null)
        {
            var stream = runtimeInfo.PSD.GetStreamInfo(runtimeInfo.LayerIndex);
            GUI.color = new Color(0.4f, 0.4f, 0.4f);
            GUI.DrawTexture(new Rect(0, 0, position.width, position.height), 
                runtimeInfo.PSD.RootTexture, ScaleMode.ScaleToFit);
            GUI.color = Color.white;

            var w = runtimeInfo.PSD.RootTexture.width;
            var h = runtimeInfo.PSD.RootTexture.height;
            
            var rate = w * 1f / h;
            var rateDetail = position.width / position.height;

            var scale = 1f;
            if (rateDetail > rate) //宽有多余的
            {
                scale = position.height / h;
            }
            else
            {
                scale = position.width / w;
            }
            
            var rect = runtimeInfo.SelectRect; //相对于窗口中心的坐标
            var filterSize = rect.size * scale;
            var filterOffset = rect.position * scale;
            var box = new Rect(new Vector2(center.x + filterOffset.x, center.y - filterOffset.y - filterSize.y), filterSize);

            GUI.Box(box, "", "EyeDropperPickedPixel");
            if (stream != null && stream.Texture != null)
            {
                GUI.DrawTexture(box, stream.Texture, ScaleMode.ScaleToFit, true);
                // EditorGUI.DrawTextureAlpha(box, stream.Texture, ScaleMode.ScaleToFit);
                // GUI.Box(box, new GUIContent(stream.Sprite.texture));
            }
        }
        else
            GUI.Label(new Rect(center - new Vector2(50, 50), new Vector2(100, 100)), "加载中...");

    }
    
    public class RuntimeInfo
    {
        public PSDInfo PSD;
        public Rect SelectRect;
        public int LayerIndex;
    }
}
