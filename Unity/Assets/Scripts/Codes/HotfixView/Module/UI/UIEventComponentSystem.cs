using System;
using UnityEngine;
using XGame;

namespace ET.Client
{
	/// <summary>
	/// 管理所有UI GameObject 以及UI事件
	/// </summary>
	[FriendOf(typeof(UIEventComponent))]
	public static class UIEventComponentSystem
	{
		[ObjectSystem]
		public class UIEventComponentAwakeSystem : AwakeSystem<UIEventComponent>
		{
			protected override void Awake(UIEventComponent self)
			{
				UIEventComponent.Instance = self;
			
				GameObject uiRoot = GameObject.Find("/Global/UI");
				ReferenceCollector referenceCollector = uiRoot.GetComponent<ReferenceCollector>();

				var blockCanvas = new GameObject("[UIBlock]").AddComponent<Canvas>();
				blockCanvas.renderMode = RenderMode.ScreenSpaceCamera;
				blockCanvas.overrideSorting = false;
				blockCanvas.sortingOrder = 30000;
				blockCanvas.worldCamera = Init.Instance.UICamera;
				self.UIBlock = blockCanvas.gameObject;
				self.UIBlock.transform.SetParent(uiRoot.transform);
				self.UIBlock.gameObject.SetActive(false);
				var blockImg = self.UIBlock.AddComponent<XImage>();
				blockImg.color = Color.clear;
			
				self.UILayers.Add((int)UILayer.Hidden, referenceCollector.Get<GameObject>(UILayer.Hidden.ToString()).transform);
				self.UILayers.Add((int)UILayer.Low, referenceCollector.Get<GameObject>(UILayer.Low.ToString()).transform);
				self.UILayers.Add((int)UILayer.Mid, referenceCollector.Get<GameObject>(UILayer.Mid.ToString()).transform);
				self.UILayers.Add((int)UILayer.High, referenceCollector.Get<GameObject>(UILayer.High.ToString()).transform);

				var uiEvents = EventSystem.Instance.GetTypes(typeof (UIEventAttribute));
				foreach (Type type in uiEvents)
				{
					object[] attrs = type.GetCustomAttributes(typeof(UIEventAttribute), false);
					if (attrs.Length == 0)
					{
						continue;
					}

					UIEventAttribute uiEventAttribute = attrs[0] as UIEventAttribute;
					AUIEvent aUIEvent = Activator.CreateInstance(type) as AUIEvent;
					self.UIEvents.Add(uiEventAttribute.UIType, aUIEvent);
				}
			}
		}
		
		public static void OnCreate(this UIEventComponent self, UI ui, string uiType)
		{
			self.UIEvents[uiType].OnCreate(ui);
		}

		public static void OnShow(this UIEventComponent self, UI ui, string uiType, params object[] args)
		{
			self.UIEvents[uiType].OnShow(ui, args);
		}

		public static Transform GetLayer(this UIEventComponent self, int layer)
		{
			return self.UILayers[layer];
		}
		
		public static void OnRemove(this UIEventComponent self, UI ui, string uiType)
		{
			try
			{
				
				self.UIEvents[uiType].OnRemove(ui);
			}
			catch (Exception e)
			{
				throw new Exception($"on remove ui error: {uiType}", e);
			}
			
		}
	}
}
