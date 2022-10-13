
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BM;
using TMPro;
using UnityEngine;
using XGame;

namespace ET.Client
{
	public static partial class UILobbyComponentSystem
	{
        private static void OnAwake(this UILobbyComponent self)
        {
	        self.serverPanel.Display(false);
	        self.bg.OnClick(() =>
	        {
		        self.EnterMap().Coroutine();
	        });
	        self.selectServer.OnClick(() =>
	        {
		        self.serverPanel.Display(true);
	        });
	        self.serverPanelBg.OnClick(() =>
	        {
		        self.serverPanel.Display(false);
	        });
	        self.latest1.gameObject.OnClick(() =>
	        {
		        var cfg = ZoneConfigCategory.Instance.Get(self.LatestEnterZones[0]);
		        self.OnSelected(cfg);
	        });
	        self.latest2.gameObject.OnClick(() =>
	        {
		        var cfg = ZoneConfigCategory.Instance.Get(self.LatestEnterZones[1]);
		        self.OnSelected(cfg);
	        });
	        self.latest3.gameObject.OnClick(() =>
	        {
		        var cfg = ZoneConfigCategory.Instance.Get(self.LatestEnterZones[2]);
		        self.OnSelected(cfg);
	        });
	        self.allServerList.OnData += AllServerList_OnData;
	        self.allServerList.OnSelected += AllServerList_OnSelected;
        }

        private static void OnSelected(this UILobbyComponent self, ZoneConfig cfg)
        {
	        self.SelectedZone = cfg.Id;
	        self.serverName.text = cfg.ShowName;
	        self.serverPanel.Display(false);
	        if (self.OnlineZones.TryGetValue(cfg.Id, out var zoneInfo))
	        {
		        var playerCount = zoneInfo.PlayerCount;
		        if (playerCount > 1000)
			        self.serverState.Skin = BPath.Image.fs_common_zy_hong_png;
		        else if (playerCount > 500)
			        self.serverState.Skin = BPath.Image.fs_common_zy_jihuo_png;
		        else
			        self.serverState.Skin = BPath.Image.fs_common_zy_lv_png;
	        }
	        else
	        {
		        self.serverState.Skin = BPath.Image.fs_common_zy_weijihuo_png;
	        }
        }

        private static void AllServerList_OnSelected(RectTransform cell, bool selected, object data, int index, Entity ui)
        {
	        var self = (UILobbyComponent)ui;
	        var cfg = (ZoneConfig)data;
			self.OnSelected(cfg);
        }

        private static void AllServerList_OnData(int index, RectTransform cell, object data, Entity rootUI)
        {
	        var referenceCollector = cell.GetComponent<UIReferenceCollector>();
	        var bg = referenceCollector.GetComponentFromGO<XImage>("bg");
	        var title = referenceCollector.GetComponentFromGO<TextMeshProUGUI>("title");
	        var state = referenceCollector.GetComponentFromGO<XImage>("state");
	        var haveRole = referenceCollector.GetComponentFromGO<RectTransform>("haveRole");
	        var roleName = referenceCollector.GetComponentFromGO<TextMeshProUGUI>("roleName");
	        var roleLevel = referenceCollector.GetComponentFromGO<TextMeshProUGUI>("roleLevel");

	        var self = (UILobbyComponent)rootUI;
	        var cfg = (ZoneConfig)data;
	        title.text = cfg.ShowName;
	        if (self.OnlineZones.TryGetValue(cfg.Id, out var zoneInfo))
	        {
		        var playerCount = zoneInfo.PlayerCount;
		        if (playerCount > 1000)
			        state.Skin = BPath.Image.fs_common_zy_hong_png;
		        else if (playerCount > 500)
			        state.Skin = BPath.Image.fs_common_zy_jihuo_png;
		        else
			        state.Skin = BPath.Image.fs_common_zy_lv_png;
				haveRole.gameObject.SetActive(zoneInfo.RoleLevel > 0);
				roleName.text = zoneInfo.RoleName;
				roleLevel.text = zoneInfo.RoleLevel.ToString();
	        }
	        else
	        {
				haveRole.gameObject.SetActive(false);
			    state.Skin = BPath.Image.fs_common_zy_weijihuo_png;
	        }
        }

        public static void OnCreate(this UILobbyComponent self)
        {
            
        }

