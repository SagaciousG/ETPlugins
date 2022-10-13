using System;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectUnLockRequestHandler: AMActorRpcHandler<Scene, ObjectUnLockARequest, ObjectUnLockAResponse>
    {
        protected override async ETTask Run(Scene scene, ObjectUnLockARequest request, ObjectUnLockAResponse response, Action reply)
        {
            scene.GetComponent<LocationComponent>().UnLock(request.Key, request.OldInstanceId, request.InstanceId);

            reply();

            await ETTask.CompletedTask;
        }
    }
}