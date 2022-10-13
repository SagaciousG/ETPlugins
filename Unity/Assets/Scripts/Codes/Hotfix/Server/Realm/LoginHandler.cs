using System;
using System.Net;


namespace ET.Server
{
    [MessageHandler(SceneType.Realm)]
    [FriendOf(typeof(AccountInfo))]
    public class LoginHandler : AMRpcHandler<LoginRequest, LoginResponse>
    {
        protected override async ETTask Run(Session session, LoginRequest request, LoginResponse response, Action reply)
        {
            //没有选择角色之前都算不在线
            
            //验证账号
            var dbComponent = session.DomainScene().GetComponent<DBComponent>();
            var result = await dbComponent.Query<AccountInfo>(
                info => info.account == request.Account && info.password == request.Password
            );
            if (result.Count == 0)
            {
                response.Error = ErrorCode.ERR_AccountOrPwNotExist;
                reply();
                return;
            }
            session.RemoveComponent<SessionAcceptTimeoutComponent>();
            // //检测账号登录情况
            // var isOnline = await CenterHelper.Call<R2C_IsOnlineAResponse>(new R2C_IsOnlineARequest() { Account = request.Account, });
            // if (isOnline.Online == 1)
            // {
            //     if (isOnline.UnitId > 0)
            //     {
            //         MessageHelper.CallActor(isOnline.UnitId, new R2G_KickOutPlayerARequest(){Account = request.Account}).Coroutine();
            //     }
            //     CenterHelper.Send(new R2C_RemovePlayerAMessage()
            //     {
            //         PlayerAccount = request.Account
            //     });
            // }
            
            
            
            

            // 随机分配一个Gate
            // StartSceneConfig config = RealmGateAddressHelper.GetGate(session.DomainZone());
            // Log.Debug($"gate address: {MongoHelper.ToJson(config)}");

            // 向gate请求一个key,客户端可以拿着这个key连接gate
            // G2R_GetLoginKeyAResponse g2RGetLoginKey = (G2R_GetLoginKeyAResponse)await MessageHelper.CallActor(
                // config.InstanceId, new R2G_GetLoginKeyARequest() { Account = request.Account });
            
            
            // CenterHelper.Send(new R2Mgr_AddPlayerAMessage()
            // {
                // ActorId = config.InstanceId,
                // GateId = g2RGetLoginKey.GateId,
                // PlayerAccount = request.Account
            // });
            
            // response.Address = config.InnerIPOutPort.ToString();
            // response.Key = g2RGetLoginKey.Key;
            // response.GateId = g2RGetLoginKey.GateId;
            reply();
        }
    }
}
