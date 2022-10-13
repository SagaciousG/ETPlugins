using System;


namespace ET.Server
{
	[MessageHandler(SceneType.Map)]
	public class C2M_PingHandler : AMRpcHandler<PingRequest, PingResponse>
	{
		protected override async ETTask Run(Session session, PingRequest request, PingResponse response, Action reply)
		{
			response.Time = TimeHelper.ServerNow();
			reply();
			await ETTask.CompletedTask;
		}
	}
}