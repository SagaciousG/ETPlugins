using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public partial class ETEntityViewer : EditorWindow
    {
        [MenuItem("ET/ET树状图 _F1")]
        static void ShowWin()
        {
            GetWindow<ETEntityViewer>().Show();
        }

        private Dictionary<AreaType, AAreaBase> _areas;

        private static bool _eventUsed;
        private static int _focusWindowID;
        private static Node _currentNode;
        private static Node _currentRoot;
        
        
        public static ETEntityViewerSetting Setting
        {
            get
            {
                if (_setting == null)
                {
                    if (File.Exists("Assets/ETEntityViewerSetting.asset"))
                    {
                        _setting = AssetDatabase.LoadAssetAtPath<ETEntityViewerSetting>("Assets/ETEntityViewerSetting.asset");
                    }
                    else
                    {
                        _setting = ScriptableObject.CreateInstance<ETEntityViewerSetting>();
                        AssetDatabase.CreateAsset(_setting, "Assets/ETEntityViewerSetting.asset");
                    }
                }

                return _setting;
            }
        }

        private static ETEntityViewerSetting _setting;

        
        private enum AreaType
        {
            Top,
            Left,
            Content,
            Inspector,
        }

        private void OnFocus()
        {
            _buildTree = true;
        }

        private abstract class AAreaBase
        {
            public AAreaBase(ETEntityViewer parent, Func<Rect> getPosition)
            {
                this.Parent = parent;
                this._getPosition = getPosition;
            }

            private Func<Rect> _getPosition;
            protected ETEntityViewer Parent;
            public Rect position => this._getPosition.Invoke();
            
            public abstract void OnGUI();

            public void Repaint()
            {
                this.Parent.Repaint();
            }
        }
        
        private void OnEnable()
        {
            this.titleContent = new GUIContent("ET视图");
            this.minSize = new Vector2(1200, 800);

            this._areas = new Dictionary<AreaType, AAreaBase>();
            
            this._areas.Add(AreaType.Top, new Top(this, () => new Rect(0, 0, this.position.width, 40)));
            this._areas.Add(AreaType.Left, new Left(this, () => new Rect(0, 40, 250, this.position.height - 40)));
            this._areas.Add(AreaType.Content, new Content(this, () => new Rect(250, 40, this.position.width - 250 - 300, this.position.height - 40)));
            this._areas.Add(AreaType.Inspector, new Inspector(this, () => new Rect(this.position.width - 300, 40, 300, this.position.height - 40)));
        }

        private void Update()
        {
            this.Repaint();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _eventUsed = false;
            foreach (var kv in this._areas)
            {
                GUILayout.BeginArea(kv.Value.position, "", "FrameBox");
                kv.Value.OnGUI();
                GUILayout.EndArea();
            }
        }
    }
}