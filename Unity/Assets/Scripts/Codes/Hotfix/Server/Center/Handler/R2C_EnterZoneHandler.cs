using System;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Center)]
    public class R2C_EnterZoneHandler : AMActorRpcHandler<Scene, R2C_EnterZoneARequest, R2C_EnterZoneAResponse>
    {
        protected override async ETTask Run(Scene scene, R2C_EnterZoneARequest request, R2C_EnterZoneAResponse response, Action reply)
        {
            var zoneInfo = scene.GetComponent<ZoneComponent>().Get(request.Zone);
            response.ZoneGateActorId = zoneInfo.ZoneGateActorId;
            reply();
            await ETTask.CompletedTask;
        }
    }
}