using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MORPH3D;

public class FacialExpression : MonoBehaviour {

    public Transform eyeLeft;

    public Transform eyeRight;

    public AnimationCurve eyeRotToLid;

    private M3DCharacterManager manager;

    // Use this for initialization
    void Start()
    {

        manager = gameObject.GetComponent<M3DCharacterManager>();

        manager.RemoveRogueContent();

        if (manager.CostumeBoundsUpdateFrequency == MORPH3D.CONSTANTS.COSTUME_BOUNDS_UPDATE_FREQUENCY.ON_MORPH)
            enabled = false;

    }

    // Update is called once per frame
    void Update () {

        // eCTRLEyelidsUpperDownUpL
        // eCTRLEyelidsUpperDownUpR

        float lEye = eyeLeft.transform.localRotation.eulerAngles.x;
        float rEye = eyeRight.transform.localRotation.eulerAngles.x;

        if (lEye > 180)
            lEye -= 360;

        if (rEye > 180)
            rEye -= 360;

        manager.SetBlendshapeValue("eCTRLEyelidsUpperDownUpL", eyeRotToLid.Evaluate(Mathf.Clamp(lEye, 0.0f,50.0f)));
        //manager.coreMorphs.morphLookup["eCTRLEyelidsUpperDownUpL"].value = eyeRotToLid.Evaluate(Mathf.Clamp(eyeLeft.transform.localRotation.eulerAngles.x, -5.0f, 50.0f));
        manager.SetBlendshapeValue("eCTRLEyelidsUpperDownUpR", eyeRotToLid.Evaluate(Mathf.Clamp(rEye, 0.0f, 50.0f)));
        //manager.coreMorphs.morphLookup["eCTRLEyelidsUpperDownUpR"].value = eyeRotToLid.Evaluate(Mathf.Clamp(eyeLeft.transform.localRotation.eulerAngles.x, -5.0f, 50.0f));

        //Debug.Log(eyeRotToLid.Evaluate(eyeRight.transform.localRotation.eulerAngles.x));
    }
}
