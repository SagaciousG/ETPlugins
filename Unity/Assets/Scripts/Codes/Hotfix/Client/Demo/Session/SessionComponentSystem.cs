namespace ET.Client
{
	[FriendOf(typeof(SessionComponent))]
	public static class SessionComponentSystem
	{
		public static void Dispose(this SessionComponent self, SessionType type)
		{
			self[type]?.Dispose();
			self[type] = null;
		}
	}
	
	public class SessionComponentDestroySystem: DestroySystem<SessionComponent>
	{
		protected override void Destroy(SessionComponent self)
		{
			foreach (var session in self.Sessions.Values)
			{
				session.Dispose();
			}
			self.Sessions.Clear();
		}
	}
}
