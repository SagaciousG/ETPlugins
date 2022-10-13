namespace ET.Server
{
    [ComponentOf(typeof(Session))]
    public class SessionUnitComponent : Entity, IAwake
    {
        public long UnitId { get; set; }
    }
}