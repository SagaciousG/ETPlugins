using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XGame;

public class GameLauncher: EditorWindow
{
    [MenuItem("Tools/Launcher _F5")]
    static void LaunchGame()
    {
        GetWindow<GameLauncher>().Show();
    }

    private string[] _scenes;
    private Dictionary<string, List<string>> _folderNames = new Dictionary<string, List<string>>();
    private HashSet<string> _opend = new HashSet<string>();
    private List<string> _lastestOpen = new List<string>();

    private void OnEnable()
    {
        _scenes = AssetDatabase.FindAssets("t:Scene");
        foreach (var pathUID in _scenes)
        {
            var path = AssetDatabase.GUIDToAssetPath(pathUID);
            var key = Path.GetDirectoryName(path).PathSymbolFormat().Replace("Assets/", "");
            if (!_folderNames.TryGetValue(key, out var dir))
            {
                dir = new List<string>();
                _folderNames[key] = dir;
            }

            dir.Add(Path.GetFileNameWithoutExtension(path));
        }

        var str = PlayerPrefs.GetString("GameLauncherOpen", "");
        var ss = str.Split('|');
        foreach (var s in ss)
        {
            if (!string.IsNullOrEmpty(s) && !_lastestOpen.Contains(s))
                _lastestOpen.Add(s);
        }
    }

    private void OnGUI()
    {
        if (_lastestOpen.Count > 0)
        {
            EditorGUILayout.LabelField("最近打开");
            EditorGUILayout.BeginHorizontal("box");
            foreach (var path in _lastestOpen)
            {
                if (GUILayout.Button(Path.GetFileNameWithoutExtension(path), GUILayout.MaxWidth(200)))
                {
                    EditorSceneManager.OpenScene(path);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        foreach (var key in _folderNames.Keys)
        {
            var open = EditorGUILayout.BeginFoldoutHeaderGroup(_opend.Contains(key), key);
            if (open)
                _opend.Add(key);
            else
                _opend.Remove(key);
            if (open)
            {
                EditorGUILayout.BeginVertical("box");
                foreach (var sceneName in _folderNames[key])
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(sceneName);
                    if (GUILayout.Button("打开"))
                    {
                        var path = $"Assets/{key.Replace("Assets/", "")}/{sceneName}.unity";
                        EditorSceneManager.OpenScene(path);
                        _lastestOpen.Insert(0, path);
                        if (_lastestOpen.Count > 3)
                            _lastestOpen.RemoveAt(3);
                        var sb = new StringBuilder();
                        foreach (var str in _lastestOpen)
                        {
                            sb.Append(str);
                            sb.Append('|');
                        }

                        PlayerPrefs.SetString("GameLauncherOpen", sb.ToString());
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
