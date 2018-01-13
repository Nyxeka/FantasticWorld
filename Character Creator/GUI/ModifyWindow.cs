using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace nyxeka
{
    public class ModifyWindow : MonoBehaviour
    {
        public SliderGroupContainer ActiveGroup { get; internal set; }

        public SliderHandler sliderPrefab;

        public SliderHandler AddSlider(SliderGroupContainer parentGroup, string sliderID, string _name, bool hasNegative, float defaultValue)
        {
            return Instantiate(sliderPrefab,transform).InitSliderHandler(this, parentGroup,sliderID, _name,hasNegative, defaultValue);
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
