using System;

namespace ET.Server
{
	[ActorMessageHandler(SceneType.Map)]
	public class C2M_TransferMapHandler : AMActorLocationRpcHandler<Unit, C2M_TransferMapALRequest, M2C_TransferMapALResponse>
	{
		protected override async ETTask Run(Unit unit, C2M_TransferMapALRequest request, M2C_TransferMapALResponse response, Action reply)
		{
			await ETTask.CompletedTask;
			//
			// string currentMap = unit.DomainScene().Name;
			// string toMap = null;
			// if (currentMap == "Map1")
			// {
			// 	toMap = "Map2";
			// }
			// else
			// {
			// 	toMap = "Map1";
			// }
			//
			// StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(unit.DomainScene().Zone, toMap);
			// TransferHelper.Transfer(unit, startSceneConfig.InstanceId, toMap).Coroutine();
			
			reply();
		}
	}
}