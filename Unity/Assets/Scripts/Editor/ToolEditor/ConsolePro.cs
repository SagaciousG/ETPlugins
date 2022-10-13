// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using System.Text;
// using UnityEditor;
// using UnityEditor.UIElements;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace XGame
// {
//     public class ConsolePro : EditorWindow
//     {
//         [MenuItem("Tools/ConsolePro")]
//         static void ShowWin()
//         {
//             GetWindow<ConsolePro>().Show();
//         }
//         
//
//         // private MethodInfo _logEntries_SetConsoleFlag;
//         private MethodInfo _logEntries_GetCountsByType;
//         private MethodInfo _logEntries_StartGettingEntries;
//         private MethodInfo _logEntries_EndGettingEntries;
//         private MethodInfo _logEntries_GetEntryInternal;
//         private MethodInfo _logEntries_GetLinesAndModeFromEntryInternal;
//         private MethodInfo _logEntries_Clear;
//         private MethodInfo _logEntries_GetEntryCount;
//         private PropertyInfo _logEntries_ConsoleFlags;
//         private string _searchStr;
//         
//         private Type _logEntryType;
//         private FieldInfo _logEntry_Message;
//         private FieldInfo _logEntry_CallstackTextStartUTF16;
//         private FieldInfo _logEntry_Mode;
//
//         private Rect logsView;
//         private Rect descView;
//         private Vector2 logsViewPos;
//         private Vector2 descViewPos;
//
//         private ConsoleLogInfo _selectedInfo;
//         private int _selectedRow;
//         private float _descHeight;
//
//         private float _viewSplit = 0.65f;
//         private bool _draging;
//         private Vector2 _beginPos;
//         private float _beginPer;
//         
//         private List<ConsoleLogInfo> _logs = new List<ConsoleLogInfo>();
//
//         private ConsoleFlags flags;
//
//         private void OnEnable()
//         {
//             Assembly unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
//             Type logEntriesType = unityEditorAssembly.GetType("UnityEditor.LogEntries");
//             // this._logEntries_SetConsoleFlag = logEntriesType.GetMethod("SetConsoleFlag", BindingFlags.Static | BindingFlags.Public);
//             this._logEntries_GetCountsByType = logEntriesType.GetMethod("GetCountsByType", BindingFlags.Static | BindingFlags.Public);
//             this._logEntries_StartGettingEntries = logEntriesType.GetMethod("StartGettingEntries", BindingFlags.Static | BindingFlags.Public);
//             this._logEntries_EndGettingEntries = logEntriesType.GetMethod("EndGettingEntries", BindingFlags.Static | BindingFlags.Public);
//             this._logEntries_GetEntryInternal = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public);
//             this._logEntries_GetLinesAndModeFromEntryInternal = logEntriesType.GetMethod("GetLinesAndModeFromEntryInternal", BindingFlags.Static | BindingFlags.Public);
//             this._logEntries_Clear = logEntriesType.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
//             this._logEntries_GetEntryCount = logEntriesType.GetMethod("GetEntryCount", BindingFlags.Static | BindingFlags.Public);
//             _logEntryType = unityEditorAssembly.GetType("UnityEditor.LogEntry");
//             this._logEntry_Message = this._logEntryType.GetField("message");
//             this._logEntry_Mode = this._logEntryType.GetField("mode");
//             this._logEntry_CallstackTextStartUTF16 = this._logEntryType.GetField("callstackTextStartUTF16");
//             
//             this._logEntries_ConsoleFlags = logEntriesType.GetProperty("consoleFlags", BindingFlags.Static | BindingFlags.Public);
//             this.titleContent = new GUIContent("ConsolePro");
//
//             var totalRow = (int) this._logEntries_StartGettingEntries.Invoke(null, new object[0]);
//             for (int i = 0; i < totalRow; i++)
//             {
//                 int mask = 0;
//                 string header = "";
//                 var pam = new object[] {i, 1, mask, header };
//                 this._logEntries_GetLinesAndModeFromEntryInternal.Invoke(null, pam);
//                 header = (string) pam[3];
//                 object entry = Activator.CreateInstance(this._logEntryType);
//                 _logEntries_GetEntryInternal.Invoke(null, new object[]{i, entry});
//                 var message = (string)this._logEntry_Message.GetValue(entry);
//                 
//                 var mode = (int)this._logEntry_Mode.GetValue(entry);
//                 var repeated = (int) this._logEntries_GetEntryCount.Invoke(entry, new object[] { i });
//                 this._logs.Add(new ConsoleLogInfo()
//                 {
//                     title = header,
//                     message = message,
//                     mode = mode,
//                     repeated = repeated
//                 });
//                 
//             }
//
//             this._logEntries_EndGettingEntries.Invoke(null, null);
//             
//             
//             Application.logMessageReceived += OnLogChange;
//         }
//
//         private void OnDisable()
//         {
//             Application.logMessageReceived -= OnLogChange;
//         }
//
//         private void OnLogChange(string condition, string stacktrace, LogType type)
//         {
//             this._logs.Add(new ConsoleLogInfo()
//             {
//                 
//             });
//             this.Repaint();
//         }
//
//         private void OnGUI()
//         {
//             this.logsView = new Rect(0, 22, this.position.width, (this.position.height - 22) * this._viewSplit);
//             this.descView = new Rect(0, this.logsView.yMax + 2, this.position.width, (this.position.height - 22) * (1 - this._viewSplit) - 2);
//             
//             var curY = 0;
//             
//
//             GUI.Box(new Rect(0, 0, this.position.width, 22), "", new GUIStyle("FrameBox"));
//             
//             if (GUI.Button(new Rect(0, curY, 60, 22), "Clear"))
//             {
//                 this._logEntries_Clear.Invoke(null, new object[]{});
//             }
//             
//             if (GUI.Button(new Rect(60, curY, 20, 20), "", new GUIStyle("ToolbarDropDownLeft")))
//             {
//                 var menu = new GenericMenu();
//                 menu.AddItem(new GUIContent("Clear On Play"), HasFlag(ConsoleFlags.ClearOnPlay), () =>
//                 {
//                     SetFlag(ConsoleFlags.ClearOnPlay, !HasFlag(ConsoleFlags.ClearOnPlay));
//                 });
//                 menu.AddItem(new GUIContent("Clear On Build"), HasFlag(ConsoleFlags.ClearOnBuild), () =>
//                 {
//                     SetFlag(ConsoleFlags.ClearOnBuild, !HasFlag(ConsoleFlags.ClearOnBuild));
//                 });
//                 menu.AddItem(new GUIContent("Clear On Recompile"), HasFlag(ConsoleFlags.ClearOnRecompile), () =>
//                 {
//                     SetFlag(ConsoleFlags.ClearOnRecompile, !HasFlag(ConsoleFlags.ClearOnRecompile));
//                 });
//                 var rect = GUILayoutUtility.GetLastRect();
//                 rect.y += EditorGUIUtility.singleLineHeight;
//                 menu.DropDown(rect);
//             }
//
//             GUI.backgroundColor = HasFlag(ConsoleFlags.Collapse)? new Color(0.2f, 0.2f, 0.2f, 1) : Color.white;
//             
//             if (GUI.Button(new Rect(80, curY, 80, 22), "Collapse", new GUIStyle("Tab onlyOne")))
//             {
//                 SetFlag(ConsoleFlags.Collapse, !HasFlag(ConsoleFlags.Collapse));
//             }
//
//             GUI.backgroundColor = Color.white;
//             
//             GUI.backgroundColor = HasFlag(ConsoleFlags.ErrorPause)? new Color(0.2f, 0.2f, 0.2f, 1) : Color.white;
//             if (GUI.Button(new Rect(160, curY, 80, 22), "Error Pause", new GUIStyle("Tab onlyOne")))
//             {
//                 SetFlag(ConsoleFlags.ErrorPause, !HasFlag(ConsoleFlags.ErrorPause));
//             }
//             GUI.backgroundColor = Color.white;
//
//             EditorGUI.BeginChangeCheck();
//             this._searchStr = EditorGUI.DelayedTextField(new Rect(260, 2, 300, 20), this._searchStr, new GUIStyle("SearchTextField"));
//             if (EditorGUI.EndChangeCheck())
//             {
//                 
//             }
//             
//             // Flags
//             int errorCount = 0, warningCount = 0, logCount = 0;
//             var par = new object[] { errorCount, warningCount, logCount };
//             this._logEntries_GetCountsByType.Invoke(null, par);
//             EditorGUI.BeginChangeCheck();
//             errorCount = (int)par[0];
//             warningCount = (int)par[1];
//             logCount = (int)par[2];
//            
//             var btnStyle = new GUIStyle("Tab onlyOne");
//             btnStyle.alignment = TextAnchor.MiddleRight;
//             GUI.backgroundColor = HasFlag(ConsoleFlags.LogLevelLog)? new Color(0f, 0.5f, 0.2f, 1) : Color.white;
//             if (GUI.Button(new Rect(this.position.width - 180, curY, 58, 22), logCount > 999 ? "999+" : logCount.ToString(), btnStyle))
//             {
//                 SetFlag(ConsoleFlags.LogLevelLog, !HasFlag(ConsoleFlags.LogLevelLog));
//             }
//             GUI.backgroundColor = Color.white;
//             GUI.DrawTexture(new Rect(this.position.width - 180, 2, 16, 16), EditorGUIUtility.IconContent("console.infoicon.sml").image);
//             
//             GUI.backgroundColor = HasFlag(ConsoleFlags.LogLevelWarning)? new Color(0.2f, 0.2f, 0.2f, 1) : Color.white;
//             if (GUI.Button(new Rect(this.position.width - 120, curY, 58, 22), warningCount > 999 ? "999+" : warningCount.ToString(), btnStyle))
//             {
//                 SetFlag(ConsoleFlags.LogLevelWarning, !HasFlag(ConsoleFlags.LogLevelWarning));
//             }
//             GUI.backgroundColor = Color.white;
//             GUI.DrawTexture(new Rect(this.position.width - 120, 2, 16, 16), EditorGUIUtility.IconContent("console.warnicon.sml").image);
//             
//             GUI.backgroundColor = HasFlag(ConsoleFlags.LogLevelError)? new Color(0.2f, 0.2f, 0.2f, 1) : Color.white;
//             if (GUI.Button(new Rect(this.position.width - 60, curY, 58, 22), errorCount > 999 ? "999+" : errorCount.ToString(), btnStyle))
//             {
//                 SetFlag(ConsoleFlags.LogLevelError, !HasFlag(ConsoleFlags.LogLevelError));
//             }
//             GUI.backgroundColor = Color.white;
//             GUI.DrawTexture(new Rect(this.position.width - 60, 2, 16, 16), EditorGUIUtility.IconContent("console.erroricon.sml").image);
//             
//             curY += 22;
//
//             // var totalRow = (int) this._logEntries_StartGettingEntries.Invoke(null, null);
//             var totalRow = this._logs.Count;
//             this.logsViewPos = GUI.BeginScrollView(this.logsView, this.logsViewPos, new Rect(0, 0, this.position.width - 14, 40 * totalRow));
//             for (int i = 0; i < totalRow; i++)
//             {
//                 var log = this._logs[i];
//                 if (!HasFlag(ConsoleFlags.Collapse))
//                 {
//                     for (int j = 0; j < log.repeated; j++)
//                     {
//                         DrawInfo(log, i + j);
//                     }
//                 }
//                 else
//                 {
//                     DrawInfo(log, i);
//                 }
//             }
//             GUI.EndScrollView();
//
//             if (!string.IsNullOrEmpty(this._selectedInfo.message))
//             {
//                 // object entry = Activator.CreateInstance(this._logEntryType);
//                 // _logEntries_GetEntryInternal.Invoke(null, new object[]{this._selectedRow, entry});
//                 // var message = (string)this._logEntry_Message.GetValue(entry);
//                 var message = this._selectedInfo.message;
//                 var style = new GUIStyle("CN Message");
//                 message = StacktraceWithHyperlinks(message);
//                 var height = style.CalcHeight(new GUIContent(message), this.position.width - 14);
//                 this.descViewPos = GUI.BeginScrollView(this.descView, this.descViewPos, new Rect(0, 0, this.position.width - 14, height));
//                 EditorGUI.SelectableLabel(new Rect(0, 0, this.position.width - 14, height), message, style);
//                 GUI.EndScrollView();
//             }
//
//             var dragArea = new Rect(0, this.logsView.yMax - 2, this.position.width, 5);
//             if (dragArea.Contains(Event.current.mousePosition))
//             {
//                 EditorGUIUtility.AddCursorRect(dragArea, MouseCursor.ResizeVertical);
//             }
//             GUIHelper.Box(new Rect(0, this.logsView.yMax, this.position.width, 1), Color.black);
//
//             switch (Event.current.type)
//             {
//                 case EventType.MouseDown:
//                     if (Event.current.button == 0 && dragArea.Contains(Event.current.mousePosition))
//                     {
//                         this._draging = true;
//                         this._beginPos = Event.current.mousePosition;
//                         this._beginPer = this._viewSplit;
//                         Event.current.Use();
//                     }
//                     break;
//                 case EventType.MouseDrag:
//                     if (this._draging)
//                     {
//                         var detail = Event.current.mousePosition - this._beginPos;
//                         var per = detail.y / (this.position.height - 22);
//                         this._viewSplit = this._beginPer + per;
//                         EditorGUIUtility.AddCursorRect(dragArea, MouseCursor.ResizeVertical);
//                         this.Repaint();
//                         Event.current.Use();
//                     }
//                     break;
//                 case EventType.MouseUp:
//                     if (this._draging)
//                         Event.current.Use();
//                     this._draging = false;
//                     break;
//             }
//         }
//
//         private void DrawInfo(ConsoleLogInfo info, int row)
//         {
//             var backgroundColor = row == this._selectedRow ? new Color(0.17f, 0.36f, 0.52f, 1f) :row % 2 == 0 ? new Color(0.21f, 0.21f, 0.21f) : new Color(0.24f, 0.24f, 0.24f);
//             var lineArea = new Rect(0, row * 40, this.position.width - 14, 40);
//             GUIHelper.Box(lineArea, backgroundColor);
//             GUI.Box(new Rect(0, row * 40, 40, 40), "", GetIconForErrorMode(info.mode));
//                 
//             var msgs = info.message.Split('\n');
//             GUI.Label(new Rect(40, row * 40 + 2, this.position.width - 14, 20), $"{info.title}", "BoldLabel");
//             GUI.Label(new Rect(40, row * 40 + 22, this.position.width - 14, 20), msgs[1], "ControlLabel");
//             if (HasFlag(ConsoleFlags.Collapse))
//             {
//                 GUI.Label(new Rect(this.position.width - 40, row * 40 + 10, 30, 20), info.repeated.ToString(), "CN CountBadge");
//             }
//            
//
//             switch (Event.current.type)
//             {
//                 case EventType.MouseDown:
//                     if (Event.current.button == 0 && lineArea.Contains(Event.current.mousePosition))
//                     {
//                         this._selectedRow = row;
//                         this._selectedInfo = info;
//                         Event.current.Use();
//                     }
//                     break;
//             }
//
//         }
//         
//         private string GetIconForErrorMode(int mode)
//         {
//             if ((mode & (1 | 2 | 16 | 64 | 256 | 2048 | 1048576 | 2097152)) != 0)
//                 return "CN EntryErrorIcon";
//             if ((mode & (128 | 512 | 4096)) != 0)
//                 return "CN EntryWarnIcon";
//             return "CN EntryInfoIcon";
//         }
//         
//         private string StacktraceWithHyperlinks(string stacktraceText)
//         {
//             StringBuilder stringBuilder = new StringBuilder();
//             // stringBuilder.Append(stacktraceText.Substring(0, callstackTextStart));
//             string[] strArray = stacktraceText.Split(new string[1]
//             {
//                 "\n"
//             }, StringSplitOptions.None);
//             for (int index = 0; index < strArray.Length; ++index)
//             {
//                 string str1 = ") (at ";
//                 int num1 = strArray[index].IndexOf(str1, StringComparison.Ordinal);
//                 if (num1 > 0)
//                 {
//                     int num2 = num1 + str1.Length;
//                     if (strArray[index][num2] != '<')
//                     {
//                         string str2 = strArray[index].Substring(num2);
//                         int length = str2.LastIndexOf(":", StringComparison.Ordinal);
//                         if (length > 0)
//                         {
//                             int num3 = str2.LastIndexOf(")", StringComparison.Ordinal);
//                             if (num3 > 0)
//                             {
//                                 string str3 = str2.Substring(length + 1, num3 - (length + 1));
//                                 string str4 = str2.Substring(0, length);
//                                 stringBuilder.Append(strArray[index].Substring(0, num2));
//                                 stringBuilder.Append("<a href=\"" + str4 + "\" line=\"" + str3 + "\">");
//                                 stringBuilder.Append(str4 + ":" + str3);
//                                 stringBuilder.Append("</a>)\n");
//                                 continue;
//                             }
//                         }
//                     }
//                 }
//                 stringBuilder.Append(strArray[index] + "\n");
//             }
//             if (stringBuilder.Length > 0)
//                 stringBuilder.Remove(stringBuilder.Length - 1, 1);
//             return stringBuilder.ToString();
//         }
//
//         private void SetFlag(ConsoleFlags flag, bool val)
//         {
//             // this._logEntries_SetConsoleFlag.Invoke(null, new object[]{(int) flag, val});
//             this.flags = val? flag | this.flags : flag & this.flags;
//         }
//
//         private bool HasFlag(ConsoleFlags flag)
//         {
//             return (flag & this.flags) != 0;
//             // return ((int) this._logEntries_ConsoleFlags.GetValue(null) & (int) flags) != 0;
//         }
//         
//         [Flags]
//         enum ConsoleFlags
//         {
//             Collapse = 1 << 0,
//             ClearOnPlay = 1 << 1,
//             ErrorPause = 1 << 2,
//             Verbose = 1 << 3,
//             StopForAssert = 1 << 4,
//             StopForError = 1 << 5,
//             Autoscroll = 1 << 6,
//             LogLevelLog = 1 << 7,
//             LogLevelWarning = 1 << 8,
//             LogLevelError = 1 << 9,
//             ShowTimestamp = 1 << 10,
//             ClearOnBuild = 1 << 11,
//             ClearOnRecompile = 1 << 12,
//             UseMonospaceFont = 1 << 13,
//             StripLoggingCallstack = 1 << 14,
//         }
//
//
//         private struct ConsoleLogInfo
//         {
//             public string title;
//             public string message;
//             public int mode;
//             public int repeated;
//         }
//     }
// }