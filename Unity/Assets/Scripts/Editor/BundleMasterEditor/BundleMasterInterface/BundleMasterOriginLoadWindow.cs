using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BM
{
    public partial class BundleMasterWindow
    {
        private void OriginLoadSettingRender()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<选中的原生资源配置信息>");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("当前选择的分包配置信息文件: ", GUILayout.ExpandWidth(false));
            AssetsOriginSetting selectAssetsOriginSetting = _selectAssetsSetting as AssetsOriginSetting;
            selectAssetsOriginSetting = (AssetsOriginSetting)EditorGUILayout.ObjectField(selectAssetsOriginSetting, typeof(AssetsOriginSetting), true, GUILayout.Width(_w / 3),GUILayout.ExpandHeight(false));
            bool noLoadSetting = selectAssetsOriginSetting == null;
            EditorGUI.BeginDisabledGroup(noLoadSetting);
            GUILayout.Label("分包名: ", GUILayout.Width(_w / 20), GUILayout.ExpandWidth(false));
            EditorGUI.BeginChangeCheck();
            var buildName = EditorGUILayout.DelayedTextField(selectAssetsOriginSetting.BuildName, GUILayout.Width(_w / 8), GUILayout.ExpandWidth(false));
            if (EditorGUI.EndChangeCheck())
            {
                selectAssetsOriginSetting.BuildName = buildName;
                needFlush = true;
            }
            GUILayout.Label("版本索引: ", GUILayout.Width(_w / 17), GUILayout.ExpandWidth(false));
            int buildIndex = 0;
            if (!noLoadSetting)
            {
                buildIndex = selectAssetsOriginSetting.BuildIndex;
            }
            buildIndex = EditorGUILayout.IntField(buildIndex, GUILayout.Width(_w / 20), GUILayout.ExpandWidth(false));
            if (!noLoadSetting)
            {
                if (buildIndex != selectAssetsOriginSetting.BuildIndex)
                {
                    selectAssetsOriginSetting.BuildIndex = buildIndex;
                    needFlush = true;
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(_h / 15);
            
            GUILayout.BeginHorizontal();
            string originAssetPath = selectAssetsOriginSetting.OriginFilePath;
            originAssetPath = EditorGUILayout.TextField(originAssetPath);
            if (!string.Equals(selectAssetsOriginSetting.OriginFilePath, originAssetPath, StringComparison.Ordinal))
            {
                selectAssetsOriginSetting.OriginFilePath = originAssetPath;
                needFlush = true;
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(_h / 8);
            
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
        }
    }
}