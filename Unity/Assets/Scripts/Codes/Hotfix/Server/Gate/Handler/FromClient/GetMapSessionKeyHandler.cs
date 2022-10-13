using System;

namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class GetMapSessionKeyHandler : AMRpcHandler<GetMapSessionKeyRequest, GetMapSessionKeyResponse>
    {
        protected override async ETTask Run(Session session, GetMapSessionKeyRequest request, GetMapSessionKeyResponse response, Action reply)
        {
            var sessionKey = (G2M_GetSessionKeyAResponse) await MessageHelper.CallActor(session.DomainScene().GetComponent<GateMapComponent>().MapActorId,
                new G2M_GetSessionKeyARequest(){UnitId = request.UnitId});
            var cfg = StartSceneConfigCategory.Instance.GetMap(session.DomainZone());
            response.MapKey = sessionKey.Key;
            response.Address = cfg.InnerIPOutPort.ToString();
            reply();
        }
    }
}