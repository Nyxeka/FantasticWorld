using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct TransformDelta
{

    public string transformTag;

    public Vector3 pose1;
    public Quaternion rot1;

    public Vector3 pose2;
    public Quaternion rot2;

    public Transform targetTransform;

    // Animation curves define the interpolation between various states/poses. All 1x1 graphs (0-1,0-1).
    public AnimationCurve toRearUp;  // rear up - blend on IK and enter rear up pose.
    public AnimationCurve toStrike; //  to strike - interpolate between rear up pose and strike pose.
    public AnimationCurve toRelease; // to release - interpolate between strike pose and blend release ik weights.

    public float toRearUpTime;
    public float toStrikeTime;
    public float toReleaseTime;

    public bool rotInclude;

}

public class AttackMove : MonoBehaviour {

    public TransformDelta moveDeltaChest;
    public TransformDelta moveDeltaWeapon;
    public TransformDelta moveDeltaLookAtTarget;

    public Vector3 charMoveDelta; // relative to character rotation.

    // for now, we'll put the same curve in all of them. (until we can find a way to represent the ones in the transform deltas in the inspector).
    public AnimationCurve toRearUp;  // rear up - blend on IK and enter rear up pose.
    public AnimationCurve toStrike; //  to strike - interpolate between rear up pose and strike pose.
    public AnimationCurve toRelease; // to release - interpolate between strike pose and blend release ik weights.
    
}
