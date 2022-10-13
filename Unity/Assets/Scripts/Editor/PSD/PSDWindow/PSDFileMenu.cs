using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Hook;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace XGame
{
    public class PSDFileMenu : EditorWindow
    {
        public static void Refresh()
        {
            GetWindow<PSDFileMenu>().RefreshList();
        }
        
        private EditorTreeView<TreeElement> _fileTree;

        private List<TreeElement> _files;
        private string _searchStr;
        private void OnEnable()
        {
            Manager.StartHook();
            
            titleContent = new GUIContent("文件");
            
            _fileTree = new EditorTreeView<TreeElement>(PSDUtility.PSDSetting.FileMenuState);
            _fileTree.OnDrawRowCallback += OnDrawRowCallback;
            _fileTree.OnSingleClickedItem += OnSingleClickedItem;
            _fileTree.OnContextClickedItem += OnContextClickedItem;
            _fileTree.OnDoubleClickItem += OnDoubleClickItem;
            
            _files = new List<TreeElement>();
            RefreshList();
        }

        private void OnDisable()
        {
            Manager.StopHook();
        }

        private void OnDoubleClickItem(int obj)
        {
            var item = _fileTree.Data.Find(obj);
            if (item is FileLayer fileLayer)
            {
                OpenPSD(fileLayer);
            }else if (item is FolderLayer folderLayer)
            {
                if (folderLayer.QuickMenu)
                {
                    _fileTree.SetSelection(new []{folderLayer.FolderPath.GetHashCode()});
                    _fileTree.FrameItem(folderLayer.FolderPath.GetHashCode());
                }
                else
                {
                    _fileTree.SetExpanded(obj, true);
                }
            }
        }

        private void OnContextClickedItem(int obj)
        {
            var item = _fileTree.Data.Find(obj);
            var menu = new GenericMenu();
            if (item is FileLayer fileLayer)
            {
                menu.AddItem(new GUIContent("编辑PSD"), false, () =>
                {
                    OpenPSD(fileLayer);
                });
                menu.AddItem(new GUIContent("重新打开"), false, () =>
                {
                    PSDOperator.Open(fileLayer.FilePath, true); 
                });
                menu.AddItem(new GUIContent("生成"), false, () =>
                {
                    if (!OpenNewScene())
                        return;
                    var info = PSDOperator.Instance.GetInfo(fileLayer.FilePath);
                    if (info == null)
                    {
                        using var psdInfo = new PSDInfo(fileLayer.FilePath, false);
                        PSDParse.Parse(psdInfo, PSDUtility.PSDSetting.ScreenSize);
                    }
                    else
                    {
                        PSDParse.Parse(info, PSDUtility.PSDSetting.ScreenSize);
                    }
                });
                menu.AddItem(new GUIContent("打开文件"), false, () =>
                {
                    CMDHelper.RunCmd($"explorer file:///{fileLayer.FilePath}");
                });
                menu.AddItem(new GUIContent("打开文件所在文件夹"), false, () =>
                {
                    System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(fileLayer.FilePath));
                });
                
            }else if (item is FolderLayer folderLayer)
            {
                if (!folderLayer.QuickMenu)
                {
                    menu.AddItem(new GUIContent("打开文件夹"), false, () =>
                    {
                        var folderInfo = new DirectoryInfo(folderLayer.FolderPath);
                        System.Diagnostics.Process.Start("explorer.exe", folderInfo.FullName);
                    });

                    if (PSDUtility.PSDSetting.LockedFolder.Contains(folderLayer.FolderPath))
                    {
                        menu.AddDisabledItem(new GUIContent("置顶"));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("置顶"), false, () =>
                        {
                            PSDUtility.PSDSetting.LockedFolder.Add(folderLayer.FolderPath);
                            var ele = _fileTree.Data.Find(1);
                            _fileTree.Data.AddElement(
                                new FolderLayer(){
                                FolderPath = folderLayer.FolderPath,
                                Depth = ele.Depth + 1,
                                Name = folderLayer.Name.Trim('\\'),
                                QuickMenu = true}, ele, 0);
                            _fileTree.Reload();
                            PSDUtility.SaveSetting();
                        });
                    }
                }
                else
                {
                    menu.AddItem(new GUIContent("取消置顶"), false, () =>
                    {
                        PSDUtility.PSDSetting.LockedFolder.Remove(folderLayer.FolderPath);
                        _fileTree.Data.RemoveElements(new[] {folderLayer.Id});
                        _fileTree.Reload();
                        PSDUtility.SaveSetting();
                    });
                }
            }
            menu.ShowAsContext();
        }

        private void OnSingleClickedItem(int obj)
        {
            var item = _fileTree.Data.Find(obj);
            if (item is FileLayer fileLayer)
            {
       
            }else if (item is FolderLayer folderLayer)
            {
          
            }
        }

        private void OnDrawRowCallback(TreeViewItemRow<TreeElement> obj)
        {
            var indent = obj.GetContentIndent.Invoke(obj.item);
            var cellRect = obj.rowRect;
            cellRect.x += indent;
            var item = obj.item.Data;
            
            var iconRect = cellRect;
            
            switch (item)
            {
                case FolderLayer folderLayer:
                    EditorGUI.LabelField(iconRect, EditorGUIUtility.IconContent("Folder Icon"));
                    cellRect.x += 20;
                    
                    EditorGUI.LabelField(cellRect, $"{folderLayer.Name}");
                    break;
                case FileLayer fileLayer:
                    EditorGUI.LabelField(iconRect, EditorGUIUtility.IconContent("RawImage Icon"));
                    cellRect.x += 20;
                    
                    EditorGUI.LabelField(cellRect, $"{fileLayer.Name}");
                    break;
                default:
                    EditorGUI.LabelField(iconRect, EditorGUIUtility.IconContent("Folder Icon"));
                    cellRect.x += 20;
                    
                    EditorGUI.LabelField(cellRect, $"{item.Name}");
                    break;
            }
        }

        private void OpenPSD(FileLayer fileLayer)
        {
            PSDOperator.Open(fileLayer.FilePath);
            if (!fileLayer.QuickMenu)
            {
                var ele = _fileTree.Data.Find(2);
                if (!ele.HasChildren || !ele.Children.Exists(a => ((FileLayer) a).FilePath == fileLayer.FilePath))
                {
                    var list = PSDUtility.PSDSetting.LatesdFile;
                    list.Insert(0, fileLayer.FilePath);
                    if (list.Count > 5)
                    {
                        list.RemoveRange(5, list.Count - 5);
                    }
                    
                    if (ele.HasChildren)
                        _fileTree.Data.RemoveElements(ele.Children);
                    foreach (var file in list)
                    {
                        _fileTree.Data.AddElement(new FileLayer()
                        {
                            Depth = ele.Depth + 1, FilePath = file,
                            Name = Path.GetFileNameWithoutExtension(file), QuickMenu = true
                        }, ele, 0);
                    }
                    _fileTree.Reload();
                    PSDUtility.SaveSetting();
                }
            }
            
        }

        private void RefreshList()
        {
            _files.Clear();
            _files.Add(new TreeElement(){Depth = -1, Name = "Root"});

            
            _files.Add(new TreeElement(){Depth = 0, Name = "置顶文件夹", Id = 1});
            
            foreach (var folderPath in PSDUtility.PSDSetting.LockedFolder)
            {
                _files.Add(new FolderLayer(){
                    FolderPath = folderPath,
                    Depth = 1,
                    Name = folderPath.Replace(PSDUtility.PSDSetting.PsdFolder, ""),
                    Id = folderPath.GetHashCode(),
                    QuickMenu = true
                });
            }
            
            _files.Add(new TreeElement(){Depth = 0, Name = "最近打开", Id = 2});

            foreach (var file in PSDUtility.PSDSetting.LatesdFile)
            {
                _files.Add(new FileLayer(){
                    FilePath = file,
                    Depth = 1,
                    Name = Path.GetFileNameWithoutExtension(file),
                    Id = file.GetHashCode(),
                    QuickMenu = true
                });
            }
            
            var folder = new DirectoryInfo(PSDUtility.PSDSetting.PsdFolder);
            if (folder.Exists)
            {
                InitTreeList(folder, 0);
            }
            _fileTree.Build(new TreeModel<TreeElement>(_files));
            _fileTree.Reload();
        }

        private void InitTreeList(DirectoryInfo folder, int depth)
        {
            foreach (var fileInfo in folder.GetFiles("*.psd"))
            {
                _files.Add(new FileLayer(){FilePath = fileInfo.FullName, Depth = depth, Name = fileInfo.Name});                    
            }

            foreach (var directoryInfo in folder.GetDirectories())
            {
                _files.Add(new FolderLayer(){FolderPath = directoryInfo.FullName, Depth = depth, Name = directoryInfo.Name});
                InitTreeList(directoryInfo, depth + 1);
            }
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(EditorGUIUtility.IconContent("d__Popup@2x"), "TE toolbarbutton", GUILayout.Width(30), GUILayout.Height(30)))
            {
                PSDPerference.Show(new Rect(25, 5, Mathf.Max(position.width * 0.9f, 300f), Mathf.Max(500, position.height * 0.5f)));
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), "TE toolbarbutton", GUILayout.Width(30), GUILayout.Height(30)))
            {
                RefreshList();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            _searchStr = EditorGUILayout.DelayedTextField(_searchStr, "", "SearchTextField");
            if (EditorGUI.EndChangeCheck())
            {
                
            }
            
            _fileTree.OnGUI(new Rect(0, 55, position.width, position.height - 60));
            
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
        
        private class FolderLayer : TreeElement
        {
            public string FolderPath;
            public bool QuickMenu;

            public override int Id
            {
                get
                {
                    if (QuickMenu)
                    {
                        return $"{FolderPath}-Quick".GetHashCode();
                    }
                    return FolderPath.GetHashCode();
                }
            }
        }
        
        private class FileLayer : TreeElement
        {
            public override int Id
            {
                get
                {
                    if (QuickMenu)
                    {
                        return $"{FilePath}-Quick".GetHashCode();
                    }
                    return FilePath.GetHashCode();
                }
            }

            public string FilePath;
            public bool QuickMenu;
        }
    }
}