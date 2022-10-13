using System;
using ET;
using TMPro;
using UnityEngine;

namespace XGame
{
    public class UIDialog 
    {
        private enum ShowType
        {
            One,
            Two,
            Three,
        }
        
        public enum ClickedBtn
        {
            Right,
            Center,
            Left
        }

        private GameObject gameObject;
        private TextMeshProUGUI title;
        private TextMeshProUGUI desc;
        private TextMeshProUGUI leftText;
        private TextMeshProUGUI centerText;
        private TextMeshProUGUI rightText;
        private XImage leftBtn;
        private XImage rightBtn;
        private XImage centerBtn;

        private static UIDialog _instance;
        private ETTask<ClickedBtn> _task;

        private static UIDialog Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UIDialog();
                    _instance.gameObject = Resources.Load<GameObject>("UIDialog");
                    _instance.Awake();
                }

                return _instance;
            }
        }
        
        private void Awake()
        {
            var rc = this.gameObject.GetComponent<UIReferenceCollector>();
            this.title = rc.GetComponentFromGO<TextMeshProUGUI>("title");
            this.desc = rc.GetComponentFromGO<TextMeshProUGUI>("desc");
            this.leftText = rc.GetComponentFromGO<TextMeshProUGUI>("leftText");
            this.centerText = rc.GetComponentFromGO<TextMeshProUGUI>("centerText");
            this.rightText = rc.GetComponentFromGO<TextMeshProUGUI>("rightText");
            this.leftBtn = rc.GetComponentFromGO<XImage>("leftBtn");
            this.rightBtn = rc.GetComponentFromGO<XImage>("rightBtn");
            this.centerBtn = rc.GetComponentFromGO<XImage>("centerBtn");
            
            this.leftBtn.OnClick(this.OnLeftClick);
            this.rightBtn.OnClick(this.OnRightClick);
            this.centerBtn.OnClick(this.OnCenterClick);
        }

        private void OnRightClick()
        {
            this._task.SetResult(ClickedBtn.Right);
            this.gameObject.SetActive(false);
        }

        private void OnCenterClick()
        {
            this._task.SetResult(ClickedBtn.Right);
            this.gameObject.SetActive(false);
        }

        private void OnLeftClick()
        {
            this._task.SetResult(ClickedBtn.Right);
            this.gameObject.SetActive(false);
        }

        public static async ETTask<ClickedBtn> Show(string desc)
        {
            Instance._task = ETTask<ClickedBtn>.Create();
            Instance.SetShow(ShowType.Two);
            Instance.desc.text = desc;
            Instance.title.text = "提示";
            Instance.leftText.text = "取 消";
            Instance.rightText.text = "确 定";
            return await Instance._task;
        }
        
        public static async ETTask<ClickedBtn> Show(string desc, string title, string centerText)
        {
            Instance._task = ETTask<ClickedBtn>.Create();
            Instance.SetShow(ShowType.One);
            Instance.desc.text = desc;
            Instance.title.text = title;
            Instance.centerText.text = centerText;
            return await Instance._task;
        }
        
        public static async ETTask<ClickedBtn> Show(string desc, string title, string leftText, string rightText)
        {
            Instance._task = ETTask<ClickedBtn>.Create();
            Instance.SetShow(ShowType.Two);
            Instance.desc.text = desc;
            Instance.title.text = title;
            Instance.leftText.text = leftText;
            Instance.rightText.text = rightText;
            return await Instance._task;
        }
        
        public static async ETTask<ClickedBtn> Show(string desc, string title, string centerText, string leftText, string rightText)
        {
            Instance._task = ETTask<ClickedBtn>.Create();
            Instance.SetShow(ShowType.Three);
            Instance.desc.text = desc;
            Instance.title.text = title;
            Instance.centerText.text = centerText;
            Instance.leftText.text = leftText;
            Instance.rightText.text = rightText;
            return await Instance._task;
        }

        private void SetShow(ShowType showType)
        {
            Instance.gameObject.SetActive(true);
            switch (showType)
            {
                case ShowType.One:
                    this.leftBtn.gameObject.SetActive(false);
                    this.rightBtn.gameObject.SetActive(false);
                    this.centerBtn.gameObject.SetActive(true);
                    break;
                case ShowType.Two:
                    this.leftBtn.gameObject.SetActive(true);
                    this.rightBtn.gameObject.SetActive(true);
                    this.centerBtn.gameObject.SetActive(false);
                    break;
                case ShowType.Three:
                    this.leftBtn.gameObject.SetActive(true);
                    this.rightBtn.gameObject.SetActive(true);
                    this.centerBtn.gameObject.SetActive(true);
                    break;
            }
        }
    }
}