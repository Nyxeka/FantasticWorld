using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MORPH3D;
using MORPH3D.FOUNDATIONS;
using System.Text;
using System.IO;
using System;

namespace nyxeka
{

    public enum SliderType
    {

        MTD, //M3D slider
        Color, //colour picker for the shader
        Text, //set text
        Index //for images and stuff.

    }

    public class M3DHandler : MonoBehaviour
    {

        private M3DCharacterManager manager;

        public SliderTree morphSliderTree;

        private bool safeToUpdateSliders = true;

        public static M3DHandler instance = null;

        public static string _neg = "_NEGATIVE_";

        private void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {
            print("initiated m3dhandler");
            manager = gameObject.GetComponent<M3DCharacterManager>();

            morphSliderTree = new SliderTree();

            // we create the tree from here, so we should be able to do a check to see if it's working and exists
            //LoadMorphCollection(Application.dataPath + "/MORPH3D/Content/M3DFemale/Figure/M3DFemale/MorphGroups.csv");

            //StartCoroutine(WaitAndRun());

            if (manager.CostumeBoundsUpdateFrequency == MORPH3D.CONSTANTS.COSTUME_BOUNDS_UPDATE_FREQUENCY.ON_MORPH)
            {

                Debug.LogError("WARNING: set recalculate bounds update frequency ENABLED. WILL cause memory leak.");
                safeToUpdateSliders = false;

            }
        }

        IEnumerator WaitAndRun()
        {
            
            yield return new WaitForSeconds(5.0f);
            LoadMorphCollection(Application.dataPath + "/MORPH3D/Content/M3DFemale/Figure/M3DFemale/MorphGroups.csv");

        }

        public void RecalculateBounds()
        {

            manager.ResyncBounds();

        }

        public static bool UpdateSliderCheck(string sliderID, float value)
        {
            if (instance != null)
            {
                if (instance.safeToUpdateSliders)
                {
                    if (instance.manager.coreMorphs.morphLookup[sliderID].value != value)
                    {
                        instance.manager.SetBlendshapeValue(sliderID, value);
                        return true;
                    }

                }
            }

            return false;

        }

        public static void UpdateSlider(string sliderID, float value)
        {
            if (instance != null)
            {
                if (instance.safeToUpdateSliders)
                {
                    if (instance.manager.coreMorphs.morphLookup.ContainsKey(sliderID))
                    {
                        instance.manager.SetBlendshapeValue(sliderID, value);
                        //print("updated slider value under: " + instance.transform.root.name);
                    }
                }
            }
        }

        public static void UpdateSlider(string sliderID, float value, SliderType sType)
        {
            if (instance != null)
            {
                if (instance.safeToUpdateSliders)
                {

                    switch (sType)
                    {
                        case SliderType.MTD:
                            if (instance.manager.coreMorphs.morphLookup.ContainsKey(sliderID))
                            {
                                instance.manager.SetBlendshapeValue(sliderID, value);
                                //print("updated slider value under: " + instance.transform.root.name);
                            }
                            else
                            {
                                Debug.Log("<color=red>ERROR: attempting to update a morph slider that doesn't appear to exist: </color>" + sliderID);
                            }
                            break;
                    }

                }
            }
        }

        /// <summary>
        /// This is solely for the purpose of generating a slider tree template using the .csv file that the MCS team provides with the MCS asson for unity.
        /// </summary>
        /// <param name="fileName">the properly formatted .csv file</param>
        /// <returns></returns>
        private bool LoadMorphCollection(string fileName)
        {

            //load directly into morph slider tree.
            
            try
            {

                string line = "";

                StreamReader csvReader = new StreamReader(fileName, Encoding.Default);

                using (csvReader)
                {

                    do
                    {

                        line = csvReader.ReadLine();
                        //Debug.Log(line);
                        if (line != null)
                        {
                            string[] entries = line.Split(',');


                            if (entries.Length == 3)
                            {//this is a line in our pre-formatting csv for sure.
                             //Debug.Log(entries[0] + entries[1] + entries[2]);
                                string[] subCategories = entries[2].Split('/');

                                morphSliderTree.AddToSubnode(subCategories, new Slider(manager.coreMorphs.morphLookup[entries[0]].value, manager.coreMorphs.morphLookup[entries[0]].displayName, entries[0], entries[2]));
                                //TO-DO: Check for a Negative value. If it has one, then get the slider and set hasnegative to true.
                                //ALSO need to write a function in sliderTree that lets you get a slider at a specific DIR.
                            }
                        }

                    } while (line != null);

                }

                csvReader.Close();

                File.WriteAllText(Application.dataPath + "/../Characters/generatedCharData.fwc", StringSerializationAPI.Serialize(typeof(SliderTree), morphSliderTree));

                return true;

            }
            catch (Exception e)
            {
                /*
                Debug.LogError("FILE IO ERROR WHEN LOADING SLIDER LIST FROM CSV: " + e.Message
                    + "\nSTACK TRACE: " + e.StackTrace
                    + "\nSOURCE: " + e.Source);
                */
                Debug.LogError(e.ToString());
                return false;
            }

            //return false;

        }
    }

}