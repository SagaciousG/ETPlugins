using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Model;
using UnityEditor;
using UnityEngine;
using XGame;

namespace ET
{
    [TypeDrawer(typeof(IDictionary), true)]
    public class DictionaryTypeDrawer : ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            var state = GUIElementStateManager.Add(GUIElementStateDomain.ComponentView, value);
            state[1] = GUILayoutHelper.Foldout(state[1], new GUIContent(memberName));
            if (state[1])
            {
                EditorGUILayout.BeginHorizontal("AC BoldHeader");
                var argTypes = memberType.GetGenericArguments();
                var labelStyle = new GUIStyle(EditorStyles.boldLabel);
                labelStyle.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField($"Key ({argTypes[0].Name})", labelStyle, GUILayout.MaxWidth(144));
                EditorGUILayout.LabelField($"Value ({argTypes[1].Name})", labelStyle);
                EditorGUILayout.EndHorizontal();
                var dic = value as IDictionary;
                foreach (var key in dic.Keys)
                {
                    EditorGUILayout.BeginHorizontal("OL box flat");
                    if (GUILayout.Button(key.ToString(), GUILayout.MaxWidth(144)))
                    {
                        
                    }

                    if (GUILayout.Button(dic[key]?.ToString() ?? "Null"))
                    {
                        
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            return value;
        }
    }
}