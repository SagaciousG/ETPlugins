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
            ComponentView componentView = (ComponentView) target;
            Entity component = componentView.Component;
            BeginDraw(component);
        }
        
        private void BeginDraw(Entity obj)
        {
            EditorGUIUtility.labelWidth = 144;
            GUIElementStateManager.Reset(GUIElementStateDomain.ComponentView);
            var type = obj.GetType();

            EditorGUILayout.TextField("InstanceId", obj.InstanceId.ToString());
            EditorGUILayout.TextField("Id", obj.Id.ToString());
            EditorGUILayout.TextField("Zone", obj.DomainZone().ToString());
            
            this.publicFieldShow = EditorGUILayout.BeginFoldoutHeaderGroup(this.publicFieldShow, "Public Field");
            if (this.publicFieldShow)
            {
                var publicFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                foreach (var info in publicFields)
                {
                    if (type.IsDefined(typeof (HideInInspector)))
                        continue;
                    ComponentViewHelper.Draw(info.FieldType, info.GetValue(obj), info.Name);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            this.publicPropertyShow = EditorGUILayout.BeginFoldoutHeaderGroup(this.publicPropertyShow, "Public Property");
            if (this.publicPropertyShow)
            {
                var publicProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var info in publicProperties)
                {
                    if (type.IsDefined(typeof (HideInInspector)))
                        continue;
                    ComponentViewHelper.Draw(info.PropertyType, info.GetValue(obj), info.Name);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            this.noPublicFieldShow = EditorGUILayout.BeginFoldoutHeaderGroup(this.noPublicFieldShow, "No Public Field");
            if (this.noPublicFieldShow)
            {
                var noPublicFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var info in noPublicFields)
                {
                    if (type.IsDefined(typeof (HideInInspector)))
                        continue;
                    ComponentViewHelper.Draw(info.FieldType, info.GetValue(obj), info.Name);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            this.noPublicPropertyShow = EditorGUILayout.BeginFoldoutHeaderGroup(this.noPublicPropertyShow, "No Public Property");
            if (this.noPublicPropertyShow)
            {
                var noPublicProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var info in noPublicProperties)
                {
                    if (type.IsDefined(typeof (HideInInspector)))
                        continue;
                    ComponentViewHelper.Draw(info.PropertyType, info.GetValue(obj), info.Name);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    public static class ComponentViewHelper
    {
        private class TypeDrawer
        {
            public TypeDrawer(Type t, bool inheritInclude, ITypeDrawer drawer)
            {
                Target = t;
                InheritInclude = inheritInclude;
                Drawer = drawer;
            }
            public Type Target;
            public bool InheritInclude;
            public ITypeDrawer Drawer;
            public bool IsInterface => Target.IsInterface;
        }
        
        private static readonly Dictionary<Type, TypeDrawer> typeDrawers = new Dictionary<Type, TypeDrawer>();
        private static readonly List<TypeDrawer> inheritDrawers = new List<TypeDrawer>();
        
        static ComponentViewHelper()
        {
            Assembly assembly = typeof (ComponentViewHelper).Assembly;
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsDefined(typeof (TypeDrawerAttribute)))
                {
                    continue;
                }

                var drawer = type.GetCustomAttribute<TypeDrawerAttribute>();
                ITypeDrawer iTypeDrawer = (ITypeDrawer) Activator.CreateInstance(type);
                var obj = new TypeDrawer(drawer.DrawerType, drawer.InheritInclude, iTypeDrawer);
                typeDrawers.Add(obj.Target, obj);
                if (obj.InheritInclude)
                {
                    inheritDrawers.Add(obj);
                }
            }
        }
        public static void Draw(Type type, object obj, string label)
        {
            if (!string.IsNullOrEmpty(label))
            {
                var matcher = Regex.Match(label, @"<(\w+)>k__BackingField");
                if (matcher.Success)
                {
                    label = matcher.Groups[1].Value;
                }
            }
            if (obj == null)
            {
                EditorGUILayout.LabelField(label, $"Null({type})");
                return;
            }

            var key = type;
            if (type.IsGenericType)
            {
                key = type.GetGenericTypeDefinition();
            }
            if (typeDrawers.TryGetValue(key, out var drawer))
            {
                drawer.Drawer.DrawAndGetNewValue(type, label, obj, null);
            }
            else
            {
                if (type.IsClass || type.IsEnum)
                {
                    var inheritFind = false;
                    foreach (var inheritDrawer in inheritDrawers)
                    {
                        var t = type.GetInterface(inheritDrawer.Target.FullName);
                        if (type.IsSubclassOf(inheritDrawer.Target) ||
                            (inheritDrawer.IsInterface && t != null))
                        {
                            inheritFind = true;
                            inheritDrawer.Drawer.DrawAndGetNewValue(type, label, obj, null);
                            break;
                        }
                    }

                    if (!inheritFind)
                    {
                        DrawObject(obj, label);
                    }
                }
                else
                {
                    Debug.LogError($"不存在TypeDrawer：{type}");
                }
            }
        }
        
        public static void DrawObject(object obj, string label)
        {
            if (obj == null)
            {
                EditorGUILayout.LabelField(label, "Null");
                return;
            }

            var state = GUIElementStateManager.Add(GUIElementStateDomain.ComponentView, obj);
            if (state.depth > 1)
            {
                EditorGUILayout.LabelField(label, "Cyclic Reference");
                return;
            }
            
            var type = obj.GetType();
            state[1001] = EditorGUILayout.Foldout(state[1001], label, "FoldoutHeader");
            if (state[1001])
            {
                EditorGUILayout.BeginVertical("FrameBox");
                state[1002] = EditorGUILayout.Foldout(state[1002], "Public Field", "FoldoutHeader");
                if (state[1002])
                {
                    var publicFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var info in publicFields)
                    {
                        if (type.IsDefined(typeof (HideInInspector)))
                            continue;
                        Draw(info.FieldType, info.GetValue(obj), info.Name);
                    }
                }
                
                state[1003] = EditorGUILayout.Foldout(state[1003], "Public Property", "FoldoutHeader");
                if (state[1003])
                {
                    var publicProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var info in publicProperties)
                    {
                        if (type.IsDefined(typeof (HideInInspector)))
                            continue;
                        Draw(info.PropertyType, info.GetValue(obj), info.Name);
                    }
                }
                
                state[1004] = EditorGUILayout.Foldout(state[1004], "No Public Field", "FoldoutHeader");
                if (state[1004])
                {
                    var noPublicFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                    foreach (var info in noPublicFields)
                    {
                        if (type.IsDefined(typeof (HideInInspector)))
                            continue;
                        Draw(info.FieldType, info.GetValue(obj), info.Name);
                    }
                }
                
                state[1005] = EditorGUILayout.Foldout(state[1005], "No Public Property", "FoldoutHeader");
                if (state[1005])
                {
                    var noPublicProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
                    foreach (var info in noPublicProperties)
                    {
                        if (type.IsDefined(typeof (HideInInspector)))
                            continue;
                        Draw(info.PropertyType, info.GetValue(obj), info.Name);
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

    }
}