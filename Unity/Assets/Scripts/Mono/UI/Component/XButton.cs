using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;

namespace XGame
{
    public class XButton : Button
    {
        private List<ActionInfo> _actions = new List<ActionInfo>();
        private List<Action> _actionsNoArgc = new List<Action>();
        protected override void Awake()
        {
            base.Awake();
            onClick.AddListener(DoClick);
        }

        private void DoClick()
        {
            foreach (var actionInfo in _actions)
            {
                actionInfo._method.Invoke(actionInfo._target, new[] {actionInfo._argc});
            }

            foreach (var action in _actionsNoArgc)
            {
                action.Invoke();
            }
        }

        public void AddClick(Action action)
        {
            _actionsNoArgc.Add(action);
        }

        public void RemoveClick(Action action)
        {
            _actionsNoArgc.Remove(action);
        }
        
        public void AddClick<T>(Action<T> action, T par)
        {
            _actions.Add(new ActionInfo()
            {
                _hashCode = action.GetHashCode(),
                _target = action.Target,
                _method = action.Method,
                _argc = par
            });
        }

        public void RemoveClick<T>(Action<T> action)
        {
            for (var index = _actions.Count - 1; index >= 0; index--)
            {
                var actionInfo = _actions[index];
                if (actionInfo._hashCode == action.GetHashCode())
                    _actions.RemoveAt(index);
            }
        }

        public void RemoveAllClick()
        {
            _actionsNoArgc.Clear();
            _actions.Clear();
        }
        
        private struct ActionInfo
        {
            public object _target;
            public int _hashCode;
            public MethodInfo _method;
            public object _argc;
        }
    }
}