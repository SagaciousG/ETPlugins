namespace ET.Server
{
    public class GateComponentAwakeSystem : AwakeSystem<GateComponent>
    {
        protected override void Awake(GateComponent self)
        {
            CenterHelper.Send(new R2C_ZoneStateAMessage(){IsOnline = 1, Zone = self.DomainZone()});
        }
    }

    public class GateComponentDestorySystem: DestroySystem<GateComponent>
    {
        protected override void Destroy(GateComponent self)
        {
            CenterHelper.Send(new R2C_ZoneStateAMessage(){IsOnline = 0, Zone = self.DomainZone()});
        }
    }

    public static class GateComponentSystem
    {
    
    }
}