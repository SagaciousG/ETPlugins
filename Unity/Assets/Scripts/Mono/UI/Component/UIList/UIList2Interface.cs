﻿using System;
using System.Collections;
 using ET;
 using UnityEngine;

namespace XGame
{
    public partial class UIList
    {
        //不通过返回true，通过返回false
        public event Func<int, object, bool> SelectCheckHandler;
        public event Action<int, RectTransform, object, Entity> OnData;
        public event Action<RectTransform, bool, object, int, Entity> OnSelected; 
        public int SelectedIndex => _selectedIndex;
        public int DataNum => _dataNum;
        public object SelectedData => _data[_selectedIndex];
        public RectTransform Content => this._content;
        public RectTransform Viewport => this._viewport;
        public RectTransform RenderCell => this._templete;
        
        public Entity RootUI { private set; get; }

        public int AlignNum
        {
            get => this._alignNum;
            set
            {
                if (this._alignNum == value)
                    return;
                this._alignNum = value;
                this.UpdateLayout();
            }
        }

        public float SpaceX
        {
            get => _spaceX;
            set
            {
                if (this._spaceX == value)
                    return;
                this._spaceX = value;
                this.UpdateLayout();
            }
        }

        public float SpaceY
        {
            get => _spaceY;
            set
            {
                if (this._spaceY == value)
                    return;
                this._spaceY = value;
                this.UpdateLayout();
            }
        }

        public ListLayout Layout
        {
            set
            {
                if (_layout == value)
                    return;
                _layout = value;
                UpdateLayout();
            }
            get => _layout;
        }

        public CenterType AutoCenter
        {
            set
            {
                if (_autoCenter == value)
                    return;
                _autoCenter = value;
                UpdateLayout();
            }
            get => _autoCenter;
        }
        
        // public void SetCellRender<T>() where T : UIListCellBase
        // {
        //     _cellRenderType = typeof (T);
        // }

        public void SetData(IList data, Entity ui)
        {
            _data = data;
            RootUI = ui;
            _dataNum = data?.Count ?? 0;
            _selectedIndex = -1;
            UpdateLayout();
        }
        
        public void ScrollToIndex(int index, float speed = 0.5f)
        {
            index = Mathf.Clamp(index, 0, int.MaxValue);
            if (Mathf.Clamp(index, _curFirstIndex, _curFirstIndex + _maxViewCellCount - 1) == index)
                return;
            _scrolling = true;
            _scrollSpeed = speed;
            Vector2Int point = Vector2Int.zero;
            var size = _templete.rect.size;
            switch (_align)
            {
                case Align.Horizontal:
                    point.x = index % _alignNum;
                    point.y = index / _alignNum;
                    break;
                case Align.Vertical:
                    point.x = index / _alignNum;
                    point.y = index % _alignNum;
                    break;
            }

            var x = point.x * size.x + point.x * _spaceX + _padding.z;
            var y = point.y * size.y + point.y * _spaceY + _padding.x;
            var maxX = Mathf.Clamp(_content.rect.width - _viewport.rect.width, 0, int.MaxValue);
            var maxY = Mathf.Clamp(_content.rect.height - _viewport.rect.height, 0, int.MaxValue);
            x = Mathf.Min(x, maxX);
            y = Mathf.Min(y, maxY);
            _toPoint = new Vector2(-x,y);
        }
        
        public void SetSelectIndex(int index, bool scrollTo = true)
        {
            if (_data == null)
            {
                return;
            }
            if (index < -1 || index >= _data.Count)
                return;
            if (_selectedIndex == index)
                return;
            if (SelectCheckHandler?.Invoke(index, _data[index]) ?? false)
                return;
            var lastIndex = _selectedIndex;
            _selectedIndex = index;
            if (_dataIndex2Cell.TryGetValue(_selectedIndex, out var cell))
            {
                this.OnSelected?.Invoke(cell, true, this._data[index], this._selectedIndex, this.RootUI);
            }

            if (_dataIndex2Cell.TryGetValue(lastIndex, out var last))
            {
                this.OnSelected?.Invoke(last, lastIndex == index, this._data[index], lastIndex, this.RootUI);
            }
            
            if (scrollTo)
                ScrollToIndex(index);
        }
        
        public void UpdateLayout()
        {
            if (_start)
            {
                OnLayoutChange();
                UpdateContentSize();
                OnCenterTypeChange();
                UpdateMaxFirstCellIndex();
                UpdateMaxViewCellCount();
                UpdateViewFirstCellIndex();
                UpdateCellCount(true);
            }
        }

        public void UpdateViewCells()
        {
            foreach (var kv in _dataIndex2Cell)
            {
                if (kv.Key < _data.Count)
                    this.OnData?.Invoke(kv.Key, kv.Value, this._data[kv.Key], this.RootUI);
            }
        }

        public void Dispose()
        {
            foreach (var cell in _poolCells)
            {
                Destroy(cell);
            }

            foreach (var cell in _dataIndex2Cell.Values)
            {
                Destroy(cell);
            }
            _poolCells.Clear();
            _dataIndex2Cell.Clear();
            _data.Clear();
            _selectedIndex = -1;
        }
    }
    
}