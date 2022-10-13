using System;


namespace ET.Server
{
	[MessageHandler(SceneType.Gate)]
	public class LoginGateHandler : AMRpcHandler<LoginGateRequest, LoginGateResponse>
	{
		protected override async ETTask Run(Session session, LoginGateRequest request, LoginGateResponse response, Action reply)
		{
			Scene scene = session.DomainScene();
			string account = scene.GetComponent<GateSessionKeyComponent>().Get(request.Key);
			if (account == null)
			{
				response.Error = ErrorCore.ERR_ConnectGateKeyError;
				response.Message = "Gate key验证失败!";
				reply();
				return;
			}
			
			session.RemoveComponent<SessionAcceptTimeoutComponent>();

			PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>();
			var player = await playerComponent.Add(account);
			
			session.AddComponent<SessionPlayerComponent>().PlayerId = player.Id;
			session.AddComponent<MailBoxComponent, MailboxType>(MailboxType.GateSession);

			response.PlayerId = player.Id;
			
			CenterHelper.Send(new G2C_AddOnlinePlayerAMessage(){GateActorId = session.DomainScene().InstanceId, PlayerAccount = account});
			
			reply();
			await ETTask.CompletedTask;
		}
	}
}