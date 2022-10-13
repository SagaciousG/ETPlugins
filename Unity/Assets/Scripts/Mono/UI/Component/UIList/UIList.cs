﻿using System;
using System.Collections;
using System.Collections.Generic;
 using System.Linq;
 using ET;
 using UnityEngine;
 using UnityEngine.EventSystems;

 namespace XGame
{
    public partial class UIList : UIComponent, IPointerDownHandler
    {
        [SerializeField]
        private RectTransform _content;
        [SerializeField]
        private RectTransform _viewport;
        [SerializeField]
        private RectTransform _templete;
        
        private IList _data;
        private bool _scrolling;
        private bool _start;
        private float _scrollSpeed;
        private int _dataNum;
        private int _selectedIndex = -1;
        private int _curFirstIndex;
        private int _lastFirstIndex;
        private int _maxViewCellCount;
        private int _maxFirstCellIndex;
        private int _viewCellCount;
        private Vector2 _toPoint;
        private Vector2 _contentPos;
        private Vector2 _viewportSize;
        private Vector2 _sizeOfList;

        private Transform _poolTrans;
        private Stack<RectTransform> _poolCells = new Stack<RectTransform>();
        
        private Dictionary<int, RectTransform> _dataIndex2Cell = new Dictionary<int, RectTransform>();
        private void Start()
        {
            _poolTrans = new GameObject("_pool").GetComponent<Transform>();
            _poolTrans.SetParent(transform, false);
            _poolTrans.gameObject.SetActive(false);
            
            _templete.gameObject.SetActive(false);
            
            _content.pivot = new Vector2(0, 1);
            _content.anchorMin = Vector2.up;
            _content.anchorMax = Vector2.up;
            _content.anchoredPosition = Vector2.zero;

            _viewportSize = _viewport.rect.size;

            _start = true;
            UpdateLayout();
        }

        private void Update()
        {
            var count = _data?.Count ?? 0;
            if (_dataNum != count)
            {
                _dataNum = count;
                OnLayoutChange();
                UpdateContentSize();
                OnCenterTypeChange();
                UpdateMaxFirstCellIndex();
                UpdateMaxViewCellCount();
                UpdateViewFirstCellIndex();
                UpdateCellCount();
            }

            if (_scrolling)
            {
                _content.anchoredPosition = Vector2.Lerp(_content.anchoredPosition, _toPoint, _scrollSpeed);
                if (Vector2.Distance(_content.anchoredPosition, _toPoint) < 0.1f)
                {
                    _content.anchoredPosition = _toPoint;
                    _scrolling = false;
                }
            }

            if (Vector2.Distance(_contentPos, _content.position) > 0.1)
            {
                _contentPos = _content.position;
                OnContentMove();
            }
        }
        
        private void OnContentMove()
        {
            UpdateViewFirstCellIndex();
            var firstIndex = Mathf.Min(_maxFirstCellIndex, _curFirstIndex);
            if (firstIndex != _lastFirstIndex)
            {
                var change = firstIndex - _lastFirstIndex;
                var changeNum = Mathf.Abs(change);
                if (changeNum >= _maxViewCellCount)
                {
                    UpdateCellCount();
                }
                else
                {
                    if (change > 0)
                    {
                        for (int i = _lastFirstIndex; i < firstIndex; i++)
                        {
                            SetCellPos(i, i + _maxViewCellCount);
                        }
                    }
                    else
                    {
                        for (int i = _lastFirstIndex - 1; i >= firstIndex; i--)
                        {
                            var dataIndex = i + _maxViewCellCount;
                            SetCellPos(dataIndex, i);
                        }
                    }
                }
            }
        }

        private void UpdateCellCount(bool focusUpdate = false)
        {
            var firstIndexChange = _curFirstIndex - _lastFirstIndex;
            var count = Mathf.Min(_maxViewCellCount, _dataNum - _curFirstIndex);
            var change = count - _viewCellCount;
            var viewCount = _viewCellCount;

            if (Mathf.Abs(change) >= _maxViewCellCount ||
                Mathf.Abs(firstIndexChange) >= _maxViewCellCount ||
                focusUpdate)
            {
                foreach (var key in _dataIndex2Cell.Keys.ToArray())
                {
                    Collect(key);
                }
                for (int i = 0; i < _maxViewCellCount; i++)
                {
                    SetCellPos(-1, _curFirstIndex + i);
                }
            }
            else if (change > 0)
            {
                if (firstIndexChange < 0)
                {
                    for (int i = firstIndexChange + change; i < 0; i++)
                    {
                        SetCellPos(_dataNum - i, _lastFirstIndex + i);
                    }
                    for (int i = 0; i < change; i++)
                    {
                        SetCellPos(-1, _curFirstIndex + i);
                    }
                }
                else
                {
                    for (int i = 0; i < change; i++)
                    {
                        SetCellPos(-1, _curFirstIndex + viewCount + i);
                    }
                }

            }
            else if (change < 0)
            {
                if (firstIndexChange > 0)
                {
                    for (int i = 0; i < firstIndexChange; i++)
                    {
                        SetCellPos(_lastFirstIndex + i, _curFirstIndex + viewCount + i);
                    }
                }
                else
                {
                    foreach (var key in _dataIndex2Cell.Keys.ToArray())
                    {
                        Collect(key);
                    }
                    for (int i = 0; i < _maxViewCellCount; i++)
                    {
                        SetCellPos(-1, _curFirstIndex + i);
                    }
                }
            }
            
        }

        private void OnClick(GameObject obj)
        {
            foreach (var kv in this._dataIndex2Cell)
            {
                if (kv.Value.Equals(obj.GetComponent<RectTransform>()))
                {
                    SetSelectIndex(kv.Key);
                    break;
                }
            }
        }

        private void Collect(int index)
        {
            if (!_dataIndex2Cell.TryGetValue(index, out var cell))
                return;
            _dataIndex2Cell.Remove(index);
            cell.name = "PoolCell";
            cell.transform.SetParent(_poolTrans, false);
            _poolCells.Push(cell);
            _viewCellCount--;
        }

        private RectTransform Fetch()
        {
            var cell = GetRender();
            cell.transform.SetParent(_content, false);
            _viewCellCount++;
            return cell;
        }
        
        private RectTransform GetRender()
        {
            if (_poolCells.Count > 0)
            {
                return _poolCells.Pop();
            }
            var obj = Instantiate(_templete.gameObject, _content, false);
            obj.SetActive(true);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;
            rect.pivot = Vector2.up;
            rect.anchoredPosition = Vector2.zero;
        
            if (this.OnSelected != null)
            {
                obj.OnClick(OnClick, obj);
            }
            
            return rect;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _scrolling = false;
        }

        public override void OnUIShow()
        {
            
        }

        public override void OnUIClose()
        {
   
        }

        public override void OnUIDispose()
        {
            Dispose();
        }
    }
}