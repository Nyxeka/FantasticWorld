using UnityEngine;

namespace nyxeka
{
    public class rpgIB : InputBehaviour
    {

        /*


        */

        public void Reset()
        {

            camOffset = new Vector3(0.0f, 0.1f, -2.0f);
            desc = "RPG-like Input Behaviour ";
            ID = "RPG Input Behaviour";

        }

        Vector3 targetVelocity;

        public Vector3 camTargetOffset;

        public float walkSpeed = 1.0f;
        public float sprintSpeed = 10.0f;
        public float jogSpeed = 2.5f;
        public float dashSpeed = 10.0f;
        public float jumpForce = 10.0f;

        public string zoomInput = "Zoom";
        public string zoomInAxis = "Mouse ScrollWheel";

        public float cameraHorizontalMult = 1.0f;
        public float cameraVerticalMult = 1.0f;

        public bool cameraLockCursor = false;

        private float camHorizontalRotOffset; //y-axis rotation offset relative to 0,0,1 vector, in degrees.
        private float camVerticalRotOffset;

        /*
        Think about drawing a castray to the camera from the root camera rotation position on the player. Have a boolean that says
        to do this and puts the camera as close to the player as it needs to be to not clip with other objects.
        */
        public float camVerticalMax = 90.0f;
        public float camVerticalMin = -45.0f;

        public float horizontalCamSens = 1.0f;
        public bool invertHorizontalWhileMoving = false;
        public float hCamSensMouse = 1.0f;
        public float verticalCamSens = 1.0f;
        public bool invertVerticalWhileWalking = false;
        public float vCamSensMouse = 1.0f;

        public float smoothTime = 0.5f;

        public float zoomMult = 1.0f;
        public float minZoom = 0.4f;
        public float maxZoom = 3.0f;
        public float zoomSens = 0.1f;
        public float zoomChangeSpeed = 0.3f;
        float zoomRefVel;
        public float zoomSpeedChangeTime = 0.3f;
        float newZoomMult;

        public float maxCamHeightOffset = 1.8f;
        private float curCamHeightOffset = 0.0f;
        public float minCamHeightOffset = 0.0f;
        private float camHeightClampDelta;

        bool noCameraClipping = true;
        bool camRayHit = false;

        private Vector3 currentVelocityRef;

        private float sprintAmount;

        private Vector3 curVel;
        private Vector3 oldPos;

        public float maxVelFOVChange = 8.0f;
        public float minFOV = 60.0f;
        public float maxFOV = 70.0f;

        //private int layerMask;

        public LayerMask layerMask;

        public string[] layersToCheckNoClip;

        private Vector3 oldUnitPos = Vector3.zero;

        public AnimationCurve unitSpeedToCamLag;

        public AnimationCurve joyStickToMovement;

        public override void Init(Unit newUnit)
        {
            base.Init(newUnit);
            targetMoveType = UnitMovementType.CharacterController;
            unit = newUnit;
            curVel = new Vector3();
            oldPos = new Vector3();
            //oldUnitPos = new Vector3();
            /*if (layersToCheckNoClip.Length > 0)
            {
                for (int i = 0; i < layersToCheckNoClip.Length; i++)
                {

                    layerMask = layerMask | (1 << LayerMask.NameToLayer(layersToCheckNoClip[i]));

                }

            }*/

            unit.SetPhysicsActive();

        }

        private void UpdateVel(float timeScale)
        {

            curVel = (oldPos - transform.position) / timeScale;
            oldPos = transform.position;

        }

        /*void OnGUI()
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), ""
                + "\nTarget Velocity: " + targetVelocity.ToString("F4")
                + "\nHorizontal Axis: " + Input.GetAxis("Horizontal").ToString()
                + "\nVertical Axis: " + Input.GetAxis("Vertical").ToString());
               
        }*/
        void Update()
        {

            if (Input.GetButtonDown("Jump"))
            {
                unit.GiveActionCommandFlush(new cmdAction(Action.Jump));
            }

            if (Input.GetButtonDown("Crouch"))
            {
                unit.GiveActionCommandFlush(new cmdAction(Action.Crouch));
            }

            /*if (Input.GetButtonDown("Fire1"))
            {
                // modify this later.
                unit.GiveActionCommandFlush(new cmdAction(Action.Attack));

            }*/

        }

        private Quaternion getCameraRotOffsetY()
        {

            return Quaternion.Euler(new Vector3(0.0f, camHorizontalRotOffset, 0.0f));

        }

        private Quaternion getCameraRotOffsetX()
        {

            return Quaternion.Euler(new Vector3(camVerticalRotOffset, 0.0f, 0.0f));

        }

        private Quaternion getCameraRotOffset()
        {

            return Quaternion.Euler(new Vector3(camVerticalRotOffset, camHorizontalRotOffset, 0.0f));

        }

