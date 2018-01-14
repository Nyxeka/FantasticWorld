using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootmotionToParent : MonoBehaviour {

    public Transform target;
    public Animator _anim;

    private void OnAnimatorMove()
    {
        target.position += _anim.deltaPosition;
        target.rotation = target.rotation * _anim.deltaRotation;
    }
}
