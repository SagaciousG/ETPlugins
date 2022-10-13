using System;
using System.Collections.Generic;
using System.Linq;
using ET;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XGame
{
    public class XPageList : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollRect
    {
        public enum Direction
        {
            Horizontal,
            Vertical
        }

        public Direction ScrollDirection;
        public RectTransform Content;
        public RectTransform Viewport;
        public GameObject LeftArrow;
        public GameObject RightArrow;
        public UIList PagePoints;
        [Tooltip("滑动幅度大于Viewport的大小*PageValue，切页成功")]
        [Range(0, 1)]
        public float PagingValue = 0.3f;

        public int PageCount => _pageCount;
        public Entity RootUI { private set; get; }
        
        private int _pageCount;
        private int _curPage;
        
        private Vector2 _beginPos;
        private Vector2 _contentPos;
        private bool _recovering;
        private Vector2 _toPos;

        public void PageTo(int page)
        {
            _curPage = Mathf.Clamp(page, 0, _pageCount - 1);
            OnCurPageChange();
        }
        private void Start()
        {
            if (LeftArrow != null)
            {
                LeftArrow.OnClick(ChangePage, -1);
            }

            if (RightArrow != null)
            {
                RightArrow.OnClick(ChangePage, 1);
            }
            
            PagePoints.OnSelected += PagePoints_OnSelected;
        }

        private void PagePoints_OnSelected(RectTransform arg1, bool arg2, object arg3, int arg4, Entity arg5)
        {
        }

        private void OnPageSelect(int arg1, object arg2)
        {
            _curPage = arg1;
            OnCurPageChange();
        }

        private void ChangePage(int num)
        {
            _curPage += num;
            _curPage = Mathf.Clamp(_curPage, 0, _pageCount - 1);
            OnCurPageChange();
        }

        private void Update()
        {
            var sizeDelta = Content.sizeDelta / Viewport.sizeDelta;
            var page = new Vector2Int(Mathf.CeilToInt(sizeDelta.x), Mathf.CeilToInt(sizeDelta.y));
            var count = ScrollDirection == Direction.Horizontal ? page.x : page.y;
            if (_pageCount != count)
            {
                _pageCount = count;
                _curPage = Mathf.Min(_curPage, ScrollDirection == Direction.Horizontal ? page.x - 1 : page.y - 1);
                var arr = Enumerable.Range(1, count).ToArray();
                PagePoints.SetData(arr, this.RootUI);
                OnCurPageChange();
            }

            if (_recovering)
            {
                Content.anchoredPosition = Vector2.Lerp(Content.anchoredPosition, _toPos, 0.5f);
                if (Vector2.Distance(_toPos, Content.anchoredPosition) < 0.1f)
                {
                    Content.anchoredPosition = _toPos;
                    _recovering = false;
                }
            }
        }
        
        private void OnCurPageChange()
        {
            var pos = Viewport.sizeDelta * new Vector2(_curPage * -1, _curPage);
            if (pos != Content.anchoredPosition)
            {
                _recovering = true;
                switch (ScrollDirection)
                {
                    case Direction.Horizontal:
                        pos.y = Content.anchoredPosition.y;
                        break;
                    case Direction.Vertical:
                        pos.x = Content.anchoredPosition.x;
                        break;
                }
                _toPos = pos;
            }

            if (RightArrow != null)
            {
                RightArrow.SetActive(_curPage < _pageCount - 1);
            }

            if (LeftArrow != null)
            {
                LeftArrow.SetActive(_curPage > 0);
            }

            if (PagePoints != null)
            {
                PagePoints.SetSelectIndex(_curPage);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _beginPos = eventData.position;
            _contentPos = Content.anchoredPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var off = eventData.position - _beginPos;
            switch (ScrollDirection)
            {
                case Direction.Horizontal:
                    if (Mathf.Abs(off.x) >= Viewport.sizeDelta.x * PagingValue)
                    {
                        _curPage += Mathf.RoundToInt(off.x.Normalize() * -1);
                    }

                    break;
                case Direction.Vertical:
                    if (Mathf.Abs(off.y) >= Viewport.sizeDelta.y * PagingValue)
                    {
                        _curPage += Mathf.RoundToInt(off.y.Normalize() * 1);
                    }

                    break;
            }

            _curPage = Mathf.Clamp(_curPage, 0, _pageCount - 1);
            OnCurPageChange();
        }

        public void OnDrag(PointerEventData eventData)
        {
            var off = eventData.position - _beginPos;
            switch (ScrollDirection)
            {
                case Direction.Horizontal:
                    off.y = 0;
                    if (_curPage == 0 && off.x > 0)
                        return;
                    if (_curPage == _pageCount - 1 && off.x < 0)
                        return;
                    break;
                case Direction.Vertical:
                    off.x = 0;
                    if (_curPage == 0 && off.y < 0)
                        return;
                    if (_curPage == _pageCount - 1 && off.y > 0)
                        return;
                    break;
            }
            Content.anchoredPosition = _contentPos + off;
        }

        public void AddScrollListener(Action onScroll)
        {
            
        }
    }
}