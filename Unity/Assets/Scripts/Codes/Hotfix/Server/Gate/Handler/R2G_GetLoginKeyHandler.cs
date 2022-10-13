using System;


namespace ET.Server
{
	[ActorMessageHandler(SceneType.Gate)]
	public class R2G_GetLoginKeyHandler : AMActorRpcHandler<Scene, R2G_GetLoginKeyARequest, R2G_GetLoginKeyAResponse>
	{
		protected override async ETTask Run(Scene scene, R2G_GetLoginKeyARequest request, R2G_GetLoginKeyAResponse response, Action reply)
		{
			long key = RandomGenerator.RandInt64();
			scene.GetComponent<GateSessionKeyComponent>().Add(key, request.Account);
			response.Key = key;
			reply();
			await ETTask.CompletedTask;
		}
	}
}