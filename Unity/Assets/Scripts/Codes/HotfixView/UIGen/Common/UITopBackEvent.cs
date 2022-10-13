namespace ET.Client
{
    [UIEvent(UIType.UITopBack)]
    public class UITopBackEvent: AUIEvent
    {
        public override void OnCreate(UI ui)
        {
            ((UITopBackComponent)ui.Component).OnCreate();
        }

        public override void OnShow(UI ui, params object[] args)
        {
            ((UITopBackComponent)ui.Component).OnShow(args);
        }

        public override void OnRemove(UI ui)
        {
            ((UITopBackComponent)ui.Component).OnRemove();
        }
    }
}