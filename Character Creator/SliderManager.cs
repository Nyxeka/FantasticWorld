using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka
{

    public class SliderManager : MonoBehaviour
    {

        public static SliderManager instance = null;

        public SliderTree tree;

        public SliderGUIHandler sliderGUIHandler;

        public DatabaseReader dbReader;

        private bool busy = false;

        private int batchSize = 20;

        private bool initialized = false;

        public void InitSliderManager(SliderGUIHandler sliderGUIHandler, DatabaseReader dbReader)
        {
            if (!initialized)
            {
                this.sliderGUIHandler = sliderGUIHandler;
                this.dbReader = dbReader;

                //tree = m3DHandler.morphSliderTree;
                InitSliderGUIHandler();
                initialized = true;
            }

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

            LoadTreeToChar();

        }


        void Awake()
        {
            instance = this;
        }

        void OnDestroy()
        {
            instance = null;
        }

        public static void UpdateSlider(Slider newSliderData)
        {
            if (instance != null)
            {
                instance.tree.SetSliderAtDir(newSliderData);
                M3DHandler.UpdateSlider(newSliderData.sliderID, Convert.ToSingle(newSliderData.sliderValue));
            }
        }

        public static bool UpdateSliderCheck(Slider newSliderData)
        {
            if (instance != null)
            {
                if(M3DHandler.UpdateSliderCheck(newSliderData.sliderID, Convert.ToSingle(newSliderData.sliderValue)))
                {
                    instance.tree.SetSliderAtDir(newSliderData);
                    return true;
                }
                
            }

            return false;
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

                    if (sliderType == typeof(float) || sliderType == typeof(int))
                    {
                        if (M3DHandler.UpdateSliderCheck(sliderList[i].sliderID, Convert.ToSingle( sliderList[i].sliderValue)))
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
            if (instance != null)
                instance.SaveMorphs();
        }

        public void SaveMorphs()
        {
            if (!busy)
            {
                //tree = sliderGUIHandler.GetTree();
                
                if (tree != null)
                {

                    //PublicToastHandler.GiveMessage("Updated Sliders: " + tree.GetCompleteSliderList().Count);

                    if (tree.sliderNodeList.Count > 0)
                    {
                        dbReader.SaveSliderTree(tree);
                    }

                    Debug.Log("<color=blue> Successfully saved character to file.</color>");
                    PublicToastHandler.GiveMessage("Saved Morphs.");
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

        }
    }

}