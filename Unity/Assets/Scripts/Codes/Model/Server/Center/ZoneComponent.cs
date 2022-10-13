using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class ZoneComponent : Entity, IAwake
    {
        public Dictionary<int, ZoneInfo> Zones = new Dictionary<int, ZoneInfo>();
    }
}