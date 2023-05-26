using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Model;
using UnityEditor.AnimatedValues;

namespace ET
{
    [CustomEditor(typeof (ComponentView))]
    public class ComponentViewEditor: Editor
    {
        private bool publicFieldShow = true;
        private bool publicPropertyShow;
        private bool noPublicFieldShow;
        private bool noPublicPropertyShow;

        public override void OnInspectorGUI()
        {
            ComponentView componentView = (ComponentView)target;
            Entity component = componentView.Component;
            EditorGUILayout.TextField("InstanceId", component.InstanceId.ToString());
            EditorGUILayout.TextField("Id", component.Id.ToString());
            EditorGUILayout.TextField("Zone", component.DomainZone().ToString());
            TypeDrawHelper.BeginDraw(component);
        }

    }
}