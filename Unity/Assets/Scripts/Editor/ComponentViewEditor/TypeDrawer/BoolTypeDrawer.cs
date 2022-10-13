using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer(typeof(bool))]
    public class BoolTypeDrawer: ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.Toggle(memberName, (bool) value);
        }
    }
}