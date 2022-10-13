using System.Linq;
using UnityEditor;
using UnityEngine;

namespace XGame
{
    public class PSDPerference : PopupWindowContent
    {
        private static Rect _rect = new Rect(0, 0, 400, 600);
        public static void Show(Rect rect) {
            PopupWindow.Show(new Rect(rect.x, rect.y, 0, 0), new PSDPerference());
            _rect = rect;
        }
        
        private SerializedObject _settingObj;
        private Vector2 _scrollPos;
        private string _colorStr;
        
        public override Vector2 GetWindowSize()
        {
            return new Vector2(_rect.width, _rect.height);
        }

        public override void OnOpen()
        {
            _settingObj = new SerializedObject(PSDUtility.PSDSetting);
        }

        public override void OnGUI(Rect rect)
        {

            var setting = PSDUtility.PSDSetting;
            EditorGUI.BeginChangeCheck();
            setting.PsdFolder = EditorGUILayout.DelayedTextField("PSD文件夹", setting.PsdFolder);
            if (EditorGUI.EndChangeCheck())
            {
                PSDFileMenu.Refresh();
            }
            
            setting.UIFolder = EditorGUILayout.DelayedTextField("预制体存放路径", setting.UIFolder);
            
            EditorGUILayout.BeginVertical();
            setting.DefaultFont = (Font) EditorGUILayout.ObjectField("默认字体", setting.DefaultFont, typeof(Font), false);
            setting.ScreenSize = EditorGUILayout.Vector2Field("屏幕尺寸", setting.ScreenSize);
            EditorGUILayout.EndVertical();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, "box", GUILayout.MaxHeight(400));

         
            this._settingObj.Update();
            var changed = EditorGUILayout.PropertyField(_settingObj.FindProperty("ComponentTypes"), new GUIContent("组件优先级"));
            EditorGUILayout.EndScrollView();
            if (changed)
                AssetDatabase.SaveAssets();
            this._settingObj.ApplyModifiedProperties();
        }
    }
}