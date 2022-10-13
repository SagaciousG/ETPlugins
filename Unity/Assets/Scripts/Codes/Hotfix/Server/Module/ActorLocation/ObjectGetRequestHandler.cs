using System;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectGetRequestHandler: AMActorRpcHandler<Scene, ObjectGetARequest, ObjectGetAResponse>
    {
        protected override async ETTask Run(Scene scene, ObjectGetARequest request, ObjectGetAResponse response, Action reply)
        {
            long instanceId = await scene.GetComponent<LocationComponent>().Get(request.Key);
            response.InstanceId = instanceId;
            reply();
        }
    }
}