using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XGame
{

//使其能在Inspector面板显示，并且可以被赋予相应值
	[Serializable]
	public class UIReferenceCollectorData
	{
		// public bool addClick;
		public string key;

		public bool IsGameObject;
		//Object并非C#基础中的Object，而是 UnityEngine.Object
		public UnityEngine.Object gameObject;
		public int SelectIndex;
		public List<string> Components = new List<string>();
		public List<string> ComponentType = new List<string>();
	}


	public class UIReferenceCollector: MonoBehaviour
	{
		//用于序列化的List
		public List<UIReferenceCollectorData> data = new List<UIReferenceCollectorData>();
		//Object并非C#基础中的Object，而是 UnityEngine.Object
		private readonly Dictionary<string, UnityEngine.Object> dict = new Dictionary<string, UnityEngine.Object>();

		private bool _isInit;

		private Dictionary<string, Component> _componetCache = new Dictionary<string, Component>();


		//使用泛型返回对应key的gameobject
		public T Get<T>(string key) where T : class
		{
			if (!_isInit)
			{
				foreach (var collectorData in data)
				{
					dict[collectorData.key] = collectorData.gameObject;
				}

				_isInit = true;
			}
			UnityEngine.Object dictGo;
			if (!dict.TryGetValue(key, out dictGo))
			{
				return null;
			}
			return dictGo as T;
		}

		public T GetComponentFromGO<T>(string key) where T : UnityEngine.Component
		{
			if (this._componetCache.TryGetValue(key, out var com))
			{
				return com as T;
			}
			GameObject go = Get<GameObject>(key);
			if (go != null)
			{
				var res = go.GetComponent<T>();
				this._componetCache[key] = res;
				return res;
			}

			return null;
		}

		public UnityEngine.Object GetObject(string key)
		{
			if (!_isInit)
			{
				foreach (var collectorData in data)
				{
					dict[collectorData.key] = collectorData.gameObject;
				}

				_isInit = true;
			}
			UnityEngine.Object dictGo;
			if (!dict.TryGetValue(key, out dictGo))
			{
				return null;
			}
			return dictGo;
		}
	
	}
}