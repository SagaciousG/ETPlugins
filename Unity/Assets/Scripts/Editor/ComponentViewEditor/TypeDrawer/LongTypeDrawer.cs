using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer(typeof(long))]
    public class LongTypeDrawer: ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.LongField(memberName, (long) value);
        }
    }
}