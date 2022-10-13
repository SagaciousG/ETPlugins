using System;
using UnityEngine;
using UnityEngine.UI;

namespace XGame
{
    public class XProgress : MonoBehaviour
    {
        public XImage Slider;

        public float Value
        {
            get => Slider.fillAmount;
            set => Slider.fillAmount = value;
        }
        
        private void Start()
        {
            if (Slider != null)
                Slider.type = Image.Type.Filled;
        }
    }
}