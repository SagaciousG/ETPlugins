//自动生成文件，请勿在此编辑
using UnityEngine.UI;
using UnityEngine;
using XGame;
using TMPro;


namespace ET.Client
{
	[ComponentOf(typeof(UI))]
	public partial class UILoginComponent : Entity, IAwake
	{
		public XImage loginBtn;
		public XImage registerBtn;
		public TMP_InputField account;
		public TMP_InputField password;
		public TextMeshProUGUI tips;

	}
}
