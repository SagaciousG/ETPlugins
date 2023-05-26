using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XGame;

namespace ET
{
    [TypeDrawer(typeof(IList), true)]
    public class IListTypeDrawer : ITypeDrawer
    {
        
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            var state = GUIElementStateManager.Add(GUIElementStateDomain.ComponentView, value);
            state[1] = GUILayoutHelper.Foldout(state[1], new GUIContent($"{memberName}"));
            if (state[1])
            {
                EditorGUILayout.BeginVertical("FrameBox");
                var list = value as IList;
                var index = 0;
                var argTypes = memberType.GetGenericArguments();
                foreach (var val in list)
                {
                    TypeDrawHelper.Draw(argTypes[0], val, $"{index++}");
                }
                EditorGUILayout.EndVertical();
            }
            return value;
        }
    }
}