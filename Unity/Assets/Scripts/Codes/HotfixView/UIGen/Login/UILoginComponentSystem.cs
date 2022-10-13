//自动生成文件，请勿编辑
using UnityEngine.UI;
using UnityEngine;
using XGame;
using TMPro;


namespace ET.Client
{
	[FriendOf(typeof(UILoginComponent))]
	public static partial class UILoginComponentSystem
	{
		[ObjectSystem]
		public class UILoginComponentAwakeSystem: AwakeSystem<UILoginComponent>
		{
			protected override void Awake(UILoginComponent self)
			{
				var rc = self.GetParent<UI>().GameObject.GetComponent<UIReferenceCollector>();
				self.loginBtn = rc.GetComponentFromGO<XImage>("loginBtn");
				self.registerBtn = rc.GetComponentFromGO<XImage>("registerBtn");
				self.account = rc.GetComponentFromGO<TMP_InputField>("account");
				self.password = rc.GetComponentFromGO<TMP_InputField>("password");
				self.tips = rc.GetComponentFromGO<TextMeshProUGUI>("tips");

				
				self.OnAwake();
			}
		}
	}
}
