using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class MapComponent : Entity, IAwake
    {
        public Dictionary<int, Map> Maps = new Dictionary<int, Map>();
    }
}