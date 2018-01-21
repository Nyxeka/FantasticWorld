using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
namespace nyxeka
{
    public class DatabaseReader : MonoBehaviour
    {

        public string defaultSliderTreePath = "Characters/default.fwc";

        private string defaultTreePath;
        [HideInInspector]
        public bool isInitialized = false;

        // Use this for initialization
        void Awake()
        {

            defaultTreePath = Application.dataPath + "/../" + defaultSliderTreePath;
            Debug.Log("<color=red>" + defaultTreePath.ToString() + "</color>");

            isInitialized = true;


        }

        //so, we'll create a tree, deserialize the file into the tree, 
        //and then we'll check the version num.

        public SliderTree GetDefaultSliderTree()
        {
            try
            {

                return StringSerializationAPI.Deserialize(typeof(SliderTree), File.ReadAllText(defaultTreePath)) as SliderTree;

            }
            catch (Exception e)
            {

                Debug.LogError(e.ToString());

                return null;

            }

        }

        public SliderTree GetSliderTree(string filename)
        {

            try
            {

                return StringSerializationAPI.Deserialize(typeof(SliderTree), File.ReadAllText(filename)) as SliderTree;

            }
            catch (Exception e)
            {

                Debug.LogError(e.ToString());

                return null;

            }

        }

        public void SaveSliderTree(SliderTree toSave, string path = "/../Characters/", string name = "default")
        {

            File.WriteAllText(Application.dataPath + path + name + ".fwc", StringSerializationAPI.Serialize(typeof(SliderTree), toSave));

        }

    }
}