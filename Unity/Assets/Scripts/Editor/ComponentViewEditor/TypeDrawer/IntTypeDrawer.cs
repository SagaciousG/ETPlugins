using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer(typeof(int))]
    public class IntTypeDrawer: ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.IntField(memberName, (int) value);
        }
    }
}