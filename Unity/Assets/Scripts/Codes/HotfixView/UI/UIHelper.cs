using BM;
using UnityEngine;

namespace ET.Client
{
    public static class UIHelper
    {
        public static async ETTask<UI> Create(string uiType, UILayer uiLayer = UILayer.Mid, params object[] args)
        {
            var scene = UIEventComponent.Instance.ClientScene();
            return await scene.GetComponent<UIComponent>().Create(uiType, uiLayer, args);
        }
        
        public static async ETTask Remove(string uiType)
        {
            var scene = UIEventComponent.Instance.ClientScene();
            scene.GetComponent<UIComponent>().Remove(uiType);
            await ETTask.CompletedTask;
        }

        public static async ETTask<UI> CreateSingleUI(UI parent, string uiType, params  object[] args)
        {
            GameObject bundleGameObject = await AssetComponent.LoadAsync<GameObject>(BPath.GetPath<BPath.UI>($"{uiType}_prefab"));
            GameObject gameObject = UnityEngine.Object.Instantiate(bundleGameObject, parent.GameObject.transform);
            var ui = parent.AddChild<UI, string, GameObject>(uiType, gameObject);
            var comType = typeof (UIComponent).Assembly.GetType($"ET.Client.{uiType}Component");
            ui.Component = ui.AddComponent(comType);
            UIEventComponent.Instance.OnCreate(ui, uiType);
            UIEventComponent.Instance.OnShow(ui, uiType, args);
            return ui;
        }

        public static async ETTask CreateUITopBack(UI parent, string title)
        {
            var ui = await CreateSingleUI(parent, UIType.UITopBack, title);
        }
    }
}