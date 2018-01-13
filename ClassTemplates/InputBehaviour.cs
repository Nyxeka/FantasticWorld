using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka
{
    public class InputBehaviour : MonoBehaviour
    {

        public string ID;

        public string desc = "";

        public Vector3 camOffset;

        public float camFOV = 60.0f;
        
        protected UnitMovementType targetMoveType;

        //public Command dynamicCommand;
        //public Queue<Command> ActionQueue;

        //only use if 
        [Header("only if not component of camera")]
        public Camera camOverride;

        [HideInInspector]
        public Camera cam;

        public bool runningTransition = false;

        [HideInInspector]
        public Unit unit;

        public virtual void Init() { }

        public virtual void Init(Unit newUnit) { }

        public virtual void InitCamera()
        {

            if (camOverride == null)
                cam = gameObject.GetComponent<Camera>();
            else
                cam = camOverride;

        }

        public virtual void ExitIB() { }

        /// <summary>
        /// Put everything that moves and controls the camera in here. (override it)
        /// </summary>
        public virtual void RunCamera() { }

        /// <summary>
        /// anything we need to do once the IB is over?
        /// </summary>
        public virtual void ExitCamera() { }
        
        /// <summary>
        /// Use for grabbing user input, and giving commands to entities.
        /// </summary>
        public virtual void RunBehaviour() { }


    }
}