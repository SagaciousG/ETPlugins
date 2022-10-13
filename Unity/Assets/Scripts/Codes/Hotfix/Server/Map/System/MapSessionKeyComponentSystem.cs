namespace ET.Server
{
    [FriendOf(typeof(MapSessionKeyComponent))]
    public static class MapSessionKeyComponentSystem
    {
        public static void Add(this MapSessionKeyComponent self, long key, long unitId)
        {
            self.sessionKey.Add(key, unitId);
            self.TimeoutRemoveKey(key).Coroutine();
        }

        public static long Get(this MapSessionKeyComponent self, long key)
        {
            self.sessionKey.TryGetValue(key, out var id);
            return id;
        }

        public static void Remove(this MapSessionKeyComponent self, long key)
        {
            self.sessionKey.Remove(key);
        }

        private static async ETTask TimeoutRemoveKey(this MapSessionKeyComponent self, long key)
        {
            await TimerComponent.Instance.WaitAsync(20000);
            self.sessionKey.Remove(key);
        }
    }
}