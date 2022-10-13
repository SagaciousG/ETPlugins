using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public enum GUIElementStateDomain
    {
        ComponentView,
    }
    
    public class GUIElementState
    {
        public int depth;
        private Dictionary<int, bool> showLabels = new Dictionary<int, bool>();
        public bool this[int key]
        {
            get
            {
                this.showLabels.TryGetValue(key, out var show);
                return show;
            }
            set
            {
                this.showLabels[key] = value;
            }
        }
    }

    public class GUIElementStateManager
    {
        private static GUIElementStateManager _instance;

        private MultiDictionary<GUIElementStateDomain, object, GUIElementState> _objs = new MultiDictionary<GUIElementStateDomain, object, GUIElementState>();

        public static GUIElementState Add(GUIElementStateDomain domain, object obj)
        {
            _instance ??= new GUIElementStateManager();
            if (!_instance._objs.TryGetValue(domain, out var dic))
            {
                dic = new Dictionary<object, GUIElementState>();
                _instance._objs[domain] = dic;
            }
            if (!dic.TryGetValue(obj, out var res))
            {
                res = new GUIElementState();
                dic[obj] = res;
            }

            res.depth++;
            return res;
        }

        public static void Clear(GUIElementStateDomain domain)
        {
            _instance ??= new GUIElementStateManager();
            if (_instance._objs.TryGetValue(domain, out var dic))
            {
                dic.Clear();
            }
        }

        public static void Reset(GUIElementStateDomain domain)
        {
            _instance ??= new GUIElementStateManager();
            if (_instance._objs.TryGetValue(domain, out var dic))
            {
                foreach (GUIElementState state in dic.Values)
                {
                    state.depth = 0;
                }
            }
        }
        
    }
    
    public static class EditorGUILayoutEx
    {
        public static void DrawMap(IDictionary map)
        {
        }

      
    }
}