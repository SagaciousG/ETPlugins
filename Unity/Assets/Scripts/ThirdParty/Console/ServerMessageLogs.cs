using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XGame
{
    public class ServerMessageLogs
    {
        public static ServerMessageLogs Instance
        {
            get
            {
                _instance ??= new ServerMessageLogs();
                return _instance;
            }
        }

        private static ServerMessageLogs _instance;
        public List<MsgContent> Msgs { get; } = new List<MsgContent>();




        private Dictionary<string, ushort> _name2Code = new Dictionary<string, ushort>();
        private Dictionary<string, Type> _name2Type = new Dictionary<string, Type>();

        private Regex _msg = new Regex(@"<msg>(.*)</msg>");
        private Regex _zone = new Regex(@"<zone>(.*)</zone>");
        private Regex _actorId = new Regex(@"<actorId>(.*)</actorId>");
        private Regex _type = new Regex(@"<type>(.*)</type>");
        private Regex _serverTime = new Regex(@"<time>(.*)</time>");
        private Regex _isSend = new Regex(@"<send>");
        private Regex _isClient = new Regex(@"<client>");
        private Regex _symbol = new Regex(@"<symbol=(\w)>");
        private Regex _desc = new Regex(@"<desc=(\w)>");
        private ServerMessageLogs()
        {
            var codes = Assembly.Load("Unity.Model.Codes");
            var inner = codes.GetType("ET.InnerMessage");
            var mongo = codes.GetType("ET.MongoMessage");
            var outer = codes.GetType("ET.OuterMessage");
            var innerFields = inner.GetFields(BindingFlags.Public | BindingFlags.Static);
            var outerFields = outer.GetFields(BindingFlags.Public | BindingFlags.Static);
            var mongoFields = mongo.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fieldInfo in innerFields)
            {
                this._name2Code.Add(fieldInfo.Name, (ushort)fieldInfo.GetValue(null));
                this._name2Type.Add(fieldInfo.Name, codes.GetType($"ET.{fieldInfo.Name}"));
            }

            foreach (FieldInfo fieldInfo in outerFields)
            {
                this._name2Code.Add(fieldInfo.Name, (ushort)fieldInfo.GetValue(null));
                this._name2Type.Add(fieldInfo.Name, codes.GetType($"ET.{fieldInfo.Name}"));
            }

            foreach (FieldInfo fieldInfo in mongoFields)
            {
                this._name2Code.Add(fieldInfo.Name, (ushort)fieldInfo.GetValue(null));
                this._name2Type.Add(fieldInfo.Name, codes.GetType($"ET.{fieldInfo.Name}"));
            }
        }
        
        public void Add(string msg, string stack)
        {
            var content = new MsgContent();
            if (this._msg.IsMatch(msg))
            {
                var res = this._msg.Match(msg);
                content.msg = res.Groups[1].Value;
            }

            if (this._zone.IsMatch(msg))
            {
                var res = this._zone.Match(msg);
                content.zone = Convert.ToInt32(res.Groups[1].Value);
            }

            if (this._type.IsMatch(msg))
            {
                var res = this._type.Match(msg);
                content.type = this._name2Type[res.Groups[1].Value];
                content.opCode = this._name2Code[res.Groups[1].Value];
                content.msgType = content.opCode / 10000 == 1? MsgType.Outer :
                        content.opCode / 10000 == 2? MsgType.Inner : MsgType.Mongo;
                content.title = res.Groups[1].Value;
            }

            if (this._actorId.IsMatch(msg))
            {
                var res = this._actorId.Match(msg);
                content.actorId = Convert.ToInt64(res.Groups[1].Value);
            }

            if (this._serverTime.IsMatch(msg))
            {
                var res = this._serverTime.Match(msg);
                content.timeTick = Convert.ToInt64(res.Groups[1].Value);
            }

            if (this._isSend.IsMatch(msg))
            {
                content.clientToServer = true;
            }
            
            content.logType = LogType.Server;
            if (this._isClient.IsMatch(msg))
            {
                content.logType = LogType.Client;
            }
            
            if (this._symbol.IsMatch(msg))
            {
                var res = this._symbol.Match(msg);
                content.symbol = res.Groups[1].Value;
            }
            if (this._desc.IsMatch(msg))
            {
                var res = this._desc.Match(msg);
                content.desc = res.Groups[1].Value;
            }

            content.stack = stack;
            this.Msgs.Add(content);
        }
        
        [Flags]
        public enum LogType
        {
            Server   =    1 << 1,
            Client   =    1 << 2,
        }
        
        [Flags]
        public enum MsgType
        {
            Inner   =    1 << 1,
            Mongo   =    1 << 2,
            Outer   =    1 << 3,
        }
        
        public struct MsgContent
        {
            public LogType logType;
            public MsgType msgType;
            public ushort opCode;
            public int zone;
            public long actorId;
            public string title;
            public string msg;
            public Type type;
            public long timeTick;
            public bool clientToServer;
            public string symbol;
            public string desc;
            public string stack;
        }
    }
}