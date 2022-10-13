namespace ET.Server
{
    public class OnlinePlayerInfoAwakeSystem: AwakeSystem<OnlinePlayerInfo, string, long>
    {
        protected override void Awake(OnlinePlayerInfo self, string account, long gateId)
        {
            self.Account = account;
        }
    }
    [FriendOf(typeof(OnlinePlayerInfo))]
    public static class OnlinePlayerInfoSystem
    {
    
    }
}