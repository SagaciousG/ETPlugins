using System;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectAddRequestHandler: AMActorRpcHandler<Scene, ObjectAddARequest, ObjectAddAResponse>
    {
        protected override async ETTask Run(Scene scene, ObjectAddARequest request, ObjectAddAResponse response, Action reply)
        {
            await scene.GetComponent<LocationComponent>().Add(request.Key, request.InstanceId);

            reply();

            await ETTask.CompletedTask;
        }
    }
}