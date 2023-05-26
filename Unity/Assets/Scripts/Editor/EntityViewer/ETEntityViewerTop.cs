using System;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public partial class ETEntityViewer
    {
      
        private class Top : AAreaBase
        {
            public Top(ETEntityViewer parent, Func<Rect> getPosition): base(parent, getPosition)
            {
            }

            public override void OnGUI()
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(EditorGUIUtility.IconContent("d__Popup@2x"), GUILayout.Width(30), GUILayout.Height(30)))
                {
                    Perference.Show(new Rect(25, 5, Mathf.Max(position.width * 0.9f, 300f), Mathf.Max(500, position.height * 0.5f)));
                }
                
                EditorGUILayout.EndHorizontal();

            }
            
        }
        
        private class Perference: PopupWindowContent
        {
            private static Rect _rect = new Rect(0, 0, 400, 600);

            public static void Show(Rect rect)
            {
                PopupWindow.Show(new Rect(rect.x, rect.y, 0, 0), new Perference());
                _rect = rect;
            }

            private Vector2 _scrollPos;
            private string _colorStr;
            private SerializedObject _settingObj;

            public override Vector2 GetWindowSize()
            {
                return new Vector2(_rect.width, _rect.height);
            }

            public override void OnClose()
            {
                AssetDatabase.SaveAssets();
            }

            public override void OnOpen()
            {
                this._settingObj = new SerializedObject(Setting);
            }

            public override void OnGUI(Rect rect)
            {
                this._scrollPos = EditorGUILayout.BeginScrollView(this._scrollPos);

                Setting.SceneTypeColor = EditorGUILayout.ColorField("Scene颜色", Setting.SceneTypeColor);
                Setting.ComponentColor = EditorGUILayout.ColorField("Component颜色", Setting.ComponentColor);
                Setting.ChildColor = EditorGUILayout.ColorField("Child颜色", Setting.ChildColor);

                var quickFlags = this._settingObj.FindProperty("QuickFlags");
                EditorGUILayout.PropertyField(quickFlags);

                if (this._settingObj.hasModifiedProperties)
                    this._settingObj.ApplyModifiedProperties();
                
                EditorGUILayout.EndScrollView();
            }
        }
    }
}