        public static async void OnShow(this UILobbyComponent self, params object[] args)
        {
	        var listResponse = await SessionHelper.Call<ZoneListResponse>(self.ClientScene(), new ZoneListRequest()
	        {
		        Account = self.ClientScene().GetComponent<PlayerComponent>().Account
	        }, SessionType.Realm);
	        self.OnlineZones = new Dictionary<int, ZoneInfo>();
	        foreach (var zone in listResponse.OnlineZones)
	        {
		        self.OnlineZones[zone.Zone] = zone;
	        }

	        var all = ZoneConfigCategory.Instance.GetAll();
	        self.allServerList.SetData(all.Values.ToArray(), self);
	        self.LatestEnterZones = listResponse.LatestEnterZones;
	        if (listResponse.LatestEnterZones.Count > 0)
	        {
		        self.SelectedZone = listResponse.LatestEnterZones[0];
		        var zoneConfig = ZoneConfigCategory.Instance.Get(self.SelectedZone);

		        self.serverName.text = zoneConfig.ShowName;
		        self.serverPanel.Display(false);
		        if (self.OnlineZones.TryGetValue(zoneConfig.Id, out var zoneInfo))
		        {
			        var playerCount = zoneInfo.PlayerCount;
			        if (playerCount > 1000)
				        self.serverState.Skin = BPath.Image.fs_common_zy_hong_png;
			        else if (playerCount > 500)
				        self.serverState.Skin = BPath.Image.fs_common_zy_jihuo_png;
			        else
				        self.serverState.Skin = BPath.Image.fs_common_zy_lv_png;
		        }
		        else
		        {
			        self.serverState.Skin = BPath.Image.fs_common_zy_weijihuo_png;
			        
		        }

		        self.latest1.gameObject.Display(listResponse.LatestEnterZones.Count > 0);
		        self.latest2.gameObject.Display(listResponse.LatestEnterZones.Count > 1);
		        self.latest3.gameObject.Display(listResponse.LatestEnterZones.Count > 2);
		        if (listResponse.LatestEnterZones.Count > 0)
					SetLatest(self.latest1, listResponse.LatestEnterZones[0], self);
		        if (listResponse.LatestEnterZones.Count > 1)
					SetLatest(self.latest2, listResponse.LatestEnterZones[1], self);
		        if (listResponse.LatestEnterZones.Count > 2)
					SetLatest(self.latest3, listResponse.LatestEnterZones[2], self);
	        }
	        else
	        {
		        self.latest1.gameObject.Display(false);
		        self.latest2.gameObject.Display(false);
		        self.latest3.gameObject.Display(false);
		        self.serverName.text = "选择服务器";
			    self.serverState.Skin = BPath.Image.fs_common_zy_weijihuo_png;
	        }
        }

        private static void SetLatest(UIReferenceCollector latest, int zone, UILobbyComponent self)
        {
	        var bg = latest.GetComponentFromGO<XImage>("bg");
	        var title = latest.GetComponentFromGO<TextMeshProUGUI>("title");
	        var state = latest.GetComponentFromGO<XImage>("state");
	        var haveRole = latest.GetComponentFromGO<RectTransform>("haveRole");
	        var roleName = latest.GetComponentFromGO<TextMeshProUGUI>("roleName");
	        var roleLevel = latest.GetComponentFromGO<TextMeshProUGUI>("roleLevel");

	        var cfg = ZoneConfigCategory.Instance.Get(zone);
	        title.text = cfg.ShowName;
	        if (self.OnlineZones.TryGetValue(zone, out var zoneInfo))
	        {
		        var playerCount = zoneInfo.PlayerCount;
		        if (playerCount > 1000)
			        state.Skin = BPath.Image.fs_common_zy_hong_png;
		        else if (playerCount > 500)
			        state.Skin = BPath.Image.fs_common_zy_jihuo_png;
		        else
			        state.Skin = BPath.Image.fs_common_zy_lv_png;
		        haveRole.gameObject.SetActive(zoneInfo.RoleLevel > 0);
		        roleName.text = zoneInfo.RoleName;
		        roleLevel.text = zoneInfo.RoleLevel.ToString();
	        }
	        else
	        {
		        haveRole.gameObject.SetActive(false);
		        state.Skin = BPath.Image.fs_common_zy_weijihuo_png;
	        }

        }

        public static void OnRemove(this UILobbyComponent self)
        {
            
        }
        
        public static async ETTask EnterMap(this UILobbyComponent self)
        {
	        if (self.SelectedZone == 0)
	        {
		        self.serverPanel.Display(true);
		        return;
	        }

	        var enterZone = await SessionHelper.Call<EnterZoneResponse>(self.ClientScene(), 
		        new EnterZoneRequest()
		        {
			        Zone = self.SelectedZone,
			        Account = self.ClientScene().GetComponent<PlayerComponent>().Account,
		        }, SessionType.Realm);
	        if (enterZone.Error > 0)
	        {
		        return;
	        }
	        self.ClientScene().GetComponent<SessionComponent>().Dispose(SessionType.Realm);
	        Session gateSession = await RouterHelper.CreateRouterSession(self.ClientScene(), NetworkHelper.ToIPEndPoint(enterZone.Address));
	        self.ClientScene().GetComponent<SessionComponent>()[SessionType.Gate] = gateSession;
	        var loginGateResponse = await SessionHelper.Call<LoginGateResponse>(self.ClientScene(),
		        new LoginGateRequest() { Key = enterZone.GateKey }, SessionType.Gate);
	        self.ClientScene().GetComponent<PlayerComponent>().MyId = loginGateResponse.PlayerId;
	        
	        await UIHelper.Remove(UIType.UILobby);
	        await UIHelper.Create(UIType.UISelectRole, UILayer.Mid);
        }
	}
}
