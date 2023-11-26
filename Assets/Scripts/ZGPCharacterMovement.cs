using UnityEngine;
using UnityEngine.InputSystem;
using ZGP.Game;

namespace ZGP.Game{
    public class ZGPCharacterMovement : MonoBehaviour
    {
        private bool IsTopDownCam => CameraProperties.cameraType == ZGPCameraProperties.CameraTypes.TopDown;
        private bool IsThirdPersonCam => CameraProperties.cameraType == ZGPCameraProperties.CameraTypes.ThirdPerson;

        public ZGPCameraProperties CameraProperties;
        public Transform AimTarget;
        public float MoveSpeed = 5.0f;
        public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;
        public float Gravity = -15.0f;
        public float FallTimeout = 0.15f;

        private bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;

        public LayerMask GroundLayers;

        private float speed;
        private float targetRotation = 0.0f;
        private float rotationVelocity;
        private float verticalVelocity;
        private float terminalVelocity = 53.0f;

        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;

        private int animIDSpeed;
        private int animIDGrounded;

    #if ENABLE_INPUT_SYSTEM 
        private PlayerInput playerInput;
    #endif
        private Animator animator;
        private CharacterController controller;
        private ZGPCharacterInputs characterInput;
        public GameObject MainCam {get; set;}

        private const float threshold = 0.01f;
        private bool hasAnimator;


        private void Start()
        {
            hasAnimator = TryGetComponent(out animator);
            controller = GetComponent<CharacterController>();
            characterInput = GetComponent<ZGPCharacterInputs>();
    #if ENABLE_INPUT_SYSTEM 
            playerInput = GetComponent<PlayerInput>();
    #endif

            AssignAnimationIDs();

            fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            hasAnimator = TryGetComponent(out animator);

            PlayerGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            PlayerRotation();
            if(IsThirdPersonCam){
                CameraRotation();
            }
        }

        private void AssignAnimationIDs()
        {
            animIDSpeed = Animator.StringToHash("Speed_f");
            animIDGrounded = Animator.StringToHash("Grounded");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            if (hasAnimator)
            {
                animator.SetBool(animIDGrounded, Grounded);
            }
        }

        private void PlayerRotation()
        {
            if (characterInput.PlayerLook.sqrMagnitude >= threshold)
            {
                Vector3 inputDirection = new Vector3(characterInput.PlayerLook.x, 0.0f, characterInput.PlayerLook.y).normalized;
                
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                    MainCam.transform.eulerAngles.y;

                transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);
            }
        }

        float pitch;
        float aim;
        float _rotationVelocity;
        private float TopClamp = 4f;
		private float BottomClamp = 2f;

        private void CameraRotation(){
            if (characterInput.CamLook.sqrMagnitude >= threshold)
            {
                
				_rotationVelocity = characterInput.CamLook.x * 10 * Time.deltaTime;
                
                aim += characterInput.CamLook.y * 10 * Time.deltaTime;

				aim = ClampAngle(aim, BottomClamp, TopClamp);
                AimTarget.transform.localPosition = new Vector3(AimTarget.transform.localPosition.x, aim, AimTarget.transform.localPosition.z);

				transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            float targetSpeed = MoveSpeed;

            if (characterInput.Move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = Mathf.Clamp(characterInput.Move.magnitude, 0f, 1f);

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(characterInput.Move.x, 0.0f, characterInput.Move.y).normalized;

            if (characterInput.Move != Vector2.zero)
            {
                targetRotation = IsThirdPersonCam ? Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                    transform.eulerAngles.y : Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                    MainCam.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                    RotationSmoothTime);

                if(characterInput.PlayerLook.sqrMagnitude <= 0 && IsTopDownCam) transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            controller.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                                new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

            if (hasAnimator)
            {
                animator.SetFloat(animIDSpeed, inputMagnitude);
            }
        }

        private void PlayerGravity()
        {
            if (Grounded)
            {
                fallTimeoutDelta = FallTimeout;

                if (verticalVelocity < 0.0f)
                {
                    verticalVelocity = -2f;
                }
            }
            else
            {
                if (fallTimeoutDelta >= 0.0f)
                {
                    fallTimeoutDelta -= Time.deltaTime;
                }
            }

            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}
    }
}
