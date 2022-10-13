using XGame;

namespace ET.Client
{
    public static class SceneChangeHelper
    {
        // 场景切换协程
        public static async ETTask SceneChangeTo(Scene clientScene, int mapId, long sceneInstanceId)
        {
            var mapConfig = MapConfigCategory.Instance.Get(mapId);
            UILoading.Show(0.3f, $"开始进入场景{mapConfig.Name}");
            clientScene.RemoveComponent<AIComponent>();
            
            CurrentScenesComponent currentScenesComponent = clientScene.GetComponent<CurrentScenesComponent>();
            currentScenesComponent.Scene?.Dispose(); // 删除之前的CurrentScene，创建新的
            Scene currentScene = SceneFactory.CreateCurrentScene(sceneInstanceId, clientScene.Zone, mapConfig.SceneName, currentScenesComponent);
            UnitComponent unitComponent = currentScene.AddComponent<UnitComponent>();
         
            // 可以订阅这个事件中创建Loading界面
            EventSystem.Instance.Publish(clientScene, new EventType.SceneChangeStart());

            UILoading.Show(0.5f, "创建角色");
            // 等待CreateMyUnit的消息
            Wait_CreateMyUnit waitCreateMyUnit = await clientScene.GetComponent<ObjectWait>().Wait<Wait_CreateMyUnit>();
            var m2CCreateMyUnit = waitCreateMyUnit.Message;
            UnitFactory.Create(currentScene, m2CCreateMyUnit.Unit);
            
            clientScene.RemoveComponent<AIComponent>();
            
            EventSystem.Instance.Publish(currentScene, new EventType.SceneChangeFinish());

            UILoading.Show(0.9f, "准备进入场景");
            // 通知等待场景切换的协程
            clientScene.GetComponent<ObjectWait>().Notify(new Wait_SceneChangeFinish());
        }
    }
}