namespace ET.Client
{
    [UIEvent(UIType.UILobby)]
    public class UILobbyEvent: AUIEvent
    {
        public override void OnCreate(UI ui)
        {
            ((UILobbyComponent)ui.Component).OnCreate();
        }

        public override void OnShow(UI ui, params object[] args)
        {
            ((UILobbyComponent)ui.Component).OnShow(args);
        }

        public override void OnRemove(UI ui)
        {
            ((UILobbyComponent)ui.Component).OnRemove();
        }
    }
}