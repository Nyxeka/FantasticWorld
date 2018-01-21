using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka.ai
{

    public struct Memory
    {
        Color32[] memory;
        Memory[] references;
    }

    public class AIFramework : MonoBehaviour
    {
        /// <summary>
        /// 32x32 visual input.
        /// </summary>
        public RenderTexture visionInput;
        /// <summary>
        /// visual processing results to be used with activation function.
        /// </summary>
        public RenderTexture visionOutput;
        /// <summary>
        /// input weights for our processing.
        /// </summary>
        public Texture2D weights;
        /// <summary>
        /// weighted value processor.
        /// </summary>
        protected RenderTexture wvp;

        public ComputeShader layer_visual_logic_processing;
        
        // Use this for initialization
        void Start()
        {
            
            visionInput.enableRandomWrite = true;
            visionOutput.enableRandomWrite = true;
            wvp.enableRandomWrite = true;

        }

        // Update is called once per frame
        void Update()
        {

        }

        void RunProcess()
        {

        }

        void StopProcess() { }

        IEnumerator VisualProcessing()
        {

            yield return null;

        }



    }
}
