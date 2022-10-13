using System;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectRemoveRequestHandler: AMActorRpcHandler<Scene, ObjectRemoveARequest, ObjectRemoveAResponse>
    {
        protected override async ETTask Run(Scene scene, ObjectRemoveARequest request, ObjectRemoveAResponse response, Action reply)
        {
            await scene.GetComponent<LocationComponent>().Remove(request.Key);

            reply();
        }
    }
}