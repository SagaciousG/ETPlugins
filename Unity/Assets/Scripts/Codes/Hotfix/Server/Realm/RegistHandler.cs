using System;

namespace ET.Server
{
    [MessageHandler(SceneType.Realm)]
    [FriendOf(typeof(AccountInfo))]
    public class RegistHandler : AMRpcHandler<RegistRequest, RegistResponse>
    {
        protected override async ETTask Run(Session session, RegistRequest request, RegistResponse response, Action reply)
        {
            var dbComponent = session.DomainScene().GetComponent<DBComponent>();
            var result = await dbComponent.Query<AccountInfo>(
                info => info.account == request.Account
            );
            if (result.Count > 0)
            {
                response.Error = ErrorCode.ERR_AccountIsExist;
                reply();
                return;
            }

            var accountInfo = dbComponent.AddComponent<AccountInfo, string, string>(request.Account, request.Password);
            await dbComponent.Insert(accountInfo);
            dbComponent.RemoveComponent<AccountInfo>();
            reply();
        }
    }
}