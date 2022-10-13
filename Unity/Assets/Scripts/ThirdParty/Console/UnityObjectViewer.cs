using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace XGame
{
    public class UnityObjectViewer : DebugWindowBase
    {
        private Dictionary<int, bool> _opendObj = new Dictionary<int, bool>();
        private int _selectedScene;
        private int _selectedComponent;
        private Vector2 _scrollPos;
        private Vector2 _scrollPos2;
        private GameObject _selectedObj;

        private bool _showPrivate = true;
        private bool _showPublic = true;
        private bool _showProperty = true;
        protected override void OnDrawWindow(int id)
        {
            var list = new List<string>(){"DontDestory"};
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                list.Add(scene.name);
            }

            if (this._selectedScene > SceneManager.sceneCount)
            {
                this._selectedScene = 0;
            }
                
            GUILayout.BeginHorizontal();
            GUILayout.Label("选择场景", GUILayout.Width(100));
            this._selectedScene = GUILayout.Toolbar(this._selectedScene, list.ToArray(), GUI.skin.button, GUI.ToolbarButtonSize.FitToContents);
            GUILayout.EndHorizontal();
            
            var rootGameObjects = this._selectedScene == 0 ? this.getDontDestroyOnLoadGameObjects() : SceneManager.GetSceneAt(this._selectedScene - 1).GetRootGameObjects();

            
            GUILayout.BeginHorizontal();
            this._scrollPos = GUILayout.BeginScrollView(this._scrollPos, GUILayout.Width(this._windowRect.width * 0.49f));
            foreach (GameObject node in rootGameObjects)
            {
                for (int i = 0; i < node.transform.childCount; i++)
                {
                    var child = node.transform.GetChild(i).gameObject;
                    GUILayout.BeginHorizontal(GUI.skin.box);
                    this._opendObj.TryGetValue(child.GetHashCode(), out var show);
                    this._opendObj[child.GetHashCode()] = GUILayout.Toggle(show, child.name);
                    GUILayout.FlexibleSpace();
                    var active = GUILayout.Toggle(child.gameObject.activeSelf, "Active", GUI.skin.button);
                    if (active != child.activeSelf)
                    {
                        child.gameObject.SetActive(active);
                    }
                    if (GUILayout.Button("查看组件属性"))
                    {
                        this._selectedObj = child.gameObject;
                    }
                    GUILayout.EndHorizontal();
                    if (show)
                    {
                        GUILayout.BeginVertical();
                        DepthDraw(child, 1);
                        GUILayout.EndVertical();
                    }
                }
            }
            GUILayout.EndScrollView();
            this._scrollPos2 = GUILayout.BeginScrollView(this._scrollPos2, GUILayout.Width(this._windowRect.width * 0.49f));
            if (this._selectedObj != null)
            {
                var components = this._selectedObj.GetComponents<Component>();
                var names = new List<string>();
                foreach (Component component in components)
                {
                    names.Add(component.GetType().Name);
                }

                if (this._selectedComponent >= components.Length)
                    this._selectedComponent = 0;

                var lineCount = Mathf.FloorToInt(_windowRect.width / 100);
                var lines = Mathf.CeilToInt(1f * names.Count / lineCount);
                for (int i = 0; i < lines; i++)
                {
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < lineCount; j++)
                    {
                        var index = i * lineCount + j;
                        if (index >= names.Count)
                            break;
                        if (GUILayout.Toggle(_selectedComponent == index, names[index], GUI.skin.button, GUILayout.Width(100)))
                        {
                            _selectedComponent = index;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                
                GUILayout.BeginHorizontal();
                this._showPrivate = GUILayout.Toggle(this._showPrivate, "私有属性", GUI.skin.button, GUILayout.ExpandWidth(false));
                this._showPublic = GUILayout.Toggle(this._showPublic, "公有属性", GUI.skin.button, GUILayout.ExpandWidth(false));
                this._showProperty = GUILayout.Toggle(this._showProperty, "显示属性", GUI.skin.button, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
                var com = components[this._selectedComponent];
                var skin = GUI.skin.textField;
                skin.richText = true;
                GUI.enabled = false;
                if (this._showPublic)
                {
                    GUILayout.TextField("公有字段--------------------");
                    var fields = com.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (FieldInfo fieldInfo in fields)
                    {
                        var attr = fieldInfo.GetCustomAttribute<ObsoleteAttribute>();
                        if (attr != null)
                            continue;
                        GUILayout.TextField($"<color=white>{fieldInfo.Name}</color> <color=yellow>{fieldInfo.FieldType}</color> <color=#B7FF4D>{fieldInfo.GetValue(com)}</color>", skin);
                    }

                    if (this._showProperty)
                    {
                        GUILayout.TextField("公有属性--------------------");
                        var properties = com.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var propertyInfo in properties)
                        {
                            var attr = propertyInfo.GetCustomAttribute<ObsoleteAttribute>();
                            if (attr != null)
                                continue;
                            GUILayout.TextField($"<color=white>{propertyInfo.Name}</color> <color=yellow>{propertyInfo.PropertyType}</color> <color=#B7FF4D>{propertyInfo.GetValue(com)}</color>", skin);
                        }

                    }
                }
                if (this._showPrivate)
                {
                    GUILayout.TextField("私有字段--------------------");
                    var fields = com.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (FieldInfo fieldInfo in fields)
                    {
                        var attr = fieldInfo.GetCustomAttribute<ObsoleteAttribute>();
                        if (attr != null)
                            continue;
                        GUILayout.TextField($"<color=white>{fieldInfo.Name}</color> <color=yellow>{fieldInfo.FieldType}</color> <color=#B7FF4D>{fieldInfo.GetValue(com)}</color>", skin);
                    }

                    if (this._showProperty)
                    {
                        GUILayout.TextField("私有属性--------------------");
                        var properties = com.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
                        foreach (var propertyInfo in properties)
                        {
                            var attr = propertyInfo.GetCustomAttribute<ObsoleteAttribute>();
                            if (attr != null)
                                continue;
                            GUILayout.TextField($"<color=white>{propertyInfo.Name}</color> <color=yellow>{propertyInfo.PropertyType}</color> <color=#B7FF4D>{propertyInfo.GetValue(com)}</color>", skin);
                        }

                    }
                }

                GUI.enabled = true;
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }

        private void DepthDraw(GameObject node, int track)
        {
            for (int i = 0; i < node.transform.childCount; i++)
            {
                var child = node.transform.GetChild(i).gameObject;
                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Space(track * 20);
                this._opendObj.TryGetValue(child.GetHashCode(), out var show);
                this._opendObj[child.GetHashCode()] = GUILayout.Toggle(show, child.name);
                GUILayout.FlexibleSpace();
                var active = GUILayout.Toggle(child.gameObject.activeSelf, "Active", GUI.skin.button);
                if (active != child.activeSelf)
                {
                    child.gameObject.SetActive(active);
                }
                if (GUILayout.Button("查看组件属性"))
                {
                    this._selectedObj = child.gameObject;
                }
                GUILayout.EndHorizontal();
                if (show)
                {
                    GUILayout.BeginVertical();
                    DepthDraw(child, track + 1);
                    GUILayout.EndVertical();
                }
            }
        }
        
        private GameObject[] getDontDestroyOnLoadGameObjects()
        {
            var allGameObjects = new List<GameObject>();
            allGameObjects.AddRange(Object.FindObjectsOfType<GameObject>());
            //移除所有场景包含的对象
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var objs = scene.GetRootGameObjects();
                for (var j = 0; j < objs.Length; j++)
                {
                    allGameObjects.Remove(objs[j]);
                }
            }
            //移除父级不为null的对象
            int k = allGameObjects.Count;
            while (--k >= 0)
            {
                if (allGameObjects[k].transform.parent != null)
                {
                    allGameObjects.RemoveAt(k);
                }
            }
            return allGameObjects.ToArray();
        }

    }
}