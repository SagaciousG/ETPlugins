namespace ET.Server
{
    [ActorMessageHandler(SceneType.Center)]
    public class R2C_AddOnlinePlayerHandler : AMActorHandler<Scene, G2C_AddOnlinePlayerAMessage>
    {
        protected override async ETTask Run(Scene entity, G2C_AddOnlinePlayerAMessage message)
        {
            entity.GetComponent<OnlinePlayerComponent>().Add(message.PlayerAccount, message.GateActorId);
            await ETTask.CompletedTask;
        }
    }
}