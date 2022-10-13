using UnityEngine;
using UnityEngine.UI;

namespace XGame
{
    public class XSlider : Slider
    {
        [SerializeField]
        private Text _sliderText;

        private bool _customText;
        
        public string text
        {
            set
            {
                _customText = true;
                if (_sliderText != null)
                    _sliderText.text = value;
            }
            get
            {
                if (_sliderText != null)
                    return _sliderText.text;
                return null;
            }
        }

        public override float value
        {
            get => base.value;
            set
            {
                if (!_customText)
                {
                    if (_sliderText != null)
                        _sliderText.text = $"{(Mathf.Floor(value / maxValue * 100))}%";
                }

                base.value = value;
            }
        }
    }
}