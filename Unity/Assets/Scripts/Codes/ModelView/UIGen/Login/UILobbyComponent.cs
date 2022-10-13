//自动生成文件，请勿在此编辑
using UnityEngine.UI;
using UnityEngine;
using XGame;
using TMPro;


namespace ET.Client
{
	[ComponentOf(typeof(UI))]
	public partial class UILobbyComponent : Entity, IAwake
	{
		public XImage bg;
		public XImage selectServer;
		public TextMeshProUGUI serverName;
		public XImage serverState;
		public RectTransform serverPanel;
		public XImage serverPanelBg;
		public UIList allServerList;
		public UIReferenceCollector latest1;
		public UIReferenceCollector latest3;
		public UIReferenceCollector latest2;

	}
}
