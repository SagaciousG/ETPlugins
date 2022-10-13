using Unity.Mathematics;
using UnityEngine;

namespace ET.Server
{
    public static class TransferHelper
    {
        public static async ETTask Transfer(Unit unit, int mapId, Vector3 position, Quaternion rotation)
        {
            // 通知客户端开始切场景
            MessageHelper.SendToClient(unit, new StartSceneChangeAMessage(){MapId = mapId, MapActorId = unit.DomainScene().InstanceId});
            var oldMap = unit.DomainScene().GetComponent<MapComponent>().GetMap(unit.Map);
            oldMap.RemoveUnit(unit.Id);
            unit.Stop(0);
            unit.RemoveComponent<AOIEntity>();
            unit.RemoveComponent<PathfindingComponent>();
            
            var newMap = unit.DomainScene().GetComponent<MapComponent>().GetMap(mapId);
            newMap.AddUnit(unit.Id);
            unit.Map = mapId;
            unit.Position = position;
            unit.Rotation = rotation;
            unit.AddComponent<AOIEntity, int, float3>(9 * 1000, position);
            unit.AddComponent<PathfindingComponent, string>(newMap.MapConfig.SceneName);
            
            // 通知客户端创建My Unit
            M2C_CreateMyUnitAMessage m2CCreateUnits = new M2C_CreateMyUnitAMessage();
            m2CCreateUnits.Unit = Server.UnitHelper.CreateUnitInfo(unit);
            MessageHelper.SendToClient(unit, m2CCreateUnits);
            await ETTask.CompletedTask;
        }
    }
}