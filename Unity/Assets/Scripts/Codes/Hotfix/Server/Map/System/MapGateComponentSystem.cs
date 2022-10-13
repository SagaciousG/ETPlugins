namespace ET.Server
{
    public class MapGateComponentAwakeSystem : AwakeSystem<MapGateComponent, long>
    {
        protected override void Awake(MapGateComponent self, long id)
        {
            self.GateActorId = id;
        }
    }

    [FriendOf(typeof(MapGateComponent))]
    public static class MapGateComponentSystem
    {
 
    }
}