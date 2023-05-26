using System;
using UnityEditor;
using UnityEngine;
using XGame;

namespace ET
{
    public partial class ETEntityViewer
    {
        private static int _windowRoot = 10000;
        private static bool _turnToCurrent;
        private static bool _buildTree;
        private class Content : AAreaBase
        {
            private Vector2 _paddingMin = new Vector2(40, 40);
            private Vector2 _posMin;
            private Vector2 _size;
            private Rect _moveZoom;
            
            private NodePool _pool;

            private int _maxDepth;
            private float _maxHeight;
            private Node _root;

            private int _till;

            public Content(ETEntityViewer parent, Func<Rect> getPosition): base(parent, getPosition)
            {
                this._size = this.position.size;
                this._pool = new NodePool();
            }

            private void BuildTree()
            {
                if (_currentRoot == null)
                    _currentRoot = this.SetNode(Root.Instance.Scene, null, 0, 0);
                else
                    _currentRoot = this.SetNode(_currentRoot.Entity, null, 0, 0);
                    
                var h = 0f;
                this.CalNode(_currentRoot, ref h);
            }

            public override void OnGUI()
            {
                if (_currentRoot == null || _currentRoot != this._root)
                {
                    this.BuildTree();
                    this._root = _currentRoot;
                }

                if (_buildTree)
                {
                    _buildTree = false;
                    this.BuildTree();
                }
                
                _windowRoot = 10000;

                var window = this.GetRootWindow();
                this.Parent.BeginWindows();
                {
                    GUILayout.BeginArea(window);
                    {
                        if (_currentNode != null)
                        {
                            var pos = new Vector2(_currentNode.Depth * 230, 60 * _currentNode.Height) + this._paddingMin;
                            var size = new Vector2(180, 40);
                            GUI.Box(new Rect(pos, size), "", "LightmapEditorSelectedHighlight");
                        }
                        Handles.BeginGUI();
                        this.DrawRootWindow();
                        Handles.EndGUI();

                        Handles.BeginGUI();
                        this.DrawNode(_currentRoot);
                        Handles.EndGUI();
                    }
                    GUILayout.EndArea();
                }
                this.Parent.EndWindows();

                if (GUI.Button(new Rect(40, this.position.height - 70, 30, 30), EditorGUIUtility.IconContent("d_Animation.FilterBySelection@2x")))
                {
                    _turnToCurrent = true;
                }
                
                if (GUI.Button(new Rect(75, this.position.height - 70, 30, 30), EditorGUIUtility.IconContent("d_RotateTool On@2x")))
                {
                    this.BuildTree();
                }

                
                if (_currentRoot.Entity != Root.Instance.Scene)
                {
                    if (GUI.Button(new Rect(40, 40, 80, 30), "返回Root"))
                    {
                        _currentRoot = null;
                        this.BuildTree();
                    }
                }
                
                if (_turnToCurrent)
                {
                    if (_currentNode != null)
                    {
                        var pos = new Vector2(_currentNode.Depth * 230, 60 * _currentNode.Height) + this._paddingMin;
                        pos -= this.position.size / 2;
                        pos = this._moveZoom.ClampIn(pos * -1);
                        this._posMin = Vector2.Lerp(this._posMin, pos, 0.5f);
                        if (Vector2.Distance(this._posMin, pos) < 1)
                        {
                            _turnToCurrent = false;
                        }
                    }
                }
                
                if (_eventUsed)
                    return;

                if (window.Contains(Event.current.mousePosition))
                {
                    EditorGUIUtility.AddCursorRect(window, MouseCursor.Pan);
                    switch (Event.current.type)
                    {
                        case UnityEngine.EventType.Layout:
                            this._posMin = Vector2.Lerp(this._moveZoom.ClampIn(this._posMin), this._posMin, 0.5f);
                            break;
                        case UnityEngine.EventType.MouseDrag:
                            this._posMin += Vector2.Scale(Event.current.delta, new Vector2(1, 1));
                            this._posMin = this._moveZoom.ClampIn(this._posMin);
                            _turnToCurrent = false;
                            break;
                        case UnityEngine.EventType.MouseUp:
                            break;
                    }
                }

                if (Event.current.type == UnityEngine.EventType.MouseDown)
                {
                    _focusWindowID = 0;
                }

            }

            private void DrawRootWindow()
            {
                Handles.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
                var win = this.GetRootWindow();
                var maxX = Mathf.CeilToInt(win.width / 10);
                var maxY = Mathf.CeilToInt(win.height / 10);
                for (int i = 0; i < maxX; i++)
                {
                    Handles.DrawLine(new Vector3(20 * i, 0), new Vector3(20 * i, win.height));                    
                }

                for (int i = 0; i < maxY; i++)
                {
                    Handles.DrawLine(new Vector3(0, 20 * i), new Vector3(win.width, 20 * i));
                }
                Handles.color = Color.white;
            }
            
            private Node SetNode(Entity entity, Node parent, int depth, int index)
            {
                var node = this._pool.Fetch();
                node.Index = index;
                node.Depth = depth;
                node.Parent = parent;
                node.Entity = entity;
                if (parent != null)
                    parent.Children.Add(node);
                var idx = 0;
                foreach (Entity child in entity.Components.Values)
                {
                    SetNode(child, node, depth + 1, idx);
                    idx++;
                }
                foreach (Entity child in entity.Children.Values)
                {
                    SetNode(child, node, depth + 1, idx);
                    idx++;
                }

                return node;
            }

            private void CalNode(Node node, ref float height)
            {
                if (node.Children.Count > 0)
                {
                    var all = 0f;
                    foreach (Node child in node.Children)
                    {
                        CalNode(child, ref height);
                        all += height;
                    }

                    node.Height = all / node.Children.Count;
                }
                else
                {
                    node.Height = ++height;
                }
            }

            private void DrawNode(Node node)
            {
                node.OnGUI(this._paddingMin, this._posMin);
                var start = new Vector2(node.Depth * 230 + 180, 60 * node.Height + 20) + this._paddingMin;
                foreach (var child in node.Children)
                {
                    DrawNode(child);
                    var end = new Vector2(child.Depth * 230, 60 * child.Height + 20) + this._paddingMin;
                    Handles.DrawBezier(start, end, start + Vector2.right * 50, end + Vector2.left * 50, Color.white, null, 2);
                }

                this._maxDepth = Mathf.Max(this._maxDepth, node.Depth);
                this._maxHeight = Mathf.Max(this._maxHeight, node.Height);
            }
            
            private Rect GetRootWindow()
            {
                var w = this._maxDepth * 230 + this._paddingMin.x + 200;
                var h = this._maxHeight * 60 + this._paddingMin.y + 200;

                this._size = new Vector2(Mathf.Max(w, this.position.width), Mathf.Max(h, this.position.height));
                var offX = Mathf.Min(this.position.width - w, 0);
                var offY = Mathf.Min(this.position.height - h, 0);
                this._moveZoom = new Rect(offX, offY, -offX, -offY);
                return new Rect(this._posMin, this._size);
            }
        }
    }
}