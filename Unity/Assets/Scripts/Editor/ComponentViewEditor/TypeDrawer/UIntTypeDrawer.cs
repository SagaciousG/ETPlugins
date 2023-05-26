using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer(typeof(uint))]
    public class UIntTypeDrawer: ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.IntField(memberName, (int)((uint) value));
        }
    }
}