using XGame;

namespace ET.Client
{
    public static partial class UITopBackComponentSystem
    {
        private static void OnAwake(this UITopBackComponent self)
        {
            self.back.OnClick<UITopBackComponent>(OnBack, self);
        }

        private static void OnBack(UITopBackComponent self)
        {
            UIHelper.Remove(self.GetParent<UI>().GetParent<UI>().Name).Coroutine();
        }

        public static void OnCreate(this UITopBackComponent self)
        {
            
        }
        
        public static void OnShow(this UITopBackComponent self, params object[] args)
        {
            self.title.text = (string)args[0];
        }
        
        public static void OnRemove(this UITopBackComponent self)
        {
            
        }
    }
}