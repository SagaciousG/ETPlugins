﻿using ET;

 namespace XGame
{
	public static class ObjectHelper
	{
		public static void Swap<T>(ref T t1, ref T t2)
		{
			(t1, t2) = (t2, t1);
		}

		public static T GetFieldValue<T>(object obj, string key)
		{
			var type = obj.GetType();
			var field = type.GetField(key);
			if (field != null)
			{
				var res = field.GetValue(obj);
				return (T) res;
			}
			Log.Error($"{type.Name}不存在字段“{key}”");
			return default;
		}
		
		public static object GetFieldValue(object obj, string key)
		{
			var type = obj.GetType();
			var field = type.GetField(key);
			if (field != null)
			{
				var res = field.GetValue(obj);
				return res;
			}
			Log.Error($"{type.Name}不存在字段“{key}”");
			return default;
		}
	}
}