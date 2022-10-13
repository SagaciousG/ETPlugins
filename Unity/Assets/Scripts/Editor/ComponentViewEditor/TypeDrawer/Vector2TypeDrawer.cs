using System;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer(typeof(Vector2))]
    public class Vector2TypeDrawer: ITypeDrawer
    {

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.Vector2Field(memberName, (Vector2) value);
        }
    }
}