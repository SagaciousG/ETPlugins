

using System.Collections.Generic;

namespace ET.Server
{
	[ActorMessageHandler(SceneType.Map)]
	public class G2M_SessionDisconnectHandler : AMActorLocationHandler<Unit, G2M_SessionDisconnectALMessage>
	{
		protected override async ETTask Run(Unit unit, G2M_SessionDisconnectALMessage message)
		{
			// MessageHelper.Broadcast(unit, new M2C_RemoveUnitsAMessage(){Units = new List<long>(){unit.Id}});
			// unit.DomainScene().GetComponent<MapDBComponent>().Send(new DB_SaveUnitAMessage(){Unit = unit});
			// unit.Dispose();
			await ETTask.CompletedTask;
		}
	}
}