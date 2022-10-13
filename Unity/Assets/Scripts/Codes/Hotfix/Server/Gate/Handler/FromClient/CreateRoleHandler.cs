using System;
using System.Collections.Generic;

namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class CreateRoleHandler : AMRpcHandler<CreateRoleRequest, CreateRoleResponse>
    {
        protected override async ETTask Run(Session session, CreateRoleRequest request, CreateRoleResponse response, Action reply)
        {
            var myPlayer = session.GetComponent<SessionPlayerComponent>().GetMyPlayer();
            var unitId = IdGenerater.Instance.GenerateUnitId(session.DomainZone());
            myPlayer.Units ??= new List<long>();
            myPlayer.Units.Add(unitId);
            var unit = session.DomainScene().AddComponent<UnitComponent>().Add(unitId);
            unit.UnitShowConfigId = request.RoleId;
            unit.Level = 1;
            unit.Name = $"角色_{unitId % 10000}";
            unit.Map = 1001;
            unit.Type = UnitType.Player;
            
            var numericComponent = unit.AddComponent<NumericComponent>();
            numericComponent.Set(NumericType.Speed, 6f); // 速度是6米每秒
            numericComponent.Set(NumericType.AOI, 15000); // 视野15米

            var dbComponent = session.DomainScene().GetComponent<DBComponent>();
            await dbComponent.Save(unit);
            await dbComponent.Save(myPlayer);
            unit.GetParent<UnitComponent>().Dispose();
            reply();
            await ETTask.CompletedTask;
        }
    }
}