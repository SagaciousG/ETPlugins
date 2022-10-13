﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GUIStyleViewer : EditorWindow
{

    Vector2 scrollPosition = new Vector2(0, 0);
    string search = "";
    GUIStyle textStyle;


    [MenuItem("Window/GUIStyleViewer", false, 100)]
    private static void OpenStyleViewer()
    {
        GUIStyleViewer window = GetWindow<GUIStyleViewer>(false, "查看内置GUIStyle");
    }

    void OnGUI()
    {
        if (textStyle == null)
        {
            textStyle = new GUIStyle("HeaderLabel");
            textStyle.fontSize = 20;
        }

        GUILayout.BeginHorizontal("HelpBox");
        GUILayout.Label("点击示例，可以将其名字复制下来", textStyle);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Search:");
        search = EditorGUILayout.TextField(search);
        GUILayout.EndHorizontal();
        GUILayout.Label("用法：");
        GUILayout.Label("      GUIStyle textStyle = new GUIStyle(\"HeaderLabel\");");
        GUILayout.Label("      textStyle.fontSize = 20;");
        GUILayout.Label("      GUILayout.Label(\"示例\", textStyle, GUILayout.Width(300));");

        GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
        GUILayout.Label("示例", textStyle, GUILayout.Width(300));
        GUILayout.Label("名字", textStyle, GUILayout.Width(300));
        GUILayout.EndHorizontal();


        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (var style in GUI.skin.customStyles)
        {
            if (style.name.ToLower().Contains(search.ToLower()))
            {
                GUILayout.Space(15);
                GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                if (GUILayout.Button(style.name, style, GUILayout.Width(300)))
                {
                    EditorGUIUtility.systemCopyBuffer = style.name;
                    Debug.Log("已复制 " + style.name);
                }
                EditorGUILayout.SelectableLabel(style.name, GUILayout.Width(300));
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
    }
}

