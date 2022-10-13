using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace XGame
{
    public struct PSDParseInfo
    {
        public string FullName;
        //导出的Prefab名
        public string Name;
        public string PublicPath;
        public string ToFolder => Setting.UIFolder;
        public PSDSetting Setting => PSDUtility.PSDSetting;
        
        public string TargetPath =>  $"{ToFolder}/{PublicPath}/prefab/{Name}.prefab";

        public PSDParseInfo(string file)
        {
            FullName = file;
            var fileName = Path.GetFileNameWithoutExtension(file);
            Name = fileName.Substring(fileName.IndexOf('@') + 1);
            var str = FullName.Replace(PSDUtility.PSDSetting.PsdFolder, "");
            PublicPath = str.Substring(0, str.LastIndexOf('\\')).Replace('\\', '/').Trim('/');
        }
    }
    
    public static class PSDParse
    {
        private static Vector2 _screenSize;
        public static void Parse(PSDInfo psdInfo, Vector2 screenSize)
        {
            _screenSize = screenSize;
   
            var canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
                canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
            }
           
            var target = psdInfo.ParseInfo.TargetPath;
            if (!Directory.Exists(Path.GetDirectoryName(target)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target));
            }
            if (File.Exists(target))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(target);
                var obj = (GameObject) PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
                PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                var objs = new List<Object>();
                UpdatePrefab(psdInfo.Root, obj, objs);
                EditorGUIUtility.PingObject(obj.gameObject);
                PrefabUtility.SaveAsPrefabAssetAndConnect(obj.gameObject, target, InteractionMode.AutomatedAction);
            }
            else
            {
                RectTransform obj = new GameObject(psdInfo.Root.Name).AddComponent<RectTransform>();
                obj.SetParent(canvas.transform, false);
                psdInfo.Root.SetTransform(obj, screenSize);
                var objs = new List<Object>();
                Group2UGUI(obj, psdInfo.Root, objs);
                if (!Directory.Exists(Path.GetDirectoryName(target)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                }
                EditorGUIUtility.PingObject(obj.gameObject);
                PrefabUtility.SaveAsPrefabAssetAndConnect(obj.gameObject, target, InteractionMode.AutomatedAction);
            }
        }

        private static void UpdatePrefab(PSDLayerGroup layer, GameObject node, List<Object> objs)
        {
            var nodeRect = node.GetComponent<RectTransform>();
            var allChild = node.transform.Children().ToList();
            layer.SetTransform(nodeRect, _screenSize);
            layer.SetVariableValue(ref nodeRect);
            var index = 0;
            foreach (var psdLayer in layer.PsdLayers)
            {
                if (psdLayer.Ignore)
                    continue;
                var nodeTrans = node.transform.Find(psdLayer.Name);
                if (nodeTrans == null)
                {
                    nodeTrans = new GameObject(psdLayer.Name).AddComponent<RectTransform>();
                    nodeTrans.SetParent(nodeRect);
                    psdLayer.SetDefaultValue((RectTransform) nodeTrans);
                }
                else
                {
                    allChild.Remove(nodeTrans);
                }
                
                if (psdLayer is PSDLayerGroup group)
                {
                    UpdatePrefab(group, nodeTrans.gameObject, objs);
                }
                else
                {
                    var rect = nodeTrans.GetComponent<RectTransform>();
                    if (rect.childCount > 0) //group变成image或text
                    {
                        rect.ClearChildren();
                        psdLayer.SetDefaultValue(rect);
                    }

                    if (psdLayer is PSDLayerText)
                    {
                        if (nodeTrans.GetComponent<Text>() == null)
                        {
                            psdLayer.SetDefaultValue(rect);
                        }
                    }
                    else if (psdLayer is PSDLayerTextPro)
                    {
                        if (nodeTrans.GetComponent<TextMeshProUGUI>() == null)
                        {
                            psdLayer.SetDefaultValue(rect);
                        }
                    }
                    else if (psdLayer is PSDLayerImage)
                    {
                        if (nodeTrans.GetComponent<Image>() == null)
                        {
                            psdLayer.SetDefaultValue(rect);
                        }
                    }
                    
                    psdLayer.SetTransform(rect, _screenSize);
                    psdLayer.SetVariableValue(ref rect);
                    nodeTrans = rect.transform;
                }
                nodeTrans.SetSiblingIndex(index);
                index++;

                if (psdLayer.Reference)
                {
                    objs.Add(nodeTrans.gameObject);
                }
            }

            foreach (var transform in allChild)
            {
                Object.DestroyImmediate(transform.gameObject);
            }
        }
        
        private static void Group2UGUI(RectTransform node, PSDLayerGroup group, List<Object> objs)
        {
            foreach (var psdLayer in group.PsdLayers)
            {
                if (psdLayer.Ignore)
                    continue;
                RectTransform lo = null;
                lo = CreateRect(psdLayer);
                if (lo != null)
                {
                    lo.transform.SetParent(node.transform, false);
                    psdLayer.SetTransform(lo, _screenSize);
                }
                if (psdLayer is PSDLayerGroup g)
                {
                    Group2UGUI(lo, g, objs);
                    g.SetVariableValue(ref lo);
                }else if (psdLayer is PSDLayerImage image)
                {
                    image.SetDefaultValue(lo);
                    image.SetVariableValue(ref lo);
                }else if (psdLayer is PSDLayerText text)
                {
                    text.SetDefaultValue(lo);
                    text.SetVariableValue(ref lo);
                }
                else if (psdLayer is PSDLayerTextPro textPro)
                {
                    textPro.SetDefaultValue(lo);
                    textPro.SetVariableValue(ref lo);
                }

                if (psdLayer.Reference)
                {
                    objs.Add(lo.gameObject);
                }
            }
        }

        private static void CopyGameObject(GameObject from, GameObject to)
        {
            var components = from.GetComponents<Component>();
            foreach (var c in components)
            {
                ComponentUtility.CopyComponent(c);
                if (to.TryGetComponent(c.GetType(), out var target))
                {
                    ComponentUtility.PasteComponentValues(target);
                }
                else
                {
                    ComponentUtility.PasteComponentAsNew(to);
                }
            }
        }
        
        private static RectTransform CreateRect(IPSDLayer layer)
        {
            var obj = new GameObject(layer.Name);
            var rect = obj.AddComponent<RectTransform>();
            return rect;
        }
    }
}