namespace ET.Client
{
    [UIEvent(UIType.UISelectRole)]
    public class UISelectRoleEvent: AUIEvent
    {
        public override void OnCreate(UI ui)
        {
            ((UISelectRoleComponent)ui.Component).OnCreate();
        }

        public override void OnShow(UI ui, params object[] args)
        {
            ((UISelectRoleComponent)ui.Component).OnShow(args);
        }

        public override void OnRemove(UI ui)
        {
            ((UISelectRoleComponent)ui.Component).OnRemove();
        }
    }
}