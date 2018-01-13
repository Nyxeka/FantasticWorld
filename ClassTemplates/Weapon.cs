using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AtkMod
{

    up,down,left,right,forward,backward,fslash,bslash,stab,thrust,parry,swing

}

public class Weapon : MonoBehaviour {

    public Transform RHand; // these are the two handholds.
    public Transform LHand;
    
    public virtual void InitWeapon() { }

    // let the subclass decide what to do based on the attack tag we give it.
    public virtual void PlayAttack(int index, bool mod = false) { }

    public virtual void PlayAttack(string tag, bool mod = false) { }

    public virtual TransformDelta GetMainTrasnformDeltaInAttackMove()
    {

        return new TransformDelta();

    }

    public virtual TransformDelta GetChestTrasnformDeltaInAttackMove()
    {

        return new TransformDelta();

    }

}
