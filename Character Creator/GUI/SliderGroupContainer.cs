using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace nyxeka
{
    public class SliderGroupContainer : MonoBehaviour
    {

        protected List<SliderGroupHandler> groupList = new List<SliderGroupHandler>();

        protected string ContentName;

        protected string directoryPath;

        public bool hasChildren;

        protected int numChildren = 0;

        public SliderGroupContainer _parent;
        
        public SliderHandler[] sliders;

        protected ModifyWindow currentActiveModWindow;

        public SliderGroupContainer SetSliders(SliderHandler[] newSliders, ModifyWindow newModWindow)
        {
            sliders = newSliders;
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].Hide();
            }
            currentActiveModWindow = newModWindow;
            return this;
        }

        protected virtual void Awake()
        {
        }

        public SliderGroupContainer UpdateParent(SliderGroupContainer newParent)
        {

            _parent = newParent;

            return this;

        }

        public void OpenSliderGroup()
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                if (!sliders[i].locked)
                {
                    sliders[i].UnHide();
                }
            }
        }

        public void CloseSliderGroup()
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                if (!sliders[i].locked)
                {
                    sliders[i].Hide();
                }
            }
        }

        public virtual SliderGroupContainer AddSliderGroup(SliderGroupContainer prefab, string directoryPath, string name, int numChildren = 0)
        {

            this.numChildren++; 
            //Debug.Log("Making new slider group directory!");
            return Instantiate(prefab, transform).InitSliderGroup(directoryPath, name,numChildren).UpdateParent(this);

        }

        public virtual SliderGroupContainer InitSliderGroup(string directoryPath, string name, int numChildren = 0)
        {
            return this;
        }

        public virtual void EnableExpanding() { }

        public virtual void RemoveSliderGroup(SliderGroupHandler toDelete)
        {
            groupList.Remove(toDelete);
        }
    }
}
