using System;
using System.Collections;
using System.Collections.Generic;
using ET;
using UnityEngine;
using UnityEngine.UI;

namespace XGame
{
    public class XPopup : MonoBehaviour
    {
        [SerializeField] private XText _title;
        [SerializeField] private Graphic _clickArea;
        [SerializeField] private XText _selectText;
        [SerializeField] private XImage _arrowOn;
        [SerializeField] private XImage _arrowOff;
        [SerializeField] private UIList _popUp;
        [SerializeField] private UIRectPointListener _areaListener;
        private IList _sourceData;
        private bool _isShowPopup;
        private bool _customRender;
        private bool _popdownOnSelect = true;
        private List<string> _showNames = new List<string>();
        
        [NonSerialized]
        public int defaultSelectIndex = 0;

        
        public event Action<int, object> SelectHandler;
        public event Func<int, object, bool> SelectCheckHandler;

        public bool ClickOtherAreaClose;

        private void InAreaListener(bool obj)
        {
            if (!obj)
            {
                _isShowPopup = false;
                Pop(false);
            }
        }

        public int selectedIndex
        {
            set => SelectIndex(value);
            get => _popUp.SelectedIndex;
        }
        
        public string label
        {
            set
            {
                if (_title != null)
                    _title.text = value;
            }
            get => _title?.text ?? "";
        }

        public object SelectedData
        {
            get => _sourceData[selectedIndex];
        }

        public void SetData(IList data, Entity rootUI, string fieldName = null)
        {
            _sourceData = data;

            if (_customRender)
            {
                _popUp.SetData(data, rootUI);
            }
            else
            {
                _showNames.Clear();
                foreach (var d in data)
                {
                    var str = string.IsNullOrEmpty(fieldName) ? d.ToString() : ObjectHelper.GetFieldValue(d, fieldName).ToString();
                    _showNames.Add(str);
                }
                _popUp.SetData(_showNames, rootUI);
            }
        }

        // public void SetCellRender<T>() where T : UIListCellBase, IUIPopupDataItem
        // {
        //     _customRender = true;
        //     _popUp.SetCellRender<T>();
        // }

        public void SelectIndex(int index)
        {
            _popUp.SetSelectIndex(index);
        }

        public void ScrollToIndex(int index)
        {
            _popUp.ScrollToIndex(index);
        }
        
        private void Awake()
        {
            _clickArea.OnClick(OnClick);
            // _popUp.SetCellRender<UIPopupCell>();
            _popUp.OnSelected += OnSelected;
            _popUp.SelectCheckHandler += OnSelectCheck;
            _areaListener.OnClickInArea += InAreaListener;
        }

        private void OnSelected(RectTransform arg1, bool arg2, object arg3, int arg4, Entity arg5)
        {
            throw new NotImplementedException();
        }

        private bool OnSelectCheck(int arg1, object arg2)
        {
            return SelectCheckHandler?.Invoke(arg1, arg2) ?? false;
        }

        private void OnSelect(int arg1, object arg2)
        {
            if (_customRender)
            {
                if (arg2 is IUIPopupDataItem item)
                {
                    _selectText.text = item.GetShowText();
                }
            }
            else
            {
                if (_showNames.Count == 0)
                {
                    _selectText.text = "";
                    return;
                }

                _selectText.text = (string) arg2;
            }
            SelectHandler?.Invoke(arg1, _sourceData[arg1]);
            if (_popdownOnSelect)
            {
                Pop(false);
            }
        }

        private void Start()
        {
            Pop(false);
            SelectIndex(defaultSelectIndex);
        }

        private void OnClick()
        {
            Pop(!_isShowPopup);
        }

        private void Pop(bool show)
        {
            _isShowPopup = show;
            _popUp.gameObject.Display(show);
            _arrowOn?.Display(show);
            _arrowOff?.Display(!show);
            if (_areaListener != null)
                _areaListener.enabled = show;
        }
        
    }
    
    public interface IUIPopupDataItem
    {
        string GetShowText();
    }
}