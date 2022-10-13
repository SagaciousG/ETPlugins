using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer(typeof(Enum), true)]
    public class EnumTypeDrawer: ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            if (memberType.IsDefined(typeof (FlagsAttribute), false))
            {
                return EditorGUILayout.EnumFlagsField(memberName, (Enum) value);
            }

            return EditorGUILayout.EnumPopup(memberName, (Enum) value);
        }
    }
}