using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class OnlinePlayerComponent : Entity, IAwake
    {
        public Dictionary<string, OnlinePlayerInfo> Players = new Dictionary<string, OnlinePlayerInfo>();
    }
}