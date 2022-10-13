using System;

namespace ET.Server
{
	[ActorMessageHandler(SceneType.Map)]
	public class C2M_TestRobotCaseHandler : AMActorLocationRpcHandler<Unit, C2M_TestRobotCaseALRequest, M2C_TestRobotCaseALResponse>
	{
		protected override async ETTask Run(Unit unit, C2M_TestRobotCaseALRequest request, M2C_TestRobotCaseALResponse response, Action reply)
		{
			response.N = request.N;
			reply();
			await ETTask.CompletedTask;
		}
	}
}