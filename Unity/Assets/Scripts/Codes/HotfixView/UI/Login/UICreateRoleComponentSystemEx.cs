
using System.Linq;
using BM;
using TMPro;
using UnityEngine;
using XGame;

namespace ET.Client
{
	public static partial class UICreateRoleComponentSystem
	{
        private static void OnAwake(this UICreateRoleComponent self)
        {
	        self.roleList.OnData += RoleList_OnData;
	        self.roleList.OnSelected += RoleList_OnSelected;
	        self.createRole.OnClick(OnCreateRole, self);
        }

        private static async void OnCreateRole(UICreateRoleComponent self)
        {
	        var data = (UnitShowConfig) self.roleList.SelectedData;
	        var response = await SessionHelper.Call<CreateRoleResponse>(self.ClientScene(), new CreateRoleRequest() { RoleId = data.Id }, SessionType.Gate);
	        UIHelper.Remove(UIType.UICreateRole).Coroutine();
        }

        private static void RoleList_OnSelected(RectTransform arg1, bool arg2, object arg3, int arg4, Entity arg5)
        {
	        var referenceCollector = arg1.GetComponent<UIReferenceCollector>();
	        var bg = referenceCollector.GetComponentFromGO<XImage>("bg");
	        var title = referenceCollector.GetComponentFromGO<TextMeshProUGUI>("title");
	        var data = (UnitShowConfig)arg3;
	        bg.Skin = arg2? BPath.Image.fs_common_tips_guanghuandi_png : BPath.Image.fs_common_tips_xunhuandi_png;
        }

        private static void RoleList_OnData(int arg1, RectTransform arg2, object arg3, Entity arg4)
        {
	        var referenceCollector = arg2.GetComponent<UIReferenceCollector>();
	        var bg = referenceCollector.GetComponentFromGO<XImage>("bg");
	        var title = referenceCollector.GetComponentFromGO<TextMeshProUGUI>("title");
	        var data = (UnitShowConfig)arg3;
	        title.text = data.Name;
        }

        public static void OnCreate(this UICreateRoleComponent self)
        {
	        UIHelper.CreateUITopBack(self.GetParent<UI>(), "创建角色").Coroutine();
        }
        
        public static void OnShow(this UICreateRoleComponent self, params object[] args)
        {
	        var allRole = UnitShowConfigCategory.Instance.GetAll();
	        self.roleList.SetData(allRole.Values.ToArray(), self);
        }	
        
        public static void OnRemove(this UICreateRoleComponent self)
        {
            
        }
	}
}
