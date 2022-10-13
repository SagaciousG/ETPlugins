using System;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectLockRequestHandler: AMActorRpcHandler<Scene, ObjectLockARequest, ObjectLockAResponse>
    {
        protected override async ETTask Run(Scene scene, ObjectLockARequest request, ObjectLockAResponse response, Action reply)
        {
            scene.GetComponent<LocationComponent>().Lock(request.Key, request.InstanceId, request.Time).Coroutine();

            reply();

            await ETTask.CompletedTask;
        }
    }
}