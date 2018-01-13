using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nyxeka
{
    public class SliderGroupHandler : SliderGroupContainer
    {

        public RectTransform _content;

        public float paddingUnderContent = 5;

        public float paddingOverContent = 5;

        public float individualHeight = 37.43f;

        public float contentHeightOffset;

        protected float defaultHeight;

        public GameObject imageOpen;
        public GameObject imageClosed;

        public UIButtonExtended bodyButton, expandButton;
        
        public Text title;

        public VerticalLayoutGroup contentLayout;

        //public SHA256 UID;

        public RectTransform _rect;

        protected bool expandable = false;

        public SliderGroupContainer[] childObjects;

        protected override void Awake()
        {
            base.Awake();
            _rect = GetComponent<RectTransform>();
            defaultHeight = _rect.rect.height;
            bodyButton.onClick.AddListener(OpenSliderList);
        }

        void OpenSliderList()
        {
            currentActiveModWindow.SetActiveGroup(this);
        }
        
        public override SliderGroupContainer AddSliderGroup(SliderGroupContainer prefab, string directoryPath, string name, int numChildren = 0)
        {
            this.numChildren++;
            childObjects[this.numChildren - 1] = Instantiate(prefab, _content.transform).InitSliderGroup(directoryPath, name, numChildren).UpdateParent(this);
            return childObjects[this.numChildren - 1];
        }
        
        public override void EnableExpanding()
        {
            expandButton.interactable = true;
            imageClosed.SetActive(true);
        }

        public override void RemoveSliderGroup(SliderGroupHandler toDelete)
        {
            //groupList.Remove(toDelete);
        }

        public override SliderGroupContainer InitSliderGroup(string directoryPath, string name, int numChildren = 0)
        {
            this.directoryPath = directoryPath;
            this.name = name;
            title.text = name;
            childObjects = new SliderGroupHandler[numChildren];
            return this;
        }

        /// <summary>
        /// Close the Slider Group.
        /// </summary>
        public void ResetState()
        {
            _content.gameObject.SetActive(false);
            if (expandable)
            {
                if (imageOpen != null && imageClosed != null)
                {
                    imageOpen.SetActive(false);
                    imageClosed.SetActive(true);
                }
            }
            UpdateHeightToFitContent();
        }

        public void ToggleContent()
        {

            if (_content.gameObject.activeSelf)//content is open
            {
                CloseContent();
                imageOpen.SetActive(false);
                imageClosed.SetActive(true);
            } else
            {
                OpenContent();
                imageOpen.SetActive(true);
                imageClosed.SetActive(false);
            }

        }

        public void OpenContent()
        {
            _content.gameObject.SetActive(true);
            UpdateHeightToFitContent();
        }

        public void CloseContent()
        {
            _content.gameObject.SetActive(false);
            UpdateHeightToFitContent();
        }

        protected void UpdateHeightToFitContent()
        {
            // start by setting some temp variables.
            Vector2 newSizeDeltaSelf = _rect.sizeDelta;
            Vector2 newSizeDeltaContent = _content.sizeDelta;

            // offset the temp variables by the new values
            newSizeDeltaContent.y = (numChildren * (individualHeight + contentLayout.spacing))
                + contentLayout.padding.top
                + contentLayout.padding.bottom
                + contentHeightOffset;

            _content.sizeDelta = newSizeDeltaContent;// resize content pane before updating this panes height.
            newSizeDeltaSelf.y = _content.gameObject.activeInHierarchy ? defaultHeight + paddingOverContent + paddingUnderContent + _content.rect.height : defaultHeight; // update this panes height to fit expanded content pane size.
            
            if (_parent is SliderGroupHandler) // now we wanna update the parent content pane to resize to fit our new expanded or closed state.
            {
                (_parent as SliderGroupHandler).contentHeightOffset += (newSizeDeltaSelf.y - _rect.sizeDelta.y);// update parent content height offset 
                (_parent as SliderGroupHandler).UpdateHeightToFitContent(); // refresh parent content size. We definitely wanna do this incase some weird thing happens where boxes are clipping over others when we open it.
            }
            _rect.sizeDelta = newSizeDeltaSelf;// finally apply our height.
        }
    }
}