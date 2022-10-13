using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XGame
{
    public class UIRectPointListener : UIBehaviour
    {
        private RectTransform _rectTransform;
        private Canvas _canvas;
        public event Action<bool> OnClickInArea;
        protected override void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            InputComponent.Instance.AddListener(OnClick);
        }

        private void OnClick(InputData data, object args)
        {
            if (OnClickInArea != null && enabled)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rectTransform, data.position,
                    _canvas.worldCamera, out var localPoint);
                OnClickInArea.Invoke(_rectTransform.rect.Contains(localPoint));
            }
        }
    }
}