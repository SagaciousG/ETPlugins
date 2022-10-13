namespace ET.Server
{
    public class MapAwakeSystem : AwakeSystem<Map, int>
    {
        protected override void Awake(Map self, int id)
        {
            self.ConfigId = id;
            self.AddComponent<MapUnitComponent>();
            self.AddComponent<AOIManagerComponent>();
        }
    }
    
    [FriendOf(typeof(MapUnitComponent))]
    public static class MapSystem
    {
        public static void AddUnit(this Map self, long unitId)
        {
            var mapUnitComponent = self.GetComponent<MapUnitComponent>();
            mapUnitComponent.AddChildWithId<MapUnit>(unitId);
        }
        
        public static void RemoveUnit(this Map self, long unitId)
        {
            var mapUnitComponent = self.GetComponent<MapUnitComponent>();

            mapUnitComponent.RemoveChild(unitId);
        }
    }
}