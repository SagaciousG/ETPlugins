using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace XGame
{
    public class UILoading 
    {
        private static UILoading _instance;

        private GameObject gameObject;
        private TextMeshProUGUI proText;
        private XImage slider;

        private Tween _tween;

        private static UILoading Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UILoading();
                    _instance.gameObject = Resources.Load<GameObject>("UILoading");
                    _instance.Awake();
                }

                return _instance;
            }
        }

        private void Awake()
        {
            var rc = this.gameObject.GetComponent<UIReferenceCollector>();
            this.proText = rc.GetComponentFromGO<TextMeshProUGUI>("proTxt");
            this.slider = rc.GetComponentFromGO<XImage>("slider");
        }

        public static void Set(float progress, string tips)
        {
            Instance.slider.fillAmount = progress;
            Instance.proText.text = tips;
        }
        
        public static void Show(float progress, string tips, Action onClose = null)
        {
            Instance.gameObject.SetActive(true);
            if (Instance._tween != null)
                Instance._tween.Kill();
            Instance._tween = DOTween.To(() => Instance.slider.fillAmount, a => Instance.slider.fillAmount = a, progress, 0.5f);
            Instance._tween.onComplete = () =>
            {
                if (progress >= 1)
                {
                    Hide();
                    onClose?.Invoke();
                }
            };
        }

        public static void Hide()
        {
            Instance.gameObject.SetActive(false);
        }
        
        
    }
}