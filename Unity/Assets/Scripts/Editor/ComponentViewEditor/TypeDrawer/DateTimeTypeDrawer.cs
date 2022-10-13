using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer(typeof(DateTime))]
    public class DateTimeTypeDrawer: ITypeDrawer
    {

        // Note: This is a very basic implementation. The ToString() method conversion will cut off milliseconds.
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            var dateString = value.ToString();
            var newDateString = EditorGUILayout.TextField(memberName, dateString);

            return newDateString != dateString
                    ? DateTime.Parse(newDateString)
                    : value;
        }
    }
}