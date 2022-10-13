using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer(typeof(double))]
    public class DoubleTypeDrawer: ITypeDrawer
    {

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.DoubleField(memberName, (double) value);
        }
    }
}