using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ET.Server
{
    [ComponentOf()]
    public class AccountInfo: Entity, IAwake<string, string>
    {
        public string account;
        public string password;
        public long createTicks;
        public List<int> LatestEnterZones;
    }
}