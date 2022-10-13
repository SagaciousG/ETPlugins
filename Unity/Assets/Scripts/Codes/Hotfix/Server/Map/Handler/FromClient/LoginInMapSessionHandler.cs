using System;
using Unity.Mathematics;

namespace ET.Server
{
    [MessageHandler(SceneType.Map)]
    public class LoginInMapSessionHandler : AMRpcHandler<LoginInMapSessionRequest, LoginInMapSessionResponse>
    {
        protected override async ETTask Run(Session session, LoginInMapSessionRequest request, LoginInMapSessionResponse response, Action reply)
        {
            var res = session.DomainScene().GetComponent<MapSessionKeyComponent>().Get(request.Key);
            if (res != request.UnitId)
            {
                response.Error = ErrorCore.ERR_ConnectMapKeyError;
                reply();
            }
            session.RemoveComponent<SessionAcceptTimeoutComponent>();
            session.AddComponent<SessionUnitComponent>().UnitId = request.UnitId;
            session.AddComponent<MailBoxComponent, MailboxType>(MailboxType.MapSession);
            
            var unit = await session.DomainScene().GetComponent<DBComponent>().Query<Unit>(request.UnitId);
            session.DomainScene().GetComponent<UnitComponent>().Add(unit);
            unit.AddComponent<MoveComponent>();
            unit.AddComponent<MailBoxComponent>();
            unit.AddComponent<AOIEntity, int, float3>(9 * 1000, unit.Position);
            unit.AddComponent<UnitSessionComponent>().MapSessionActorId = session.InstanceId;

            unit.AddLocation().Coroutine();
            await TransferHelper.Transfer(unit, unit.Map, unit.Position, unit.Rotation);
            reply();
            await ETTask.CompletedTask;
        }
    }
}