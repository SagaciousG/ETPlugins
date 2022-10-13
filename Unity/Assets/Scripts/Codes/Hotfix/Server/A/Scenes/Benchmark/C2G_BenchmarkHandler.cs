using System;

namespace ET.Server
{
    [MessageHandler(SceneType.BenchmarkServer)]
    public class C2G_BenchmarkHandler: AMRpcHandler<BenchmarkRequest, BenchmarkResponse>
    {
        protected override async ETTask Run(Session session, BenchmarkRequest request, BenchmarkResponse response, Action reply)
        {            
            BenchmarkServerComponent benchmarkServerComponent = session.DomainScene().GetComponent<BenchmarkServerComponent>();
            if (benchmarkServerComponent.Count++ % 1000000 == 0)
            {
                Log.Debug($"benchmark count: {benchmarkServerComponent.Count} {TimeHelper.ClientNow()}");
            }
            reply();
            await ETTask.CompletedTask;
        }
    }
}