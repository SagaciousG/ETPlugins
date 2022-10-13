using System;
using UnityEngine;
using UnityEngine.UI;

namespace XGame
{
    public class XScrollRect : ScrollRect, IScrollRect
    {

        private event Action _onScrollMove;
        protected override void Start()
        {
            base.Start();
            onValueChanged.AddListener(OnScrollMove);
        }

        private void OnScrollMove(Vector2 arg0)
        {
            _onScrollMove?.Invoke();
        }

        public void AddScrollListener(Action onScroll)
        {
            _onScrollMove += onScroll;
        }
    }
}