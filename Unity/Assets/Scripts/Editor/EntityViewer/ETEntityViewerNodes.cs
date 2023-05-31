using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public partial class ETEntityViewer
    {
        private class NodePool
        {
            private Stack<Node> _stack = new();
            private Stack<Node> _using = new();

            public Node Fetch()
            {
                if (this._stack.Count > 0)
                {
                    return this._stack.Pop();
                }

                var node = new Node();
                this._using.Push(node);
                return node;
            }

            public void RecycleAll()
            {
                foreach (var node in this._using)
                {
                    node.Parent = null;
                    node.Children.Clear();
                    this._stack.Push(node);
                }
                this._using.Clear();
            }
        }

        
        
        private class Node
        {
            public Node Parent;
            public int Depth;
            public int Index;
            public float Height;
            public Entity Entity;
            public List<Node> Children = new List<Node>();

            public bool Fold
            {
                get
                {
                    return Setting.FoldNodes.Contains($"{this.Entity.GetType().Name}_{this.Entity.Id}");
                }
            }
            public int WindowID { get; private set; }
            private Rect _position;

            public string Name
            {
                get
                {
                    var winName = "";
                    if (this.Entity is Scene scene)
                        winName = $"Scene({scene.Name})";
                    else
                        winName = this.Entity.GetType().Name;
                    return winName;
                }
            }

            public SearchDomain Domain
            {
                get
                {
                    if (FindInParent(_clientNode, this))
                    {
                        return SearchDomain.Client;
                    }

                    if (FindInParent(_serverNode, this))
                        return SearchDomain.Server;
                    return SearchDomain.All;
                }
            }

            public void OnGUI(Vector2 padding, Vector2 posMin)
            {
                var pos = new Vector2(this.Depth * 230, 60 * this.Height);
                var size = new Vector2(180, 40);
                this._position = new Rect(pos + posMin + padding, size);
                if (this.Entity is Scene)
                {
                    GUI.backgroundColor = Setting.SceneTypeColor + Color.gray;
                }
                else if (this.Entity.IsComponent)
                {
                    GUI.backgroundColor = Setting.ComponentColor + Color.gray;
                }
                GUI.Window(++_windowRoot, this._position, DrawWindow, this.Name);
                GUI.backgroundColor = Color.white;
            }

            private void DrawWindow(int id)
            {
                this.WindowID = id;
                if (this.Entity is Scene scene)
                    GUILayout.Label(scene.Id.ToString());
                else
                    GUILayout.Label(this.Entity.Id.ToString());
                
                if (_searchRes.Contains(this))
                {
                    GUI.color = new Color(0, 1, 0.3f, 0.3f);
                    GUI.backgroundColor = Color.clear;
                    GUI.Box(new Rect(0, 0, 40, 40), EditorGUIUtility.IconContent("check@2x"));
                    GUI.backgroundColor = Color.white;
                    GUI.color = Color.white;
                }

                if (Event.current.button == 0 && Event.current.type == UnityEngine.EventType.MouseDown)
                {
                    _focusWindowID = id;
                    _currentNode = this;
                    _eventUsed = true;
                }
                
                if (Event.current.button == 1 && Event.current.type == UnityEngine.EventType.ContextClick)
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Ping"), false, () =>
                    {
                        var field = typeof(Entity).GetField("viewGO", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        Selection.activeGameObject = (GameObject)field.GetValue(this.Entity);
                        EditorGUIUtility.PingObject((GameObject)field.GetValue(this.Entity));
                    });
                    menu.AddItem(new GUIContent("设为Root"), false, () =>
                    {
                        _currentRoot = this;
                    });
                    menu.AddItem(new GUIContent("关注节点[Name]"), false, () =>
                    {
                        Setting.QuickFlags.Add(this.Entity.GetType().Name);
                    });
                    menu.AddItem(new GUIContent("关注节点[Name+ID]"), false, () =>
                    {
                        Setting.QuickFlags.Add($"{this.Entity.GetType().Name}_{this.Entity.Id}");
                    });
                    menu.ShowAsContext();
                }
            

                if (new Rect(0, 0, this._position.width, this._position.height).Contains(Event.current.mousePosition))
                    _eventUsed = true;
            }
        }
    }
}