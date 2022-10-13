﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class BuildInIcon : EditorWindow
{

    [MenuItem(("Window/BuildInIcon"))]
    static void Init()
    {
        EditorWindow.GetWindow<BuildInIcon>("MyUnityTextureWindow");
    }

    Vector2 m_Scroll;
    List<Texture2D> _texture2Ds = new List<Texture2D>();

    void Awake()
    {

        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var info = typeof(EditorGUIUtility).GetMethod("GetEditorAssetBundle", flags);
        var bundle = info.Invoke(null, new object[0]) as AssetBundle;
        UnityEngine.Object[] objs = bundle.LoadAllAssets();
        if (null != objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] is Texture2D icon)
                {
                    _texture2Ds.Add(icon);
                }
            }
        }

    }

    void OnGUI()
    {
        m_Scroll = GUILayout.BeginScrollView(m_Scroll);
        float width = 50f;
        int count = Mathf.FloorToInt(position.width / width) - 1;

        for (int i = 0; i < _texture2Ds.Count; i += count)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < count; j++)
            {
                int index = i + j;
                if (index < _texture2Ds.Count)
                {
                    if (GUILayout.Button(new GUIContent(_texture2Ds[index]), GUILayout.Width(width),
                        GUILayout.Height(30)))
                    {
                        GUIUtility.systemCopyBuffer = _texture2Ds[index].name;
                        ShowNotification(new GUIContent(_texture2Ds[index]));
                    }
                }
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }
}