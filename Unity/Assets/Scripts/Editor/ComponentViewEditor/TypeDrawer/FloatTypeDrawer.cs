using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer(typeof(float))]
    public class FloatTypeDrawer: ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.FloatField(memberName, (float) value);
        }
    }
}