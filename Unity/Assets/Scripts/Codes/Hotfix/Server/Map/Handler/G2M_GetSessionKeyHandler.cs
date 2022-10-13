using System;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Map)]
    public class G2M_GetSessionKeyHandler : AMActorRpcHandler<Scene,G2M_GetSessionKeyARequest, G2M_GetSessionKeyAResponse>
    {
        protected override async ETTask Run(Scene unit, G2M_GetSessionKeyARequest request, G2M_GetSessionKeyAResponse response, Action reply)
        {
            var id = RandomGenerator.RandInt64();
            unit.GetComponent<MapSessionKeyComponent>().Add(id , request.UnitId);
            response.Key = id;
            reply();
            await ETTask.CompletedTask;
        }
    }
}