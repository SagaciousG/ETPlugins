using System;
using System.Reflection;
using ET;
using UnityEditor;
using UnityEngine;

namespace XGame
{
    public static class GUIHelper
    {
        public static void Box(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.Box(rect, "", "WhiteBackground");
            GUI.color = Color.white;
        }

        public static TextAnchor TextAlignGUI(TextAnchor align)
        {
            return align;
        }

        public static bool Click(Rect area)
        {
            if (Event.current.type == EventType.MouseUp)
            {
                if (area.Contains(Event.current.mousePosition))
                {
                    return true;
                }
            }

            return false;
        }

        public static GUIContent GetName(Type type)
        {
            var res = new GUIContent();
            var nameAttribute = type.GetCustomAttribute<NameAttribute>();
            if (nameAttribute != null)
            {
                res.text = nameAttribute.Name;
                res.tooltip = nameAttribute.Tooltips;
            }
            else
            {
                res.text = type.Name;
            }

            return res;
        }
        
        public static NameAttribute GetNameAttribute(FieldInfo field)
        {
            return field.GetCustomAttribute<NameAttribute>();
        }

        public static void PopTips(GUIContent content)
        {
            if (string.IsNullOrEmpty(content.tooltip))
                return;
           
            var pos = Event.current.mousePosition;
            EditorPopTips.ShowWin(content,  pos);
        }
    }

    public class EditorPopTips: PopupWindowContent
    {
        private static EditorPopTips _popUp;
        
        public static void ShowWin(GUIContent content, Vector2 pos)
        {
            var win = new EditorPopTips();
            win.content = content;
            PopupWindow.Show(new Rect(pos - new Vector2(0, 200), win.GetWindowSize()), win);
        }

        private GUIContent content;

        public override Vector2 GetWindowSize()
        {
            var h = Mathf.CeilToInt((this.content.tooltip?.Length ?? 0) * 18f / 200);
            var size = new Vector2(h == 1? (this.content.tooltip?.Length ?? 0) * 18 : 200, h * 18);
            size = Vector2.Max(new Vector2(200, 200), size);
            return size;
        }

        public override void OnGUI(Rect rect)
        {
            var skin = GUI.skin.label;
            skin.wordWrap = true;
            skin.richText = true;
            skin.alignment = TextAnchor.UpperLeft;
            EditorGUILayout.LabelField(this.content.tooltip, skin, GUILayout.Height(this.editorWindow.position.height));
        }
    }
}