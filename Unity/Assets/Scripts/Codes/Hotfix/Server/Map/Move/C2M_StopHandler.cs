using System.Collections.Generic;
using UnityEngine;

namespace ET.Server
{
	[ActorMessageHandler(SceneType.Map)]
	public class C2M_StopHandler : AMActorLocationHandler<Unit, C2M_StopALMessage>
	{
		protected override async ETTask Run(Unit unit, C2M_StopALMessage message)
		{
			unit.Stop(0);
			await ETTask.CompletedTask;
		}
	}
}