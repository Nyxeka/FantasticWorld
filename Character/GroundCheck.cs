using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    public bool isGrounded { get; private set; }

    private Collider _col;
    
    public LayerMask mask;

    public float groundAngle;

    public float distance = 0.5f;

    public Transform front;
    public Transform middle;
    public Transform back;

    RaycastHit rFront;
    RaycastHit rMiddle;
    RaycastHit rBack;

    bool f;
    bool m;
    bool b;

    float fAngle; // angle between front raycast hits.
    float bAngle; // angle between back raycast hits.
    float averageAngle;

	// Use this for initialization
	void Start () {
        _col = GetComponent<Collider>();
	}
    
    private void FixedUpdate()
    {
        f = Physics.Raycast(front.position, Vector3.down, out rFront, distance, mask);
        m = Physics.Raycast(middle.position, Vector3.down, out rMiddle, distance, mask);
        b = Physics.Raycast(back.position, Vector3.down, out rBack, distance, mask);
        
        
        if (f && m && b)
        {
            fAngle = Vector3.SignedAngle(rFront.point - rMiddle.point, transform.rotation * Vector3.forward, transform.rotation * Vector3.left);
            //Debug.DrawLine(rMiddle.point, rFront.point,Color.red,0.5f);
            bAngle = Vector3.SignedAngle(rMiddle.point - rBack.point, transform.rotation * Vector3.forward, transform.rotation * Vector3.left);
            //bAngle = Vector3.Angle(rBack.point, rMiddle.point);
            //Debug.DrawLine(rBack.point, rMiddle.point, Color.red, 0.5f);
            //Debug.DrawLine(rMiddle.point, transform.position +(transform.rotation * Vector3.right), Color.blue, 0.5f);
            averageAngle = (fAngle + bAngle)/2;
            groundAngle = 0;
            //Debug.Log(groundAngle);
        }
        isGrounded = false;
        if (f || m || b)
        {
            isGrounded = true;
        }

    }
}
