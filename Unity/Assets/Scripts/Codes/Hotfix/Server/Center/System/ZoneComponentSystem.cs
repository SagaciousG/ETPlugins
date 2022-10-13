namespace ET.Server
{
    public class ZoneComponentAwakeSystem: AwakeSystem<ZoneComponent>
    {
        protected override void Awake(ZoneComponent self)
        {
            var cfgs = ZoneConfigCategory.Instance.GetAll();
            foreach (var zoneConfig in cfgs.Values)
            {
                var cfg = StartSceneConfigCategory.Instance.GetGate(zoneConfig.Id);
                var zoneInfo = self.AddChild<ZoneInfo, int, long>(zoneConfig.Id, cfg.InstanceId);
                self.Zones.Add(zoneConfig.Id, zoneInfo);
            }
        }
    }



    [FriendOf(typeof(ZoneComponent))]
    public static class ZoneComponentSystem
    {

        public static ZoneInfo Get(this ZoneComponent self, int zone)
        {
            self.Zones.TryGetValue(zone, out var info);
            return info;
        }
        public static void ChangeState(this ZoneComponent self, int zone, int online)
        {
            if (self.Zones.TryGetValue(zone, out var zoneInfo))
            {
                zoneInfo.IsOnline = online == 1;
            }
        }
        
        public static void AddToZone(this ZoneComponent self, int zone, long playerId)
        {
            if (self.Zones.TryGetValue(zone, out var zoneInfo))
            {
                zoneInfo.Add(playerId);
            }
        }
        
        public static void RemoveFromZone(this ZoneComponent self, int zone, long playerId)
        {
            if (self.Zones.TryGetValue(zone, out var zoneInfo))
            {
                zoneInfo.Remove(playerId);
            }
        }
    }
}