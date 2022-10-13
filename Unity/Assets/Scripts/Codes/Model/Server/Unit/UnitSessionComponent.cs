namespace ET.Server
{
    [ComponentOf(typeof(Unit))]
    public class UnitSessionComponent : Entity, IAwake, ITransfer
    {
        public long MapSessionActorId { get; set; }
    }
}