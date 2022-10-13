namespace ET.Server
{
    public static class CenterHelper
    {
        public static async ETTask<T> Call<T>(IActorRequest request) where T : IActorResponse
        {
            var center = StartSceneConfigCategory.Instance.GetByCenterName("Center");
            var response = (T) await MessageHelper.CallActor(center.InstanceId, request);
            return response;
        }
        
        public static void Send(IActorMessage request) 
        {
            var center = StartSceneConfigCategory.Instance.GetByCenterName("Center");
            MessageHelper.SendActor(center.InstanceId, request);
        }
    }
}