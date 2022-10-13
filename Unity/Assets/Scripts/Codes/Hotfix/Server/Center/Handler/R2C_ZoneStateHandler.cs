namespace ET.Server
{
    [ActorMessageHandler(SceneType.Center)]
    public class R2C_ZoneStateHandler : AMActorHandler<Scene, R2C_ZoneStateAMessage>
    {
        protected override async ETTask Run(Scene entity, R2C_ZoneStateAMessage message)
        {
            entity.GetComponent<ZoneComponent>().ChangeState(message.Zone, message.IsOnline);
            await ETTask.CompletedTask;
        }
    }
}