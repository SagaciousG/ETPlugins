using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer(typeof(string))]
    public class StringTypeDrawer: ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            if (string.IsNullOrEmpty(memberName))
                return EditorGUILayout.DelayedTextField((string) value);
            return EditorGUILayout.DelayedTextField(memberName, (string) value);
        }
    }
}