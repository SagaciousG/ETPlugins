namespace ET.Client
{
	[Event(SceneType.Client)]
	public class LoginFinishEvent: AEvent<EventType.LoginFinish>
	{
		protected override async ETTask Run(Scene scene, EventType.LoginFinish args)
		{
			await UIHelper.Remove(UIType.UILogin);
			await UIHelper.Create(UIType.UILobby, UILayer.Mid);
		}
	}
	
	[Event(SceneType.Client)]
	public class AppStartEnterLoginEvent: AEvent<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(Scene scene, EventType.AppStartInitFinish args)
		{
			await UIHelper.Create(UIType.UILogin, UILayer.Mid);
		}
	}
}
