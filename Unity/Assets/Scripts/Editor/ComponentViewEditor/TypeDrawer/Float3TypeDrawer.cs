using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer(typeof(float3))]
    public class Float3TypeDrawer: ITypeDrawer
    {

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.Vector3Field(memberName, (float3) value);
        }
    }
}