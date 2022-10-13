namespace ET.Server
{
    public class GateMapComponentAwakeSystem: AwakeSystem<GateMapComponent, long>
    {
        protected override void Awake(GateMapComponent self, long a)
        {
            self.MapActorId = a;
        }
    }

    public static class GateMapComponentSystem
    {
        public static async ETTask Add(this GateMapComponent self, Unit unit)
        {
            
        }
    }
}