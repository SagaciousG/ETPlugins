using System;


namespace ET.Server
{
	[MessageHandler(SceneType.Gate)]
	public class C2G_PingHandler : AMRpcHandler<PingRequest, PingResponse>
	{
		protected override async ETTask Run(Session session, PingRequest request, PingResponse response, Action reply)
		{
			response.Time = TimeHelper.ServerNow();
			reply();
			await ETTask.CompletedTask;
		}
	}
}