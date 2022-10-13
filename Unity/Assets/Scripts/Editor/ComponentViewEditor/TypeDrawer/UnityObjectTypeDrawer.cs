using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer(typeof(UnityEngine.Object), true)]
    public class UnityObjectTypeDrawer: ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            if (string.IsNullOrEmpty(memberName))
                return EditorGUILayout.ObjectField((UnityEngine.Object) value, memberType, true);
            return EditorGUILayout.ObjectField(memberName, (UnityEngine.Object) value, memberType, true);
        }
    }
}