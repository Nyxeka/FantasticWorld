using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StatType
{

}

public class Stat : MonoBehaviour {

    public bool isStatic = false;
    public float 
        Base = 0,
        Mod = 0,
        Scalar = 1,
        Max = 1,
        Min = 0;
    

    
    public virtual bool AskForConnection(StatRelation statIn)
    {
        return true;
    }
}

public class StatRelation
{
    public Stat targetStat;
    public Stat baseStat;
    public StatType baseType;
}
