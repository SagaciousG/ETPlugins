//自动生成文件，请勿编辑
using UnityEngine.UI;
using UnityEngine;
using XGame;
using TMPro;


namespace ET.Client
{
	[FriendOf(typeof(UITopBackComponent))]
	public static partial class UITopBackComponentSystem
	{
		[ObjectSystem]
		public class UITopBackComponentAwakeSystem: AwakeSystem<UITopBackComponent>
		{
			protected override void Awake(UITopBackComponent self)
			{
				var rc = self.GetParent<UI>().GameObject.GetComponent<UIReferenceCollector>();
				self.back = rc.GetComponentFromGO<XImage>("back");
				self.title = rc.GetComponentFromGO<TextMeshProUGUI>("title");

				
				self.OnAwake();
			}
		}
	}
}
