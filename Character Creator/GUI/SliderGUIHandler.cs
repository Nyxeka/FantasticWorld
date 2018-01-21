using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace nyxeka
{

    [Serializable]
    public struct SliderGroupSection
    {

        public RectTransform groupSection;

        public RectTransform content;

    }

    public class SliderGUIHandler : MonoBehaviour
    {
        
        public SliderGroupContainer[] groupContainers;

        SliderTree tree;

        public SliderGroupContainer sliderGroupPrefab;

        public Vector2 scrollPosition = Vector2.zero;

        [HideInInspector]
        public bool mouseOnGUI;

        public bool mouseHoverGUI;

        public RectTransform sliderWindowToRefresh;

        public ModifyWindow modifyWindow;

        int numTimes = 0;

        public void Update()
        {
            /*if (EventSystem.current.IsPointerOverGameObject())
            {
                mouseOnGUI = true;
            }*/
            //EventSystem.current.IsPointerOverGameObject()
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()){
                    mouseOnGUI = true;
                }else
                {
                    mouseOnGUI = false;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                mouseOnGUI = false;
            }


        }

        protected virtual void Start()
        {
            //We need to load up the sliders, and create the UI sliders from the list of sliders.
           
        }

        /// <summary>
        /// Creates the Slider group GUI elements in their proper hierarchy.
        /// </summary>
        /// <param name="dict">Provide this with the appropriate Slider Tree's Dictionary.</param>
        private void CreateSliderGroupTree(Dictionary<String, SliderNode> dict, string curDir = ">/", int layer = 0, SliderGroupContainer curGroup = null)
        {
            
            if (curGroup != null)
                curGroup.EnableExpanding();
            var enumerator = dict.GetEnumerator();
            int i = 0;
            try
            {
                while (enumerator.MoveNext())
                {
                    numTimes += 1;
                    SliderNode curSliderNode = enumerator.Current.Value;
                    string curNodeName = enumerator.Current.Key;
                    
                    if (layer == 0)
                    {
                        //start by running through the list of lists, and apply to tabs.
                        if (groupContainers.Length >= dict.Count)//assume this is true for now.
                        {
                            if (curSliderNode.sliderNodeList.Count > 0)
                            {
                                CreateSlidersFromGroup(false, ref curSliderNode, ref groupContainers[i], curDir, curNodeName, layer);
                                CreateSliderGroupTree(curSliderNode.sliderNodeList, curDir + curNodeName + "/", 1, groupContainers[i]);
                                //curGroup.AddSliderGroup(sliderGroupPrefab, curDir + curNodeName + "/", curNodeName).SetSliders(sliderHandlerList.ToArray(), modifyWindow)
                            }

                            i++;
                        }
                    }else
                    {
                        if (curSliderNode.sliderNodeList.Count > 0)// if the slider node list has sub slider groups...
                        {
                            CreateSlidersFromGroup(true, ref curSliderNode, ref curGroup, curDir, curNodeName, layer);
                        } else
                        {
                            CreateSlidersFromGroup(false, ref curSliderNode, ref curGroup, curDir, curNodeName, layer);
                        }
                    }
                }
            }
            catch (Exception e)
            {

                Debug.Log(e.ToString());

            }
            finally
            {
                enumerator.Dispose();
            }

        }

        private void CreateSlidersFromGroup(bool hasChildren, ref SliderNode curSliderNode, ref SliderGroupContainer curGroup, string curDir, string curNodeName, int layer, bool tab = false)
        {
            List<SliderHandler> sliderHandlerList = new List<SliderHandler>();
            //create the list of sliders from the current slider group and add them to the modify window.
            var enum2 = curSliderNode.sliderList.GetEnumerator();
            Slider s = null;
            try
            {
                while (enum2.MoveNext())
                {
                    s = enum2.Current.Value;
                    string _sName = enum2.Current.Key;
                    if (s.sliderID.Contains(M3DHandler._neg))
                        continue;
                    if (curSliderNode.sliderList.ContainsKey(s.sliderName + M3DHandler._neg)) // does the slider have a negative counterpart?
                    {
                        //Debug.Log("Found negative for slider: " + s.sliderName);
                        Slider _temp;
                        
                        if (curSliderNode.sliderList.TryGetValue(s.sliderName + M3DHandler._neg, out _temp)) //access the _temp value.
                            sliderHandlerList.Add(modifyWindow.AddSlider(curGroup, s, _temp, Convert.ToSingle(s.sliderValue) - Convert.ToSingle(_temp.sliderValue)));
                        else// temp value doesn't exist, skipping.
                            sliderHandlerList.Add(modifyWindow.AddSlider(curGroup, s, Convert.ToSingle(s.sliderValue)));
                        //Debug.Log("negative slider name: " + _temp.sliderName);
                    }
                    else
                    {
                        sliderHandlerList.Add(modifyWindow.AddSlider(curGroup, s, Convert.ToSingle(s.sliderValue)));
                    }
                    //Debug.Log("Created a Slider.");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                Debug.Log(s.sliderValue.GetType().ToString());
                
            }
            finally
            {
                enum2.Dispose();
            }

            if (hasChildren)// if it has children, start creating a list of slider-groups under the current directory, after adding the sliders to the modify window.
                CreateSliderGroupTree(curSliderNode.sliderNodeList, curDir + curNodeName + "/", layer + 1, curGroup.AddSliderGroup(sliderGroupPrefab, curDir + curNodeName + "/", curNodeName, curSliderNode.sliderNodeList.Count).SetSliders(sliderHandlerList.ToArray(), modifyWindow));
            else //just add the sliders to the modify window.
                curGroup.AddSliderGroup(sliderGroupPrefab, curDir + curNodeName + "/", curNodeName).SetSliders(sliderHandlerList.ToArray(), modifyWindow);
            sliderHandlerList.Clear();
        }

        public void SetTree(SliderTree newTree)
        {
            tree = newTree;
            if (tree != null)
            {
                CreateSliderGroupTree(tree.sliderNodeList);
            }
            else
            {
                Debug.Log("<color=red>Tree loaded but still null!</color>");
            }
        }

        public SliderTree GetTree()
        {
            return tree;
        }
        

    }
}