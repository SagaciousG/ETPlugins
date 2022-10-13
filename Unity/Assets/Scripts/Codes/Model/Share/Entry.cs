namespace ET
{
    namespace EventType
    {
        public struct InitShare
        {
        }   
        
        public struct InitServer
        {
        } 
        
        public struct InitClient
        {
        } 
    }
    
    public static class Entry
    {
        public static void Start()
        {
            StartAsync().Coroutine();
        }
        
        private static async ETTask StartAsync()
        {
            WinPeriod.Init();
            
            MongoHelper.Init();
            ProtobufHelper.Init();
            
            Game.AddSingleton<NetServices>();
            Game.AddSingleton<Root>();
            await Game.AddSingleton<ConfigComponent>().LoadAsync();

            
            await EventSystem.Instance.PublishAsync(Root.Instance.Scene, new EventType.InitShare());
#if !DOTNET
            if (Init.Instance.GlobalConfig.CodeMode == CodeMode.Server || Init.Instance.GlobalConfig.CodeMode == CodeMode.ClientServer)
            {
#endif
                await EventSystem.Instance.PublishAsync(Root.Instance.Scene, new EventType.InitServer());
                Log.Debug($"Run Server");
#if !DOTNET
            }
            await EventSystem.Instance.PublishAsync(Root.Instance.Scene, new EventType.InitClient());
#endif
            
        }
    }
}