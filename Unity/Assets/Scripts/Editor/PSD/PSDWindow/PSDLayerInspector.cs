using System;
using UnityEditor;
using UnityEngine;

namespace XGame
{
    public class PSDLayerInspector : EditorWindow
    {

        public static PSDLayerInspector Instance;
        private int _hotControl;

        private PSDSelectedLayer _selectedLayer;

        private string[] _quickTags;

        private string _key;
        private string _val;
        private void OnEnable()
        {
            Instance = this;
            titleContent = new GUIContent("属性");

            _quickTags = new[]
            {
                "ref", "ignore", "name=xxx", "empty"
            };
        }

        private void OnDisable()
        {
            Instance = null;
        }

        private void OnGUI()
        {
            var selected = PSDUtility.SelectedLayer;
            
             if (selected == null)
                return;
            if (selected.IsPSD)
            {
                var psdLayer = selected.PSDLayer;
                EditorGUILayout.LabelField("PSD层级的名称");
    
                
                psdLayer.RealName = EditorGUILayout.DelayedTextField(psdLayer.RealName, EditorStyles.textArea, GUILayout.Height(100));

                EditorGUILayout.LabelField("快速添加标签");
                var col = Mathf.FloorToInt(position.width / 100);
                var raw = Mathf.CeilToInt(_quickTags.Length * 1f / col);
                for (var i = 0; i < raw; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < col; j++)
                    {
                        var index = i * col + j;
                        if (index >= _quickTags.Length)
                            break;
                        var quickTag = _quickTags[index];
                        if (GUILayout.Button(quickTag, "TE toolbarbutton", GUILayout.Width(100)))
                        {
                            psdLayer.AddTag(quickTag);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("k", GUILayout.Width(20));
                _key = EditorGUILayout.TextField(_key, GUILayout.Width(50));
                EditorGUILayout.LabelField("v", GUILayout.Width(20));
                _val = EditorGUILayout.TextField(_val);
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    psdLayer.AddTag($"{_key}={_val}");
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUI.BeginChangeCheck();
                var setName = EditorGUILayout.DelayedTextField("名称", selected.PSDLayer.Name);
                if (EditorGUI.EndChangeCheck())
                {
                    psdLayer.SetName(setName);
                }
                if (psdLayer?.Tags != null)
                {
                    foreach (var tag in psdLayer.Tags)
                    {
                        var color = psdLayer.ValidTag(tag) ? Color.white : (psdLayer.IsOldTag(tag) ? Color.yellow : Color.red);
                        var style = new GUIStyle("AppToolbarButtonLeft");
                        GUI.backgroundColor = color;
                        EditorGUILayout.BeginHorizontal(style);
                        var kv = tag.Split('=');
                        EditorGUILayout.LabelField(kv[0], GUILayout.MinWidth(50), GUILayout.MaxWidth(200));
                        if (kv.Length > 1)
                        {
                            EditorGUI.BeginChangeCheck();
                            var val = EditorGUILayout.DelayedTextField(kv[1], GUILayout.MinWidth(100));
                            if (EditorGUI.EndChangeCheck())
                            {
                                var newTag = $"{kv[0]}={val}";
                                psdLayer.RealName = psdLayer.RealName.Replace(tag, newTag);
                            }
                        }
                        else
                        {
                            GUILayout.Label("", GUILayout.MinWidth(100));
                        }                        
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            psdLayer.RealName = psdLayer.RealName.Replace($"@{tag}", "");
                            break;
                        }
                        EditorGUILayout.EndHorizontal();
                        GUI.backgroundColor = Color.white;
                    }
                }
                
                switch (psdLayer)
                {
                    case PSDLayerGroup g:
                        break;
                    case PSDLayerImage img:
                        break;
                    case PSDLayerText textLayer:
                        GUI.enabled = false;
                        EditorGUILayout.LabelField("Text", textLayer.Text);
                        EditorGUILayout.LabelField("ShowText", textLayer.ShowText);
                        EditorGUILayout.LabelField("FontSize", textLayer.FontSize.ToString());
                        EditorGUILayout.ColorField("Color", textLayer.Color);
                        GUI.enabled = true;

                        textLayer.Align = GUIHelper.TextAlignGUI(textLayer.Align);
                        textLayer.HorizontalWrapMode = (HorizontalWrapMode) EditorGUILayout.EnumPopup("Horizontal", textLayer.HorizontalWrapMode);
                        textLayer.VerticalWrapMode = (VerticalWrapMode) EditorGUILayout.EnumPopup("Vertical", textLayer.VerticalWrapMode);
                        textLayer.RaycastTarget = EditorGUILayout.Toggle("RaycastTarget", textLayer.RaycastTarget);
                        break;
                    // case PSDLayerTextPro textPro:
                        // GUI.enabled = false;
                        // EditorGUILayout.LabelField("Text", textPro.Text);
                        // EditorGUILayout.LabelField("ShowText", textPro.ShowText);
                        // EditorGUILayout.LabelField("FontSize", textPro.FontSize.ToString());
                        // EditorGUILayout.ColorField("Color", textPro.Color);
                        // GUI.enabled = true;
                        // break;
                }

                if (GUI.changed)
                {
                    PSDOperator.Refresh();
                }
            }
            else
            {
                selected.Object.name = EditorGUILayout.TextField("Name", selected.Object.name);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                
                EditorGUILayout.EndScrollView();
                if (GUI.changed)
                {
                    _hotControl = GUIUtility.hotControl;
                }
                if (EditorUtility.IsDirty(selected.Object))
                {
                    if (_hotControl != GUIUtility.hotControl)
                        PrefabUtility.SavePrefabAsset(selected.Object);
                }
            }

        }
    }
}