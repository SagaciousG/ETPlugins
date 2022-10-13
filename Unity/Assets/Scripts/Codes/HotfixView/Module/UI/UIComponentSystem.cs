using System.Collections.Generic;
using BM;
using UnityEngine;

namespace ET.Client
{
    /// <summary>
    /// 管理Scene上的UI
    /// </summary>
    [FriendOf(typeof(UIComponent))]
    public static class UIComponentSystem
    {
        public static async ETTask<UI> Create(this UIComponent self, string uiType, UILayer uiLayer, params object[] args)
        {
            if (self.UIs.TryGetValue(uiType, out var ui))
            {
                UIEventComponent.Instance.OnShow(ui, uiType, args);
                return ui;
            }
			
            GameObject bundleGameObject = await AssetComponent.LoadAsync<GameObject>(BPath.GetPath<BPath.UI>($"{uiType}_prefab"));
            GameObject gameObject = UnityEngine.Object.Instantiate(bundleGameObject, UIEventComponent.Instance.GetLayer((int)uiLayer));
            ui = self.AddChild<UI, string, GameObject>(uiType, gameObject);
            if (!self.Name2Type.TryGetValue($"{uiType}", out var comType))
            {
                comType = typeof (UIComponent).Assembly.GetType($"ET.Client.{uiType}Component");
                self.Name2Type[$"{uiType}"] = comType;
            }
            ui.Component = ui.AddComponent(comType);
            UIEventComponent.Instance.OnCreate(ui, uiType);
            UIEventComponent.Instance.OnShow(ui, uiType, args);
            self.UIs.Add(uiType, ui);
            return ui;
        }

        public static void Remove(this UIComponent self, string uiType)
        {
            if (!self.UIs.TryGetValue(uiType, out UI ui))
            {
                return;
            }
            AssetComponent.UnLoadByPath(BPath.GetPath<BPath.UI>($"{uiType}_prefab"));
            UIEventComponent.Instance.OnRemove(ui, uiType);
			
            self.UIs.Remove(uiType);
            ui.Dispose();
        }

        public static UI Get(this UIComponent self, string name)
        {
            UI ui = null;
            self.UIs.TryGetValue(name, out ui);
            return ui;
        }
    }
}