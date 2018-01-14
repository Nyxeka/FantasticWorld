using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace nyxeka
{

    public class UnitCharacter : Unit
    {
        #region variables

        [SerializeField]
        private Animator anim; // unit animator. 

        private Vector3 targetVelocity; // the movement velocity we want to get to

        private float inputMagnitude;

        private float inputAngle;

        private float rawInputAngle;

        private float sprintAmount = 0.0f;

        private float walkStartAngle;

        private float walkStopAngle;

        private float fallingTime;

        public float facingDirectionAngle;

        private bool idle;

        private bool isRU;

        private bool isFalling;
        private bool isJumping;
        private bool crouchToggle;

        private Vector3 velocity;

        public FullBodyBipedIK ikMod;

        public Transform lookAtBeacon;

        public Vector3 beaconOffset;

        public float deaccelerationRate = 1.0f;

        private Vector3 oldVel;
        private Vector3 oldPos;

        private Vector3 curVel;

        private float oldRotVel;
        private float curRotVel;

        public float unitClampLookAtAngle = 90.0f;

        public float unitRunningLookAtAngle = 90.0f;

        public float maxCCUnitLookAtAngle = 25.0f;

        public float lookAtBeaconMoveSpeed = 2.5f;
        public float artificialGravity = -9.8f;

        public float jumpForce = 2.0f;

        bool canJump;

        Action nextAction = Action.None;

        Vector3 nextActionVector;

        float nextFloatVector;

        private float xRotUpHillOffset;

        private float xRotOffset2 = 0.0f;

        public LookAtIK lookIK;

        private GrounderFBBIK groundIK;

        Vector3 lookAtVector;

        #endregion

        public void SetStanding(bool standing)
        {

            this.idle = standing;

        }

        void Start()
        {
            rb = gameObject.GetComponent<Rigidbody>();

            navAgent = gameObject.GetComponent<NavMeshAgent>();

            controller = gameObject.GetComponent<CharacterController>();

            lookAtVector = Vector3.forward;

            actionQueue = new Queue<Command>();

            groundIK = GetComponentInChildren<GrounderFBBIK>();
        }

        /// <summary>
        /// clamps a float between two values, wrapping it if it goes over
        /// </summary>
        /// <param name="toChange">float to wrap</param>
        /// <param name="min">min wrap value</param>
        /// <param name="max">max wrap value</param>
        /// <param name="offsetDelta">amount to add or substract to wrap</param>
        /// <returns></returns>
        float ClampFloatOffset(float toChange, float min, float max, float offsetDelta)
        {

            float newOffset = toChange;

            if (newOffset > max)
            {

                newOffset -= offsetDelta;

            }
            if (newOffset < min)
            {

                newOffset += offsetDelta;

            }

            return newOffset;

        }

        void FixedUpdate()
        {
            if (controlsActive)
            {

                switch (currentUnitMoveType)
                {
                    case UnitMovementType.CharacterController:
                        HandleMovementCharController();
                        break;
                    case UnitMovementType.NavMeshAgent:
                        HandleMovementNavMeshAgent();
                        break;
                    case UnitMovementType.Rigidbody:
                        HandleMovementRB();
                        break;
                }
            } else
            {
                //StillLookAt();
            }
            UpdateAnim();
            UpdateVel(Time.fixedDeltaTime);
        }

        public override void SetPhysicsActive()
        {
            controlsActive = true;

            unitClampLookAtAngle = unitRunningLookAtAngle;

        }

        public override void SetPhysicsInactive()
        {
            controlsActive = false;

            targetVelocity = Vector3.zero;

            transform.rotation = Quaternion.Euler(Vector3.zero);
            sprintAmount = 0.0f;

            curVel = Vector3.zero;

            fallingTime = 0.0f;

            isJumping = false;

            isFalling = false;

            crouchToggle = false;

            unitClampLookAtAngle = maxCCUnitLookAtAngle;

        }

        private void HandleMovementNavMeshAgent() { }

        private void HandleMovementRB() { }

        private void HandleMovementCharController()
        {
            //HandleSpeedCharController();
            //HandleTurningCharController();
            if (!controlsActive)
            {

                SetPhysicsInactive();

            }
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {

            float angle = Vector3.SignedAngle((transform.rotation * Vector3.up), hit.normal, transform.rotation * Vector3.left);
            xRotUpHillOffset = angle;
        }

        /*private void RunLookAt()
        {

            if (lookAtBeacon)
            {
                Vector3 newLookAtOffset = beaconOffset;
                Quaternion lookAtRotation = Quaternion.AngleAxis(Mathf.Clamp(targetRotOffset, -90, 90), Vector3.up);

                newLookAtOffset = (lookAtRotation * transform.rotation) * newLookAtOffset;

                lookAtBeacon.transform.position = Vector3.Lerp(lookAtBeacon.transform.position, transform.position + newLookAtOffset, lookAtBeaconMoveSpeed * Time.fixedDeltaTime);
            }

        }

        private void StillLookAt()
        {
            if (lookIK)
                lookIK.solver.IKPositionWeight = 1.0f;
            if (lookAtBeacon)
            {
                Vector3 newLookAtOffset = beaconOffset;
                Quaternion lookAtRotation = Quaternion.LookRotation(lookAtVector);
                newLookAtOffset = lookAtRotation * beaconOffset;

                if (Quaternion.Angle(lookAtRotation, transform.rotation) < unitClampLookAtAngle)
                {
                    lookAtBeacon.transform.position = Vector3.Lerp(lookAtBeacon.transform.position, transform.position + newLookAtOffset, lookAtBeaconMoveSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    lookAtBeacon.transform.position = Vector3.Lerp(lookAtBeacon.transform.position, transform.position + (transform.rotation * beaconOffset), lookAtBeaconMoveSpeed * Time.fixedDeltaTime);
                }
            }

        }*/

        private void HandleTurningCharController()
        {



        }

        private int WrapBetween(int a, int min, int max)
        {

            if (a < min)
                return a + (max - min);
            if (a > max)
                return a - (max - min);

            return 0;

        }

        private void RotateTowardsTargetDirection()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetVelocity), 2.0f * Time.fixedDeltaTime);
        }

        private void UpdateAnim()
        {

            //rawInputAngle = 

            facingDirectionAngle = transform.rotation.eulerAngles.y;

            inputAngle = Vector3.SignedAngle((Quaternion.AngleAxis(facingDirectionAngle, Vector3.up) * Vector3.forward), targetVelocity, Vector3.up);

            rawInputAngle = Vector3.SignedAngle(Vector3.forward, targetVelocity, Vector3.up);

            inputMagnitude = targetVelocity.magnitude;

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Start Walking Blend Tree"))
            {
                walkStartAngle = inputAngle;
            } else // not standing still.
            {
                walkStopAngle = inputAngle;
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking Blend Tree"))
            {
                RotateTowardsTargetDirection();
            }

            if (ikMod.references.leftFoot.position.y > ikMod.references.rightFoot.position.y)
            {
                isRU = false;
            } else
            {
                isRU = true;
            }



            //so, we have target velocity.
            //let's check our current rotation.

            anim.SetFloat("InputMagnitude", inputMagnitude);
            anim.SetFloat("InputAngle", inputAngle);
            anim.SetFloat("RawInputAngle", rawInputAngle);
            anim.SetFloat("WalkStartAngle", walkStartAngle);
            anim.SetFloat("WalkStopAngle", walkStopAngle);
            anim.SetFloat("SprintFactor", sprintAmount);
            anim.SetFloat("curMagnitude", new Vector3(curVel.x, 0.0f, curVel.z).magnitude);
            anim.SetFloat("FallingTime", fallingTime);
            anim.SetFloat("IsRU", isRU ? 1 : 0);

            anim.SetBool("IsJump", isJumping);
            anim.SetBool("IsFalling", isFalling);
            anim.SetBool("IsCrouch", crouchToggle);

        }

        private void UpdateVel(float timeScale)
        {
            oldVel = curVel;

            curVel = (transform.position - oldPos) / timeScale;
            //curVel.y = 0.0f;

            oldPos = transform.position;
        }

        public override void GiveCommand(Command newCMD)
        {

            if (newCMD.cmdData == cmdType.Move && newCMD.name == "move")
            {

                targetVelocity = newCMD.getCommandVector();

            }

            if (newCMD.cmdData == cmdType.Move && newCMD.name == "LookAt")
            {

                lookAtVector = newCMD.getCommandVector();

            }

            if (newCMD.cmdData == cmdType.Float)
            {

                if (newCMD.name == "Sprint")
                {

                    sprintAmount = newCMD.getCommandFloat();

                }

            }

        }
    }
}