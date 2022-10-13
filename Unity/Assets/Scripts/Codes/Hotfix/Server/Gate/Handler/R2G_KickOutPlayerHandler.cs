using System;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Gate)]
    public class R2G_KickOutPlayerHandler : AMActorRpcHandler<Scene, R2G_KickOutPlayerARequest, G2R_KickOutPlayerAResponse>
    {
        protected override async ETTask Run(Scene scene, R2G_KickOutPlayerARequest request, G2R_KickOutPlayerAResponse response, Action reply)
        {
            var playerComponent = scene.GetComponent<PlayerComponent>();
            var player = playerComponent.GetByAccount(request.Account);
            CenterHelper.Send(new R2C_RemoveOnlinePlayerAMessage() { PlayerAccount = player.Account });
            
            // MessageHelper.SendToClient(player, new G2C_KickOutAMessage());
            reply();
            // await TimerComponent.Instance.WaitAsync(1000);
            // scene.GetComponent<NetKcpComponent>().GetChild<Session>(player.SessionId).Dispose();
            await ETTask.CompletedTask;
        }
    }
}