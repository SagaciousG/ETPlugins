namespace ET.Client
{
    public static class SessionHelper
    {
        public static async ETTask<T> Call<T>(Scene zoneScene, IRequest request, SessionType type) where T : IResponse
        {
            return (T) await Call(zoneScene.GetComponent<SessionComponent>()[type], request);
        }

        public static void Send(Scene zoneScene, IMessage message, SessionType type)
        {
            zoneScene.GetComponent<SessionComponent>()[type].Send(message);
            OpcodeHelper.LogMsg(zoneScene.Zone, 
                0, message, true, true
            );
        }

        public static ETTask<IResponse> Call(Session session, IRequest request)
        {
            OpcodeHelper.LogMsg(session.DomainZone(), 
                0, request, true, true
            );
            return session.Call(request);
        }

    }
}