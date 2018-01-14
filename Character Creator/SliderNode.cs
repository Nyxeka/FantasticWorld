using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
namespace nyxeka
{

    public class SliderNode
    {

        // non-binary tree system

        public SliderNode()
        {

            sliderNodeList = new Dictionary<string, SliderNode>();
            sliderList = new Dictionary<string, Slider>();
        }

        public Dictionary<string, SliderNode> sliderNodeList;

        public Dictionary<string, Slider> sliderList;

        public string customName;

        public void SetCustomName(string newName)
        {

            customName = newName;

        }
        
        internal List<Slider> CreateAndReturnCompleteSliderList()
        {

            List<Slider> newList = new List<Slider>();

            // build the list:

            newList = sliderList.Values.ToList();



            var enumerator = sliderNodeList.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    SliderNode value = enumerator.Current.Value;
                    string key = enumerator.Current.Key;
                    //var element = enumerator.Current;
                    // loop body goes here



                    //create the list from each subnode
                    List<Slider> listFromNode = value.CreateAndReturnCompleteSliderList();

                    // then, add the objects from the list to our big list.
                    for (int i = 0; i < listFromNode.Count; i++)
                    {

                        newList.Add(listFromNode[i]);

                    }

                }
            }
            finally
            {
                enumerator.Dispose();
            }
            
            return newList;

        }

        public void AddToSubnode(string[] nodeDirectory, Slider newSlider, int level = 0)
        {

            //sliderNodeList.Add
            if (nodeDirectory.Length == level)
            {
                //final dir.
                sliderList.Add(newSlider.sliderName, newSlider);

            }
            else
            {
                SliderNode tmp;
                if (sliderNodeList.TryGetValue(nodeDirectory[level], out tmp))
                {

                    tmp.AddToSubnode(nodeDirectory, newSlider, level + 1);

                }
                else
                {
                    // the directory doesn't exist; Create a new directory.
                    sliderNodeList.Add(nodeDirectory[level], new SliderNode());
                    // check to see if it works.
                    sliderNodeList.TryGetValue(nodeDirectory[level], out tmp);
                    //give it the rest of the info to deal with and distribute amongst its various subnode directories.
                    tmp.AddToSubnode(nodeDirectory, newSlider, level + 1);

                }
            }

            //sliderNodeList.Add(nodeDirectory

        }

        public void SetSliderValue(string sliderID, object value)
        {

            Slider outValue;

            if (sliderList.TryGetValue(sliderID, out outValue))
            {

                outValue.sliderValue = value;

            }

        }

        public void SetSliderAtDir(Slider sliderIndex, int level = 0)
        {

            // get the DIR, the ID of the slider, and the value of the slider.

            // so, first we need to pass this onto the appropriate slider node.

            string[] dirSplit = sliderIndex.dir.Split();

            if (dirSplit.Length > level + 1)
            {

                SliderNode outValue;
                if (sliderNodeList.TryGetValue(sliderIndex.sliderID, out outValue))
                {

                    outValue.SetSliderAtDir(sliderIndex, level + 1);

                }

            }
            else if (dirSplit.Length == level + 1)
            {

                //we're in this directory, so we're gonna be setting the slider here.
                SliderNode outValue;
                if (sliderNodeList.TryGetValue(sliderIndex.sliderID, out outValue))
                {

                    outValue.SetSliderValue(sliderIndex.sliderID, sliderIndex.sliderValue);

                }

            }

        }

        public SliderNode GetNodeAtDir(string dir)
        {

            string[] dirSplit = dir.Split('/');
            if (dirSplit.Length > 1)
            {

                //find the slider node of string one, and give it the rest of them.
                string newDir = dirSplit[1];

                for (int i = 2; i < dirSplit.Length; i++)
                {

                    newDir += "/" + dirSplit[i];

                }

                SliderNode outValue;

                if (sliderNodeList.TryGetValue(dirSplit[0], out outValue))
                {

                    return outValue.GetNodeAtDir(newDir);

                }

            }
            else
            {

                if (dirSplit.Length == 1)
                {

                    SliderNode outValue;

                    if (sliderNodeList.TryGetValue(dirSplit[0], out outValue))
                    {

                        return outValue;

                    }

                }

            }

            return null;

        }


    }
}