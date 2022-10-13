using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BM
{
    public partial class BundleMasterWindow
    {
        private void AssetsLoadMainRender(bool noTable)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<入口场景>");
            GUILayout.EndHorizontal();
            GUILayout.BeginArea(new Rect(_w / 1.5f, 175, 400, 200));
            GUI.color = new Color(0.9921569F, 0.7960784F, 0.509804F);
            GUILayout.Label("初始场景是不需要打进AssetBundle里的, 这\n里填的初始场景会自动放入 Build Settings 中\n的 Scenes In Build 里。");
            GUI.color = Color.white;
            GUILayout.EndArea();
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(noTable);
            if (GUILayout.Button("增加一个入口场景", GUILayout.Width(_w / 6), GUILayout.ExpandWidth(false)))
            {
                _assetLoadTable.InitScene.Add(null);
                needFlush = true;
            }
            if (GUILayout.Button("清空所有入口场景", GUILayout.Width(_w / 6), GUILayout.ExpandWidth(false)))
            {
                _assetLoadTable.InitScene.Clear();
                needFlush = true;
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            scrollScenePos = EditorGUILayout.BeginScrollView(scrollScenePos, false, false, GUILayout.Height(_h / 6), GUILayout.ExpandHeight(true));
            if (!noTable)
            {
                HashSet<int> needRemoveScene = new HashSet<int>();
                for (int i = 0; i < _assetLoadTable.InitScene.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    SceneAsset sceneAsset = _assetLoadTable.InitScene[i];
                    if (sceneAsset != null)
                    {
                        SceneAsset asset = (SceneAsset)EditorGUILayout.ObjectField(sceneAsset, typeof(SceneAsset), false, GUILayout.Width(_w / 3),GUILayout.ExpandHeight(false));
                        if (asset == null || asset != sceneAsset)
                        {
                            _assetLoadTable.InitScene[i] = asset;
                            needFlush = true;
                        }
                    }
                    else
                    {
                        SceneAsset asset = (SceneAsset)EditorGUILayout.ObjectField(null, typeof(SceneAsset), false, GUILayout.Width(_w / 3),GUILayout.ExpandHeight(false));
                        if (asset != null)
                        {
                            _assetLoadTable.InitScene[i] = asset;
                            needFlush = true;
                        }
                    }
                    if (GUILayout.Button("将此场景从入口场景中移除", GUILayout.Width(_w / 6), GUILayout.ExpandWidth(false)))
                    {
                        needRemoveScene.Add(i);
                    }
                    GUILayout.EndHorizontal();
                }
                if (needRemoveScene.Count > 0)
                {
                    List<SceneAsset> changeSceneList = new List<SceneAsset>();
                    for (int i = 0; i < _assetLoadTable.InitScene.Count; i++)
                    {
                        if (!needRemoveScene.Contains(i))
                        {
                            changeSceneList.Add(_assetLoadTable.InitScene[i]);
                        }
                    }
                    _assetLoadTable.InitScene = changeSceneList;
                    needFlush = true;
                }
                    
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(" <配置索引索引文件信息> ");
            GUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(noTable);
            GUILayout.BeginHorizontal();
            GUILayout.Label("相对构建路径文件夹名称[BundlePath]: ", GUILayout.Width(_w / 4.5f), GUILayout.ExpandWidth(false));
            EditorGUI.BeginChangeCheck();
            var bundlePath = EditorGUILayout.DelayedTextField(_assetLoadTable.BundlePath, GUILayout.Width(_w / 8), GUILayout.ExpandWidth(false));
            if (EditorGUI.EndChangeCheck())
            {
                _assetLoadTable.BundlePath = bundlePath;
                needFlush = true;
            }
            GUILayout.Label("是否启用绝对路径: ", GUILayout.Width(_w / 9), GUILayout.ExpandWidth(false));
            EditorGUI.BeginChangeCheck();
            var enableRelativePathChange = EditorGUILayout.Toggle(_assetLoadTable.EnableRelativePath, GUILayout.Width(_w / 80), GUILayout.ExpandWidth(false));
            if (EditorGUI.EndChangeCheck())
            {
                _assetLoadTable.EnableRelativePath = enableRelativePathChange;
                needFlush = true;
            }
            GUILayout.Label("绝对路径: ", GUILayout.Width(_w / 16), GUILayout.ExpandWidth(false));
            EditorGUI.BeginChangeCheck();
            var relativePath = EditorGUILayout.DelayedTextField(_assetLoadTable.RelativePath, GUILayout.Width(_w / 2.5f), GUILayout.ExpandWidth(false));
            if (EditorGUI.EndChangeCheck())
            {
                _assetLoadTable.RelativePath = relativePath;
                needFlush = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(_h / 64);
            GUILayout.BeginHorizontal();
            GUILayout.Label("加密文件夹名称: ", GUILayout.Width(_w / 10), GUILayout.ExpandWidth(false));
            EditorGUI.BeginChangeCheck();
            var encryptPathFolder = EditorGUILayout.DelayedTextField(_assetLoadTable.EncryptPathFolder, GUILayout.Width(_w / 8), GUILayout.ExpandWidth(false));
            if (EditorGUI.EndChangeCheck())
            {
                _assetLoadTable.EncryptPathFolder = encryptPathFolder;
                needFlush = true;
            }
            GUILayout.Label("Assets下代码生成路径: ", GUILayout.Width(_w / 7), GUILayout.ExpandWidth(false));
            EditorGUI.BeginChangeCheck();
            var generateCodeScriptPath = EditorGUILayout.DelayedTextField(_assetLoadTable.GenerateCodeScriptPath, GUILayout.Width(_w / 4), GUILayout.ExpandWidth(false));
            if (EditorGUI.EndChangeCheck())
            {
                _assetLoadTable.GenerateCodeScriptPath = generateCodeScriptPath;
                needFlush = true;
            }
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            GUILayout.BeginHorizontal();
            GUILayout.Label(" <所有分包配置文件> ");
            GUILayout.EndHorizontal();
            //处理单个分包
            GUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.ExpandHeight(true));
            foreach (string guid in AssetDatabase.FindAssets($"t:{nameof(AssetsSetting)}"))
            {
                AssetsSetting loadSetting = AssetDatabase.LoadAssetAtPath<AssetsSetting>(AssetDatabase.GUIDToAssetPath(guid));
                GUILayout.BeginHorizontal();
                GUILayout.Label(loadSetting.BuildName, GUILayout.Width(100));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(loadSetting, typeof(AssetsLoadSetting), false);
                EditorGUI.EndDisabledGroup();
                GUILayout.Label("是否启用当前分包配置 ", GUILayout.Width(150));
                if (!noTable)
                {
                    bool enable = _assetLoadTable.AssetsSettings.Contains(loadSetting);
                    bool enableChange = EditorGUILayout.Toggle(enable, GUILayout.Width(30));
                    if (enable != enableChange)
                    {
                        if (enableChange)
                        {
                            _assetLoadTable.AssetsSettings.Add(loadSetting);
                        }
                        else
                        {
                            _assetLoadTable.AssetsSettings.Remove(loadSetting);
                        }
                        needFlush = true;
                    }
                }

                if (loadSetting is AssetsLoadSetting assetsSetting)
                {
                    GUILayout.Label("生成路径代码", GUILayout.Width(100));
                    EditorGUI.BeginChangeCheck();
                    assetsSetting.GeneratePathCode = EditorGUILayout.Toggle(assetsSetting.GeneratePathCode, GUILayout.Width(30));
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.needFlush = true;
                    }
                }
                
                GUI.color = new Color(0.9921569F, 0.7960784F, 0.509804F);
                if (GUILayout.Button("查看‍"))
                {
                    if (_selectAssetsSetting == loadSetting)
                        _selectAssetsSetting = null;
                    else
                        _selectAssetsSetting = loadSetting;
                }
                
                GUI.color = Color.white;
                
                GUI.color = new Color(0.9921569F, 0.2745098F, 0.282353F);
                if (GUILayout.Button("X"))
                {
                    _assetLoadTable.AssetsSettings.Remove(_selectAssetsSetting);
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_selectAssetsSetting));
                    needFlush = true;
                }
                GUI.color = Color.white;
                
                GUILayout.EndHorizontal();
                
                
                if (_selectAssetsSetting == loadSetting)
                {
                    EditorGUILayout.BeginVertical("box");
                    if (_selectAssetsSetting is AssetsLoadSetting)
                    {
                        AssetsLoadSettingRender();
                    }
                    else
                    {
                        OriginLoadSettingRender();
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }
    }
}