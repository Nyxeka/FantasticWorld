using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka
{

    public class SliderManager : MonoBehaviour
    {

        protected static SliderManager mInstance;

        public SliderTree tree;

        public SliderGUIHandler sliderGUIHandler;

        public DatabaseReader dbReader;

        private bool busy = false;

        private int batchSize = 20;

        public void InitSliderManager(SliderGUIHandler sliderGUIHandler, DatabaseReader dbReader)
        {
            
            this.sliderGUIHandler = sliderGUIHandler;
            this.dbReader = dbReader;

            //tree = m3DHandler.morphSliderTree;
            InitSliderGUIHandler();

        }

        void InitSliderGUIHandler()
        {
            
            tree = dbReader.GetDefaultSliderTree(); //works for now.

            if (tree == null)
            {

                tree = dbReader.GetSliderTree(Application.dataPath + "/../Characters/backup.fwc");

            }

            sliderGUIHandler.SetTree(tree);

            if (sliderGUIHandler)
            {
                if (sliderGUIHandler.CheckDelegateNull())
                {
                    SliderGUIHandler.MorphSliderUpdateDelegate += UpdateM3D;
                    Debug.Log("Assigned slider to morph update delegate.");
                }
            }

            LoadTreeToChar();

        }

        void Awake()
        {
            mInstance = this;
        }

        void OnDestroy()
        {
            mInstance = null;
        }

        public static void UpdateSlider(string sliderID, float newValue)
        {
            if (mInstance != null)
            {
                mInstance.UpdateM3D(sliderID, newValue);
            }
        }

        public void UpdateM3D(string sliderID, float newValue)
        {

            // update the tree in M3DHandler slider tree.
            //Debug.Log("Updating morph");

            M3DHandler.UpdateSlider(sliderID, newValue);

            //Debug.Log("<color=green>Updated morph</color>");

        }

        public void Cleanup()
        {

            SliderGUIHandler.MorphSliderUpdateDelegate -= UpdateM3D;

        }

        IEnumerator UpdateMorphs()
        {
            //slowly update all character morphs. 
            List<SliderIndexInfo> sliderList = tree.GetCompleteSliderList();

            Debug.Log("Loading Morphs from File. Slider Count: " + sliderList.Count.ToString());

            if (!busy)
            {
                busy = true;

                int count = 0;
                for (int i = 0; i < sliderList.Count; i++)
                {

                    Type sliderType = sliderList[i].value.GetType();

                    if (sliderType == typeof(float) || sliderType == typeof(int) || sliderType == typeof(System.Single))
                    {
                        if (M3DHandler.UpdateSliderCheck(sliderList[i].sliderID, (float)sliderList[i].value))
                        {

                            count++;

                        }

                    }
                    if (count > batchSize)
                    {

                        yield return null;

                    }

                    //can also check for things like "colours" and stuff like that, once we get more complicated.

                }

                Debug.Log("Successfully loaded morphs from file.");

                //m3DHandler.RecalculateBounds();

                busy = false;
            }

        }

        public void LoadTreeToChar()
        {
            //Debug.Log("");
            StartCoroutine("UpdateMorphs");

        }

        public void SaveMorphs()
        {
            if (!busy)
            {
                tree = sliderGUIHandler.GetTree();

                if (tree != null)
                {
                    if (tree.sliderNodeList.Count > 0)
                        dbReader.SaveSliderTree(tree);

                    Debug.Log("<color=blue> Successfully saved character to file.</color>");
                }

                else
                {

                    Debug.Log("<color=red>Warning: Cancelled attrempt to over-write default character with 'null'</color>");

                }
            }
            else
            {

                Debug.Log("<color=yellow>Save morphs cancelled, since sliders are still busy loading.</color>");

            }


        }

        void OnDisable()
        {
            SaveMorphs();
            Cleanup();
            Debug.Log("Cleaned up Delegates.");

        }
    }

}