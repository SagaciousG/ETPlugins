using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer(typeof(char))]
    public class CharTypeDrawer: ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            var str = EditorGUILayout.TextField(memberName, ((char) value).ToString());
            return str.Length > 0? str[0] : default (char);
        }
    }
}