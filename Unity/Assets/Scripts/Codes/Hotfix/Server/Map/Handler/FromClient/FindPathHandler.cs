using System.Collections.Generic;
using UnityEngine;

namespace ET.Server
{
	[ActorMessageHandler(SceneType.Map)]
	public class FindPathHandler : AMActorLocationHandler<Unit, FindPathALMessage>
	{
		protected override async ETTask Run(Unit unit, FindPathALMessage message)
		{
			Vector3 target = message.Position;

			unit.FindPathMoveToAsync(target).Coroutine();
			
			await ETTask.CompletedTask;
		}
	}
}