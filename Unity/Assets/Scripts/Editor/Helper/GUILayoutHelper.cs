using System;
using System.Collections.Generic;
using System.Reflection;
using ET;
using UnityEditor;
using UnityEngine;
using UnityEngineInternal;

namespace XGame
{
    public static class GUILayoutHelper
    {
        private static Type EnumDataUtility
        {
            get
            {
                return _enumDataUtility ??= typeof (GameObject).Assembly.GetType("UnityEngine.EnumDataUtility");
            }
        }

        private static MethodInfo GetCachedEnumData
        {
            get
            {
                if (_getCachedEnumData == null)
                {
                    _getCachedEnumData = EnumDataUtility.GetMethod("GetCachedEnumData", BindingFlags.Static | BindingFlags.NonPublic);
                }

                return _getCachedEnumData;
            }
        }
        
        private static MethodInfo EnumFlagsToInt
        {
            get
            {
                if (_enumFlagsToInt == null)
                {
                    _enumFlagsToInt = EnumDataUtility.GetMethod("EnumFlagsToInt", BindingFlags.Static | BindingFlags.NonPublic);
                }

                return _enumFlagsToInt;
            }
        }
        
        private static MethodInfo IntToEnumFlags
        {
            get
            {
                if (_intToEnumFlags == null)
                {
                    _intToEnumFlags = EnumDataUtility.GetMethod("IntToEnumFlags", BindingFlags.Static | BindingFlags.NonPublic);
                }

                return _intToEnumFlags;
            }
        }

        private static MethodInfo _getCachedEnumData;
        private static MethodInfo _enumFlagsToInt;
        private static MethodInfo _intToEnumFlags;
        private static Type _enumDataUtility;


        public static void HelpLabel(GUIContent label, GUIStyle style = null, bool explandWidth = true)
        {
            if (string.IsNullOrEmpty(label.text))
                return;
            style ??= EditorStyles.label;
            EditorGUILayout.BeginHorizontal();
            EditorStyles.label.CalcMinMaxWidth(label, out var min, out var max);
            EditorGUILayout.LabelField(label, style, GUILayout.Width(min));
            var width = EditorGUIUtility.labelWidth - min;
            if (!string.IsNullOrEmpty(label.tooltip))
            {
                var enabled = GUI.enabled;
                GUI.enabled = true;
                if (GUILayout.Button(EditorGUIUtility.IconContent("d__Help"), "IconButton", GUILayout.Width(24), GUILayout.Height(24)))
                {
                    GUIHelper.PopTips(label);
                }

                width -= 24;
                GUI.enabled = enabled;
            }
            
            if (explandWidth)
                EditorGUILayout.LabelField("", GUILayout.Width(width));
            EditorGUILayout.EndHorizontal();
        }
        
