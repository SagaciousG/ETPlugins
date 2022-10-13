using System.Collections.Generic;

namespace ET.Server
{
    [ChildOf(typeof(ZoneComponent))]
    public class ZoneInfo : Entity, IAwake<int, long>
    {
        public int ConfigId { get; set; }
        public bool IsOnline { get; set; }
        
        public long ZoneGateActorId { get; set; }
        public HashSet<long> PlayerIds = new HashSet<long>();
    }
}