﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka
{

    public class SliderTree
    {
        
        // non-binary tree system

        public Dictionary<string, SliderNode> sliderNodeList;

        private List<Slider> completeSliderList;

        private string versionHash;

        public SliderTree()
        {

            sliderNodeList = new Dictionary<string, SliderNode>();

            //in case the templates of these change.
            versionHash = StringSerializationAPI.Serialize(typeof(SliderTree), this);
            versionHash += StringSerializationAPI.Serialize(typeof(SliderNode), new SliderNode());

        }



        public void AddSliderNode(string name, SliderNode newSliderNode)
        {

            sliderNodeList.Add(name, newSliderNode);

        }

        /// <summary>
        /// Update the Tree's list of references to sliders.
        /// </summary>
        public List<Slider> GetCompleteSliderList()
        {

            return completeSliderList;

        }

        public void SetSliderInSliderList(int index, float value)
        {
            if (completeSliderList != null)
            {
                if (index < completeSliderList.Count)
                {
                    Debug.Log("Updating Slider at index: " + index);
                    SetSliderAtDir(completeSliderList[index]);
                }
            }
        }

        // to-do: add function that lets us grab values from the tree and write values to the tree, with a provided DIR.
        // also need to be able to get a list of all subnodes, or a list of subnodes in a specific DIR.

        /// <summary>
        /// Iterative creation of a list by walking through each subnode and adding it to a list. Returns the list of Sliders.
        /// </summary>
        /// <returns></returns>
        public void UpdateCompleteSliderList()
        {
            
            List<Slider> newList = new List<Slider>();

            // build the list:

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

                        newList.Add(listFromNode[i].SetIndexInTree(i));

                    }

                }
            }
            catch (System.Exception e)
            {

                Debug.LogError(e.ToString());

            }
            finally
            {
                enumerator.Dispose();
            }

            completeSliderList = newList;

        }

        /// <summary>
        /// Finds the subnode at the specified directory and returns it.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public SliderNode GetNodeAtDir(string dir)
        {
            //Debug.Log("<color=blue>Attempt to access directory: " + dir + ", here we go!</color>");
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
                    //Debug.Log("<color=blue>Attempt to access directory: " + dir + ", success!</color>");
                    return outValue.GetNodeAtDir(newDir);

                }
                else
                {

                    //Debug.Log("<color=red>Warning: tried to access node at directory: " + dir + ", failed!</color>");

                }

            }
            else if (dirSplit.Length == 1)
            {

                SliderNode outValue;

                if (sliderNodeList.TryGetValue(dirSplit[0], out outValue))
                {
                    //Debug.Log("<color=blue>Attempt to access directory: " + dir + ", success!</color>");
                    return outValue;

                }
                else
                {

                    //Debug.Log("<color=red>Warning: tried to access node at directory: " + dir + ", failed!</color>");

                }

            }

            return null;

        }

        //public Slider GetSliderAtDir(string dir){return null;}

        /// <summary>
        /// Set the value of a slider at the specified directory. Need to update this so that the "level" int is in a separate private recursive function.
        /// </summary>
        /// <param name="sliderIndex"></param>
        /// <param name="level"></param>
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
                    Debug.Log("Setting slider... " + sliderIndex.sliderName);
                    outValue.SetSliderAtDir(sliderIndex, level + 1);

                } else
                {
                    Debug.Log("Slider does not exist in this node!");
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

        /// <summary>
        /// A recursive function used for adding new sliders to specific directories.
        /// </summary>
        /// <param name="nodeDirectory"></param>
        /// <param name="newSlider"></param>
        /// <param name="level"></param>
        public void AddToSubnode(string[] nodeDirectory, Slider newSlider, int level = 0)
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

    }
}
