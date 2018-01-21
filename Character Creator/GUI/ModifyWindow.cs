using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace nyxeka
{
    public class ModifyWindow : MonoBehaviour
    {
        public SliderGroupContainer ActiveGroup { get; internal set; }

        public SliderHandler sliderPrefab;

        public SliderHandler AddSlider(SliderGroupContainer parentGroup, Slider _slider, float defaultValue)
        {
            return Instantiate(sliderPrefab, transform).InitSliderHandler(this, parentGroup, _slider, defaultValue);
        }

        public SliderHandler AddSlider(SliderGroupContainer parentGroup, Slider _slider, Slider _sliderNeg, float defaultValue)
        {
            return Instantiate(sliderPrefab, transform).InitSliderHandler(this, parentGroup, _slider, _sliderNeg, defaultValue);
        }

        public void SetActiveGroup(SliderGroupContainer newActiveGroup)
        {
            if (ActiveGroup != null)
                ActiveGroup.CloseSliderGroup();
            ActiveGroup = newActiveGroup;
            ActiveGroup.OpenSliderGroup();

        }
    }
}
