using System;
using System.Collections.Generic;
using ET;
using UnityEngine;

namespace BM
{
    public partial class BPath
    {
        [StaticField]
        private static BPath _instance;

        public static BPath Instance
        {
            get
            {
                _instance??= new BPath();
                return _instance;
            }
        }

        private MultiDictionary<Type, string, string> _namePath = new MultiDictionary<Type, string, string>();
        
        public static string GetPath<T>(string name)
        {
            var type = typeof (T);
            if (Instance._namePath.TryGetValue(type, name, out var path))
            {
                return path;
            }
            var fieldInfo = type.GetField(name);
            if (fieldInfo != null)
            {
                Instance._namePath.Add(type, name, (string) fieldInfo.GetValue(null));
                return Instance._namePath[type][name];
            }

            return null;
        }
    }
}