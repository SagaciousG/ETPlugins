using System;
using System.Collections.Generic;

namespace ET.Server
{
    [MessageHandler(SceneType.Realm)]
    [FriendOf(typeof(AccountInfo))]
    public class EnterZoneHandler : AMRpcHandler<EnterZoneRequest, EnterZoneResponse>
    {
        protected override async ETTask Run(Session session, EnterZoneRequest request, EnterZoneResponse response, Action reply)
        {
            var res = await CenterHelper.Call<R2C_EnterZoneAResponse>(new R2C_EnterZoneARequest() { Zone = request.Zone });
            var cfg = StartSceneConfigCategory.Instance.GetGate(request.Zone);
            var getLoginKeyAResponse = (R2G_GetLoginKeyAResponse) await MessageHelper.CallActor(res.ZoneGateActorId, new R2G_GetLoginKeyARequest(){Account = request.Account});

            DBComponent dbComponent = session.DomainScene().GetComponent<DBComponent>();
            var accountInfo = await dbComponent.QueryFirst<AccountInfo>(a => a.account == request.Account);
            accountInfo.LatestEnterZones ??= new List<int>();
            accountInfo.LatestEnterZones.Remove(request.Zone);
            accountInfo.LatestEnterZones.Insert(0, request.Zone);
            dbComponent.Save(accountInfo).Coroutine();
            response.Address = cfg.InnerIPOutPort.ToString();
            response.GateKey = getLoginKeyAResponse.Key;
            reply();
        }
    }
}