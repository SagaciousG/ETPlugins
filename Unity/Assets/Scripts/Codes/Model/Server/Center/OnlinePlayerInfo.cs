using System.Collections.Generic;

namespace ET.Server
{
    [ChildOf(typeof(OnlinePlayerComponent))]
    public class OnlinePlayerInfo : Entity, IAwake<string, long>
    {
        public string Account { get; set; }
    }
}