using System;
using System.Collections.Generic;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Center)]
    [FriendOf(typeof(ZoneComponent))]
    [FriendOf(typeof(ZoneInfo))]
    public class R2C_ServerZoneStateHandler : AMActorRpcHandler<Scene, R2C_ServerZoneStateARequest, R2C_ServerZoneStateAResponse>
    {
        protected override async ETTask Run(Scene scene, R2C_ServerZoneStateARequest request, R2C_ServerZoneStateAResponse response, Action reply)
        {
            var zoneComponent = scene.GetComponent<ZoneComponent>();
            foreach (ZoneInfo zoneInfo in zoneComponent.Zones.Values)
            {
                if (zoneInfo.IsOnline)
                {
                    response.OnlineZones ??= new List<int>();
                    response.PlayerCount ??= new List<int>();
                    response.OnlineZones.Add(zoneInfo.ConfigId);
                    response.PlayerCount.Add(zoneInfo.PlayerIds.Count);
                }
            }

            reply();
            await ETTask.CompletedTask;
        }
    }
}