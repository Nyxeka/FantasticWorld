using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace nyxeka
{
    public class SliderHandler : MonoBehaviour
    {

        public Text title;

        public bool hasNegative;

        public string sliderID;
        
        protected float sliderValue;

        public UnityEngine.UI.Slider _sliderInput;

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

        public SliderHandler InitSliderHandler(ModifyWindow modifyWindow, SliderGroupContainer parentGroup, string sliderID, string newName, bool hasNegative, float defaultValue = 0)
        {
            this.parentWindow = modifyWindow;
            this.parentGroup = parentGroup;
            this.sliderID = sliderID;
            this.hasNegative = hasNegative;
            sliderValue = defaultValue/100;
            _sliderInput.value = sliderValue;
            title.text = newName;
            if (hasNegative)
            {
                _sliderInput.minValue = -1;
            } else
            {
                _sliderInput.minValue = 0;
            }

            return this;
        }

        void UpdateM3DSlider(float value)
        {
            //print("attempting to update slider value...");
            //so, we get sliderID, check if negative. Then check if value is negative. Swap it to positive and apply.
            if (hasNegative)
            {
                if (value < 0)
                {
                    M3DHandler.UpdateSliderCheck(sliderID, 0);
                    M3DHandler.UpdateSlider(sliderID + M3DHandler._neg, Mathf.Abs(value*100));
                } else if (value > 0)
                {
                    M3DHandler.UpdateSlider(sliderID, value*100);
                    M3DHandler.UpdateSliderCheck(sliderID + M3DHandler._neg, 0);
                }
                else//if value == 0
                {
                    M3DHandler.UpdateSliderCheck(sliderID, 0);
                    M3DHandler.UpdateSliderCheck(sliderID + M3DHandler._neg, 0);
                }
            } else
            {
                M3DHandler.UpdateSlider(sliderID, value * 100);
            }
        }

    }
}