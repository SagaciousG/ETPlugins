using System.Net;

namespace ET.Server
{
    public static class SceneFactory
    {
        public static async ETTask<Scene> CreateServerScene(Entity parent, long id, long instanceId, int zone, string name, SceneType sceneType, StartSceneConfig startSceneConfig = null)
        {
            await ETTask.CompletedTask;
            Scene scene = EntitySceneFactory.CreateScene(id, instanceId, zone, sceneType, name, parent);

            scene.AddComponent<MailBoxComponent, MailboxType>(MailboxType.UnOrderMessageDispatcher);

            switch (scene.SceneType)
            {
                case SceneType.Router:
                    scene.AddComponent<RouterComponent, IPEndPoint, string>(startSceneConfig.OuterIPPort,
                        startSceneConfig.StartProcessConfig.InnerIP
                    );
                    break;
                case SceneType.RouterManager:
                    scene.AddComponent<HttpComponent, string>($"http://{startSceneConfig.OuterIPPort}/");
                    break;
                case SceneType.Realm:
                    scene.AddComponent<NetServerComponent, IPEndPoint>(startSceneConfig.InnerIPOutPort);
                    scene.AddComponent<DBComponent, string, string>(startSceneConfig.DBConnection, startSceneConfig.DBName);
                    break;
                case SceneType.Gate:
                    scene.AddComponent<GateComponent>();
                    var map = StartSceneConfigCategory.Instance.GetMap(zone);
                    scene.AddComponent<PlayerComponent>();
                    scene.AddComponent<GateSessionKeyComponent>();
                    scene.AddComponent<GateMapComponent,long>(map.InstanceId);
                    scene.AddComponent<NetServerComponent, IPEndPoint>(startSceneConfig.InnerIPOutPort);
                    scene.AddComponent<DBComponent, string, string>(startSceneConfig.DBConnection, startSceneConfig.DBName);
                    break;
                case SceneType.Map:
                    var gate = StartSceneConfigCategory.Instance.GetGate(zone);
                    scene.AddComponent<MapComponent>();
                    scene.AddComponent<UnitComponent>();
                    scene.AddComponent<MapSessionKeyComponent>();
                    scene.AddComponent<MapGateComponent, long>(gate.InstanceId);
                    scene.AddComponent<NetServerComponent, IPEndPoint>(startSceneConfig.InnerIPOutPort);
                    scene.AddComponent<DBComponent, string, string>(startSceneConfig.DBConnection, startSceneConfig.DBName);
                    break;
                case SceneType.Location:
                    scene.AddComponent<LocationComponent>();
                    break;
                case SceneType.Center:
                    scene.AddComponent<ZoneComponent>();
                    scene.AddComponent<DBComponent, string, string>(startSceneConfig.DBConnection, startSceneConfig.DBName);
                    scene.AddComponent<OnlinePlayerComponent>();
                    break;
                case SceneType.BenchmarkServer:
                    scene.AddComponent<BenchmarkServerComponent>();
                    scene.AddComponent<NetServerComponent, IPEndPoint>(startSceneConfig.OuterIPPort);
                    break;
                case SceneType.BenchmarkClient:
                    scene.AddComponent<BenchmarkClientComponent>();
                    break;
            }

            return scene;
        }
    }
}