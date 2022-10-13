using System;
using System.Collections.Generic;

namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class RoleListHandler : AMRpcHandler<RoleListRequest, RoleListResponse>
    {
        protected override async ETTask Run(Session session, RoleListRequest request, RoleListResponse response, Action reply)
        {
            var playerId = session.GetComponent<SessionPlayerComponent>().PlayerId;
            var dbComponent = session.DomainScene().GetComponent<DBComponent>();
            var player = await dbComponent.Query<Player>(playerId);
            if (player.Units != null)
            {
                response.Units ??= new List<SimpleUnit>();
                foreach (long unitId in player.Units)
                {
                    var unit = await dbComponent.Query<Unit>(unitId);
                    response.Units.Add(new SimpleUnit(){UnitId = unitId, Level = unit.Level, Name = unit.Name});
                }
            }

            reply();
        }
    }
}