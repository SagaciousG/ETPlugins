using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.PSD.Xmp;

#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;
using Screen = UnityEngine.Screen;

namespace XGame
{
    public class DebugWindowLog : DebugWindowBase
    {
        public static string[] IgnoreStackType = new []
        {
            "ET.UnityLogger",
            "ET.Log",
            "ET.Logger"
        };
        
        
        private bool _logShow = true;
        private bool _warningShow = false;
        private bool _errorShow = true;
        private bool _fatalShow = true;
        private bool _locked = true;

        private Vector2 _scrollPos1;
        private Vector2 _scrollPos2;
        private int _selectedIndex;

        private static Regex _atFile = new Regex(@"at (.*) in (.*)\:(\d+)");

        protected override void OnDrawWindow(int id)
        {
            if(!this._isInEditor)
                GUI.DragWindow(new Rect(0, 0, _windowRect.width - 20, 20));

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Clear", GUILayout.Width(60)))
                {
                    ConsoleLogs.Instance.Clear();
                }

                this._locked = GUILayout.Toggle(this._locked, "Lock");
                
                GUILayout.FlexibleSpace();
                GUI.contentColor = Color.white;
                this._logShow = GUILayout.Toggle(this._logShow, ConsoleLogs.Instance.logCount.ToString(), GUILayout.MinWidth(40));
                GUI.contentColor = Color.yellow;
                this._warningShow = GUILayout.Toggle(this._warningShow, ConsoleLogs.Instance.warnCount.ToString(), GUILayout.MinWidth(40));
                GUI.contentColor = new Color(0.9f, 0.3f, 0.3f, 1);
                this._errorShow = GUILayout.Toggle(this._errorShow, ConsoleLogs.Instance.errorCount.ToString(), GUILayout.MinWidth(40));
                GUI.contentColor = Color.red;
                this._fatalShow = GUILayout.Toggle(this._fatalShow, ConsoleLogs.Instance.fatalCount.ToString(), GUILayout.MinWidth(40));
                GUI.contentColor = Color.white;
            }
            GUILayout.EndHorizontal();

       
            this._scrollPos1 = GUILayout.BeginScrollView(this._scrollPos1, GUILayout.Height(this._windowRect.height * 0.6f));
            {
                for (int i = 0; i < ConsoleLogs.Instance.Logs.Count; i++)
                {
                    ConsoleLogs.LogInfo logInfo = ConsoleLogs.Instance.Logs[i];
                    if (!this._logShow && logInfo.logType == LogType.Log)
                        continue;
                    if (!this._warningShow && logInfo.logType == LogType.Warning)
                        continue;
                    if (!this._errorShow && logInfo.logType == LogType.Error)
                        continue;
                    if (!this._fatalShow && logInfo.logType == LogType.Exception)
                        continue;
                        
                    GUILayout.BeginHorizontal(GUILayout.Height(30));
                    GUI.contentColor = logInfo.logType == LogType.Warning? Color.yellow
                            : logInfo.logType == LogType.Error? new Color(0.9f, 0.3f, 0.3f, 1)
                            : logInfo.logType == LogType.Exception? Color.red : Color.white;
                    var titles = logInfo.title.Split('\n');
                    var res = GUILayout.Toggle(this._selectedIndex == i, 
                        $"[{logInfo.Hour:00}:{logInfo.Minute:00}:{logInfo.Second:00}]{titles[0]}");
                    GUI.contentColor = Color.white;
                    if (res)
                        this._selectedIndex = i;
                    if (logInfo.repeated > 0)
                        GUILayout.Label(logInfo.repeated.ToString(), GUILayout.MaxWidth(60));
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndScrollView();
            if (this._locked)
            {
                this._scrollPos1 = new Vector2(0, 10000);
            }

            this._scrollPos2 = GUILayout.BeginScrollView(this._scrollPos2, GUILayout.Height(this._windowRect.height * 0.4f - 40));
            {
                if (this._selectedIndex >= 0 && this._selectedIndex < ConsoleLogs.Instance.Logs.Count)
                {
                    var log = ConsoleLogs.Instance.Logs[this._selectedIndex];
                    var logs = log.stack.Split('\n');
                    var style = new GUIStyle()
                    {
                        wordWrap = true,
                        stretchWidth = true,
                        richText = true,
                    };
                    var titles = log.title.Split('\n');
                    foreach (string title in titles)
                    {
                        if (IgnoreTrack(title))
                            continue;
                        var color = "#ffffff";
                        var msg = StacktraceWithHyperlinks(title, out var file, out var line);
                        if (GUILayout.Button($"<color={color}>{msg}</color>", style))
                        {
#if UNITY_EDITOR    
                            if (!string.IsNullOrEmpty(file))
                                InternalEditorUtility.OpenFileAtLineExternal(file, line);
#endif
                        }
                    }
                    foreach (string str in logs)
                    {
                        if (IgnoreTrack(str))
                            continue;
                        var color = "#ffffff";
                        var msg = StacktraceWithHyperlinks(str, out var file, out var line);
                        if (GUILayout.Button($"<color={color}>{msg}</color>", style))
                        {
#if UNITY_EDITOR
                            if (!string.IsNullOrEmpty(file))
                                InternalEditorUtility.OpenFileAtLineExternal(file, line);
#endif
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }

        public static bool IgnoreTrack(string line)
        {
            foreach (string s in IgnoreStackType)
            {
                if (line.StartsWith(s))
                    return true;
            }

            return false;
        }
        
        public static string StacktraceWithHyperlinks(string stacktraceText, out string file, out int line)
        {
            var stringBuilder = new StringBuilder();
             string str1 = ") (at ";
             file = "";
             line = 0;
             int num1 = stacktraceText.IndexOf(str1, StringComparison.Ordinal);
             if (num1 > 0)
             {
                 int num2 = num1 + str1.Length;
                 if (stacktraceText[num2] != '<')
                 {
                     string str2 = stacktraceText.Substring(num2);
                     int length = str2.LastIndexOf(":", StringComparison.Ordinal);
                     if (length > 0)
                     {
                         int num3 = str2.LastIndexOf(")", StringComparison.Ordinal);
                         if (num3 > 0)
                         {
                             string str3 = str2.Substring(length + 1, num3 - (length + 1));
                             string str4 = str2.Substring(0, length);
                             stringBuilder.Append(stacktraceText.Substring(0, num2));
                             stringBuilder.Append("<a href=\"" + str4 + "\" line=\"" + str3 + "\">");
                             stringBuilder.Append(str4 + ":" + str3);
                             stringBuilder.Append("</a>)");
                             file = str4;
                             line = Convert.ToInt32(str3);
                         }
                     }
                 }
             }
             else if (_atFile.IsMatch(stacktraceText))
             {
                 var result = _atFile.Match(stacktraceText);
                 stringBuilder.Append(result.Groups[1]);
                 stringBuilder.Append($"(at <a href=\"{result.Groups[2]}\" line=\"{result.Groups[3]}\">{result.Groups[2]}:{result.Groups[3]}</a>)");
                 file = result.Groups[2].Value;
                 line = Convert.ToInt32(result.Groups[3].Value);
             }
             else
             {
                 file = "";
                 line = 0;
                 return stacktraceText;
             }
             return stringBuilder.ToString();
        }
    }
}