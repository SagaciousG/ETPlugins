namespace ET.Client
{
	[MessageHandler(SceneType.Client)]
	public class M2C_CreateUnitsHandler : AMHandler<M2C_CreateUnitsAMessage>
	{
		protected override async ETTask Run(Session session, M2C_CreateUnitsAMessage message)
		{
			Scene currentScene = session.DomainScene().CurrentScene();
			UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
			
			foreach (UnitInfo unitInfo in message.Units)
			{
				if (unitComponent.Get(unitInfo.UnitId) != null)
				{
					continue;
				}
				Unit unit = UnitFactory.Create(currentScene, unitInfo);
			}
			await ETTask.CompletedTask;
		}
	}
}
