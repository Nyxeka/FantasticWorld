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
        private Transform model;

        [SerializeField]
        private Animator unitAnim; // unit animator. 

        private Vector3 targetVelocity; // the movement velocity we want to get to

        [SerializeField]
        private float maxSpeed = 7.5f; // in meters per second

        [SerializeField]
        private float maxTurnSpeed = 4.0f; //maximum revolutions per second in any direction

        [SerializeField]
        private float accelerationTime = 0.5f;

        public Transform pelvisIKTargetPivot;

        public FullBodyBipedIK ikMod;

        public Transform lookAtBeacon;

        public Vector3 beaconOffset;

        public bool faceVelocity = true; // rotate on the y axis to face the direction of our current velocity. 

        private Vector3 velocity;

        public float deaccelerationRate = 1.0f;

        private Vector3 oldVel;
        private Vector3 oldPos;

        private Vector3 curVel;

        private float oldRotVel;
        private float curRotVel;

        public float minVelocityTurn = 0.1f;

        private float yRotOffset = 0.0f; //offset from unit facing forwards.

        private float zRotOffset = 0.0f;

        private float xRotOffset = 0.0f;

        private float oldYRotOffset = 0.0f;

        public float maxMagDifToTranslate = 0.5f;

        //delay in seconds that we want to be checking our magnitude at.
        public float magnitudeCheckDelay = 0.25f;

        public int maxQueueSize = 200;

        private Queue<FloatQueueData> magStream;

        private float oldMagnitude;

        public bool updateQueueDebugCount = true;

        public float turnAcceleration = 0.5f;

        private float turnSpeed;

        private float rotationVelocityRef;

        //minimum turning in degrees per second before we slow down turning
        public float minTurnSlowSpeed = 10f;

        public float slowDownTurnTime = 0.5f;

        private float walkStartRotation;

        private float targetRotOffset;

        private Vector3 lookAtVector;

        public float unitClampLookAtAngle = 90.0f;

        public float unitRunningLookAtAngle = 90.0f;

        public float maxCCUnitLookAtAngle = 25.0f;

        public float lookAtBeaconMoveSpeed = 2.5f;

        private float inputMagnitude;

        private float sprintAmount = 0.0f;

        public float turnSpeedLerp = 2.0f;

        public float artificialGravity = -9.8f;

        public float turningTime = 0.2f;

        public float sprintTiltAngle = 35f;

        private Vector3 curVelocityRef;

        private float fallingTime;

        private Vector3 navTargetLocation;

        private bool isFalling;
        private bool isJumping;
        private bool crouchToggle;

        public float jumpForce = 2.0f;

        public float maximumUpVelocity = 25.0f;

        public float jumpDelay = 0.1f;

        public float maxJumpWaitTime = 0.5f;

        bool canJump;

        Action nextAction = Action.None;
        Vector3 nextActionVector;
        float nextFloatVector;

        bool waitToJump = false;

        float extraSpeed;

        public float maxDownVelocity = -40.0f;

        public float leanTurnMult = 0.25f;

        public float maxLeanTurnAngle = 45.0f;

        public float maxLeanAngle = 35.0f;

        public bool turnOffAnim = false;

        public float footDistanceRange = 0.1f;
        public float footDistanceOffset = 0.12f;

        public AnimationCurve footWeightCurve;

        Vector3 accel;

        float refBankingVel;

        public bool leanFowards;

        public bool extendStrideLength;

        private float xRotUpHillOffset;

        private float xRotOffset2 = 0.0f;

        public float maxAngleGoingUpHill = 45.0f;

        private float refVelxRot = 0.0f;

        public LookAtIK lookIK;

        public WeaponHandler weaponHandler;

        //private GameObject rightFootLand;
        //private GameObject leftFootLand;

        //public Transform leftFoot;
        //public Transform rightFoot;

        //public GrounderFBBIK grounder;

        #endregion

        void Start()
        {
            //rightFootLand = new GameObject();
            //leftFootLand = new GameObject();
            //magStream = (sizeOfMagStream > 0) ? new float[sizeOfMagStream] : new float[1];

            rb = gameObject.GetComponent<Rigidbody>();

            navAgent = gameObject.GetComponent<NavMeshAgent>();

            controller = gameObject.GetComponent<CharacterController>();
            if (!model)
            {

                model = transform;

            }

            magStream = new Queue<FloatQueueData>();

            lookAtVector = Vector3.forward;

            actionQueue = new Queue<Command>();
            /*if (grounder)
            {
                grounder.ik.solver.rightFootEffector.target = rightFootLand.transform;
                grounder.ik.solver.leftFootEffector.target = leftFootLand.transform;
            }*/

        }

        /*void OnGUI()
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), ""
                + "\nTracked Velocity: " + curVel.ToString("F4")
                + "\noldMagnitude: " + oldMagnitude.ToString()
                + "\nTarget Velocity Magnitude: " + targetVelocity.magnitude.ToString()
                + "\nVelocity We're Applying: " + velocity.ToString("F4")
                + "\ntargetRotOffset: " + targetRotOffset.ToString()
                + "\nyRotOffset: " + yRotOffset.ToString()
                + "\ncurVelocityRef: " + curVelocityRef.ToString()
                + "\ncurVel: " + curVel.ToString("F5")
                + "\nController Slope: " + controller.isGrounded
                + "\nCurrent Acceleration: " + GetCurrentAcceleration().ToString()
                + "\nzRotOffset: " + zRotOffset.ToString()
                + "\nxRotOffset: " + xRotOffset.ToString()
                + "\ncurrent magnitude: " + curVel.magnitude.ToString() 
                + "\nxRotUpHillOffset: " + xRotUpHillOffset.ToString() 
                + "\nxRotOffset2: " + xRotOffset2.ToString());
        }*/

        void RunMagnitudeDelay()
        {
            magStream.Enqueue(new FloatQueueData(targetVelocity.magnitude, Time.time));

            //need a way to check if we haven't moved in a while.
            while (Time.time - magStream.Peek().timeStamp > magnitudeCheckDelay)
            {
                oldMagnitude = magStream.Dequeue().magnitude;
            }

            if (magStream.Count > maxQueueSize)
            {

                //something is going pretty horribly wrong.
                magStream.Clear();

            }

        }
        
        void ResetMagnitudeQueue()
        {

            if (magStream.Count > 0)
            {

                magStream.Clear();

            }

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

            accel = GetCurrentAcceleration();
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
                
                StillLookAt();

            }


            updateVel(Time.fixedDeltaTime);
            updateRotVel(Time.fixedDeltaTime);



            //Everything calculated. Send data to mecanim controller

            if (unitAnim)
            {
                if (!turnOffAnim)
                    updateAnim();
            }

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

            model.transform.rotation = Quaternion.Euler(Vector3.zero);

            sprintAmount = 0.0f;

            curVel = Vector3.zero;

            fallingTime = 0.0f;

            isJumping = false;

            isFalling = false;

            crouchToggle = false;

            unitClampLookAtAngle = maxCCUnitLookAtAngle;

        }

        private void HandleMovementNavMeshAgent()
        {

            //in here, we'll be using the action queue
            //So, we'll grab the target movement location from the top of the stack
            

        }

        private void HandleMovementRB()
        {



        }

        private void HandleMovementCharController()
        {

            HandleSpeedCharController();
            HandleTurningCharController();
            if (!controlsActive)
            {

                SetPhysicsInactive();

            }
        }

        private void Jump()
        {
            if (canJump & !waitToJump)
                StartCoroutine(doJump());
        }

        private IEnumerator doJump()
        {

            isJumping = true;
            waitToJump = true;
            yield return new WaitForSeconds(jumpDelay);
            velocity.y = jumpForce;
            isJumping = false;
            yield return new WaitForSeconds(maxJumpWaitTime);
            waitToJump = false;
            
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            
            float angle = Vector3.SignedAngle((transform.rotation * Vector3.up), hit.normal, transform.rotation*Vector3.left);
            xRotUpHillOffset = angle;
        }

        private void HandleSpeedCharController()
        {

            //so, we want to rotate the character on z and x axis relative to its turning speed and either acceleration or current magnitude.
            //acceleration? lets try both.
            //so, for z rot, we just need to get the turning speed and rotate on z by that much.

            RunMagnitudeDelay();

            //controller.

            //if target velocity is lower than it was a second ago, switch to delayed version
            if (targetVelocity.magnitude < oldMagnitude | targetVelocity.magnitude < 0.1)
            { // not holding down joystick. Slow down.
                
                
                velocity = (velocity.y * Vector3.up) + 
                    (Quaternion.Euler(0.0f,yRotOffset,0.0f) * 
                        (Vector3.forward * 
                            (
                                Mathf.Lerp
                                (
                                    new Vector2(curVel.x,curVel.z).magnitude, oldMagnitude, deaccelerationRate * Time.fixedDeltaTime
                                )
                            )
                        )
                    );
                inputMagnitude = oldMagnitude;
            }
            else
            { //Holding down joystick. Speed up
                //velocity = transform.rotation * (Vector3.forward * (Mathf.Lerp(curVel.magnitude, targetVelocity.magnitude + (sprintAmount * maxSpeed), 0.5f)));
                //velocity = Vector3.SmoothDamp(curVel, transform.rotation * (Vector3.forward * (oldMagnitude + (sprintAmount * maxSpeed))), ref curVelocityRef, accelerationTime * Time.fixedDeltaTime, Mathf.Infinity, Time.fixedDeltaTime);
                velocity = (velocity.y * Vector3.up) + (Quaternion.Euler(0.0f, yRotOffset, 0.0f) * Vector3.forward * oldMagnitude * Mathf.Lerp(1.0f,1.5f,sprintAmount));
                
                inputMagnitude = targetVelocity.magnitude;
            }

            //clamp velocity.
            if (velocity.magnitude > (maxSpeed * (1.0f+sprintAmount)))
            {
                float oldVelY = velocity.y;
                velocity = velocity.normalized * maxSpeed * (1.0f+sprintAmount);
                velocity.y = oldVelY;
            }
            //velocity.y = curVel.y;
            velocity.y += artificialGravity * Time.fixedDeltaTime;
            if (actionQueue.Count > 0)
            {
                if (actionQueue.Peek().getAction() == Action.Jump)
                {

                    actionQueue.Dequeue();
                    Jump();

                }else if (actionQueue.Peek().getAction() == Action.Attack)
                {

                    actionQueue.Dequeue();
                    PlayAttack();

                }
            }

            

            if (controller)
            {
                
                controller.Move(velocity * Time.fixedDeltaTime);

                //if (controller.collisionFlags == (CollisionFlags.Below))
                if (controller.isGrounded)
                {
                    fallingTime = 0;
                    canJump = true;
                    isFalling = false;
                    velocity.y = Mathf.Clamp(velocity.y, -1.0f, maximumUpVelocity);
                }
                else {
                    canJump = false;
                    if (velocity.y > 0)
                    {
                        isFalling = false;
                    }
                    else
                    {
                        fallingTime += Time.fixedDeltaTime;
                        isFalling = true;
                    }
                }
            }
            else {
                transform.position = transform.position + (velocity * Time.fixedDeltaTime);
            }

        }

        private void PlayAttack()
        {

            weaponHandler.PlayAttackDTV();

        }

        private void RunLookAt()
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

        }

        private void HandleTurningCharController()
        {

            
            float newYRotOffset = yRotOffset;

            Quaternion leanAngle;

            if (targetVelocity.magnitude > minVelocityTurn)
            {// we are moving.
                if (lookIK)
                    lookIK.solver.IKPositionWeight = 0.0f;
                targetRotOffset = Vector3.SignedAngle((Quaternion.AngleAxis(yRotOffset, Vector3.up) * Vector3.forward), targetVelocity, Vector3.up);
                
                walkStartRotation = targetRotOffset;
                if (curVel.magnitude > 0.01f)
                {

                    newYRotOffset = Mathf.SmoothDampAngle(yRotOffset + 180, yRotOffset + 180 + targetRotOffset, ref rotationVelocityRef, turningTime, maxTurnSpeed * 360*3, Time.fixedDeltaTime) - 180;

                }
                else
                {

                    newYRotOffset = yRotOffset + targetRotOffset;

                }
                RunLookAt();
                if (leanFowards)
                    xRotOffset = ((new Vector2(velocity.x, velocity.z).magnitude) / maxSpeed) * maxLeanAngle;
                else
                    xRotOffset = 0.0f;

            }
            else {
                targetRotOffset = 0.0f;
                walkStartRotation = 0.0f;

                StillLookAt();
                xRotOffset = ((curVel.magnitude - targetVelocity.magnitude) / maxSpeed) * -maxLeanAngle;

                
            }

            turnSpeed = newYRotOffset - yRotOffset;
            
            //turnSpeed = Mathf.Lerp(turnSpeed, newYRotOffset - yRotOffset, turnSpeedLerp*Time.fixedDeltaTime);
            if (Mathf.Abs(turnSpeed) > (minTurnSlowSpeed * Time.fixedDeltaTime))
            {

                yRotOffset = newYRotOffset;

            }
            else if (targetVelocity.magnitude < minVelocityTurn)
            {
                turnSpeed = Mathf.Lerp(turnSpeed, 0.0f, slowDownTurnTime * Time.fixedDeltaTime);
                yRotOffset += turnSpeed * Time.fixedDeltaTime;

            }
            yRotOffset = ClampFloatOffset(yRotOffset, -180, 180, 360);
            //Debug.Log("yRotOffset: " + yRotOffset.ToString
            //zRotOffset = Mathf.Lerp(0.0f, Mathf.Clamp(-targetRotOffset * leanTurnMult, -maxLeanAngle, maxLeanAngle), curVel.magnitude / maxSpeed);
            
            if (new Vector2(curVel.x,curVel.y).magnitude > minVelocityTurn)
            {
                
                zRotOffset = Mathf.SmoothDamp(zRotOffset,Mathf.Clamp(-turnSpeed * 6.5f,-maxLeanTurnAngle,maxLeanTurnAngle),ref refBankingVel,turningTime,Mathf.Infinity,Time.fixedDeltaTime );

            } else
            {

                zRotOffset = Mathf.Lerp(zRotOffset, 0.0f, 0.5f);

            }

            if (xRotUpHillOffset > 0 && curVel.magnitude > 4.0f && controller.isGrounded)
            {

                xRotOffset2 = Mathf.SmoothDamp(xRotOffset2,Mathf.Clamp(xRotUpHillOffset,0.0f,maxAngleGoingUpHill),ref refVelxRot,0.1f,Mathf.Infinity,Time.fixedDeltaTime);
                //xRotOffset2 = 0.0f - Mathf.Clamp(xRotUpHillOffset, 0.0f, maxAngleGoingUpHill);
            } else
            {
                xRotOffset2 = 0.0f;
            }

            xRotOffset -= xRotOffset2;

            leanAngle = Quaternion.Euler(xRotOffset, 0.0f, zRotOffset);
            
            //transform.rotation = Quaternion.AngleAxis(yRotOffset, Vector3.up);
            transform.rotation = Quaternion.Euler(0.0f, yRotOffset, 0.0f);
            model.rotation = transform.rotation * leanAngle;

        }
        //################################
        //################################
        //################################
        private int wrapBetween(int a, int min, int max)
        {

            if (a < min)
                return a + (max - min);
            if (a > max)
                return a - (max - min);

            return 0;

        }

        private void updateAnim()
        {
            if (targetVelocity.magnitude > oldMagnitude)
            {
                unitAnim.SetFloat("InputMagnitude", targetVelocity.magnitude);
            }
            else
            {

                unitAnim.SetFloat("InputMagnitude", targetVelocity.magnitude);

            }

            //unitAnim.SetFloat("InputAngle", Mathf.Clamp(turnSpeed / Time.fixedDeltaTime,-90,90), 0.25f, Time.fixedDeltaTime);
            //unitAnim.SetFloat("RawInputAngle", yRotOffset);
            //unitAnim.SetFloat("WalkStartAngle", walkStartRotation);
            //unitAnim.SetFloat("WalkStopAngle", walkStartRotation);
            unitAnim.SetFloat("SprintFactor", sprintAmount);
            unitAnim.SetFloat("curMagnitude", new Vector3(curVel.x,0.0f,curVel.z).magnitude);
            unitAnim.SetFloat("FallingTime", fallingTime);

            unitAnim.SetBool("IsJump",isJumping);
            unitAnim.SetBool("IsFalling", isFalling);
            unitAnim.SetBool("IsCrouch", crouchToggle);
            if (extendStrideLength)
            {
                if (ikMod)
                {
                    //So, we're gonna add the thingy here.
                    //so, lets see if we can find the animation loop percentage.
                    float newWeight = 0.0f;
                    float percentage = 0.0f;
                    float currentAnimPercent = unitAnim.GetFloat("runLoopPercent");
                    float distanceRight = Mathf.Abs(footDistanceOffset - currentAnimPercent);
                    float distanceLeft = Mathf.Abs(footDistanceOffset + 0.5f - currentAnimPercent);
                    if (distanceRight < footDistanceRange)
                    {

                        percentage = (currentAnimPercent - (footDistanceOffset - footDistanceRange)) / (footDistanceRange * 2);

                        newWeight = (footWeightCurve.Evaluate(percentage));
                        ikMod.solver.rightFootEffector.positionWeight = newWeight;
                    }
                    else if (distanceLeft < footDistanceRange)
                    {

                        percentage = (currentAnimPercent - 0.5f - (footDistanceOffset - footDistanceRange)) / (footDistanceRange * 2);

                        newWeight = (footWeightCurve.Evaluate(percentage));
                        ikMod.solver.leftFootEffector.positionWeight = newWeight;
                    }
                    else
                    {

                        ikMod.solver.leftFootEffector.positionWeight = 0.0f;
                        ikMod.solver.rightFootEffector.positionWeight = 0.0f;

                    }



                }
            }
        }

        private void updateVel(float timeScale)
        {
            oldVel = curVel;

            curVel = (transform.position - oldPos)/timeScale;
            //curVel.y = 0.0f;

            oldPos = transform.position;
        }

        private void updateRotVel(float timeScale)
        {

            oldRotVel = curRotVel;
            curRotVel = (transform.rotation.eulerAngles.y - oldYRotOffset) / timeScale;
            oldYRotOffset = transform.rotation.eulerAngles.y;


        }

        public override float GetSpeedPercent()
        {
            return new Vector2(curVel.x, curVel.z).magnitude / maxSpeed;
        }

        public override Vector3 GetCurrentVelocity()
        {

            return curVel;

        }

        public override Vector3 GetCurrentAcceleration()
        {

            return curVel - oldVel;

        }

        public override float GetMaxSpeed()
        {

            return maxSpeed;

        }

        public override Vector3 GetTargetVelocity()
        {

            return targetVelocity;

        }

        public override float GetRotationSpeed()
        {

            return curRotVel;

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