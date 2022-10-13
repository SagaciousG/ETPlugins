namespace ET.Client
{
    [UIEvent(UIType.UILogin)]
    public class UILoginEvent: AUIEvent
    {
        public override void OnCreate(UI ui)
        {
            ((UILoginComponent)ui.Component).OnCreate();
        }

        public override void OnShow(UI ui, params object[] args)
        {
            ((UILoginComponent)ui.Component).OnShow(args);
        }

        public override void OnRemove(UI ui)
        {
            ((UILoginComponent)ui.Component).OnRemove();
        }
    }
}