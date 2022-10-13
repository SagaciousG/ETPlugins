using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XGame
{
    
    public class DebugEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Debug _F6")]
        static void ShowWin()
        {
            GetWindow<DebugEditorWindow>().Show();
        }

        private int _selectedWin;
        private string[] _showNames = new[] { "Log", "工具", "协议" };
        private Type[] _windowTypes = new[]
        {
            typeof (DebugWindowLog), 
            typeof (DebugWindowTools),
            typeof (DebugWindowServerCMD)
        };

        private List<DebugWindowBase> _wins = new List<DebugWindowBase>();

        private void OnEnable()
        {
            this._wins.Clear();
            foreach (Type windowType in this._windowTypes)
            {
                var win = (DebugWindowBase)Activator.CreateInstance(windowType);
                this._wins.Add(win);
                win.Init(this.position, true);
            }
        }

        private void OnGUI()
        {
            this._selectedWin = GUILayout.Toolbar(this._selectedWin, this._showNames);
            var cur = this._wins[this._selectedWin];
            
            cur.Draw();
        }
    }
}