
using TMPro;
using UnityEngine;
using XGame;

namespace ET.Client
{
	public static partial class UISelectRoleComponentSystem
	{
        private static void OnAwake(this UISelectRoleComponent self)
        {
	        self.roleList.OnData += RoleList_OnData;
	        self.roleList.OnSelected += RoleList_OnSelected;
	        self.createRole.OnClick(OnCreateRole, self);
        }

        private static async void RoleList_OnSelected(RectTransform arg1, bool arg2, object arg3, int arg4, Entity arg5)
        {
	        var data = (SimpleUnit)arg3;
	        var self = (UISelectRoleComponent)arg5;
	        var mapKey = await SessionHelper.Call<GetMapSessionKeyResponse>(self.ClientScene(), new GetMapSessionKeyRequest()
	        {
		        UnitId = data.UnitId
	        }, SessionType.Gate);
	        var mapSession = await RouterHelper.CreateRouterSession(self.ClientScene(), NetworkHelper.ToIPEndPoint(mapKey.Address));
	        self.ClientScene().GetComponent<SessionComponent>()[SessionType.Map] = mapSession;
	        await SessionHelper.Call(mapSession, new LoginInMapSessionRequest()
	        {
		        Key = mapKey.MapKey,
		        UnitId = data.UnitId
	        });
	        await SessionHelper.Call<SelectRoleResponse>(self.ClientScene(), new SelectRoleRequest() { UnitId = data.UnitId }, SessionType.Gate);
	        await UIHelper.Remove(UIType.UISelectRole);
        }

        private static async void OnCreateRole(UISelectRoleComponent self)
        {
	        var ui = await UIHelper.Create(UIType.UICreateRole, UILayer.Mid);
	        await ui.WaitForClose();
	        self.OnShow();
        }

        private static void RoleList_OnData(int arg1, RectTransform arg2, object arg3, Entity arg4)
        {
	        var referenceCollector = arg2.GetComponent<UIReferenceCollector>();
	        var bg = referenceCollector.GetComponentFromGO<XImage>("bg");
	        var title = referenceCollector.GetComponentFromGO<TextMeshProUGUI>("title");
	        var data = (SimpleUnit) arg3;
	        title.text = $"{data.Name}    Lv.{data.Level}";
        }

        public static void OnCreate(this UISelectRoleComponent self)
        {
	        UIHelper.CreateUITopBack(self.GetParent<UI>(), "选择角色").Coroutine();
        }
        
        public static async void OnShow(this UISelectRoleComponent self, params object[] args)
        {
	        var roleList = await SessionHelper.Call<RoleListResponse>(self.ClientScene(), new RoleListRequest() { }, SessionType.Gate);
	        self.roleList.SetData(roleList.Units, self);
        }
        
        public static void OnRemove(this UISelectRoleComponent self)
        {
            
        }
	}
}
