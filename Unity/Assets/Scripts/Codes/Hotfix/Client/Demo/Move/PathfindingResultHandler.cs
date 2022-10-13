using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
	[MessageHandler(SceneType.Client)]
	public class PathfindingResultHandler : AMHandler<FindPathResultAMessage>
	{
		protected override async ETTask Run(Session session, FindPathResultAMessage message)
		{
			Unit unit = session.DomainScene().CurrentScene().GetComponent<UnitComponent>().Get(message.unitId);

			float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);

			await unit.GetComponent<MoveComponent>().MoveToAsync(message.Points, speed);
		}
	}
}
