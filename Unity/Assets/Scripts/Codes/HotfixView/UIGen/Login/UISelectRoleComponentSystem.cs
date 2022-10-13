//自动生成文件，请勿编辑
using UnityEngine.UI;
using UnityEngine;
using XGame;


namespace ET.Client
{
	[FriendOf(typeof(UISelectRoleComponent))]
	public static partial class UISelectRoleComponentSystem
	{
		[ObjectSystem]
		public class UISelectRoleComponentAwakeSystem: AwakeSystem<UISelectRoleComponent>
		{
			protected override void Awake(UISelectRoleComponent self)
			{
				var rc = self.GetParent<UI>().GameObject.GetComponent<UIReferenceCollector>();
				self.roleList = rc.GetComponentFromGO<UIList>("roleList");
				self.createRole = rc.GetComponentFromGO<XImage>("createRole");

				
				self.OnAwake();
			}
		}
	}
}
