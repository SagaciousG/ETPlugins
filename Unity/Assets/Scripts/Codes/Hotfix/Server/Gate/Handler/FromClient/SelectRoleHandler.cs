using System;

namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class SelectRoleHandler : AMRpcHandler<SelectRoleRequest, SelectRoleResponse>
    {
        protected override async ETTask Run(Session session, SelectRoleRequest request, SelectRoleResponse response, Action reply)
        {
            var myPlayer = session.GetComponent<SessionPlayerComponent>().GetMyPlayer();
            myPlayer.UnitId = request.UnitId;
            reply();
        }
    }
}