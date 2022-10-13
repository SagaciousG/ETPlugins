using System;
using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    public enum UILayerType
    {
        Lower,
        Bottom,
        UI,
        Alert,
        Top,
    }
    
    [RequireComponent(typeof(ReferenceCollector))]
    public class UIOptions : MonoBehaviour
    {
        public Action OnStartEvent;
        public UILayerType Layer = UILayerType.UI;
        [Tooltip("是否是全屏界面")]
        public bool FullScreenUI = true;
        [Tooltip("是否是Canvas")]
        public bool Canvas = true;
        [Tooltip("是否添加RaycastTarget组件")]
        public bool RaycastTarget = true;

        public string CellRenderType;
        public string ScriptPath;

        private UIComponent[] _components;
        private void Start()
        {
            OnStartEvent?.Invoke();
        }

        private Transform[] FindWithStop(Transform trans)
        {
            var list = new List<Transform>();
            for (int i = 0; i < trans.childCount; i++)
            {
                var node = trans.GetChild(i);
                if (node.GetComponent<UIOptions>() != null)
                {
                    continue;
                }
                if (node.GetComponent<UIComponent>() != null)
                {
                    list.Add(node);
                }

                if (node.childCount > 0)
                {
                    var nodes = FindWithStop(node);
                    list.AddRange(nodes);
                }
            }

            return list.ToArray();
        }
        
        public void OnUIShow()
        {
            var nodes = FindWithStop(transform);
            _components = new UIComponent[nodes.Length];
            for (var index = 0; index < nodes.Length; index++)
            {
                var node = nodes[index];
                _components[index] = node.GetComponent<UIComponent>();
            }

            foreach (var uiComponent in _components)
            {
                uiComponent.OnUIShow();
            }
        }

        public void OnUIClose()
        {
            foreach (var uiComponent in _components)
            {
                uiComponent.OnUIClose();
            }
        }

        public void OnUIDispose()
        {
            foreach (var uiComponent in _components)
            {
                uiComponent.OnUIDispose();
            }
        }
    }
}