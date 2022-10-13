using System;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer(typeof(Rect))]
    public class RectTypeDrawer: ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.RectField(memberName, (Rect) value);
        }
    }
}