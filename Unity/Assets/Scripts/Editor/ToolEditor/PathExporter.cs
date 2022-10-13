using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XGame;

  public class PathExporter : EditorWindow
{
    [MenuItem("Tools/文件夹导航")]
    private static void ShowWindow()
    {
        GetWindow<PathExporter>().Show();
    }
    
    private Dictionary<string, string> _namePath;

    private void OnEnable()
    {
        _namePath = new Dictionary<string, string>()
        {
            {"缓存目录", $"{Application.persistentDataPath}"},
        };
    }


    private void OnGUI()
    {
        foreach (var kv in _namePath)
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField(kv.Key, GUILayout.Width(200));
            EditorGUILayout.LabelField(kv.Value);
            if (GUILayout.Button("打开", GUILayout.Width(100)))
            {
               CMDHelper.OpenFolderInExplorer(kv.Value.Replace("/", "\\"));
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
