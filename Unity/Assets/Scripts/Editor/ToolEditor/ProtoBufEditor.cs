﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
 using System.Text;
 using System.Text.RegularExpressions;
 using ET;
 using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
 using XGame;

 public class ProtoBufEditor : EditorWindow
{
    [MenuItem("Tools/ProtoBufEditor")]
    static void ShowWin()
    {
        GetWindow<ProtoBufEditor>().Show();
    }

    private static string _dirPath;
    
    private List<string> _baseTypes;

    private List<ProtoBufFileInfo> _files;
    private List<ProtoBodyInfo> _showList;
    private int _fileIndex;
    
    private Vector2 _scrollPos1;
    private Vector2 _scrollPos2;

    private ProtoBufFileInfo _selectInfo;
    private string _searchField;
    private List<ProtoBufFileInfo> _findedFiles = new List<ProtoBufFileInfo>();

    private string _newTableName;

    private bool _requireRefresh;
    

    private Dictionary<string, string[]> _msgType2Keys;

    private List<string> _protoMsgTypes = new List<string>();

    private void OnEnable()
    {
        this._protoMsgTypes.Add("None");
        this._protoMsgTypes.Add("IMessage");
        var types = typeof (IMessage).Module.GetTypes();
        foreach (Type type in types)
        {
            if (type.IsInterface)
            {
                var interfaces = type.GetInterfaces();
                if (interfaces.Contains(typeof (IMessage)))
                {
                    this._protoMsgTypes.Add(type.Name);
                }
            }
        }
        
        
        this._msgType2Keys = new Dictionary<string, string[]>();
        this._msgType2Keys[nameof (IActorResponse)]         = new[] { "int32|Error|91", "string|Message|92" };
        this._msgType2Keys[nameof (IResponse)]              = new[] { "int32|Error|91", "string|Message|92" };
        this._msgType2Keys[nameof (IActorLocationResponse)] = new[] { "int32|Error|91", "string|Message|92" };
        this._msgType2Keys[nameof (ICenterResponse)]        = new[] { "int32|Error|91", "string|Message|92" };
        
        this._baseTypes = new List<string>()
        {
            "int32", "int64", "sint32", "sint64", 
            "string", "float", "double", "uint32", "uint64",
            "Unity.Mathematics.float3", "Unity.Mathematics.quaternion"
        };
        _dirPath = $"../公共/Proto";
        this._files = new List<ProtoBufFileInfo>();
        foreach (var filePath in Directory.GetFiles(_dirPath))
        {
            if (filePath.EndsWith(".meta"))
                continue;
            this._files.Add(
                new ProtoBufFileInfo(Path.GetFileNameWithoutExtension(filePath), filePath));
        }

        
        foreach (ProtoBufFileInfo fileInfo in this._files)
        {
            foreach (ProtoBodyInfo info in fileInfo.ProtoBodyInfos)
            {
                CreateRecordList(info);
            }
        }
        this._selectInfo = this._files[0];

        this._showList = new List<ProtoBodyInfo>();
        this._showList.AddRange(this._selectInfo.ProtoBodyInfos);
    }

