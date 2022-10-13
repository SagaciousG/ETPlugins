using System;
using System.Collections.Generic;
using System.Text;

namespace ET
{
    public static class OpcodeHelper
    {
        [StaticField]
        private static readonly HashSet<Type> ignoreDebugLogMessageSet = new HashSet<Type>
        {
            typeof(PingRequest),
            typeof(PingResponse),
            typeof(BenchmarkRequest),
            typeof(BenchmarkResponse),
        };

        private static bool IsNeedLogMessage(Type opcode)
        {
            if (ignoreDebugLogMessageSet.Contains(opcode))
            {
                return false;
            }

            return true;
        }

        public static bool IsOuterMessage(ushort opcode)
        {
            return opcode < OpcodeRangeDefine.OuterMaxOpcode;
        }

        public static bool IsInnerMessage(ushort opcode)
        {
            return opcode >= OpcodeRangeDefine.InnerMinOpcode;
        }

        public static void LogMsg(int zone, long actorId, object message, bool inClient, bool isSend, string description = null)
        {
            if (!IsNeedLogMessage(message.GetType()))
            {
                return;
            }

            var sb = new StringBuilder();
            sb.Append("[Message]");
            if (message is IRequest)
            {
                sb.Append("<symbol=R>");
            }
            else if (message is IResponse)
            {
                sb.Append("<symbol=Q>");
            }
            else
            {
                sb.Append("<symbol=M>");
            }
            if (isSend)
                sb.Append($"<send>");
            if (inClient)
                sb.Append("<client>");
            sb.Append($"<type>{message.GetType().Name}</type>");
            sb.Append($"<msg>{message}</msg>");
            sb.Append($"<time>{TimeHelper.ServerNow()}</time>");
            if (zone > 0)
                sb.Append($"<zone>{zone}</zone>");
            if (actorId > 0)
                sb.Append($"<actorId>{actorId}</actorId>");
            sb.Append($"<desc={description}>");
            Log.Debug(sb.ToString());
        }
    }
}