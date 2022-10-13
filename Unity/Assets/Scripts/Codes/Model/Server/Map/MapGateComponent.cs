using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class MapGateComponent : Entity, IAwake<long>
    {
        public long GateActorId { get; set; }
    }
}