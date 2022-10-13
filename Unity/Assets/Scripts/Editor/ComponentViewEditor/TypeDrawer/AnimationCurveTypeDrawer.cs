using System;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer(typeof (AnimationCurve))]
    public class AnimationCurveTypeDrawer: ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.CurveField(memberName, (AnimationCurve) value);
        }
    }
}