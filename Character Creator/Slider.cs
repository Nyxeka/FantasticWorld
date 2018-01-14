using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace nyxeka
{
    public class Slider
    {
        public object sliderValue;

        public string sliderName;

        public string dir;

        public string sliderID;

        public bool hasNegative;

        public int indexInTree;

        public Slider(object sliderValue, string sliderName = "un-named slider", string sliderID = "", string dir = "")
        {
            this.sliderName = sliderName;
            this.sliderID = sliderID;
            this.sliderValue = sliderValue;
            this.dir = dir;
        }

        public Slider SetIndexInTree(int newIndex)
        {
            indexInTree = newIndex;
            return this;
        }

    }

}