using System;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;

namespace XGame
{
    public class DebugWindowServerCMD : DebugWindowBase
    {
        private string[] _tabNames = new[] { "All", "Server", "Client" };
        private int[] _tabVals = new[] { 6, 2, 4 };
        private int _selectedIndex;
        private bool _locked = true;
        private string _searchStr;
        private Vector2 _scrollPos;
        private Vector2 _scrollPos2;
        private Vector2 _scrollPos3;

        private int _selectedLog;
        private bool _showStack = true;
        protected override void OnDrawWindow(int id)
        {
            if(!this._isInEditor)
                GUI.DragWindow(new Rect(0, 0, _windowRect.width - 20, 20));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                ServerMessageLogs.Instance.Msgs.Clear();
            }


            this._searchStr = GUILayout.TextField(this._searchStr, GUILayout.Width(100));
            
            this._locked = GUILayout.Toggle(this._locked, "Lock");
            this._showStack = GUILayout.Toggle(this._showStack, "ShowStack");
            
            GUILayout.FlexibleSpace();
            this._selectedIndex = GUILayout.Toolbar(this._selectedIndex, this._tabNames);
            GUILayout.EndHorizontal();
            
            var list = ServerMessageLogs.Instance.Msgs;
            var flag = this._tabVals[this._selectedIndex];

            if (this._locked)
            {
                this._scrollPos = new Vector2(0, Single.MaxValue);
            }
            GUILayout.BeginHorizontal();
            this._scrollPos = GUILayout.BeginScrollView(this._scrollPos, GUILayout.Width(0.6f * this._windowRect.width));
            for (int i = 0; i < list.Count; i++)
            {
                ServerMessageLogs.MsgContent content = list[i];
                if (((int)content.logType & flag) != (int)content.logType)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(this._searchStr))
                {
                    if (!content.title.Contains(this._searchStr) && !content.opCode.ToString().Contains(this._searchStr))
                    {
                        continue;
                    }
                }

                GUI.contentColor = content.clientToServer? Color.green : Color.white;
                var res = GUILayout.Toggle(i == this._selectedLog,
                    $"[{content.symbol}][{content.logType}]{DateTime.FromBinary(content.timeTick):[hh:mm:ss]} {content.opCode} {content.title}");
                GUI.contentColor = Color.white;
                if (res)
                    this._selectedLog = i;
            }
            GUILayout.EndScrollView();

            GUILayout.BeginVertical();
            var height = this._showStack ? this._windowRect.height * 0.5f : this._windowRect.height;
            this._scrollPos2 = GUILayout.BeginScrollView(this._scrollPos2, GUILayout.Height(height));
            if (this._selectedLog >= 0 && this._selectedLog < list.Count)
            {
                var content = list[this._selectedLog];
                var dateTime = DateTime.FromFileTime(content.timeTick);
                GUILayout.Label($"Time :  {dateTime:yyyy-MM-d hh:mm:ss} {dateTime.Millisecond}");
                GUILayout.Label($"Zone : {content.zone}");
                GUILayout.Label($"Actor : {content.actorId}");
                GUILayout.Label($"MsgType : {content.msgType}");
                GUILayout.Label($"Opcode : {content.opCode} {content.title}");
                var obj = MongoDB.Bson.Serialization.BsonSerializer.Deserialize(content.msg, content.type);
                DrawObj(obj, 0);
            }
            GUILayout.EndScrollView();

            this._scrollPos3 = GUILayout.BeginScrollView(this._scrollPos3);
            if (this._selectedLog >= 0 && this._selectedLog < list.Count)
            {
                var content = list[this._selectedLog];
                var logs = content.stack.Split('\n');
                var style = new GUIStyle() { wordWrap = true, stretchWidth = true, richText = true, };
                foreach (string str in logs)
                {
                    if (DebugWindowLog.IgnoreTrack(str))
                        continue;
                    var color = "#ffffff";
                    var msg = DebugWindowLog.StacktraceWithHyperlinks(str, out var file, out var line);
                    if (GUILayout.Button($"<color={color}>{msg}</color>", style))
                    {
#if UNITY_EDITOR
                        if (!string.IsNullOrEmpty(file))
                            InternalEditorUtility.OpenFileAtLineExternal(file, line);
#endif
                    }
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawObj(object obj, int space)
        {
            var fields = obj.GetType().GetFields();
            foreach (var fieldInfo in fields)
            {
                object val = fieldInfo.GetValue(obj);
                if (fieldInfo.FieldType.IsValueType)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    GUILayout.Label($"{fieldInfo.Name}  =  {val}");
                    GUILayout.EndHorizontal();
                }
                else if (fieldInfo.FieldType == typeof(string))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    GUILayout.Label($"{fieldInfo.Name}  =  {val}");
                    GUILayout.EndHorizontal();
                }
                else if (val is IList list)
                {
                    GUILayout.Label($"--------{fieldInfo.Name} {fieldInfo.FieldType.Name}--------");
                    foreach (object o in list)
                    {
                        this.DrawObj(o, space + 10);
                    }
                }
                else
                {
                    GUILayout.Label($"--------{fieldInfo.Name} {fieldInfo.FieldType.Name}--------");
                    if (val == null)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(space);
                        GUILayout.Label($"{fieldInfo.Name}  =  null");
                        GUILayout.EndHorizontal();                 
                    }
                    else
                    {
                        DrawObj(val, space + 10);
                    }
                    
                }
            }

            var properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                object val = property.GetValue(obj);
                if (property.PropertyType.IsValueType)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    GUILayout.Label($"{property.Name}  =  {val}");
                    GUILayout.EndHorizontal();
                }
                else if (property.PropertyType == typeof(string))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    GUILayout.Label($"{property.Name}  =  {val}");
                    GUILayout.EndHorizontal();
                }
                else if (property.PropertyType.IsArray)
                {
                    GUILayout.Label($"--------{property.Name} {property.PropertyType.Name}--------");
                    if (val == null)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(space);
                        GUILayout.Label($"{property.Name}  =  null");
                        GUILayout.EndHorizontal();                 
                    }
                    else
                    {
                        var arr = val as IEnumerable;
                        foreach (object o in arr)
                        {
                            DrawObj(o, space + 10);
                        }
                    }
                }
                else
                {
                    GUILayout.Label($"--------{property.Name} {property.PropertyType.Name}--------");
                    if (val == null)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(space);
                        GUILayout.Label($"{property.Name}  =  null");
                        GUILayout.EndHorizontal();                 
                    }
                    else
                    {
                        DrawObj(val, space + 10);
                    }
                    
                }
            }
        }
        
        
    }
}