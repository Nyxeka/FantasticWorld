using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka
{

    public class SliderManager : MonoBehaviour
    {

        public static SliderManager mInstance;

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

            tree.UpdateCompleteSliderList();

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

        public static void UpdateSlider(string sliderID, float newValue, int index)
        {
            if (mInstance != null)
            {
                mInstance.tree.GetCompleteSliderList()[index].sliderValue = newValue;
                M3DHandler.UpdateSlider(sliderID, newValue);
            }
        }

        public static bool UpdateSliderCheck(string sliderID, float value, int index)
        {
            if (mInstance != null)
            {
                if(M3DHandler.UpdateSliderCheck(sliderID, value))
                {
                    mInstance.tree.GetCompleteSliderList()[index].sliderValue = value;
                    return true;
                }
                
            }

            return false;
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
            
            List<Slider> sliderList = tree.GetCompleteSliderList();

            Debug.Log("Loading Morphs from File. Slider Count: " + sliderList.Count.ToString());
            PublicLoadingBar.AddNewStuff(sliderList.Count, 10);
            if (!busy)
            {
                busy = true;

                int count = 0;
                for (int i = 0; i < sliderList.Count; i++)
                {

                    Type sliderType = sliderList[i].sliderValue.GetType();

                    if (sliderType == typeof(float) || sliderType == typeof(int) || sliderType == typeof(System.Single))
                    {
                        if (M3DHandler.UpdateSliderCheck(sliderList[i].sliderID, (float)sliderList[i].sliderValue))
                        {

                            count++;

                        }

                    }
                    if (count > batchSize)
                    {
                        count = 0;// reset the count for the next iteration.
                        yield return null;

                    }
                    PublicLoadingBar.UpdateProgress(1);
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

        public static void SaveCurrentCharacter()
        {
            mInstance.SaveMorphs();
        }

        public void SaveMorphs()
        {
            if (!busy)
            {
                tree = sliderGUIHandler.GetTree();

                if (tree != null)
                {
                    if (tree.sliderNodeList.Count > 0)
                    {
                        PublicLoadingBar.AddNewStuff(1, 5);
                        dbReader.SaveSliderTree(tree);
                        PublicLoadingBar.UpdateProgress(1);
                    }

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