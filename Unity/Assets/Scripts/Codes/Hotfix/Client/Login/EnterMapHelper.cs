using System;
using XGame;

namespace ET.Client
{
    public static class EnterMapHelper
    {
        public static async ETTask EnterMapAsync(Scene clientScene)
        {
            try
            {
                UILoading.Show(0.1f, "申请切换地图");
                var g2CEnterMap = await SessionHelper.Call<G2C_EnterMapResponse>(clientScene, new C2G_EnterMapRequest(), SessionType.Gate);
                clientScene.GetComponent<PlayerComponent>().MyId = g2CEnterMap.MyId;
                
                // 等待场景切换完成
                await clientScene.GetComponent<ObjectWait>().Wait<Wait_SceneChangeFinish>();
                UILoading.Show(1f, "完成");
                
                EventSystem.Instance.Publish(clientScene, new EventType.EnterMapFinish());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }	
        }
    }
}