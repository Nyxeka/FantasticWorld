using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class WeaponHandler : MonoBehaviour {

    public Transform[] transformList;

    public Weapon defaultWeaponDeprecateThisVariable;

    private Weapon curWeapon;

    private bool busy = false;

    public LookAtIK chestLookAt;

    private void Start()
    {
        defaultWeaponDeprecateThisVariable.InitWeapon();
    }

    public Weapon GetMainWeapon()
    {

        return null;

    }

    /// <summary>
    /// deprecate this function, it's for testing!
    /// </summary>
    public void PlayAttackDTV()
    {
        if (!busy)
        {
            StartCoroutine(RunAttackMotions(transformList[0], defaultWeaponDeprecateThisVariable.GetMainTrasnformDeltaInAttackMove()));
            StartCoroutine(RunAttackMotions(transformList[5], defaultWeaponDeprecateThisVariable.GetChestTrasnformDeltaInAttackMove(),true));
        }

    }

    public IEnumerator RunAttackMotions(Transform targetTransform, TransformDelta delta, bool chest = false)
    {
        busy = true;
        yield return null;

        float timer = 0.0f;

        Vector3 defaultPos = targetTransform.localPosition;
        Quaternion defaultRot = targetTransform.localRotation;

        float time1;
        float time2;
        float time3;

        time1 = delta.toRearUpTime;
        time2 = time1 + delta.toStrikeTime;
        time3 = time2 + delta.toReleaseTime;

        if (chest)
        {
            Debug.Log("enabling chestLookAtIK");
            chestLookAt.enabled = true;

        }

        while (timer < time3)
        {
            timer += Time.fixedDeltaTime;

            //first second, we will go to rear up pose.

            if (timer < time1)
            {

                // default pose to rear up
                targetTransform.localPosition = Vector3.Lerp(defaultPos, delta.pose1, delta.toRearUp.Evaluate(timer / delta.toRearUpTime));
                if (delta.rotInclude)
                    targetTransform.localRotation = Quaternion.Lerp(defaultRot, delta.rot1, delta.toRearUp.Evaluate((timer - 0.05f) / delta.toRearUpTime));

                if (chest)
                    chestLookAt.solver.IKPositionWeight = Mathf.Lerp(0, 1.0f, delta.toRearUp.Evaluate(timer / delta.toRearUpTime));

            }

            if (timer < time2 && timer > time1)
            {

                // rear up to attack
                targetTransform.localPosition = Vector3.Slerp(delta.pose1, delta.pose2, delta.toStrike.Evaluate((timer-time1) / delta.toStrikeTime));
                if (delta.rotInclude)
                    targetTransform.localRotation = Quaternion.Lerp(delta.rot1, delta.rot2, delta.toStrike.Evaluate((timer - time1 - 0.05f) / delta.toStrikeTime));


            }

            if (timer > time2)
            {

                // attack to default.
                targetTransform.localPosition = Vector3.Lerp(delta.pose2, defaultPos, delta.toRelease.Evaluate((timer - time2) / delta.toReleaseTime));
                if (delta.rotInclude)
                    targetTransform.localRotation = Quaternion.Lerp(delta.rot2, defaultRot, delta.toRelease.Evaluate((timer - time2 - 0.05f) / delta.toReleaseTime));

                if (chest)
                    chestLookAt.solver.IKPositionWeight = Mathf.Lerp(1.0f, 0, delta.toRelease.Evaluate((timer - time2) / delta.toReleaseTime));

            }

            yield return new WaitForFixedUpdate();
        }

        if (chest)
        {

            chestLookAt.enabled = false;

        }

        busy = false;
    }

}