        private static void HelpButton(GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label.tooltip))
            {
                var enabled = GUI.enabled;
                GUI.enabled = true;
                if (GUILayout.Button(EditorGUIUtility.IconContent("d__Help"), "IconButton", GUILayout.Width(24), GUILayout.Height(24)))
                {
                    GUIHelper.PopTips(label);
                }

                GUI.enabled = enabled;
            }
            EditorGUILayout.EndHorizontal();
        }
        
        //只能用于单选
        public static void EnumPopup(GUIContent content, ref Enum val)
        {
            Type enumType = val.GetType();
            var names = Enum.GetNames(enumType);
            var index = Array.IndexOf(names, val.ToString());
            var list = new List<GUIContent>();
            foreach (string name in names)
            {
                var fieldInfo = enumType.GetField(name);
                var nameAttribute = GUIHelper.GetNameAttribute(fieldInfo);
                if (nameAttribute != null)
                {
                    list.Add(new GUIContent(nameAttribute.Name, nameAttribute.Tooltips));
                }
                else
                {
                    list.Add(new GUIContent(name));
                }
            }

            EditorGUILayout.BeginHorizontal();
            HelpLabel(content);
            var newIndex = EditorGUILayout.Popup(index, list.ToArray());
            if (newIndex > -1 && newIndex < names.Length)
            {
                Enum.TryParse(enumType, names[newIndex], false, out var res);
                val = (Enum) res;
            }
            if (newIndex > -1 && newIndex < list.Count)
                HelpButton(list[newIndex]);

   

            EditorGUILayout.EndHorizontal();
        }
        
        public static void EnumPopup<T>(string title, ref T val) where T : Enum
        {
            EnumPopup(new GUIContent(title, ""), ref val);
        }
        public static void EnumPopup<T>(GUIContent content, ref T val) where T : Enum
        {
            Enum v0 = (Enum)val;
            EnumPopup(content, ref v0);
            val = (T)v0;
        }

        public static void DrawField(FieldInfo fieldInfo, object obj)
        {
            var nameAttribute = GUIHelper.GetNameAttribute(fieldInfo);
            var label = new GUIContent(fieldInfo.Name);
            if (nameAttribute != null)
            {
                label = new GUIContent(nameAttribute.Name, nameAttribute.Tooltips);
            }

            var val = fieldInfo.GetValue(obj);
            if (fieldInfo.FieldType.IsEnum)
            {
                var e = (Enum)val;
                GUILayoutHelper.EnumPopup(label, ref e);
                fieldInfo.SetValue(obj, e);
            }else if (fieldInfo.FieldType.IsClass)
            {
                
            }
            else
            {
                switch (val)
                {
                    case int iv:
                        iv = EditorGUILayout.IntField(label, iv);
                        fieldInfo.SetValue(obj, iv);
                        break;
                    case float fv:
                        fv = EditorGUILayout.FloatField(label, fv);
                        fieldInfo.SetValue(obj, fv);
                        break;
                }
            }
        }

        public static void EnumFlagsField<T>(GUIContent label, ref T val) where T : Enum
        {
            var v0 = (Enum)val;
            EnumFlagsField(label, ref v0);
            val = (T)v0;
        }
        public static void EnumFlagsField(GUIContent label, ref Enum val)
        {
            Type enumType = val.GetType();
            var names = Enum.GetNames(enumType);
            var list = new List<GUIContent>();
            foreach (string name in names)
            {
                var fieldInfo = enumType.GetField(name);
                var nameAttribute = GUIHelper.GetNameAttribute(fieldInfo);
                if (nameAttribute != null)
                {
                    list.Add(new GUIContent(nameAttribute.Name, nameAttribute.Tooltips));
                }
                else
                {
                    list.Add(new GUIContent(name));
                }
            }

            var getDisplayName = new Func<string, string>(s =>
            {
                var fieldInfo = enumType.GetField(s);
                var nameAttribute = GUIHelper.GetNameAttribute(fieldInfo);
                return nameAttribute?.Name ?? s;
            });
            var enumData = GetCachedEnumData.Invoke(null, new object[] { enumType, true, getDisplayName });
            var mask = (int) EnumFlagsToInt.Invoke(null, new []{enumData, val});
            
            EditorGUILayout.BeginHorizontal();
            HelpLabel(label);
            EditorGUI.BeginChangeCheck();
            mask = EditorGUILayout.MaskField(mask, list.ToStringArray());
            if (EditorGUI.EndChangeCheck())
            {
                val = (Enum) IntToEnumFlags.Invoke(null, new object[] { enumType, mask });
            }

            var enable = GUI.enabled;
            GUI.enabled = true;
            if (GUILayout.Button(EditorGUIUtility.IconContent("d__Help"), "IconButton", GUILayout.Width(24), GUILayout.Height(24)))
            {
                for (int i = 0; i < names.Length; i++)
                {
                    string name = names[i];
                    if (name == val.ToString())
                    {
                        GUIHelper.PopTips(list[i]);
                        break;
                    }
                }
            }

            GUI.enabled = enable;
            EditorGUILayout.EndHorizontal();
        }
        
        public static bool DelayedLongField(string label, ref long val)
        {
            return DelayedLongField(new GUIContent(label), ref val);
        }
        
        public static bool DelayedLongField(GUIContent label, ref long val)
        {
            EditorGUILayout.BeginHorizontal();
           
            HelpLabel(label);
            EditorGUI.BeginChangeCheck();
            val = EditorGUILayout.LongField(val);
            EditorGUILayout.EndHorizontal();
            return EditorGUI.EndChangeCheck();
        }

        public static bool DelayedIntField(string label, ref int val)
        {
            return DelayedIntField(new GUIContent(label), ref val);
        }
        
        public static bool DelayedIntField(GUIContent label, ref int val)
        {
            EditorGUILayout.BeginHorizontal();
           
            HelpLabel(label);
            EditorGUI.BeginChangeCheck();
            val = EditorGUILayout.DelayedIntField(val);
            EditorGUILayout.EndHorizontal();
            return EditorGUI.EndChangeCheck();
        }
        
        public static bool IntField(string label, ref int val)
        {
            return IntField(new GUIContent(label), ref val);
        }
        
        public static bool IntField(GUIContent label, ref int val)
        {
            EditorGUILayout.BeginHorizontal();
            HelpLabel(label);
            EditorGUI.BeginChangeCheck();
            val = EditorGUILayout.IntField(val);
            EditorGUILayout.EndHorizontal();
            return EditorGUI.EndChangeCheck();
        }

        public static bool DelayedFloatField(string label, ref float val)
        {
            return DelayedFloatField(new GUIContent(label), ref val);
        }
        public static bool DelayedFloatField(GUIContent label, ref float val)
        {
            EditorGUILayout.BeginHorizontal();
            HelpLabel(label);
            EditorGUI.BeginChangeCheck();
            val = EditorGUILayout.DelayedFloatField(val);
            EditorGUILayout.EndHorizontal();
            return EditorGUI.EndChangeCheck();
        }
        
        public static bool FloatField(string label, ref float val)
        {
            return FloatField(new GUIContent(label), ref val);
        }
        public static bool FloatField(GUIContent label, ref float val)
        {
            EditorGUILayout.BeginHorizontal();
            HelpLabel(label);
            val = EditorGUILayout.FloatField(val);
            EditorGUILayout.EndHorizontal();
            return EditorGUI.EndChangeCheck();
        }

        public static void TextField(string label, ref string val)
        {
            TextField(new GUIContent(label), ref val);
        }
        public static void TextField(GUIContent label, ref string val)
        {
            EditorGUILayout.BeginHorizontal();
            HelpLabel(label);
            val = EditorGUILayout.TextField(val);
            EditorGUILayout.EndHorizontal();
        }
        
        public static bool DelayedTextField(string label, ref string val)
        {
            return DelayedTextField(new GUIContent(label), ref val);
        }
        public static bool DelayedTextField(GUIContent label, ref string val)
        {
            EditorGUILayout.BeginHorizontal();
            HelpLabel(label);
            EditorGUI.BeginChangeCheck();
            val = EditorGUILayout.DelayedTextField(val);
            EditorGUILayout.EndHorizontal();
            return EditorGUI.EndChangeCheck();
        }
        
        public static bool ToggleField(string label, ref bool val)
        {
            return ToggleField(new GUIContent(label), ref val);
        }

        public static bool ToggleField(GUIContent label, ref bool val)
        {
            EditorGUILayout.BeginHorizontal();
            HelpLabel(label);
            EditorGUI.BeginChangeCheck();
            val = EditorGUILayout.Toggle(val);
            EditorGUILayout.EndHorizontal();
            return EditorGUI.EndChangeCheck();
        }

        public static bool DelayedMinMaxSlider(GUIContent label, ref float min, ref float max, float lower, float upper)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            HelpLabel(label);
            min = Mathf.Clamp(EditorGUILayout.FloatField(min, GUILayout.Width(50)), lower, max);
            EditorGUILayout.MinMaxSlider(ref min, ref max, lower, upper);
            max = Mathf.Clamp(EditorGUILayout.FloatField(max, GUILayout.Width(50)), min, upper);
            EditorGUILayout.EndHorizontal();
            return EditorGUI.EndChangeCheck();
        }
        
        public static bool Slider(GUIContent label, ref float val, float lower, float upper)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            HelpLabel(label);
            val = EditorGUILayout.Slider(val, lower, upper);
            EditorGUILayout.EndHorizontal();
            return EditorGUI.EndChangeCheck();
        }

        public static void TitleLabel(GUIContent label)
        {
            HelpLabel(label, "IN TitleText");
        }
        
        public static bool Foldout(bool on, GUIContent content, params GUILayoutOption[] options)
        {
            var style = new GUIStyle("TE toolbarbutton");
            style.alignment = TextAnchor.MiddleLeft;
            return Foldout(on, content, style, options);
        }

        public static bool Foldout(bool on, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            HelpLabel(new GUIContent("", content.tooltip), style);
            content.image = EditorGUIUtility.IconContent(on? "arrow-down" : "arrow-right").image;
            if (GUILayout.Button(content, style, options))
            {
                on = !on;
            }
            EditorGUILayout.EndHorizontal();
            return on;
        }
        
        public static bool ToogleButton(ref bool on, GUIContent content, params GUILayoutOption[] options)
        {
            var btn = new GUIStyle(GUI.skin.button);
            btn.richText = true;
            return ToogleButton(ref on, content, btn, options);
        }
        
        public static bool ToogleButton(ref bool on, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            return ToogleButton(ref on, content, Color.green, style, options);
        }
        public static bool ToogleButton(ref bool on, GUIContent content, Color onColor, GUIStyle style,  params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            var c = GUI.backgroundColor;
            GUI.backgroundColor = on ? onColor : Color.white;
            bool res = false;
            if (GUILayout.Button(content, style, options))
            {
                res = true;
                on = !on;
            }
            GUI.backgroundColor = c;
            HelpLabel(new GUIContent("", content.tooltip));
            EditorGUILayout.EndHorizontal();
            return res;
        }

        public static bool Vector3Field(GUIContent label, ref Vector3 val)
        {
            EditorGUILayout.BeginHorizontal();
            HelpLabel(label);
            EditorGUI.BeginChangeCheck();
            val = EditorGUILayout.Vector3Field("", val);
            EditorGUILayout.EndHorizontal();
            return EditorGUI.EndChangeCheck();
        }
        
        public static int Popup(GUIContent label, int index, string[] options)
        {
            EditorGUILayout.BeginHorizontal();
            HelpLabel(label);
            index = EditorGUILayout.Popup(index, options);
            EditorGUILayout.EndHorizontal();
            return index;
        }

        public static object GetCurrentGUILayoutGroup()
        {
            var currentType = typeof (GUILayoutUtility).GetField("current", BindingFlags.Static | BindingFlags.NonPublic);
            var current = currentType.GetValue(null);
            var layoutGroupsType = current.GetType().GetField("layoutGroups", BindingFlags.Instance | BindingFlags.NonPublic);
            var layoutGroups = (GenericStack)layoutGroupsType.GetValue(current);
            return layoutGroups.Peek();
        }

        
        
        public static Vector2 GetCurrentLayoutGroupSize()
        {
            var group = GetCurrentGUILayoutGroup();
            var rectField = group.GetType().GetField("rect", BindingFlags.Instance | BindingFlags.Public);
            var rect = (Rect) rectField.GetValue(group);
            return rect.size;
        }
    }
}