﻿﻿using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
  using XGame;
  using Object = UnityEngine.Object;

public class XGUI 
{
    public static Transform Canvas
    {
        get
        {
            var canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
                canvas = Object.FindObjectOfType<Canvas>();
            }

            return canvas.transform;
        }
    }
    
    [MenuItem("GameObject/XGUI/UIList", priority = 10)]
    public static void UIList(MenuCommand command)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UIElement/XUIList.prefab");
        var context = command.context as GameObject;
        var slider = Object.Instantiate(prefab, context != null ? context.transform : Canvas, false);
        slider.name = "UIList";
        Selection.activeObject = slider;
    }
    
    [MenuItem("GameObject/XGUI/UIRoot", priority = -1)]
    public static void XUIRoot(MenuCommand command)
    {
        var panel = new GameObject("UIRoot", typeof(RectTransform), typeof(UIOptions));
        var context = command.context as GameObject;
        // panel.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        panel.transform.SetParent(context ? context.transform : Canvas, false);
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMax = Vector2.one;
        panelRect.anchorMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelRect.offsetMin = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        Selection.activeObject = panel;
    }
    
    [MenuItem("GameObject/XGUI/XImage", priority = -1)]
    public static void XUIXImage(MenuCommand command)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UIElement/XImage.prefab");
        var context = command.context as GameObject;
        var obj = Object.Instantiate(prefab, context != null ? context.transform : Canvas, false);
        obj.name = "XImage";
        Selection.activeObject = obj;
    }
    
    [MenuItem("GameObject/XGUI/XButton", priority = -1)]
    public static void XUIButton(MenuCommand command)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UIElement/XButton.prefab");
        var context = command.context as GameObject;
        var obj = Object.Instantiate(prefab, context != null ? context.transform : Canvas, false);
        obj.name = "XButton";
        Selection.activeObject = obj;
    }
    
    [MenuItem("GameObject/XGUI/XText", priority = -1)]
    public static void XUIXText(MenuCommand command)
    {
        var panel = CreateText();
        var context = command.context as GameObject;
        panel.transform.SetParent(context ? context.transform : Canvas, false);
        Selection.activeObject = panel;
    }
    
        
    [MenuItem("GameObject/XGUI/XSlider", priority = -1)]
    public static void XUIXSlider(MenuCommand command)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UIElement/XSlider.prefab");
        var context = command.context as GameObject;
        var slider = Object.Instantiate(prefab, context != null ? context.transform : Canvas, false);
        slider.name = "XSlider";
        Selection.activeObject = slider;
    }
    
    [MenuItem("GameObject/XGUI/XInputField", priority = -1)]
    public static void XInputField(MenuCommand command)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UIElement/XInputField.prefab");
        var context = command.context as GameObject;
        var obj = Object.Instantiate(prefab, context != null ? context.transform : Canvas, false);
        obj.name = "XInputField";
        Selection.activeObject = obj;
    }
    
    [MenuItem("GameObject/XGUI/XToggle", priority = -1)]
    public static void XToggle(MenuCommand command)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UIElement/XToggle.prefab");
        var context = command.context as GameObject;
        var obj = Object.Instantiate(prefab, context != null ? context.transform : Canvas, false);
        obj.name = "XToggle";
        Selection.activeObject = obj;
    }
    
    [MenuItem("GameObject/XGUI/XPopup", priority = -1)]
    public static void XPopup(MenuCommand command)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UIElement/XPopup.prefab");
        var context = command.context as GameObject;
        var obj = Object.Instantiate(prefab, context != null ? context.transform : Canvas, false);
        obj.name = "XPopup";
        Selection.activeObject = obj;
    }


    private static Text CreateText()
    {
        var obj = new GameObject("XText");
        var t = obj.AddComponent<XText>();
        t.text = "文本";
        t.fontSize = 28;
        t.alignment = TextAnchor.MiddleCenter;
        return t;
    }
}
