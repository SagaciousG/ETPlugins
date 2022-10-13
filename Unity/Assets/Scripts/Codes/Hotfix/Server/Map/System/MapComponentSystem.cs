namespace ET.Server
{
    public class MapComponentAwakeSystem : AwakeSystem<MapComponent>
    {
        protected override void Awake(MapComponent self)
        {
            var mapConfigs = MapConfigCategory.Instance.GetAll();
            foreach (var mapConfig in mapConfigs.Values)
            {
                var map = self.AddChild<Map, int>(mapConfig.Id);
                self.Maps.Add(mapConfig.Id, map);
            }
        }
    }

    [FriendOf(typeof(MapComponent))]
    public static class MapComponentSystem
    {
        public static Map GetMap(this MapComponent self, int mapId)
        {
            self.Maps.TryGetValue(mapId, out var map);
            return map;
        }
    }
}