    private void CreateRecordList(ProtoBodyInfo info)
    {
        info.ReorderableList = new ReorderableList(info.Fields,
                    typeof(ProtoField), true, true, true, true);
        var fields = info.Fields;

        info.ReorderableList.drawElementCallback = (rect, i, isActive, isFocused) =>
        {
            GUI.Box(new Rect(rect.x, rect.y + 2, rect.width, rect.height - 4), "", "FrameBox");
            var field = fields[i];

            var curX = rect.x;
            var y = rect.y;
            field.Repeated = (StructType) EditorGUI.EnumPopup(new Rect(curX + 10,  y + rect.height / 2 - 10, 100, 20), field.Repeated);
            curX += 100;
            
            field.IsUnityType = EditorGUI.Toggle(new Rect(curX + 40, y, 60, rect.height), field.IsUnityType);
            curX += 100;
            
            if (field.IsUnityType)
            {
                field.Type = EditorGUI.TextField(new Rect(curX + 2, y + rect.height / 2 - 10, 150 - 2, 20), field.Type);
            }
            else
            {
                var list = this.GetTypes("None");
                list.AddRange(this._baseTypes);
                var fIndex = list.IndexOf(field.Type);
                if (field.Repeated == StructType.Map)
                {
                    var kIndex = list.IndexOf(field.MapKey);
                    var vIndex = list.IndexOf(field.MapVal);
                    EditorGUI.LabelField(new Rect(curX + 2, y + 9, 20, 20), "K");
                    EditorGUI.LabelField(new Rect(curX + 2, y + 31, 20, 20), "V");
                    kIndex = EditorGUI.Popup(new Rect(curX + 22, y + 9, 150 - 22, 20), kIndex, list.ToArray());
                    vIndex = EditorGUI.Popup(new Rect(curX + 22, y + 31, 150 - 22, 20), vIndex, list.ToArray());
                    field.MapKey = kIndex > -1? list[kIndex] : field.MapKey;
                    field.MapVal = vIndex > -1? list[vIndex] : field.MapVal;
                    field.Type = $"map<{field.MapKey},{field.MapVal}>";
                }
                else
                {
                    fIndex = EditorGUI.Popup(new Rect(curX + 2, y + rect.height / 2 - 10, 150 - 2, 20), fIndex, list.ToArray());
                    field.Type = fIndex > -1? list[fIndex] : field.Type;
                }
            }
            curX += 150;

            field.Name = EditorGUI.TextField(new Rect(curX + 2, y + rect.height / 2 - 10, 100 - 2, 20), field.Name);
            curX += 100;
            
            field.Code = EditorGUI.IntField(new Rect(curX + 2, y + rect.height / 2 - 10, 100 - 2, 20), field.Code);
            curX += 100;
            var style = new GUIStyle("ScriptText") { wordWrap = true };
            field.Note = EditorGUI.TextField(new Rect(curX + 2, y + 8, rect.width - curX - 2, rect.height - 16), field.Note, style);
        };
        info.ReorderableList.drawHeaderCallback = rect =>
        {
            var curX = rect.x;
            var style = new GUIStyle(){alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 14, richText = true};
            EditorGUI.LabelField(new Rect(curX, rect.y, 100, 20), "<color=white>Repeated</color>", style);
            curX += 100;
            EditorGUI.LabelField(new Rect(curX, rect.y, 100, 20), "<color=white>UnityStruct</color>", style);
            curX += 100;
            EditorGUI.LabelField(new Rect(curX, rect.y, 150, 20), "<color=white>Type</color>", style);
            curX += 150;
            EditorGUI.LabelField(new Rect(curX, rect.y, 100, 20), "<color=white>Name</color>", style);
            curX += 100;
            EditorGUI.LabelField(new Rect(curX, rect.y, 100, 20), "<color=white>Code</color>", style);
            curX += 100;
            EditorGUI.LabelField(new Rect(curX, rect.y, rect.width - curX, 20), "<color=white>注释</color>", style);
        };
     
        info.ReorderableList.elementHeight = 60;
    }

