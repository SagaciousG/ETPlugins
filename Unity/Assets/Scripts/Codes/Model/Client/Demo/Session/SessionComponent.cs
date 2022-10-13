using System.Collections.Generic;

namespace ET.Client
{
	[ComponentOf(typeof(Scene))]
	public class SessionComponent: Entity, IAwake, IDestroy
	{
		public Dictionary<SessionType, Session> Sessions = new Dictionary<SessionType, Session>();

		public Session this[SessionType type]
		{
			get
			{
				this.Sessions.TryGetValue(type, out var res);
				return res;
			}
			set
			{
				this.Sessions[type] = value;
			}
		}
	}
}
