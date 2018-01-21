using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka.ai
{

    public static class KernelNames
    {

        public static string WeightValues = "NNAddWeights1024";
        public static string SumWeights = "NNSumWeights256";
        public static string CreateOutput = "NNCreateOutput";
        
    }

    public static class AITexNames
    {
        public static int 
            INPUT_TEX_WIDTH = 32,
            OUTPUT_TEX_WIDTH = 32,
            AXON_TEX_WIDTH = 1024,
            STAT_INPUT_WIDTH = 8,
            STAT_INFLUENCE_WIDTH = 8,
            HORMONE_TEX_WIDTH = 8,
            MEMORY_TEX_WIDTH = 8192,
            NEW_THOUGHT_BUFFER = 2048;
    }
}