    private void DoSearch()
    {
        this._showList.Clear();
        this._findedFiles.Clear();
        if (!string.IsNullOrEmpty(this._searchField))
        {
            foreach (ProtoBufFileInfo file in this._files)
            {
                bool finded = false;
                foreach (var bodyInfo in file.ProtoBodyInfos)
                {
                    if (bodyInfo.RealName.ToLower().Contains(this._searchField.ToLower()))
                    {
                        this._showList.Add(bodyInfo);
                        finded = true;
                    }
                }
                if (finded)
                    this._findedFiles.Add(file);
            }

            if (this._findedFiles.IndexOf(this._selectInfo) == -1 && this._findedFiles.Count > 0)
            {
                this._selectInfo = this._findedFiles[0];
            }
        }
        else
        {
            var bodys = this._selectInfo.ProtoBodyInfos;
            this._showList.AddRange(bodys);
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, 200, this.position.height));
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("保存全部", "Tab onlyOne"))
        {
            foreach (var file in this._files)
            {
                file.Save();
            }
            this.ShowNotification(new GUIContent("保存成功"));
        }

        if (GUILayout.Button("编译全部", "Tab onlyOne"))
        {
            foreach (var file in this._files)
            {
                file.Save();
            }
            ToolsEditor.Proto2CS();
            this.ShowNotification(new GUIContent("编译成功"));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        this._newTableName = EditorGUILayout.TextField(this._newTableName);
        if (GUILayout.Button("新建"))
        {
            var filePath = $"{_dirPath}/{this._newTableName}.proto";
            File.Create(filePath).Dispose();
            File.WriteAllLines(filePath, new []
            {
                "//备注",
                "syntax = \"proto3\";",
                "package ET;",
                ""
            });
            this._files.Add(
                new ProtoBufFileInfo(Path.GetFileNameWithoutExtension(filePath), filePath));
        }
        EditorGUILayout.EndHorizontal();
        this._scrollPos1 = EditorGUILayout.BeginScrollView(this._scrollPos1, "box");
        for (int i = 0; i < this._files.Count; i++)
        {
            ProtoBufFileInfo info = this._files[i];
            GUI.backgroundColor = this._selectInfo == info? Color.gray : Color.white;
            if (GUILayout.Button(info.FileName, "Tab onlyOne", GUILayout.Width(195), GUILayout.Height(40)))
            {
                this._selectInfo = info;
                this._fileIndex = i;
                this._requireRefresh = true;
            }

            GUI.backgroundColor = Color.white;
            
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
        
        GUILayout.BeginArea(new Rect(205, 0, this.position.width - 205, 80), "","box");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("保存", "Tab onlyOne", GUILayout.Width(80), GUILayout.Height(20)))
        {
            this._selectInfo.Save();
            this.ShowNotification(new GUIContent("保存成功"));
        }

        if (GUILayout.Button("新建", "Tab onlyOne", GUILayout.Width(80), GUILayout.Height(20)))
        {
            var info = new ProtoBodyInfo(this._selectInfo)
            {
                Name = $"__NewMessage{this._selectInfo.ProtoBodyInfos.Count}",
            };
            info.Fields.Add(new ProtoField()
            {
                Code = 90,
                Name = "RpcId",
                Type = "int32"
            });
            this._selectInfo.ProtoBodyInfos.Add(info);
            CreateRecordList(info);
            info.Foldout = true;
            this._scrollPos2.y = float.MaxValue;
            this._requireRefresh = true;
        }
        
        if (GUILayout.Button("编译", "Tab onlyOne", GUILayout.Width(80), GUILayout.Height(20)))
        {
            this._selectInfo.Save();
            ToolsEditor.Proto2CS();
            this.ShowNotification(new GUIContent("编译成功"));
        }
        
        if (GUILayout.Button("...", "Tab onlyOne", GUILayout.Width(40), GUILayout.Height(20)))
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("重置"), false, () =>
            {
                if (EditorUtility.DisplayDialog("提示", "重置将丢失已编辑的数据，是否重置?", "是", "取消"))
                {
                    for (int i = 0; i < this._files.Count; i++)
                    {
                        if (this._selectInfo == this._files[i])
                        {
                            this._files[i] = new ProtoBufFileInfo(this._selectInfo.FileName, this._selectInfo.FilePath);
                            this._selectInfo = this._files[i];
                            break;
                        }
                    }          
                }
            });
            menu.AddItem(new GUIContent("打开"), false, () =>
            {
                System.Diagnostics.Process.Start("notepad++.exe", this._selectInfo.FilePath);           
            });
            menu.ShowAsContext();
        }
        
        EditorGUI.BeginChangeCheck();
        this._searchField = EditorGUILayout.TextField(this._searchField, new GUIStyle("SearchTextField"));
        if (EditorGUI.EndChangeCheck())
        {
            this._requireRefresh = true;
        }

        if (!string.IsNullOrEmpty(this._searchField))
        {
            if (GUILayout.Button("X", "Tab onlyOne", GUILayout.Width(40), GUILayout.Height(20)))
            {
                this._searchField = "";
                this._requireRefresh = true;
            }
            var fileIndex = this._findedFiles.IndexOf(this._selectInfo);
            fileIndex = fileIndex == -1? 0 : fileIndex;
            if (GUILayout.Button("←", "Tab onlyOne", GUILayout.Width(40), GUILayout.Height(20)))
            {
                if (fileIndex - 1 > 0)
                {
                    this._selectInfo = this._findedFiles[fileIndex - 1];
                }
            }
            
            GUILayout.Label($"{fileIndex + 1}/{this._findedFiles.Count}", "CenteredLabel", GUILayout.Width(50));
            
            if (GUILayout.Button("→", "Tab onlyOne", GUILayout.Width(40), GUILayout.Height(20)))
            {
                if (fileIndex + 1 < this._findedFiles.Count)
                {
                    this._selectInfo = this._findedFiles[fileIndex + 1];
                }
            }
        }
        
        
        GUILayout.EndHorizontal();
        var boxStyle = new GUIStyle("ScriptText");
        boxStyle.wordWrap = true;
        this._selectInfo.Note = EditorGUILayout.TextField("描述", this._selectInfo.Note, boxStyle);
        GUILayout.EndArea();
        
        GUILayout.BeginArea(new Rect(205, 85, this.position.width - 205, this.position.height - 85));
        this._scrollPos2 = EditorGUILayout.BeginScrollView(this._scrollPos2);
        foreach (ProtoBodyInfo info in this._showList)
        {
            if (info.Parent != this._selectInfo)
                continue;
            var style = new GUIStyle("HeaderButton") { alignment = TextAnchor.MiddleLeft};
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = info.Removed ? Color.red : Color.white;
            if (GUILayout.Button(new GUIContent($"{(info.Foldout ? '▼' : '▶')} {info.RealName}", info.Note), style, GUILayout.Height(25)))
            {
                info.Foldout = !info.Foldout;
                if (info.Foldout)
                {
                    var list = this.GetTypes("None");
                    list.AddRange(this._baseTypes);
                    foreach (var field in info.Fields)
                    {
                        var fIndex = list.IndexOf(field.Type);
                        field.IsUnityType = fIndex == -1;
                    }
                }
            }

            GUI.backgroundColor = Color.white;
            if (GUILayout.Button(new GUIContent("M", "迁移"), "HeaderButton", GUILayout.Width(25), GUILayout.Height(25)))
            {
                var menu = new GenericMenu();
                foreach (var file in this._files)
                {
                    menu.AddItem(new GUIContent(file.FileName), false, () =>
                    {
                        this._selectInfo.ProtoBodyInfos.Remove(info);
                        file.ProtoBodyInfos.Add(info);
                        info.Parent = file;
                    });
                }
                menu.ShowAsContext();
            }
            if (GUILayout.Button(new GUIContent("C", "克隆"), "HeaderButton", GUILayout.Width(25), GUILayout.Height(25)))
            {
                var json = JsonUtility.ToJson(info);
                var obj = JsonUtility.FromJson<ProtoBodyInfo>(json);
                CreateRecordList(obj);
                this._selectInfo.ProtoBodyInfos.Add(obj);
            }
            
            var content = new GUIContent(info.Removed? "←" : "X", info.Removed? "撤销删除" : "删除");
            if (GUILayout.Button(content, "HeaderButton", GUILayout.Width(25), GUILayout.Height(25)))
            {
                info.Removed = !info.Removed;
            }
            
            EditorGUILayout.EndHorizontal();
            if (info.Foldout)
            {
                EditorGUILayout.BeginVertical("U2D.createRect");
                
                info.Name = EditorGUILayout.TextField("Name", info.Name);
                EditorGUI.BeginChangeCheck();
                var typeIndex = this._protoMsgTypes.IndexOf(info.MessageType);
                typeIndex = EditorGUILayout.Popup("Type", typeIndex, this._protoMsgTypes.ToArray());
                if (typeIndex == -1)
                {
                    info.MessageType = "None";
                }
                if (EditorGUI.EndChangeCheck())
                {
                    info.MessageType = this._protoMsgTypes[typeIndex];
                    if (this._msgType2Keys.TryGetValue(info.MessageType, out var fields))
                    {
                        foreach (var field in fields)
                        {
                            var ss = field.Split('|');
                            if (info.Fields.Find(a => a.Name == ss[1]) == null)
                            {
                                info.Fields.Add(new ProtoField()
                                {
                                    Code = Convert.ToInt32(ss[2]),
                                    Name = ss[1],
                                    Type = ss[0]
                                });
                            }
                            
                        }
                    }
                }
                
                if (info.MessageType.EndsWith("Request"))
                {
                    string newResponseType = info.MessageType.Replace("Request", "Response");
                    List<string> list = this.GetTypes(newResponseType);
                    var index = list.IndexOf(info.ResponseName);

                    EditorGUILayout.BeginHorizontal();
                    var predictedName = info.RealName.Replace("Request", "Response");
                    if (string.IsNullOrEmpty(info.ResponseName))
                        index = list.IndexOf(predictedName);
                    
                    index = EditorGUILayout.Popup("ResponseType", index, list.ToArray());
                    if (list.IndexOf(predictedName) > -1)
                    {
                        if (GUILayout.Button($"Select [{predictedName}]"))
                        {
                            index = list.IndexOf(predictedName);
                        }
                    }
                    else if (info.ResponseName != predictedName)
                    {
                        if (GUILayout.Button($"Create [{predictedName}] in"))
                        {
                            var msgType = newResponseType;
                            this._files[this._fileIndex].ProtoBodyInfos.Add(new ProtoBodyInfo(this._selectInfo)
                            {
                                MessageType = msgType,
                                Name = info.Name,
                                Fields = GetDefaultFields(msgType)
                            });
                        }
                        this._fileIndex = EditorGUILayout.Popup(this._fileIndex, this._files.ToStringArray());
                    }else if (info.ResponseName == predictedName && index == -1)
                    {
                        if (GUILayout.Button($"Change [{predictedName}] Type to [{newResponseType}]"))
                        {
                            var bodyInfo = this.Find(predictedName);
                            bodyInfo.MessageType = newResponseType;
                        }
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    if (index >= 0)
                    {
                        info.ResponseName = list[index];
                    }
                }

                #region 注释

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("多行注释");
                if (info.Notes != null)
                {
                    info.MutiNotes.AddRange(info.Notes);
                    info.Notes = null;
                }

                if (info.MutiNotes != null)
                {
                    string text = "";
                    for (int i = 0; i < info.MutiNotes.Count; i++)
                    {
                        text += info.MutiNotes[i];
                        text += "\n";
                    }

                    text = text.Trim('\n');
                    var newText = EditorGUILayout.TextArea(text);
                    if (text != newText)
                    {
                        var ss = newText.Split('\n');
                        info.MutiNotes.Clear();
                        foreach (string s in ss)
                        {
                            info.MutiNotes.Add(s);
                        }
                    }
                }

                EditorGUILayout.EndVertical();

                #endregion

                #region 属性

                EditorGUILayout.BeginVertical("box");
                if (info.ReorderableList == null)
                    CreateRecordList(info);
                info.ReorderableList.DoLayoutList();
                EditorGUILayout.EndVertical();

                #endregion

                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
        
        if (this._requireRefresh)
            this.DoSearch();
    }

    public List<string> GetTypes(params string[] types)
    {
        var res = new List<string>();
        foreach (ProtoBufFileInfo fileInfo in this._files)
        {
            foreach (var customType in fileInfo.ProtoBodyInfos)
            {
                foreach (string type in types)
                {
                    if (customType.MessageType == type)
                    {
                        res.Add(customType.RealName);
                        break;
                    }
                }
            }
        }
            
        return res;
    }

    public ProtoBodyInfo Find(string realName)
    {
        foreach (ProtoBufFileInfo fileInfo in this._files)
        {
            foreach (var customType in fileInfo.ProtoBodyInfos)
            {
                if (customType.RealName == realName)
                {
                    return customType;
                }
            }
        }

        return null;
    }

    private List<ProtoField> GetDefaultFields(string type)
    {
        var res = new List<ProtoField>()
        {
            new ProtoField(){Code = 90, Name = "RpcId", Type = "int32"}
        };
        foreach (var field in this._msgType2Keys[type])
        {
            var ss = field.Split('|');
            res.Add(new ProtoField()
            {
                Code = Convert.ToInt32(ss[2]),
                Name = ss[1],
                Type = ss[0]
            });
        }

        return res;
    }

    public class ProtoBufFileInfo
    {
        public string FileName;
        public string FilePath;
        
        private static string[] HEADER = new[] {"syntax = \"proto3\";", "package ET;"};
        
        private List<ProtoBodyInfo> _customTypes;
   
        public List<ProtoBodyInfo> ProtoBodyInfos;
        public string Note;
        private State _curState;
        private bool _isReadBody;
        private bool _isMainBody;
        private ProtoBodyInfo _curBody;
        private long _length;

        public List<string> GetTypes(params string[] types)
        {
            var res = new List<string>();
            foreach (var customType in this._customTypes)
            {
                foreach (string type in types)
                {
                    if (customType.MessageType == type)
                    {
                        res.Add(customType.RealName);
                        break;
                    }
                }
            }
            
            return res;
        }
        
        public ProtoBufFileInfo(string name, string filePath)
        {
            this.ProtoBodyInfos = new List<ProtoBodyInfo>();
            this._customTypes = new List<ProtoBodyInfo>();

            this.FileName = name;
            this.FilePath = filePath;
            Parse(filePath);
            
            this.ProtoBodyInfos.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
        }

        private void Parse(string filePath)
        {
            this._length = File.ReadAllBytes(filePath).LongLength;
            var lines = File.ReadLines(filePath).ToArray();
            var index = 0;
            var notes = new Dictionary<int, string>();
            for (int j = 0; j < lines.Length; j++)
            {
                var line = lines[j].Trim();
                if (index == 0 && line.StartsWith("//")) //第1行固定为注释行
                {
                    this.Note = line.TrimStartString("//");
                    continue;
                }
                if (index < 3)
                {
                    index++;
                    continue;
                }

                if (!this._isReadBody)
                {
                    if (BeginRead(line))
                    {
                        this._isReadBody = true;
                        this._curBody = new ProtoBodyInfo(this);
                        this.ProtoBodyInfos.Add(this._curBody);
                    }
                }

                if (this._isReadBody)
                {
                    if (this._curState == State.ReadNote)
                    {
                        if (Regex.IsMatch(line, @"/// +<summary>"))
                        {
                        }else if (Regex.IsMatch(line, @"/// +</summary>"))
                            this._curState = State.None;
                        else
                            this._curBody.MutiNotes.Add(line.Replace("///", "").Trim());
                    }
                    else
                    {
                        if (line.StartsWith("//"))
                        {
                            if (line.StartsWith("//ResponseType"))
                                this._curBody.ResponseName = line.Replace("//ResponseType", "").Trim();
                            else 
                            {
                                if (!this._isMainBody)
                                    this._curBody.Notes.Add(line.Replace("//", "").Trim());
                                notes.Add(j, line.Replace("//", "").Trim());
                            }
                        } else if (line.StartsWith("message"))
                        {
                            var ss = line.Replace("message", "").Split(new []{"//"}, StringSplitOptions.RemoveEmptyEntries);
                            if (!this._customTypes.Contains(this._curBody))
                                this._customTypes.Add(this._curBody);
                            if (ss.Length == 1)
                                this._curBody.MessageType = "None";
                            else
                            {
                                this._curBody.MessageType = ss[1].Trim();
                            }

                            var realName = ss[0].Trim();
                            var nameResult = realName;
                            if (this._curBody.MessageType == "None")
                            {
                            }
                            else if (this._curBody.MessageType.EndsWith("Request"))
                            {
                                nameResult = nameResult.Replace("ALRequest", "");
                                nameResult = nameResult.Replace("ARequest", "");
                                nameResult = nameResult.Replace("Request", "");
                            }
                            else if (this._curBody.MessageType.EndsWith("Response"))
                            {
                                nameResult = nameResult.Replace("ALResponse", "");
                                nameResult = nameResult.Replace("AResponse", "");
                                nameResult = nameResult.Replace("Response", "");
                            }
                            else if (this._curBody.MessageType.EndsWith("Message"))
                            {
                                nameResult = nameResult.Replace("ALMessage", "");
                                nameResult = nameResult.Replace("AMessage", "");
                                nameResult = nameResult.Replace("Message", "");
                            }
                            this._curBody.Name = nameResult;
                        }else if (line == "{")
                        {
                            this._isMainBody = true;
                        }else if (line == "}")
                        {
                            this._isMainBody = false;
                            this._isReadBody = false;
                            
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(line)) continue;
                            var field = new ProtoField();
                            this._curBody.Fields.Add(field);

                            if (line.Contains("repeated"))
                            {
                                field.Repeated = StructType.Array;
                            }
                            else if (Regex.IsMatch(line, @"map<(.*),(.*)>"))
                            {
                                field.Repeated = StructType.Map;
                                var match = Regex.Match(line, @"map<(.*),(.*)>");
                                field.MapKey = match.Groups[1].Value.Trim();
                                field.MapVal = match.Groups[2].Value.Trim();
                            }
                            line = line.Replace("repeated", "").Replace(";", "").Trim();

                            var note = Regex.Match(line, @"//.*");
                            if (note.Success)
                            {
                                field.Note = note.Value.Replace("//", "").Trim();
                                line = line.Replace(note.Value, "").Trim();
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(field.Note))
                                {
                                    if (notes.ContainsKey(j - 1))
                                        field.Note = notes[j - 1];
                                }
                            }
                            
                            var ss = line.Split(' ');
                            var arr = new string[4];
                            var arri = 0;
                            for (int i = 0; i < ss.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(ss[i]))
                                    arr[arri++] = ss[i];
                            }
                            field.Type = arr[0];
                            field.Name = arr[1];
                            field.Code = Convert.ToInt32(arr[3]);
                        }
                    }
                }
                
            }
        }

        public void Save()
        {
            var filePath = $"{_dirPath}/{this.FileName}.proto";
            var lines = this.ToLines();
            File.WriteAllLines(filePath, lines);
        
        }

        private List<string> ToLines()
        {
            List<string> lines = new List<string>();
            if (!string.IsNullOrEmpty(this.Note))
                lines.Add(this.Note);
            lines.AddRange(HEADER);
            lines.Add("");
            foreach (ProtoBodyInfo protoBodyInfo in this.ProtoBodyInfos)
            {
                if (protoBodyInfo.Removed)
                    continue;
                if (protoBodyInfo.MutiNotes.Count > 0)
                {
                    lines.Add("/// <summary>");
                    foreach (string mutiNote in protoBodyInfo.MutiNotes)
                    {
                        lines.Add($"/// {mutiNote}");
                    }
                    lines.Add("/// </summary>");
                }

                if (!string.IsNullOrEmpty(protoBodyInfo.ResponseName))
                    lines.Add($"//ResponseType {protoBodyInfo.ResponseName}");
                lines.Add($"message {protoBodyInfo.RealName}{(protoBodyInfo.MessageType == "None" ? "" : $" // {protoBodyInfo.MessageType.ToString()}")}");
                lines.Add("{");
                foreach (ProtoField protoField in protoBodyInfo.Fields)
                {
                    var type = protoField.Type;
                    switch (protoField.Repeated)
                    {
                        case StructType.Array:
                            type = $"repeated {protoField.Type}";
                            break;
                        case StructType.Map:
                            type = $"map<{protoField.MapKey},{protoField.MapVal}>";
                            break;
                    }

                    lines.Add(
                        $"  {type} {protoField.Name} = {protoField.Code}; {(string.IsNullOrEmpty(protoField.Note) ? "" : $"// {protoField.Note}")}");
                }
                lines.Add("}");
                lines.Add("");
            }

            return lines;
        }

        private bool BeginRead(string line)
        {
            line = line.Trim();
            if (Regex.IsMatch(line, @"/// +<summary>"))
            {
                this._curState = State.ReadNote;
                return true;
            }

            if (line.StartsWith("//"))
                return true;

            if (line.StartsWith("message"))
                return true;
            return false;
        }

        public override string ToString()
        {
            return this.FileName;
        }

        private enum State
        {
            None,
            ReadNote,
        }

    }
    
    [Serializable]
    public class ProtoBodyInfo
    {
        public ProtoBufFileInfo Parent;

        public ProtoBodyInfo(ProtoBufFileInfo p)
        {
            this.Parent = p;
        }
        
        public string Note
        {
            get
            {
                var sb = new StringBuilder();
                if (this.Notes?.Count > 0)
                {
                    foreach (string note in this.Notes)
                    {
                        sb.AppendLine(note);
                    }
                }else if (this.MutiNotes?.Count > 0)
                {
                    foreach (string mutiNote in this.MutiNotes)
                    {
                        sb.AppendLine(mutiNote);
                    }
                }

                return sb.ToString();
            }
        }

        public string RealName
        {
            get
            {
                if (this.MessageType == "None")
                {
                    return this.Name;
                }

                var baseType = typeof (IMessage);
                var type = baseType.Module.GetType($"ET.{this.MessageType}");
                var interfaces = type.GetInterfaces();
                if (this.MessageType.EndsWith("Request"))
                {
                    if (type == typeof (IActorLocationRequest) || interfaces.Contains(typeof (IActorLocationRequest)))
                    {
                        return $"{this.Name}ALRequest";
                    }

                    if (type == typeof (IActorRequest) || interfaces.Contains(typeof (IActorRequest)))
                    {
                        return $"{this.Name}ARequest";
                    }

                    return $"{this.Name}Request";
                }

                if (this.MessageType.EndsWith("Response"))
                {
                    if (type == typeof (IActorLocationResponse) || interfaces.Contains(typeof (IActorLocationResponse)))
                    {
                        return $"{this.Name}ALResponse";
                    }

                    if (type == typeof (IActorResponse) || interfaces.Contains(typeof (IActorResponse)))
                    {
                        return $"{this.Name}AResponse";
                    }

                    return $"{this.Name}Response";
                }

                if (this.MessageType.EndsWith("Message"))
                {
                    if (type == typeof (IActorLocationMessage) || interfaces.Contains(typeof (IActorLocationMessage)))
                    {
                        return $"{this.Name}ALMessage";
                    }

                    if (type == typeof (IActorMessage) || interfaces.Contains(typeof (IActorMessage)))
                    {
                        return $"{this.Name}AMessage";
                    }

                    return $"{this.Name}Message";
                }

                return this.Name;
            }
        }
        
        public bool Foldout;
        public bool Removed;
        public ReorderableList ReorderableList;
        
        
        public string Name;
        public List<string> Notes = new List<string>();
        public List<string> MutiNotes = new List<string>();
        public List<ProtoField> Fields = new List<ProtoField>();
        public string MessageType = "None";
        public string ResponseName;
    }
    
    [Serializable]
    public class ProtoField
    {
        public string Type;
        public string MapKey;
        public string MapVal;
        public string Name;
        public int Code;
        public string Note;
        public StructType Repeated;
        public bool IsUnityType;
    }

    // public enum ProtoMsgType
    // {
    //     None,
    //     IActorRequest,
    //     IActorResponse,
    //     IActorMessage,
    //     IActorLocationRequest,
    //     IActorLocationResponse,
    //     IActorLocationMessage,
    //     IRequest,
    //     IResponse,
    //     IMessage,
    // }
    
    public enum StructType
    {
        None,
        Array,
        Map
    }
}
