using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using XGame;

namespace ET
{
    public partial class ETEntityViewer
    {
        private static HashSet<Node> _searchRes = new();
        private static string _searchStr;
        private class Left : AAreaBase
        {
            private Vector2 _scrollPos;
            private SearchType _searchType;
            public Left(ETEntityViewer parent, Func<Rect> getPosition): base(parent, getPosition)
            {
            }

            public override void OnGUI()
            {
                GUILayoutHelper.TitleLabel(new GUIContent("快速搜索"));
                GUILayout.BeginVertical("box");
                {
                    var index = 0;
                    EditorGUILayout.BeginHorizontal();
                    foreach (string quickFlag in Setting.QuickFlags)
                    {
                        if (index > 0 && index % 2 == 0)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }

                        if (GUILayout.Button(quickFlag, GUILayout.Width(this.position.width * 0.48f)))
                        {
                            _searchRes.Clear();
                            if (Regex.IsMatch(quickFlag, @"\w+_\d+"))
                            {
                                _searchStr = quickFlag.Substring(0, quickFlag.IndexOf('_'));
                                SearchByName(_currentRoot);
                                var id = Convert.ToInt64(quickFlag.Substring(quickFlag.IndexOf('_') + 1));
                                var list = new HashSet<Node>();
                                foreach (Node node in _searchRes)
                                {
                                    if (node.Entity.Id == id)
                                    {
                                        list.Add(node);
                                    }
                                }

                                _searchRes = list;
                            }
                            else
                            {
                                _searchStr = quickFlag;
                                SearchByName(_currentRoot);
                            }
                        }
                        index++;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayoutHelper.TitleLabel(new GUIContent("搜索"));
                GUILayout.BeginVertical("box");
                {
                    GUILayoutHelper.EnumPopup("搜索类型", ref this._searchType);
                    
                    EditorGUI.BeginChangeCheck();
                    _searchStr = EditorGUILayout.DelayedTextField(_searchStr, "", "SearchTextField");
                    if (EditorGUI.EndChangeCheck())
                    {
                        _searchRes.Clear();
                        if (string.IsNullOrEmpty(_searchStr))
                            return;
                        switch (this._searchType)
                        {
                            case SearchType.Name:
                                SearchByName(_currentRoot);
                                break;
                            case SearchType.ID:
                                SearchByID(_currentRoot, Convert.ToInt64(_searchStr));
                                break;
                            case SearchType.InstanceID:
                                SearchByInstanceID(_currentRoot, Convert.ToInt64(_searchStr));
                                break;
                        }
                    }

                    var index = 0;
                    GUILayoutHelper.TitleLabel(new GUIContent("搜索结果"));
                    this._scrollPos = EditorGUILayout.BeginScrollView(this._scrollPos);
                    foreach (Node node in _searchRes)
                    {
                        var on = node == _currentNode;
                        if (GUILayoutHelper.ToogleButton(ref on, new GUIContent(node.Name)))
                        {
                            _focusWindowID = node.WindowID;
                            _currentNode = node;
                            _turnToCurrent = true;
                        }

                        index++;
                    }
                    EditorGUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
                
                
            }

        }
        
        
        private static void SearchByName(Node node)
        {
            if (node.Entity.GetType().Name.ToLower().Contains(_searchStr.ToLower()))
            {
                _searchRes.Add(node);
            }
            else if (node.Entity is Scene scene)
            {
                if (scene.Name.Contains(_searchStr))
                    _searchRes.Add(node);
            }

            foreach (Node child in node.Children)
            {
                SearchByName(child);
            }
        }

        private static void SearchByID(Node node, long id)
        {
            if (node.Entity.Id == id)
            {
                _searchRes.Add(node);
            }
            foreach (Node child in node.Children)
            {
                SearchByID(child, id);
            }
        }
            
        private static Node SearchByInstanceID(Node node, long instanceID)
        {
            Node lastNode = null;
            if (node.Entity.InstanceId == instanceID)
            {
                _searchRes.Add(node);
                return node;
            }
            foreach (Node child in node.Children)
            {
                lastNode = SearchByInstanceID(child, instanceID);
            }

            return lastNode;
        }

        private enum SearchType
        {
            Name,
            ID,
            InstanceID,
        }
    }
}