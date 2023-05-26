using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XGame;

namespace ET
{
    [TypeDrawer(typeof(Stack<>))]
    public class StackTypeDrawer : ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            var argTypes = memberType.GetGenericArguments();
            var state = GUIElementStateManager.Add(GUIElementStateDomain.ComponentView, value);
            state[1] = GUILayoutHelper.Foldout(state[1], new GUIContent($"{memberName} {memberType.Name} {argTypes[0].Name}"));
            if (state[1])
            {
                EditorGUILayout.BeginVertical("FrameBox");
                var list = value as IEnumerable;
                var index = 0;
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