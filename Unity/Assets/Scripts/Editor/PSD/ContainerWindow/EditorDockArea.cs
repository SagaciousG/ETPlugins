 using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Object = System.Object;

 namespace XGame
 {

     public static class EditorDockArea
     {
         private static Type _dockAreaType;

         /// <summary>
         /// 类型
         /// </summary>
         public static Type DockAreaType
         {
             get
             {
                 if (_dockAreaType == null)
                     _dockAreaType = typeof(EditorWindow).Assembly.GetType("UnityEditor.DockArea");
                 return _dockAreaType;
             }
         }

         /// <summary>
         /// 创建实例
         /// </summary>
         /// <returns></returns>
         public static object CreateInstance()
         {
             return ScriptableObject.CreateInstance(DockAreaType);
         }

         /// <summary>
         /// 添加Tab
         /// </summary>
         /// <param name="instance"></param>
         /// <param name="window"></param>
         /// <param name="sendPaneEvents"></param>
         public static void AddTab(object instance, EditorWindow window, bool sendPaneEvents = true)
         {
             MethodInfo mInfo = DockAreaType.GetMethod("AddTab", BindingFlags.Instance | BindingFlags.Public, null,
                 new Type[] {typeof(EditorWindow), typeof(bool)}, null);
             if (mInfo == null) return;
             mInfo.Invoke(instance, new object[] {window, sendPaneEvents});
         }

         /// <summary>
         /// 设置坐标
         /// </summary>
         /// <param name="instance"></param>
         /// <param name="position"></param>
         public static void SetPosition(object instance, Rect position)
         {
             PropertyInfo pInfo = DockAreaType.GetProperty("position", BindingFlags.Instance | BindingFlags.Public);
             if (pInfo == null) return;
             pInfo.SetValue(instance, position);
         }

         /// <summary>
         /// 获取坐标
         /// </summary>
         /// <param name="instance"></param>
         /// <returns></returns>
         public static Rect GetPosition(object instance)
         {
             PropertyInfo pInfo = DockAreaType.GetProperty("position", BindingFlags.Instance | BindingFlags.Public);
             if (pInfo == null) return default(Rect);
             return (Rect) pInfo.GetValue(instance);
         }
     }

 }