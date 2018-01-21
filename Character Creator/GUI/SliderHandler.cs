using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace nyxeka
{
    public class SliderHandler : MonoBehaviour
    {

        public Text title;

        public bool hasNegative = false;

        public UnityEngine.UI.Slider _sliderInput;

        public nyxeka.Slider _slider;
        public nyxeka.Slider _sliderNeg;

        public bool locked = false;

        public SliderGroupContainer parentGroup;

        public ModifyWindow parentWindow;

        private void Start()
        {

            _sliderInput.onValueChanged.AddListener(UpdateM3DSlider);

        }

        private void Awake()
        {
            if (_sliderInput == null)
            {
                _sliderInput = gameObject.GetComponent<UnityEngine.UI.Slider>();
            }
        }

        public void UnHide()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetLockState(bool locked)
        {
            if (locked) Lock();
            else Unlock();
        }

        public void Lock()
        {
            locked = true;
        }

        public void Unlock()
        {
            locked = false;
            if (parentWindow.ActiveGroup != parentGroup)
            {
                Hide();
            }
        }

        public SliderHandler InitSliderHandler(ModifyWindow modifyWindow, SliderGroupContainer parentGroup, Slider _slider, float defaultValue = 0)
        {
            this.parentWindow = modifyWindow;
            this.parentGroup = parentGroup;
            this._slider = _slider;
            this.hasNegative = false;
            _sliderInput.value = defaultValue / 100;
            title.text = _slider.sliderName;
            _sliderInput.minValue = 0;

            return this;
        }
        public SliderHandler InitSliderHandler(ModifyWindow modifyWindow, SliderGroupContainer parentGroup, Slider _slider, Slider _sliderNeg, float defaultValue = 0)
        {
            //Debug.Log("Creating Sliders with negative... " + _slider.sliderName);
            this.parentWindow = modifyWindow;
            this.parentGroup = parentGroup;
            this._slider = _slider;
            this._sliderNeg = _sliderNeg;
            this.hasNegative = true;
            _sliderInput.value = defaultValue / 100;
            title.text = _slider.sliderName;
            _sliderInput.minValue = -1;

            return this;
        }

        void UpdateM3DSlider(float value)
        {

            if (hasNegative)
            {
                if (value < 0)
                {
                    _sliderNeg.sliderValue = Mathf.Abs(value * 100);
                    _slider.sliderValue = 0;
                } else if (value > 0)
                {
                    _slider.sliderValue = value * 100;
                    _sliderNeg.sliderValue = 0;
                }
                else//if value == 0
                {
                    _slider.sliderValue = 0;
                    _sliderNeg.sliderValue = 0;
                }
                SliderManager.UpdateSlider(_slider);
                SliderManager.UpdateSlider(_sliderNeg);
            } else
            {
                _slider.sliderValue = value * 100;
                SliderManager.UpdateSlider(_slider);
            }
        }

    }
}