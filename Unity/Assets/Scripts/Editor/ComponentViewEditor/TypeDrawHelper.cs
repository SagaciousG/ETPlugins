using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using XGame;

namespace ET
{
    public static class TypeDrawHelper
    {
        private static bool publicFieldShow = true;
        private static bool publicPropertyShow;
        private static bool noPublicFieldShow;
        private static bool noPublicPropertyShow;

        private static object _drawTarget;
        private static GUIElementState _state;
        
        public static void BeginDraw(object obj)
        {
            if (_drawTarget != obj)
            {
                GUIElementStateManager.Reset(GUIElementStateDomain.ComponentView);
                _state = GUIElementStateManager.Add(GUIElementStateDomain.ComponentView, obj);
            }

            _drawTarget = obj;
            EditorGUIUtility.labelWidth = 144;
            var type = obj.GetType();
            publicFieldShow = EditorGUILayout.BeginFoldoutHeaderGroup(publicFieldShow, "Public Field");
            if (publicFieldShow)
            {
                var publicFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                foreach (var info in publicFields)
                {
                    if (type.IsDefined(typeof (HideInInspector)))
                        continue;
                    Draw(info.FieldType, info.GetValue(obj), info.Name);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            publicPropertyShow = EditorGUILayout.BeginFoldoutHeaderGroup(publicPropertyShow, "Public Property");
            if (publicPropertyShow)
            {
                var publicProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var info in publicProperties)
                {
                    if (type.IsDefined(typeof (HideInInspector)))
                        continue;
                    Draw(info.PropertyType, info.GetValue(obj), info.Name);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            noPublicFieldShow = EditorGUILayout.BeginFoldoutHeaderGroup(noPublicFieldShow, "No Public Field");
            if (noPublicFieldShow)
            {
                var noPublicFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var info in noPublicFields)
                {
                    if (type.IsDefined(typeof (HideInInspector)))
                        continue;
                    Draw(info.FieldType, info.GetValue(obj), info.Name);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            noPublicPropertyShow = EditorGUILayout.BeginFoldoutHeaderGroup(noPublicPropertyShow, "No Public Property");
            if (noPublicPropertyShow)
            {
                var noPublicProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var info in noPublicProperties)
                {
                    if (type.IsDefined(typeof (HideInInspector)))
                        continue;
                    Draw(info.PropertyType, info.GetValue(obj), info.Name);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
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
        
        private static readonly Dictionary<Type, TypeDrawer> typeDrawers = new();
        private static readonly List<TypeDrawer> inheritDrawers = new();
        private static readonly DefaultStructDrawer _defaultStructDrawer = new();
        
        static TypeDrawHelper()
        {
            Assembly assembly = typeof (TypeDrawHelper).Assembly;
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
                else if (type.IsValueType)
                {
                    _defaultStructDrawer.DrawAndGetNewValue(type, label, obj, null);
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

       
            if (_state.depth > 1)
            {
                EditorGUILayout.LabelField(label, "Cyclic Reference");
                return;
            }
            
            var type = obj.GetType();
            _state[1001] = GUILayoutHelper.Foldout(_state[1001], new GUIContent(label));
            if (_state[1001])
            {
                EditorGUILayout.BeginVertical("FrameBox");
                {
                    var boxSize = GUILayoutHelper.GetCurrentLayoutGroupSize();
                    EditorGUILayout.BeginVertical("FrameBox");
                    _state[1002] = GUILayoutHelper.Foldout(_state[1002], new GUIContent("Public Field"));
                    if (_state[1002])
                    {
                        var publicFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                        foreach (var info in publicFields)
                        {
                            if (type.IsDefined(typeof (HideInInspector)))
                                continue;
                            Draw(info.FieldType, info.GetValue(obj), info.Name);
                        }
                    }

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical("FrameBox");
                    _state[1003] = GUILayoutHelper.Foldout(_state[1003], new GUIContent("Public Property"));
                    if (_state[1003])
                    {
                        var publicProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                        foreach (var info in publicProperties)
                        {
                            if (type.IsDefined(typeof (HideInInspector)))
                                continue;
                            Draw(info.PropertyType, info.GetValue(obj), info.Name);
                        }
                    }

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical("FrameBox");
                    _state[1004] = GUILayoutHelper.Foldout(_state[1004], new GUIContent("No Public Field"));
                    if (_state[1004])
                    {
                        var noPublicFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                        foreach (var info in noPublicFields)
                        {
                            if (type.IsDefined(typeof (HideInInspector)))
                                continue;
                            Draw(info.FieldType, info.GetValue(obj), info.Name);
                        }
                    }

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical("FrameBox");
                    _state[1005] = GUILayoutHelper.Foldout(_state[1005], new GUIContent("No Public Property"));
                    if (_state[1005])
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
                EditorGUILayout.EndVertical();
            }
        }
    }
}