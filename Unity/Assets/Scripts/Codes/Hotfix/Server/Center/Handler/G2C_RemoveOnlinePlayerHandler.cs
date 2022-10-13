namespace ET.Server
{
    [ActorMessageHandler(SceneType.Center)]
    public class G2C_RemoveOnlinePlayerHandler : AMActorHandler<Scene, R2C_RemoveOnlinePlayerAMessage>
    {
        protected override async ETTask Run(Scene entity, R2C_RemoveOnlinePlayerAMessage message)
        {
            entity.GetComponent<OnlinePlayerComponent>().Remove(message.PlayerAccount);
            await ETTask.CompletedTask;
        }
    }
}