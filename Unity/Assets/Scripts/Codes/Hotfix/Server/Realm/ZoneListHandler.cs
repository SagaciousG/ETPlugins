using System;
using System.Collections.Generic;

namespace ET.Server
{
    [MessageHandler(SceneType.Realm)]
    [FriendOf(typeof(AccountInfo))]
    public class ZoneListHandler : AMRpcHandler<ZoneListRequest, ZoneListResponse>
    {
        protected override async ETTask Run(Session session, ZoneListRequest request, ZoneListResponse response, Action reply)
        {
            var zoneState = await CenterHelper.Call<R2C_ServerZoneStateAResponse>(new R2C_ServerZoneStateARequest());
            var dbComponent = session.DomainScene().GetComponent<DBComponent>();
            var accountInfo = await dbComponent.QueryFirst<AccountInfo>(a => a.account == request.Account);
            
            var list = new List<ET.ZoneInfo>();
            for (int i = 0; i < zoneState.OnlineZones.Count; i++)
            {
                int zone = zoneState.OnlineZones[i];
                int count = zoneState.PlayerCount[i];
                
                list.Add(new ET.ZoneInfo() { PlayerCount = count, Zone = zone});
            }

            response.OnlineZones = list;
            response.LatestEnterZones = accountInfo.LatestEnterZones;
            reply();
        }
    }
}