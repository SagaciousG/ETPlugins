namespace ET.Server
{
    public class OnlinePlayerComponentAwakeSystem : AwakeSystem<OnlinePlayerComponent>
    {
        protected override void Awake(OnlinePlayerComponent self)
        {
            
        }
    }

    [FriendOf(typeof(OnlinePlayerComponent))]
    public static class OnlinePlayerComponentSystem
    {
        public static void Add(this OnlinePlayerComponent self, string account, long gateId)
        {
            var playerInfo = self.AddChild<OnlinePlayerInfo, string, long>(account, gateId);
            self.Players.Add(account, playerInfo);
        }

        public static void Remove(this OnlinePlayerComponent self, string account)
        {
            if (self.Players.TryGetValue(account, out var playerInfo))
            {
                self.RemoveChild(playerInfo.InstanceId);
                self.Players.Remove(account);
            }
        }

        public static OnlinePlayerInfo Get(this OnlinePlayerComponent self, string account)
        {
            self.Players.TryGetValue(account, out var player);
            return player;
        }

        public static bool Contain(this OnlinePlayerComponent self, string account)
        {
            return self.Players.ContainsKey(account);
        }
    }
}