namespace ET.Server
{
    public class ZoneInfoAwakeSystem : AwakeSystem<ZoneInfo, int, long>
    {
        protected override void Awake(ZoneInfo self, int a, long actorId)
        {
            self.ConfigId = a;
            self.ZoneGateActorId = actorId;
        }
    }

    [FriendOf(typeof(ZoneInfo))]
    public static class ZoneInfoSystem
    {
        public static void Add(this ZoneInfo self, long playerId)
        {
            if (!self.PlayerIds.Contains(playerId))
            {
                self.PlayerIds.Add(playerId);
            }
        }
        
        public static void Remove(this ZoneInfo self, long playerId)
        {
            if (self.PlayerIds.Contains(playerId))
            {
                self.PlayerIds.Remove(playerId);
            }
        }
    }
}