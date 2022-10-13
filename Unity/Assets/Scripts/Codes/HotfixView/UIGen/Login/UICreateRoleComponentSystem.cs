//自动生成文件，请勿编辑
using UnityEngine.UI;
using UnityEngine;
using XGame;


namespace ET.Client
{
	[FriendOf(typeof(UICreateRoleComponent))]
	public static partial class UICreateRoleComponentSystem
	{
		[ObjectSystem]
		public class UICreateRoleComponentAwakeSystem: AwakeSystem<UICreateRoleComponent>
		{
			protected override void Awake(UICreateRoleComponent self)
			{
				var rc = self.GetParent<UI>().GameObject.GetComponent<UIReferenceCollector>();
				self.roleList = rc.GetComponentFromGO<UIList>("roleList");
				self.createRole = rc.GetComponentFromGO<XImage>("createRole");

				
				self.OnAwake();
			}
		}
	}
}
