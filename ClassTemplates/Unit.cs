using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace nyxeka
{
    public struct FloatQueueData
    {
        private float _Magnitude;
        private float _TimeStamp;

        public float magnitude
        {
            get { return _Magnitude; }
            set { _Magnitude = magnitude; }
        }

        public float timeStamp
        {
            get { return _TimeStamp; }
            set { _TimeStamp = timeStamp; }
        }

        public FloatQueueData(float newFloat, float newTime)
        {

            _Magnitude = newFloat;
            _TimeStamp = newTime;

        }
    }

    public enum UnitMovementType
    {

        CharacterController, NavMeshAgent, Rigidbody

    }

    public class Unit : MonoBehaviour
    {
        public Queue<Command> actionQueue;

        public UnitMovementType currentUnitMoveType;
        protected UnitMovementType oldUnitMoveType;
        
        protected NavMeshAgent navAgent;
        protected CharacterController controller;
        protected Rigidbody rb;
        
        public bool controlsActive = true;

        public Vector3 getPosition()
        {

            return transform.position;

        }

        public void SetUnitMoveType(UnitMovementType newMoveType)
        {

            currentUnitMoveType = newMoveType;
            
            switch (currentUnitMoveType)
            {
                
                case UnitMovementType.CharacterController:
                    if(controller) {
                        deactivateOldMoveTypeController();
                        controller.gameObject.SetActive(true);
                    }else
                    {
                        currentUnitMoveType = oldUnitMoveType;
                    }
                    break;
                case UnitMovementType.NavMeshAgent:
                    if (navAgent)
                    {
                        deactivateOldMoveTypeController();
                        navAgent.gameObject.SetActive(true);
                    }else
                    {
                        currentUnitMoveType = oldUnitMoveType;
                    }
                    break;
                case UnitMovementType.Rigidbody:
                    if (rb)
                    {
                        deactivateOldMoveTypeController();
                        rb.gameObject.SetActive(true);
                    }else
                    {
                        currentUnitMoveType = oldUnitMoveType;
                    }
                    break;

            }

        }

        private void deactivateOldMoveTypeController()
        {

            switch (oldUnitMoveType)
            {

                case UnitMovementType.CharacterController:
                    if (controller)
                    {
                        controller.gameObject.SetActive(false);
                    }
                    break;
                case UnitMovementType.NavMeshAgent:
                    if (navAgent)
                    {
                        navAgent.gameObject.SetActive(false);
                    }
                    break;
                case UnitMovementType.Rigidbody:
                    if (rb)
                    {
                        rb.gameObject.SetActive(false);
                    }
                    break;

            }
        }

        public virtual void SetPhysicsActive()
        {

            controlsActive = true;

        }

        public virtual void SetPhysicsInactive()
        {

            controlsActive = false;

        }

        public virtual Vector3 GetCurrentVelocity()
        {

            return Vector3.zero;

        }

        public virtual Vector3 GetCurrentAcceleration()
        {

            return Vector3.zero;

        }

        public virtual float GetMaxSpeed()
        {

            return 0;

        }

        public virtual float GetSpeedPercent()
        {

            return 0;

        }

        public virtual Vector3 GetTargetVelocity()
        {

            return Vector3.zero;

        }

        public virtual float GetRotationSpeed()
        {

            return 0;

        }

        public void GiveActionCommand(Command newCMD)
        {

            actionQueue.Enqueue(newCMD);

        }

        public void GiveActionCommandFlush(Command newCMD)
        {

            actionQueue.Clear();
            actionQueue.Enqueue(newCMD);

        }

        public virtual void GiveCommand(Command newCMD)
        {

        }
    }
}