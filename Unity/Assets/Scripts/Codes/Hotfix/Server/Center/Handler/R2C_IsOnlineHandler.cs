using System;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Center)]
    public class R2C_IsOnlineHandler : AMActorRpcHandler<Scene, R2C_IsOnlineARequest, R2C_IsOnlineAResponse>
    {
        protected override async ETTask Run(Scene scene, R2C_IsOnlineARequest request, R2C_IsOnlineAResponse response, Action reply)
        {
            var onlinePlayerComponent = scene.GetComponent<OnlinePlayerComponent>();
            response.Online = onlinePlayerComponent.Contain(request.Account) ? 1 : 0;
            var player = onlinePlayerComponent.Get(request.Account);
            reply();
            await ETTask.CompletedTask;
        }
    }
}