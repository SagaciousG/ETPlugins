namespace ET.Client
{
    public abstract class AUIEvent
    {
        public abstract void OnCreate(UI ui);
        public abstract void OnShow(UI ui, params object[] args);
        public abstract void OnRemove(UI ui);
    }
}