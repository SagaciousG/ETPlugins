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
        private static SearchType _searchType;
        private static SearchDomain _searchDomain;
        private class Left : AAreaBase
        {
            private Vector2 _scrollPos;
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
                                SearchNode();
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
                                SearchNode();
                            }
                        }
                        index++;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Client"))
                {
                    _focusWindowID = _clientNode.WindowID;
                    _currentNode = _clientNode;
                    _turnToCurrent = true;
                }
                if (GUILayout.Button("Server"))
                {
                    _focusWindowID = _serverNode.WindowID;
                    _currentNode = _serverNode;
                    _turnToCurrent = true;
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayoutHelper.TitleLabel(new GUIContent("搜索"));
                GUILayout.BeginVertical("box");
                {
                    GUILayoutHelper.EnumPopup("搜索类型", ref _searchType);
                    GUILayoutHelper.EnumPopup("搜索类型", ref _searchDomain);
                    
                    EditorGUI.BeginChangeCheck();
                    _searchStr = EditorGUILayout.DelayedTextField(_searchStr, "", "SearchTextField");
                    if (EditorGUI.EndChangeCheck())
                    {
                        SearchNode();
                    }

                    GUILayoutHelper.TitleLabel(new GUIContent("搜索结果"));
                    this._scrollPos = EditorGUILayout.BeginScrollView(this._scrollPos);
                    foreach (Node node in _searchRes)
                    {
                        var on = node == _currentNode;
                        var btn = new GUIStyle(GUI.skin.button);
                        btn.richText = true;
                        btn.alignment = TextAnchor.UpperLeft;
                        if (GUILayoutHelper.ToogleButton(ref on, new GUIContent($"{node.Name}\n<color=#888888>[{node.Domain}]{node.Parent?.Name}</color>"), btn, GUILayout.Height(40)))
                        {
                            _focusWindowID = node.WindowID;
                            _currentNode = node;
                            _turnToCurrent = true;
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
                
                
            }
        }

        private static bool FindInParent(Node source, Node node)
        {
            if (node.Parent == null)
                return false;
            if (node == source)
                return true;
            return FindInParent(source, node.Parent);
        }

        private static void SearchNode()
        {
            
            Node node = _currentRoot;
            switch (_searchDomain)
            {
                case SearchDomain.All:
                    node = _currentRoot;
                    break;
                case SearchDomain.Client:
                    if (FindInParent(_clientNode, _currentRoot))
                    {
                        node = _currentRoot;
                    }
                    break;
                case SearchDomain.Server:
                    if (FindInParent(_serverNode, _currentRoot))
                    {
                        node = _currentRoot;
                    }
                    break;
            }
            _searchRes.Clear();
            if (string.IsNullOrEmpty(_searchStr))
                return;
            switch (_searchType)
            {
                case SearchType.Name:
                    SearchByName(node);
                    break;
                case SearchType.ID:
                    SearchByID(node, Convert.ToInt64(_searchStr));
                    break;
                case SearchType.InstanceID:
                    SearchByInstanceID(node, Convert.ToInt64(_searchStr));
                    break;
            }
        }
        
        private static void SearchByName(Node node)
        {
            if (node.Fold)
                return;
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
            if (node.Fold)
                return;
            if (node.Entity.Id == id)
            {
                _searchRes.Add(node);
            }
            foreach (Node child in node.Children)
            {
                SearchByID(child, id);
            }
        }
            
        private static void SearchByInstanceID(Node node, long instanceID)
        {
            if (node.Fold)
                return;
            if (node.Entity.InstanceId == instanceID)
            {
                _searchRes.Add(node);
            }
            foreach (Node child in node.Children)
            {
                SearchByInstanceID(child, instanceID);
            }
        }

        private enum SearchType
        {
            Name,
            ID,
            InstanceID,
        }
        
        private enum SearchDomain
        {
            All,
            Client,
            Server,
        }
    }
}