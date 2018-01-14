using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace nyxeka
{
    public class CharacterCreatorIB : InputBehaviour
    {

        SliderGUIHandler sliderGUIHandler;

        public GUISkin newGUISkin;

        SliderManager sliderManager;

        private float yRotTarget = 180.0f;

        private float yPosTarget = 0.0f;

        private float yRotSelf = 0;

        private float xRotSelf = 0;

        private float xRotTarget = 0;

        private float xPosTarget = 0;

        private float zoom = 1;

        public UnityEvent onInit;
        public UnityEvent onExit;

        [Space(15)]
        public float xRotTargetMin = -45f;
        public float xRotTargetMax = 45f;

        [Space(15)]
        public float xRotSelfMin = -20.0f;
        public float xRotSelfMax = 20.0f;

        [Space(15)]
        public float yRotSelfMin = -20.0f;
        public float yRotSelfMax = 20.0f;

        [Space(15)]
        public float zoomMin = 0.3f;
        public float zoomMax = 4.0f;

        [Space(15)]

        public float xPosTargetMin = -0.25f;
        public float xPosTargetMax = 0.25f;

        [Space(15)]
        public float yPosTargetMin = -1.0f; // in meters.
        public float yPosTargetMax = 1.0f;

        [Space(15)]
        public float yPosInitial = 1.0f;

        [Space(15)]
        public float mouseSensX = 1.0f;
        public float mouseSensY = 1.0f;

        private Vector3 camTarget;

        private Vector3 camTargetOffset;

        private Vector3 camSelfOffset;

        private Vector2 oldMousePos = Vector2.zero;
        private Vector2 mousePos = Vector2.zero;
        private Vector2 deltaMouse = Vector2.zero;
        //Unit unit;

        public void Reset()
        {

            //camOffset = new Vector3(0.0f, 0.1f, -2.0f);
            desc = "Character Creation Menu";
            ID = "Used for character looks modification";

        }

        public override void Init(Unit newUnit)
        {
            onInit.Invoke();
            unit = newUnit;
            if (!sliderGUIHandler)
            {
                if (!(sliderGUIHandler = gameObject.GetComponent<SliderGUIHandler>()))
                {

                    sliderGUIHandler = gameObject.AddComponent<SliderGUIHandler>();

                }
            }
            if (!sliderManager)
            {
                if (!(sliderManager = gameObject.GetComponent<SliderManager>()))
                {

                    sliderManager = gameObject.AddComponent<SliderManager>();

                }
            }
            sliderGUIHandler.enabled = true;
            sliderManager.enabled = true;
            sliderManager.InitSliderManager(sliderGUIHandler, gameObject.GetComponent<DatabaseReader>());
            //sliderGUIHandler.SetGUISkin(newGUISkin);
            unit.SetPhysicsInactive();


            camTargetOffset = Vector3.zero;
            camTarget = unit.transform.position + Vector3.up * yPosInitial;
            camSelfOffset = Vector3.zero;
            //yRotTarget = 180.0f;

            //camOffset = transform.position - camTarget;
            zoom = 1.0f;
            camOffset = Vector3.back * 2 * zoom;
            

            Debug.Log("initialized CharacterCreatorIB");


        }

        public override void RunBehaviour()
        {
            base.RunBehaviour();
        }

        private void UpdateMouseAxis()
        {

            //mousePos.x = Input.GetAxis("Mouse X");
            //mousePos.y = Input.GetAxis("Mouse Y");

            //deltaMouse = oldMousePos - mousePos;

            //oldMousePos = mousePos;

            deltaMouse.x = Input.GetAxis("Mouse X");
            deltaMouse.y = Input.GetAxis("Mouse Y");


        }

        public override void RunCamera()
        {

            UpdateMouseAxis();
            // so, we're gonna start by grabbing user input,
            // we need to grab mouse button input and movement input so long as it's not over a GUI.
            // or give the user an area that they can use. We'll start by grabbing clicks from outside gui.

            if (!sliderGUIHandler.mouseOnGUI)
            {

                if (Input.GetMouseButton(0))// left click
                {

                    xRotTarget += deltaMouse.y * -mouseSensX;
                    yRotTarget += deltaMouse.x * mouseSensY;

                }
                if (Input.GetMouseButton(1))// right click
                {

                    xPosTarget += deltaMouse.x * -mouseSensX * 0.01f;
                    yPosTarget += deltaMouse.y * -mouseSensY * 0.01f;

                }
                if (Input.GetMouseButton(2))// middle click
                {

                    xRotSelf += deltaMouse.y * -mouseSensX;
                    yRotSelf += deltaMouse.x * mouseSensY;

                }

                

            }

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                zoom -= Input.GetAxis("Mouse ScrollWheel");
            }

            // then, we're going to clamp the variables to their rotations, mins and maxes
            ClampVars();
            // finally, we're going to translate the variables into something that we can apply to the camera transform, and then do it.

            camTargetOffset.x = xPosTarget;
            camTargetOffset.y = yPosTarget;

            Quaternion rotTarget = Quaternion.Euler(new Vector3(xRotTarget, yRotTarget, 0.0f));
            Quaternion rotSelf = Quaternion.Euler(new Vector3(xRotSelf, yRotSelf, 0.0f));

            // first, let's set the position relative to the target and various offsets.
            cam.transform.position = camTarget + ((rotTarget * camOffset) * zoom) + (camTargetOffset.y * Vector3.up) + (rotTarget * (camTargetOffset.x * Vector3.right));

            // now, we're going to apply the rotations.
            cam.transform.rotation = rotTarget * rotSelf;

            unit.GiveCommand(new cmdMove(camTarget + (camOffset * 2) + Vector3.down, "LookAt"));

        }

        private void ClampVars()
        {

            xRotTarget = Mathf.Clamp(xRotTarget, xRotTargetMin, xRotTargetMax);

            yRotTarget = ClampAngle(yRotTarget);

            xPosTarget = Mathf.Clamp(xPosTarget, xPosTargetMin, xPosTargetMax);

            yPosTarget = Mathf.Clamp(yPosTarget, yPosTargetMin, yPosTargetMax);

            xRotSelf = Mathf.Clamp(xRotSelf, xRotSelfMin, xRotSelfMax);

            yRotSelf = Mathf.Clamp(yRotSelf, yRotSelfMin, yRotSelfMax);

            zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);

        }

        private float ClampAngle(float angle)
        {
            if (angle < 0f)
                return angle + (360f * (int)((angle / 360f) + 1));
            else if (angle > 360f)
                return angle - (360f * (int)(angle / 360f));
            else
                return angle;
        }

        public override void ExitIB()
        {
            sliderManager.enabled = false;
            sliderGUIHandler.enabled = false;
            onExit.Invoke();
        }

        public override void InitCamera()
        {
            base.InitCamera();
        }
    }

}