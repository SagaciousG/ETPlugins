using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Model;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer(typeof(IDictionary), true)]
    public class DictionaryTypeDrawer : ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            var state = GUIElementStateManager.Add(GUIElementStateDomain.ComponentView, value);
            state[1] = EditorGUILayout.Foldout(state[1], memberName, "FoldoutHeader");
            if (state[1])
            {
                EditorGUILayout.BeginVertical("box");
                var width = EditorGUIUtility.currentViewWidth;
                EditorGUILayout.BeginHorizontal("AC BoldHeader");
                var argTypes = memberType.GetGenericArguments();
                EditorGUILayout.LabelField($"Key ({argTypes[0].Name})", GUILayout.Width(width / 2));
                EditorGUILayout.LabelField($"Value ({argTypes[1].Name})", GUILayout.Width(width / 2));
                EditorGUILayout.EndHorizontal();
                var dic = value as IDictionary;
                foreach (var key in dic.Keys)
                {
                    EditorGUILayout.BeginHorizontal("OL box flat");
                    if (GUILayout.Button(key.ToString(), GUILayout.Width(width / 2)))
                    {
                        
                    }

                    if (GUILayout.Button(dic[key]?.ToString() ?? "Null", GUILayout.Width(width / 2)))
                    {
                        
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            return value;
        }
    }
}