        void ClampRotations()
        {
            //make sure to reset it horizontally to stay between -180 and 180
            if (camHorizontalRotOffset > 180)
                camHorizontalRotOffset -= 360;

            if (camHorizontalRotOffset < 180)
                camHorizontalRotOffset += 360;

            //clamp it vertically
            if (camVerticalRotOffset > camVerticalMax)
                camVerticalRotOffset = camVerticalMax;
            if (camVerticalRotOffset < camVerticalMin)
                camVerticalRotOffset = camVerticalMin;

        }

        private void HandleZoom()
        {

            zoomMult = zoomMult + (Input.GetAxis("Mouse ScrollWheel") * zoomSens);

            zoomMult = Mathf.Clamp(zoomMult, minZoom, maxZoom);

        }

        public override void RunCamera()
        {

            //Get Inputs

            HandleZoom();

            camHorizontalRotOffset += (Input.GetAxis("HorizontalTurn") * horizontalCamSens);
            camVerticalRotOffset += (Input.GetAxis("VerticalTurn") * verticalCamSens);

            ClampRotations();

            //Apply Inputs to Camera
            Vector3 newCamOffset = camOffset;

            Quaternion camRotOffset = getCameraRotOffset();

            newCamOffset = camRotOffset * newCamOffset;

            newZoomMult = Mathf.SmoothDamp(newZoomMult, Mathf.Lerp(zoomMult, zoomMult + zoomChangeSpeed, (unit.GetSpeedPercent())), ref zoomRefVel, zoomSpeedChangeTime);

            newCamOffset = newCamOffset * newZoomMult;

            float camMaxHeightDif = camVerticalMax - camVerticalMin;

            float camCurHeightPercent = camVerticalRotOffset / camMaxHeightDif;

            camHeightClampDelta = maxCamHeightOffset - minCamHeightOffset;

            curCamHeightOffset = minCamHeightOffset + (camHeightClampDelta * camCurHeightPercent);

            cam.transform.rotation = camRotOffset;

            // here we move the newCamOffset to the players position.

            oldUnitPos = Vector3.SmoothDamp(oldUnitPos, unit.transform.position, ref currentVelocityRef, smoothTime, Mathf.Infinity, Time.fixedDeltaTime);

            newCamOffset = newCamOffset + (oldUnitPos + camTargetOffset + (curCamHeightOffset * Vector3.up));

            //now for the castray stuff to checkif the camera would clip in its new location
            Vector3 targetLocation = unit.transform.position + (curCamHeightOffset * Vector3.up);

            float distance = Vector3.Distance(cam.transform.position, newCamOffset);

            float angle = Vector3.Angle((cam.transform.rotation * Vector3.forward), (cam.transform.position - unit.transform.position));

            Debug.DrawRay(unit.transform.position + (Vector3.up * 1), newCamOffset - unit.transform.position, Color.red);

            if (noCameraClipping)
            {
                //do a raycast from the unit to the camera.
                //grab the distance of the first hit point
                RaycastHit hitLocation;
                if (Physics.Raycast(unit.transform.position + (Vector3.up * 1), newCamOffset - targetLocation, out hitLocation, (targetLocation - newCamOffset).magnitude+2, layerMask))
                {
                    camRayHit = true;
                    newCamOffset = hitLocation.point;
                }
                else
                    camRayHit = false;

            }

            //cam.transform.position = Vector3.Lerp(cam.transform.position, newCamOffset, Time.fixedDeltaTime*lerpMult*angle);

            //cam.transform.position = Vector3.SmoothDamp(cam.transform.position, newCamOffset, ref currentVelocityRef, smoothTime, Mathf.Infinity, Time.fixedDeltaTime);

            cam.transform.position = newCamOffset;

            unit.GiveCommand(new cmdMove(transform.rotation * Vector3.forward, "LookAt"));

            UpdateVel(Time.fixedDeltaTime);

            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Mathf.Lerp(minFOV, maxFOV, Mathf.Clamp(curVel.magnitude, 0.0f, maxVelFOVChange) / maxVelFOVChange), 0.5f * Time.fixedDeltaTime);

        }


        /// <summary>
        /// Handle input and provide commands based on said input.
        /// </summary>
        public override void RunBehaviour()
        {
            //reset command
            targetVelocity = Vector3.zero;

            sprintAmount = Input.GetAxis("Sprint");

            unit.GiveCommand(new cmdFloat("Sprint", sprintAmount));
            
            targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0.0f,Input.GetAxis("Vertical"));

            targetVelocity = getCameraRotOffsetY() * targetVelocity;
            
            targetVelocity = targetVelocity * (walkSpeed + sprintAmount);
            //give command
            unit.GiveCommand(new cmdMove(targetVelocity));
            
        }

        public override void ExitIB()
        {

            unit.GiveCommand(new cmdMove(Vector3.zero));

        }

    }
}