namespace ET.Client
{
	[MessageHandler(SceneType.Client)]
	public class M2C_RemoveUnitsHandler : AMHandler<M2C_RemoveUnitsAMessage>
	{
		protected override async ETTask Run(Session session, M2C_RemoveUnitsAMessage message)
		{	
			UnitComponent unitComponent = session.DomainScene().CurrentScene()?.GetComponent<UnitComponent>();
			if (unitComponent == null)
			{
				return;
			}
			foreach (long unitId in message.Units)
			{
				unitComponent.Remove(unitId);
			}

			await ETTask.CompletedTask;
		}
	}
}
