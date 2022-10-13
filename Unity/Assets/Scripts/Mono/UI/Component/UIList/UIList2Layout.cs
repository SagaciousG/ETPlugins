using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    public partial class UIList
    {
        public enum Align
        {
            Horizontal,
            Vertical
        }
        
        public enum ListLayout
        {
            Vertical,
            Horizontal,
            Grid
        }
        
        public enum CenterType
        {
            None,
            MiddleCenter,
            VerticalCenter,
            HorizontalCenter,
        }

        [SerializeField]
        private ListLayout _layout = ListLayout.Vertical;
        [SerializeField]
        private Align _align = Align.Horizontal;
        [SerializeField]
        private int _alignNum = 1;
        [SerializeField]
        private float _spaceX;
        [SerializeField]
        private float _spaceY;
        [SerializeField]
        private Vector4 _padding; //上下左右
        [SerializeField]
        private CenterType _autoCenter;


        private void OnLayoutChange()
        {
            switch (_layout)
            {
                case ListLayout.Horizontal:
                    _align = Align.Vertical;
                    _alignNum = 1;
                    _spaceY = 0;
                    break;
                case ListLayout.Vertical:
                    _align = Align.Horizontal;
                    _alignNum = 1;
                    _spaceX = 0;
                    break;
                case ListLayout.Grid:
                    break;
            }
        }

        private void OnCenterTypeChange()
        {
            switch (_autoCenter)
            {
                case CenterType.None:
                    _sizeOfList = _viewportSize;
                    break;
                case CenterType.HorizontalCenter:
                    _sizeOfList = new Vector2(Mathf.Min(_content.sizeDelta.x, _viewportSize.x), _viewportSize.y);
                    break;
                case CenterType.MiddleCenter:
                    _sizeOfList = new Vector2(Mathf.Min(_content.sizeDelta.x, _viewportSize.x), Mathf.Min(_content.sizeDelta.y, _viewportSize.y));
                    break;
                case CenterType.VerticalCenter:
                    _sizeOfList = new Vector2(_viewportSize.x, Mathf.Min(_content.sizeDelta.y, _viewportSize.y));
                    break;
            }

            // _viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _sizeOfList.x);
            // _viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _sizeOfList.y);
        }
        
        private void UpdateContentSize()
        {
            OnLayoutChange();
            var size = _templete.rect.size;
            var rowCount = 0; //行
            var colCount = 0; //列
            switch (_align)
            {
                case Align.Vertical:
                    rowCount = Mathf.Min(_alignNum, _dataNum);
                    colCount = Mathf.Min(Mathf.CeilToInt(_dataNum * 1f / _alignNum), _dataNum);
                    break;
                case Align.Horizontal:
                    colCount = Mathf.Min(_alignNum, _dataNum);
                    rowCount = Mathf.Min(Mathf.CeilToInt(_dataNum * 1f / _alignNum), _dataNum);
                    break;
            }

            var h = size.y * rowCount + _spaceY * (rowCount - 1) + _padding.x + _padding.y;
            var w = size.x * colCount + _spaceX * (colCount - 1) + _padding.z + _padding.w;
            _content.sizeDelta = new Vector2(w, h);
        }
        
        private void SetCellPos(int fromIndex, int toIndex)
        {
            if (toIndex < _dataNum)
            {
                if (!_dataIndex2Cell.TryGetValue(fromIndex, out var cell))
                {
                    cell = Fetch();
                }
                else
                {
                    _dataIndex2Cell.Remove(fromIndex);
                }
                var size = _templete.rect.size;
                Vector2Int point = Vector2Int.zero;
                switch (_align)
                {
                    case Align.Horizontal:
                        point.x = toIndex % _alignNum;
                        point.y = toIndex / _alignNum;
                        break;
                    case Align.Vertical:
                        point.x = toIndex / _alignNum;
                        point.y = toIndex % _alignNum;
                        break;
                }

                var x = point.x * size.x + point.x * _spaceX + _padding.z;
                var y = point.y * size.y + point.y * _spaceY + _padding.x;

                _dataIndex2Cell[toIndex] = cell;
                this.OnData?.Invoke(toIndex, cell, this._data[toIndex], this.RootUI);
                if (this._selectedIndex > -1)
                    this.OnSelected?.Invoke(cell, toIndex == this._selectedIndex, this._data[toIndex], toIndex, this.RootUI);
                cell.anchoredPosition = new Vector2(x, -y);
            }
            else
            {
                Collect(fromIndex);
            }
        }
        
        private void UpdateMaxViewCellCount()
        {
            var size = _viewport.rect.size;
            var rect = _templete.rect.size;
            var rowMax = 0;
            var colMax = 0;
            var rowNeed = 0;
            var colNeed = 0;
            switch (_align)
            {
                case Align.Horizontal:
                    colMax = Mathf.CeilToInt((size.y + _spaceY) / (rect.y + _spaceY)) + 1;
                    rowNeed = Mathf.Min(_alignNum, _dataNum);
                    colNeed = Mathf.Min(Mathf.CeilToInt(_dataNum * 1f / _alignNum), colMax);
                    break;
                case Align.Vertical:
                    rowMax = Mathf.CeilToInt((size.x + _spaceX) / (rect.x + _spaceX)) + 1;
                    colNeed = Mathf.Min(_alignNum, _dataNum);
                    rowNeed = Mathf.Min(Mathf.CeilToInt(_dataNum * 1f / _alignNum), rowMax);
                    break;
            }

            _maxViewCellCount = Mathf.Min(rowNeed * colNeed, _dataNum);
        }
        
        private void UpdateViewFirstCellIndex()
        {
            var size = _templete.rect.size;
            _lastFirstIndex = _curFirstIndex;
            var index = 0;
            switch (_align)
            {
                case Align.Horizontal:
                    var h = _content.anchoredPosition.y - size.y - _padding.x;
                    var col = Mathf.FloorToInt(h / (size.y + _spaceY)) + 1;
                    col = col < 0 ? 0 : col;
                    index = col * _alignNum;
                    break;
                case Align.Vertical:
                    var w = -1 * _content.anchoredPosition.x - size.x - _padding.z;
                    var raw = Mathf.FloorToInt(w / (size.x + _spaceX)) + 1;
                    raw = raw < 0 ? 0 : raw;
                    index = raw * _alignNum;
                    break;
            }

            _curFirstIndex = Mathf.Min(index, _maxFirstCellIndex);
        }
        
        private void UpdateMaxFirstCellIndex()
        {
            var size = _templete.rect.size;
            switch (_align)
            {
                case Align.Horizontal:
                    var diffY = _content.sizeDelta.y - _viewport.rect.height - _padding.x - _padding.y;
                    var col = Mathf.FloorToInt((diffY + _spaceY) / (size.y + _spaceY));
                    col = col < 0 ? 0 : col;
                    _maxFirstCellIndex = col * _alignNum;
                    break;
                case Align.Vertical:
                    var diffX = _content.sizeDelta.x - _viewport.rect.width - _padding.z - _padding.w;
                    var raw = Mathf.FloorToInt((diffX + _spaceX) / (size.x + _spaceX));
                    raw = raw < 0 ? 0 : raw;
                    _maxFirstCellIndex = raw * _alignNum;
                    break;
            }
        }
    }
}