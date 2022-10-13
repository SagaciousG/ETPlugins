using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspose.PSD.FileFormats.Psd.Layers;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace XGame
{
    public class PSDOperator : EditorWindow
    {
        public static PSDOperator Instance;

        public static void Refresh()
        {
            Instance.Repaint();
        }
        public static void Open(string file, bool refresh = false)
        {
            Instance.AddFile(file, refresh);
        }
        
        private List<PSDInfo> _psdInfos = new List<PSDInfo>();

        private PSDInfo _selectInfo;

        private EditorTreeView<PSDItem> _psdTree;
        private EditorTreeView<GameObjectItem> _gameObjectTree;

        private List<PSDItem> _psdItems = new List<PSDItem>();
        private List<GameObjectItem> _prefabTreeItems = new List<GameObjectItem>();
        private Dictionary<int, string> _uName2refName = new Dictionary<int, string>();
        private Vector2 _titleScrollPos;
        private int _loadingIndex;
        private bool _readed;
        
        public PSDInfo GetInfo(string path)
        {
            foreach (var psdInfo in _psdInfos)
            {
                if (psdInfo.ParseInfo.FullName == path)
                    return psdInfo;
            }

            return null;
        }
        private void AddFile(string file, bool refresh)
        {
            for (var i = 0; i < _psdInfos.Count; i++)
            {
                var psdInfo = _psdInfos[i];
                if (psdInfo.ParseInfo.FullName == file)
                {
                    if (_selectInfo.Equals(psdInfo) && !refresh)
                        return;
                    if (refresh)
                    {
                        _psdInfos[i] = new PSDInfo(file, true);
                        psdInfo = _psdInfos[i];
                    }

                    _selectInfo = psdInfo;
                    OnSelectPSD();
                    return;
                }
            }

            _psdInfos.Add(new PSDInfo(file, true));
            _selectInfo = _psdInfos.Last();
            OnSelectPSD();
        }

        private void ReadFile(PSDInfo psdInfo)
        {
            _psdItems.Clear();
            _prefabTreeItems.Clear();

            _psdItems.Add(new PSDItem(new PSDLayerGroup() {RealName = "Root"}, true) {Depth = -1});
            _prefabTreeItems.Add(new GameObjectItem(null){Depth = -1});
            if (psdInfo != null && !psdInfo.Loading)
            {
                BuildPSDTree(new PSDItem(psdInfo.Root, false){UID = psdInfo.Root.UName.GetHashCode()}, 0);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(psdInfo.ParseInfo.TargetPath);
                if (prefab != null)
                {
                    BuildPrefabTree(new GameObjectItem(prefab), 0);
                    _uName2refName.Clear();
                }

                _readed = true;
            }

            
            _psdTree.Build(new TreeModel<PSDItem>(_psdItems));
            _psdTree.Reload();
            _psdTree.ExpandAll();
            
            _gameObjectTree.Build(new TreeModel<GameObjectItem>(_prefabTreeItems));
            _gameObjectTree.Reload();
            _gameObjectTree.ExpandAll();
            
            _psdTree.Data.ModelChanged += OnModelChanged;
        }
        
        private void BuildPSDTree(PSDItem tree, int depth)
        {
            tree.Depth = depth;
            _psdItems.Add(tree);
            if (tree.PsdLayer is PSDLayerGroup layerGroup)
            {
                tree.Children = new List<TreeElement>();
                for (int i = 0; i < layerGroup.PsdLayers.Count; i++)
                {
                    var item =  new PSDItem(layerGroup.PsdLayers[i], false);
                    item.Parent = tree;
                    item.UID = item.UName.GetHashCode();
                    BuildPSDTree(item, depth + 1);
                }   
            }
        }
        
        private void BuildPrefabTree(GameObjectItem tree, int depth)
        {
            tree.Depth = depth;
            if (tree.Target.gameObject.GetComponent<Text>() != null)
                tree.LayerType = LayerType.Text;
            else if (tree.Target.gameObject.GetComponent<Graphic>() != null)
                tree.LayerType = LayerType.Image;
            else
                tree.LayerType = LayerType.GameObject;
            _prefabTreeItems.Add(tree);
            if (tree.Target.transform.childCount > 0)
            {
                tree.Children = new List<TreeElement>();
                for (int i = 0; i < tree.Target.transform.childCount; i++)
                {
                    var item =  new GameObjectItem(tree.Target.transform.GetChild(i).gameObject);
                    item.Parent = tree;
                    BuildPrefabTree(item, depth + 1);
                }   
            }
        }
        
        private void OnEnable()
        {
            Instance = this;
            titleContent = new GUIContent("编辑文件");
            _psdTree = new EditorTreeView<PSDItem>(new TreeViewState());
            _psdTree.OnDrawRowCallback += PsdTreeOnDrawRowCallback;
            _psdTree.OnSingleClickedItem += PsdTreeOnSingleClickedItem;
            _psdTree.OnContextClickedItem += PsdTreeOnContextClickedItem;
            _psdTree.OnDoubleClickItem += PsdTreeOnOnDoubleClickItem;
            _psdTree.OnDropVerify += PsdTreeOnDropVerify;
            
            _gameObjectTree = new EditorTreeView<GameObjectItem>(new TreeViewState());
            _gameObjectTree.OnDrawRowCallback += OnDrawRowCallback;
            _gameObjectTree.OnSingleClickedItem += OnSingleClickedItem;
            _gameObjectTree.OnDoubleClickItem += OnSingleClickedItem;
            
        }

        private void OnDisable()
        {
            Instance = null;
            foreach (var psdInfo in _psdInfos)
            {
                psdInfo.Dispose();
            }
        }

        private void OnModelChanged()
        {
            _psdTree.Reload();
        }

        private void OnSingleClickedItem(int obj)
        {
            var item = _gameObjectTree.Data.Find(obj);
            PSDUtility.SelectedLayer = new PSDSelectedLayer(item.Target);
            
            _psdTree.FrameItem(obj);
            _psdTree.SetSelection(new []{obj});
            PSDLayerInspector.Instance.Repaint();
        }

        private void OnDrawRowCallback(TreeViewItemRow<GameObjectItem> obj)
        {
            var indent = obj.GetContentIndent.Invoke(obj.item);
            var cellRect = obj.rowRect;
            cellRect.x += indent;

            if (_psdTree.Data.Data.FirstOrDefault(a => obj.item.Data.UName.GetHashCode() == a.LinkName.GetHashCode()) == null)
            {
                obj.item.Data.Tag = UpdateTag.Remove;
            }
            else
            {
                obj.item.Data.Tag = UpdateTag.None;
            }
            
            var iconName = GetIcon(obj.item.Data.Tag);
            if (!string.IsNullOrEmpty(iconName))
            {
                var icon = EditorGUIUtility.IconContent(iconName);
                var iconRect = obj.rowRect;
                iconRect.y -= 5;
                EditorGUI.LabelField(iconRect, icon);
            }
            
            var flagRect = cellRect;
            flagRect.y -= 5;
            EditorGUI.LabelField(flagRect, EditorGUIUtility.IconContent(GetIcon(obj.item.Data.LayerType)));

            cellRect.x += 20;
            _uName2refName.TryGetValue(obj.item.Data.Target.GetInstanceID(), out var refName);
            EditorGUI.LabelField(cellRect, $"<size=15>{obj.item.Data.Name}</size> <color=#807D7D><size=13>{refName}</size></color>"
                , new GUIStyle(){richText = true});

        }


        #region PSDTree
        private void PsdTreeOnOnDoubleClickItem(int id)
        {
            var layer = _psdTree.Data.Find(id);
            if (layer.PsdLayer?.IsRoot ?? false)
                return;
        }
 
        private bool PsdTreeOnDropVerify(TreeViewItem arg1, List<TreeViewItem> arg2)
        {
            var parent = (TreeViewItem<PSDItem>) arg1;
            if (parent == null)
                return false;
            if (parent.Data.LayerType == LayerType.Group)
                return true;
            return false;
        }

        private void PsdTreeOnContextClickedItem(int id)
        {
            var layer = _psdTree.Data.Find(id);
            if (layer.PsdLayer?.IsRoot ?? false)
                return;
            var menu = new GenericMenu ();
            menu.AddItem(new GUIContent("删除层级"), false, RemoveLayer, new MenuItemArgs(id));
            if (layer.LayerType == LayerType.Group)
            {
                menu.AddItem (new GUIContent ("Add/Folder"), false, AddFolder, new MenuItemArgs(id));
            }
            menu.AddItem(new GUIContent("添加标签/引用(@ref)"), false, AddTag, 
                new MenuItemArgs(id)
                {
                    StrParam = "@ref"
                });
            menu.AddItem(new GUIContent("添加标签/忽略(@ignore)"), false, AddTag, 
                new MenuItemArgs(id)
                {
                    StrParam = "@ignore"
                });
            if (layer.LayerType == LayerType.Image)
            {
                menu.AddItem(new GUIContent("添加标签/空(@empty)"), false, AddTag,
                    new MenuItemArgs(id)
                    {
                        StrParam = "@empty"
                    });
            }

            if (layer.LayerType == LayerType.Text)
            {
                menu.AddItem(new GUIContent("添加标签/转大写(@up)"), false, AddTag,
                    new MenuItemArgs(id)
                    {
                        StrParam = "@up"
                    });
            }
            menu.ShowAsContext ();
        }

        private void PsdTreeOnSingleClickedItem(int obj)
        {
            var item = _psdTree.Data.Find(obj);
            if (item == null)
                return;
            PSDUtility.SelectedLayer = new PSDSelectedLayer(item.PsdLayer);
            _gameObjectTree.FrameItem(obj);
            _gameObjectTree.SetSelection(new []{obj});
            PSDUtility.RuntimeInfo.LayerIndex = item.PsdLayer.LayerIndex;
            PSDUtility.RuntimeInfo.SelectRect = item.PsdLayer.Rect;
            PSDLayerInspector.Instance.Repaint();
        }

        private void PsdTreeOnDrawRowCallback(TreeViewItemRow<PSDItem> obj)
        {
            var indent = obj.GetContentIndent.Invoke(obj.item);
            var cellRect = obj.rowRect;
            cellRect.x += indent;
            var treeLayer = obj.item.Data;
            if (_gameObjectTree.Data.Data.FirstOrDefault(a => a.UName.GetHashCode() == treeLayer.LinkName.GetHashCode()) == null)
            {
                if (treeLayer.PsdLayer?.Ignore ?? false)
                    treeLayer.Tag = UpdateTag.Ignore;
                else
                    treeLayer.Tag = UpdateTag.Add;
            }
            else
            {
                treeLayer.Tag = UpdateTag.None;
            }
            
            var iconName = GetIcon(treeLayer.Tag);
            if (!string.IsNullOrEmpty(iconName))
            {
                var icon = EditorGUIUtility.IconContent(iconName);
                var rowRect = obj.rowRect;
                rowRect.y -= 5;
                EditorGUI.LabelField(rowRect, icon);
            }

            var iconRect = cellRect;
            iconRect.y -= 5;
            EditorGUI.LabelField(iconRect, EditorGUIUtility.IconContent(GetIcon(treeLayer.LayerType)));
            cellRect.x += 20;
            
            var color = "000000";
            if (treeLayer.PsdLayer == null)
                color = "FF9966";
            else
                color = treeLayer.PsdLayer.Reference ? "0097BD" : (EditorGUIUtility.isProSkin ? "ffffff" : "000000");
            var nameTag = $"<size=15><color=#{color}>{treeLayer.Name}</color></size>";
            var labelTag = $"<color=#807D7D><size=13>{treeLayer.RealName}</size></color>";
            EditorGUI.LabelField(cellRect, $"{nameTag} {labelTag}", 
                new GUIStyle(){richText = true});
        }
       

        
        private void RemoveLayer(object obj)
        {
            var p = (MenuItemArgs) obj;
            var layer = _psdTree.Data.Find(p.Id);
            _psdTree.Data.RemoveElements(new List<int>(){p.Id});
            _psdTree.Reload();
        }
        
        
        private void AddFolder(object obj)
        {
            var p = (MenuItemArgs) obj;
            var layer = _psdTree.Data.Find(p.Id);
            var childrenCount = layer.Children?.Count ?? 0;
            var newLayer = new PSDItem(new PSDLayerGroup()
            {
                RealName = $"NewLayer{childrenCount}"
            }, true);
            _psdTree.Data.AddElement(newLayer, layer, childrenCount);
            _psdTree.SetSelection(new List<int>(){newLayer.Id});
            _psdTree.FrameItem(newLayer.Id);
        }
                
        private void AddTag(object obj)
        {
            var p = (MenuItemArgs) obj;
            var layer = _psdTree.Data.Find(p.Id);
            if (!layer.RealName.Contains(p.StrParam))
            {
                if (layer.PsdLayer != null)
                {
                    layer.PsdLayer.RealName = layer.RealName + p.StrParam;
                }
                else
                {
                    layer.RealName = layer.RealName + p.StrParam;
                }
            }
        }
        
        #endregion
        
        
        
        
        private void OnGUI()
        {
            if (_selectInfo == null)
                return;
            GUI.enabled = !_selectInfo.Loading;
            GUILayout.BeginArea(new Rect(5, 0, position.width - 10, 20));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存", "TE toolbarbutton", GUILayout.Width(60)))
            {
                var fileInfo = new FileInfo(_selectInfo.ParseInfo.FullName);
                if (fileInfo.LastWriteTime.Ticks != _selectInfo.LastWriteTime.Ticks)
                {
                    if (!EditorUtility.DisplayDialog("警告", 
                        "文件已发生更改，继续保存将会覆盖已更改的内容！是否覆盖?", "是", "否"))
                    {
                        return;
                    }
                }
                Save(_selectInfo);
            }
            
            if (GUILayout.Button("旧标签转换", "TE toolbarbutton",GUILayout.Width(100)))
            {
                ReplaceOldTag();
            }
            if (GUILayout.Button("删除错误标签", "TE toolbarbutton",GUILayout.Width(100)))
            {
                RemoveInvalidTag();
            }
            if (GUILayout.Button("定位工具", "TE toolbarbutton",GUILayout.Width(100)))
            {
                GetWindow<PSDPngViewWindow>().Show();
            }

            if (GUILayout.Button("生成","TE toolbarbutton", GUILayout.Width(60)))
            {
                if (_selectInfo.Saving)
                {
                    EditorUtility.DisplayDialog("警告", "文件保存中，请误操作！", "知道了");
                    return;
                }
                if (!OpenNewScene())
                    return;
                
                PSDParse.Parse(_selectInfo, PSDUtility.PSDSetting.ScreenSize);
                
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUI.enabled = true;
            
            GUILayout.BeginArea(new Rect(5, 20, position.width - 10, 20));
            _titleScrollPos = EditorGUILayout.BeginScrollView(_titleScrollPos);
            EditorGUILayout.BeginHorizontal();
            for (var i = 0; i < _psdInfos.Count; i++)
            {
                var psdInfo = _psdInfos[i];
                GUI.backgroundColor = psdInfo.Loading ? Color.yellow : (_selectInfo.Equals(psdInfo) 
                    ? Color.green 
                    : new Color(0.8f, 0.8f, 0.8f, 1));
                EditorGUILayout.BeginHorizontal("Tab onlyOne", GUILayout.MaxWidth(120));
                
                if (psdInfo.Saving)
                {
                    EditorGUILayout.LabelField(EditorGUIUtility.IconContent($"d_WaitSpin0{_loadingIndex++}"), GUILayout.Width(20));
                    _loadingIndex = _loadingIndex > 5 ? 0 : _loadingIndex;
                }
                
                if (GUILayout.Button(psdInfo.ParseInfo.Name, "CenteredLabel", GUILayout.MaxWidth(100)))
                {
                    if (_selectInfo.Equals(psdInfo))
                        return;
                    _selectInfo = psdInfo;
                    OnSelectPSD();
                }

                var fileInfo = new FileInfo(psdInfo.ParseInfo.FullName);
                if (GUILayout.Button("X", "CenteredLabel", GUILayout.Width(20)))
                {
                    if (fileInfo.Length != psdInfo.OriginalSize)
                    {
                        if (EditorUtility.DisplayDialog("警告", 
                            "文件已发生更改，是否保存？", "是", "否"))
                        {
                            Save(psdInfo);
                        }
                    }
                    
                    _psdInfos.Remove(psdInfo);
                    psdInfo.Dispose();
                    if (_psdInfos.Count > 0)
                        _selectInfo = _psdInfos.First();
                    else
                        _selectInfo = null;
                    OnSelectPSD();
                }

                // if (psdInfo.OriginalSize != fileInfo.Length)
                // {
                //     if (GUILayout.Button(EditorGUIUtility.IconContent("Save@2x"), GUILayout.Width(20)))
                //     {
                //         Save(psdInfo);
                //     }
                // }

                EditorGUILayout.EndHorizontal();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            
            GUILayout.EndArea();

            if (_selectInfo == null )
                return;
            if (_selectInfo.Loading)
            {
                GUI.Label(new Rect(5, 40, position.width * 0.5f - 5, position.height - 45), "Loading");
            }
            else
            {
                if (!_readed)
                    ReadFile(_selectInfo);
                _psdTree.OnGUI(new Rect(5, 40, position.width * 0.5f - 5, position.height - 45));
            }
            _gameObjectTree.OnGUI(new Rect(position.width * 0.5f, 40, position.width * 0.5f - 5, position.height - 45));
        }

        private bool OpenNewScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != "")
            {
                var res = EditorUtility.DisplayDialog("提示", "生成UI需要在空场景中进行，即将创建新场景，是否继续？", "继续", "取消");
                if (res)
                {
                    if (activeScene.isDirty)
                    {
                        var save = EditorUtility.DisplayDialog("提示", $"场景{activeScene.name}有更改，是否保存", "是", "否");
                        if (save)
                            EditorSceneManager.SaveScene(activeScene);
                    }
                    EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                }
                return res;
            }

            return true;
        }
        
        private void OnSelectPSD()
        {
            _readed = false;
            PSDUtility.RuntimeInfo.PSD = _selectInfo;
            ReadFile(_selectInfo);
        }
        
        private void Save(PSDInfo psdInfo)
        {
            if (psdInfo.Saving)
            {
                EditorUtility.DisplayDialog("警告", "文件正在储存中，请误操作！", "知道了");
                return;
            }
            var layers = _psdTree.Data.Data;
            Task.Run(() =>
            {
                psdInfo.Saving = true;
                DoSave(psdInfo, layers);
                psdInfo.Saving = false;
            });
        }

        private void DoSave(PSDInfo psdInfo, PSDItem[] layers)
        {
            var time = DateTime.Now.Ticks;
            

            var psdImg = psdInfo.PsdImage;
            
            try
            {
                var psdLayers = new List<Layer>();
                var dividerIndex = new Stack<KeyValuePair<PSDItem, int>>();
                for (var index = 1; index < layers.Length; index++)
                {
                    var treeLayer = layers[index];
                    var layer = psdInfo.GetLayer(treeLayer.UID);
                    if (layer == null) //是一个新的Group
                    {
                        while (dividerIndex.Count > 0)
                        {
                            if (!treeLayer.Parent.Equals(dividerIndex.Peek().Key))
                            {
                                var ele = dividerIndex.Pop();
                                var group = psdImg.Layers[ele.Value];
                                group.DisplayName = $"Layer group: {ele.Key.RealName}";
                                psdLayers.Add(group);
                            }
                            else
                                break;
                        }

                        var idx = psdImg.Layers.Length;
                        psdImg.AddLayerGroup(treeLayer.Name, idx, false);
                        psdLayers.Add(psdImg.Layers[idx]);
                        if (treeLayer.HasChildren)
                        {
                            dividerIndex.Push(new KeyValuePair<PSDItem, int>(treeLayer, idx + 1));
                        }
                        else
                        {
                            var group = (LayerGroup) psdImg.Layers[idx + 1];
                            group.DisplayName = $"Layer group: {treeLayer.RealName}";
                            psdLayers.Add(group);
                        }
                    }
                    else if (layer.IsRoot)
                    {

                    }
                    else
                    {
                        if (layer is PSDLayerGroup layerGroup) //
                        {
                            while (dividerIndex.Count > 0)
                            {
                                if (!treeLayer.Parent.Equals(dividerIndex.Peek().Key))
                                {
                                    var ele = dividerIndex.Pop();
                                    var group = psdImg.Layers[ele.Value];
                                    group.DisplayName = $"Layer group: {ele.Key.RealName}";
                                    psdLayers.Add(group);
                                }else
                                    break;
                            }
                            psdLayers.Add(psdImg.Layers[layerGroup.LayerDividerIndex]);

                            dividerIndex.Push(new KeyValuePair<PSDItem, int>(treeLayer, layerGroup.LayerIndex));
                            if (!treeLayer.HasChildren)
                            {
                                var ele = dividerIndex.Pop();
                                var group = psdImg.Layers[ele.Value];
                                group.DisplayName = $"Layer group: {ele.Key.RealName}";
                                psdLayers.Add(group);
                            }
                        }
                        else
                        {
                            while (dividerIndex.Count > 0)
                            {
                                if (!treeLayer.Parent.Equals(dividerIndex.Peek().Key))
                                {
                                    var ele = dividerIndex.Pop();
                                    var group = psdImg.Layers[ele.Value];
                                    group.DisplayName = $"Layer group: {ele.Key.RealName}";
                                    psdLayers.Add(group);
                                }
                                else
                                    break;
                            }

                            var cur = psdImg.Layers[layer.LayerIndex];
                            cur.DisplayName = treeLayer.RealName;
                            
                            psdLayers.Add(cur);
                        }
                    }
                }

                while (dividerIndex.Count > 0)
                {
                    var ele = dividerIndex.Pop();
                    var group = psdImg.Layers[ele.Value];
                    group.DisplayName = $"Layer group: {ele.Key.RealName}";
                    psdLayers.Add(group);
                }

                Debug.Log($"FormatLayerEnd --- {(DateTime.Now.Ticks - time) / 10000000f}");
                time = DateTime.Now.Ticks;
                
                var arr = psdLayers.ToArray();
                psdImg.Layers = arr;
                psdImg.Save();
                
                File.Copy($"{psdInfo.ParseInfo.FullName}.temp", psdInfo.ParseInfo.FullName, true);
                Debug.Log($"SaveEnd --- {(DateTime.Now.Ticks - time) / 10000000f}");
            }
            catch (Exception e)
            {
                throw;
            }

            for (int i = 0; i < _psdInfos.Count; i++)
            {
                if (_psdInfos[i].Equals(psdInfo))
                {
                    var path = psdInfo.ParseInfo.FullName;
                    psdInfo.Dispose();
                    _psdInfos[i] = new PSDInfo(path, true);
                }
            }
        }
        
        private void RemoveInvalidTag()
        {
            var layers = _psdTree.Data.Data;
            for (var index = 2; index < layers.Length; index++)
            {
                var treeLayer = layers[index];
                if (treeLayer.PsdLayer?.Tags != null)
                {
                    foreach (var tag in treeLayer.PsdLayer.Tags)
                    {
                        if (!treeLayer.PsdLayer.ValidTag(tag) && !treeLayer.PsdLayer.IsOldTag(tag))
                        {
                            treeLayer.PsdLayer.RealName = treeLayer.PsdLayer.RealName.Replace($"@{tag}", "");
                        }
                    }

                }
            }
            _psdTree.Reload();
        }

        private void ReplaceOldTag()
        {
            var layers = _psdTree.Data.Data;
            for (var index = 2; index < layers.Length; index++)
            {
                var treeLayer = layers[index];
                if (treeLayer.PsdLayer?.Tags != null)
                {
                    treeLayer.PsdLayer.ReplaceOldTags();
                }
            }
            _psdTree.Reload();
        }

        
        private string GetIcon(UpdateTag tag)
        {
            switch (tag)
            {
                case UpdateTag.Remove:
                    return "d_winbtn_mac_close_h";
                case UpdateTag.Add:
                    return "d_winbtn_mac_max_h";
                case UpdateTag.Update:
                    return "d_winbtn_mac_min";
                case UpdateTag.Ignore:
                    return "winbtn_mac_min_h@2x";
            }

            return null;
        }

        private string GetIcon(LayerType type)
        {
            switch (type)
            {
                case LayerType.Group:
                    return "Folder Icon";
                case LayerType.Image:
                    return "RawImage Icon";
                case LayerType.Text:
                    return "Text Icon";
                case LayerType.GameObject:
                    return "PreMatCube@2x";
            }

            return null;
        }
        
        private enum UpdateTag
        {
            None,
            Add,
            Update,
            Remove,
            Ignore,
        }
        
        private enum LayerType
        {
            Group,
            Image,
            Text,
            GameObject
        }

        
        private class PSDItem : TreeElement
        {
            public override int Id
            {
                get => GetHashCode();
            }
            
            public override string Name => PsdLayer.Name;

            public string UName
            {
                get
                {
                    if (Depth == -1)
                        return "";
                    var p = (PSDItem) Parent;
                    return $"{p?.UName}-{Name}{PsdLayer.LayerIndex}".Trim('-');
                }
            }

            public string LinkName
            {
                get
                {
                    if (Depth == -1)
                        return "";
                    var p = (PSDItem) Parent;
                    return $"{p?.LinkName}-{Name}".Trim('-');
                }
            }

            public string RealName
            {
                get => PsdLayer.RealName;
                set => PsdLayer.RealName = value;
            }
            
            public UpdateTag Tag;

            public LayerType LayerType
            {
                get
                {
                    switch (PsdLayer)
                    {
                        case PSDLayerImage image:
                            return LayerType.Image;
                        case PSDLayerText text:
                            return LayerType.Text;
                        case PSDLayerGroup group:
                            return LayerType.Group;
                    }

                    return LayerType.Group;
                }
            }

            public int UID;
            public readonly IPSDLayer PsdLayer;
            public readonly bool IsAddNew;

            public PSDItem(IPSDLayer psdLayer, bool isAddNew)
            {
                PsdLayer = psdLayer;
                IsAddNew = isAddNew;
            }
        }
        
        private class GameObjectItem : TreeElement
        {
            public override int Id
            {
                get => GetHashCode();
            }

            public override string Name
            {
                get
                {
                    if (Target != null)
                        return Target.name;
                    return "Root";
                }
                set => Target.name = value;
            }

            public string UName
            {
                get
                {
                    if (Depth == -1)
                        return "";
                    var p = (GameObjectItem) Parent;
                    return $"{p?.UName}-{Name}".Trim('-');
                }
            }

            public LayerType LayerType;
            public UpdateTag Tag;
            
            public readonly GameObject Target;
            
            public GameObjectItem(GameObject layer)
            {
                Target = layer;
            }
        }
        
        private class MenuItemArgs
        {
            public MenuItemArgs(int id)
            {
                Id = id;
            }
            public int Id;
            public string StrParam;
        }
    }
}