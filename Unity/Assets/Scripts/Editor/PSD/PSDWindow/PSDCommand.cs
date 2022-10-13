﻿using System;
using UnityEditor;
using UnityEngine;

namespace XGame
{
    public static class PSDCommand
    {
        //显示Timeline
        [MenuItem("UGUI/PSD2UGUI")]
        public static void Show()
        {
            Type fileMenu = typeof(PSDFileMenu);
            Type operatePlane = typeof(PSDOperator);
            Type attr = typeof(PSDLayerInspector);
            //创建最外层容器
            object containerInstance = EditorContainerWindow.CreateInstance();
            //创建分屏容器
            object split1 = EditorSplitView.CreateInstance();
            //设置根容器
            EditorContainerWindow.SetRootView(containerInstance, split1);
            //添加menu容器和工具容器
            object menuDockAreaInstance = EditorDockArea.CreateInstance();
            EditorDockArea.SetPosition(menuDockAreaInstance, new Rect(0, 0, 300, 800));
            EditorWindow menuWindow = (EditorWindow) ScriptableObject.CreateInstance(fileMenu);
            //menuWindow.position = new Rect(0,0,150,800);
            EditorDockArea.AddTab(menuDockAreaInstance, menuWindow);
            EditorSplitView.AddChild(split1, menuDockAreaInstance);
            
            //添加timeline窗体
            object timelineDockAreaInstance = EditorDockArea.CreateInstance();
            EditorDockArea.SetPosition(timelineDockAreaInstance, new Rect(0, 0, 1000, 800));
            EditorWindow timelineWindow = (EditorWindow) ScriptableObject.CreateInstance(operatePlane);
            EditorDockArea.AddTab(timelineDockAreaInstance, timelineWindow);
            EditorSplitView.AddChild(split1, timelineDockAreaInstance);
            
            //属性窗口
            object attrDockArea = EditorDockArea.CreateInstance();
            EditorDockArea.SetPosition(attrDockArea, new Rect(0, 0, 300, 800));
            EditorWindow attrWindow = (EditorWindow) ScriptableObject.CreateInstance(attr);
            EditorDockArea.AddTab(attrDockArea, attrWindow);
            EditorSplitView.AddChild(split1, attrDockArea);
            
            EditorEditorWindow.MakeParentsSettingsMatchMe(menuWindow);
            EditorEditorWindow.MakeParentsSettingsMatchMe(timelineWindow);
            EditorEditorWindow.MakeParentsSettingsMatchMe(attrWindow);

            EditorContainerWindow.SetPosition(containerInstance, new Rect(400, 100, 1600, 800));
            EditorSplitView.SetPosition(split1, new Rect(0, 0, 1600, 800));
            EditorContainerWindow.Show(containerInstance, 0, true, false, true);
            EditorContainerWindow.OnResize(containerInstance);
        }
    }
}