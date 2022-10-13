using System;
using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    public class ConsoleLogs
    {
        public static ConsoleLogs Instance
        {
            get
            {
                _instance ??= new ConsoleLogs();
                return _instance;
            }
        }

        public int logCount => this._logCount;
        public int warnCount => this._warnCount;
        public int errorCount => this._errorCount;
        public int fatalCount => this._fatalCount;
        public List<LogInfo> Logs => this._logs;

        private static ConsoleLogs _instance;
        private List<LogInfo> _logs = new List<LogInfo>();

        private LogInfo _last;

        private int _logCount;
        private int _warnCount;
        private int _errorCount;
        private int _fatalCount;
        
        public void Add(string title, string stack, LogType type)
        {
            if (title.StartsWith("[Message]")) //协议消息
            {
                ServerMessageLogs.Instance.Add(title.Replace("[Message]", ""), stack);
                return;
            }
            
            
            switch (type)
            {
                case LogType.Log:
                case LogType.Assert:
                    this._logCount++;
                    break;
                case LogType.Warning:
                    this._warnCount++;
                    break;
                case LogType.Error:
                    this._errorCount++;
                    break;
                case LogType.Exception:
                    this._fatalCount++;
                    break;
            }
            
            var hour = DateTime.Now.Hour;
            var min = DateTime.Now.Minute;
            var second = DateTime.Now.Second;
            if (title == this._last.title && stack == this._last.stack && type == this._last.logType
                && hour == this._last.Hour && min == this._last.Minute && second == this._last.Second)
            {
                this._last.repeated += 1;
                this._logs[^1] = this._last;
            }
            else
            {
                var id = this._logs.Count;
                this._logs.Add(new LogInfo()
                {
                    stack = stack,
                    title = title,
                    logType = type,
                    Hour = hour,
                    Minute = min,
                    Second = second,
                    id = id
                });
                this._last = this._logs[^1];
            }
        }

        public void Clear()
        {
            this._logs.Clear();
            this._warnCount = 0;
            this._errorCount = 0;
            this._fatalCount = 0;
            this._logCount = 0;
        }
        
        

        public struct LogInfo
        {
            public string title;
            public string stack;
            public LogType logType;
            public int Hour;
            public int Minute;
            public int Second;
            public int repeated;
            public int id;
        }
    }
}