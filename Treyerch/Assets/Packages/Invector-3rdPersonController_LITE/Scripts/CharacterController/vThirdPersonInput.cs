using UnityEngine;

namespace Invector.vCharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region Variables       

        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;
        public int slapMouse = 0;

        [Header("Camera Input")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        public vThirdPersonController cc;
        public vThirdPersonCamera tpCamera;
        public Camera cameraMain;

        [HideInInspector]
        public bool allowMovement = true;

        [HideInInspector]
        public bool isSlapping = false;
        [HideInInspector]
        public bool finishedSlap = true;
        #endregion

        protected virtual void Start()
        {
            transform.parent = null;
            InitilizeController();
            InitializeTpCamera();
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            cc.ControlRotationType();       // handle the controller rotation type
        }

        protected virtual void Update()
        {
            InputHandle();                  // update the input methods
            cc.UpdateAnimator();            // updates the Animator Parameters
        }

        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            if (cc != null)
            {
                cc._rigidbody = cc.characterRigidbody;
                cc._capsuleCollider = cc.characterCapsuleCollider;
                cc.Init();
            }
        }

        protected virtual void InitializeTpCamera()
        {
            if (tpCamera == null)
            {
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                if (allowMovement)
                {
                    SlapInput();

                    if (!isSlapping && finishedSlap)
                    {
                        MoveInput();
                        SprintInput();
                        StrafeInput();
                        JumpInput();
                    }
                }

                CameraInput();
            }
        }

        public virtual void MoveInput()
        {
            cc.input.x = Input.GetAxis(horizontalInput);
            cc.input.z = Input.GetAxis(verticallInput);
        }

        public void SlapInput()
        {
            if (cc.isGrounded)
            {
                if (Input.GetMouseButton(slapMouse))
                {
                    finishedSlap = false;
                    isSlapping = true;
                }
                else
                {
                    isSlapping = false;
                }
            }
        }

        protected virtual void CameraInput()
        {
            if (cameraMain && allowMovement)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;

            var Y = Input.GetAxis(rotateCameraYInput);
            var X = Input.GetAxis(rotateCameraXInput);

            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
            if (Input.GetKeyDown(strafeInput))
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (Input.GetKeyDown(sprintInput))
                cc.Sprint(true);
            else if (Input.GetKeyUp(sprintInput))
                cc.Sprint(false);
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove && !cc.CheckIfRoof();
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(jumpInput) && JumpConditions())
                cc.Jump();
        }

        #endregion       
    }
}