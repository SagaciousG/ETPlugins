//自动生成文件，请勿编辑
using UnityEngine.UI;
using UnityEngine;
using XGame;
using TMPro;


namespace ET.Client
{
	[FriendOf(typeof(UILobbyComponent))]
	public static partial class UILobbyComponentSystem
	{
		[ObjectSystem]
		public class UILobbyComponentAwakeSystem: AwakeSystem<UILobbyComponent>
		{
			protected override void Awake(UILobbyComponent self)
			{
				var rc = self.GetParent<UI>().GameObject.GetComponent<UIReferenceCollector>();
				self.bg = rc.GetComponentFromGO<XImage>("bg");
				self.selectServer = rc.GetComponentFromGO<XImage>("selectServer");
				self.serverName = rc.GetComponentFromGO<TextMeshProUGUI>("serverName");
				self.serverState = rc.GetComponentFromGO<XImage>("serverState");
				self.serverPanel = rc.GetComponentFromGO<RectTransform>("serverPanel");
				self.serverPanelBg = rc.GetComponentFromGO<XImage>("serverPanelBg");
				self.allServerList = rc.GetComponentFromGO<UIList>("allServerList");
				self.latest1 = rc.GetComponentFromGO<UIReferenceCollector>("latest1");
				self.latest3 = rc.GetComponentFromGO<UIReferenceCollector>("latest3");
				self.latest2 = rc.GetComponentFromGO<UIReferenceCollector>("latest2");

				
				self.OnAwake();
			}
		}
	}
}
