namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class GateMapComponent: Entity, IAwake<long>
    {
        public long MapActorId { get; set; }
    }
}