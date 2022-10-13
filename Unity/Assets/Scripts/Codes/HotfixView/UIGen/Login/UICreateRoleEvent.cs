namespace ET.Client
{
    [UIEvent(UIType.UICreateRole)]
    public class UICreateRoleEvent: AUIEvent
    {
        public override void OnCreate(UI ui)
        {
            ((UICreateRoleComponent)ui.Component).OnCreate();
        }

        public override void OnShow(UI ui, params object[] args)
        {
            ((UICreateRoleComponent)ui.Component).OnShow(args);
        }

        public override void OnRemove(UI ui)
        {
            ((UICreateRoleComponent)ui.Component).OnRemove();
        }
    }
}