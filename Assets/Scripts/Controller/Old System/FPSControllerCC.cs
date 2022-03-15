using System;
using System.Collections;
using UnityEngine;
namespace CSE5912.PolyGamers
{
    public class FPSControllerCC : MonoBehaviour
    {
        private CharacterController characterController;
        private Animator characterAnimator;

        [Header("Speed")]
        public float SprintingSpeed;
        public float WalkSpeed;
        public float SprintingSpeedWhenCrouched;
        public float WalkSpeedWhenCrouched;
        public float DashSpeed;
        // determine player current speed
        private float currentSpeed;

        [Header("Physics")]
        [SerializeField] private float mass = 3f;
        private Vector3 impact = Vector3.zero;
        public float Gravity;
        public float JumpHeight;
        public float CrouchHeight;

        private Transform characterTransform;
        private Vector3 movementDirection;
        private Vector3 playerVelocity;
        // player velocity before perform second Move()
        private float velocity;

        private bool isCrouched;
        private bool isSprinted;
        private bool isDashPressed;

        private float controllerHeight;
        private float horizontalInput, verticalInput;

        // fake timer
        private float coyoteTime = 0;
        // increase to 0.5f for double jump
        private const float const_maxCoyoteTime = 0.1f;

        private WaitForSeconds waitOneSeconds = new WaitForSeconds(0.1f);
        [SerializeField] private ParticleSystem forwardDashParticle;
        [SerializeField] private ParticleSystem backwardDashParticle;

        // Perform Dash with cool down
        private CooldownTimer cooldownTimer = new CooldownTimer(2f);

        private static FPSControllerCC instance;
        public static FPSControllerCC Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this; 
        }
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            characterTransform = transform;
            controllerHeight = characterController.height;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            // apply the impact force:
            if (impact.magnitude > 0.2)
                characterController.Move(impact * Time.deltaTime);

            // consumes the impact energy each cycle:
            impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

            currentSpeed = WalkSpeed;
            // faking isGrounded
            if (characterController.isGrounded)
                coyoteTime = 0;
            else
                coyoteTime += Time.deltaTime;

            if (isDashPressed)
            {
                currentSpeed = DashSpeed;
                isDashPressed = false;
            }
            else
            {
                // determine current speed;
                if (isCrouched)
                    currentSpeed = isSprinted ? SprintingSpeedWhenCrouched : WalkSpeedWhenCrouched;
                else
                    currentSpeed = isSprinted ? SprintingSpeed : WalkSpeed;
            }
            HandleAnimation();

            // skill cool down
            if (cooldownTimer.IsActive)
                cooldownTimer.Update(Time.deltaTime);
        }

        public void AddImpact(Vector3 dir, float force)
        {

            dir.Normalize();
            if (dir.y < 0)
                dir.y = -dir.y; // reflect down force on the ground
            impact += dir.normalized * force / mass;
        }
        public bool isGrounded()
        {
            return coyoteTime < const_maxCoyoteTime;
        }

        public void ProcessMove(Vector2 movementInput)
        {
            if (!characterController.enabled)
                return;

            horizontalInput = movementInput.x;
            verticalInput = movementInput.y;
            movementDirection =
                characterTransform.TransformDirection(new Vector3(horizontalInput, 0, verticalInput));
            // (1) first Move() that handle player movement
            characterController.Move(currentSpeed * Time.deltaTime * movementDirection);
            velocity = GetVeloctiy(characterController.velocity);
            // (2) second Move() that apply gravity to player
            playerVelocity.y -= Gravity * Time.deltaTime;
            // player grounded => stick with ground
            if (isGrounded() && playerVelocity.y < 0f)
                playerVelocity.y = -2f;
            characterController.Move(playerVelocity * Time.deltaTime);
        }

        private float GetVeloctiy(Vector3 velocityHolder)
        {
            velocityHolder.y = 0;
            return velocityHolder.magnitude;
        }

        // Perform Jump
        public void PerformJump()
        {
            if (isGrounded())
            {
                // calculate jump height
                playerVelocity.y = Mathf.Sqrt(JumpHeight * Gravity * 2f);
                if (characterAnimator)
                    characterAnimator.SetTrigger("Jump");
            }
        }

        // Perform Crouch
        public void PerformCrouch()
        {
            if (isGrounded())
            {
                var currentHeight = isCrouched ? controllerHeight : CrouchHeight;
                StartCoroutine(DoCrouch(currentHeight));
                isCrouched = !isCrouched;
            }
        }

        // Perform Dash
        public void PerformDash()
        {
            // Dashing when running
            if (isGrounded() && velocity > 5.0f && Math.Abs(horizontalInput) != 1)
            {
                if (cooldownTimer.IsActive) return;
                cooldownTimer.Start();
                isDashPressed = true;
                PlayDashParticle();
            }
        }

        public void PerformInspect()
        {
            if (characterAnimator)
            {
                characterAnimator.SetTrigger("Inspect");
                //WeaponSkinManager.Instance.ChangeCurrentWeaponSkin();
            }
               
        }

        // Handle Sprint
        public void DoSprint()
        {
            isSprinted = !isSprinted;
        }

        private void HandleAnimation()
        {
            // walk animation when press both W and A/D
            if (Math.Abs(horizontalInput) > 0 && Math.Abs(verticalInput) > 0 && !isSprinted)
                velocity /= (float)Math.Sqrt(2);
            if (characterAnimator)
                characterAnimator.SetFloat("Velocity", velocity, 0.25f, Time.deltaTime);
        }

        IEnumerator DoCrouch(float targetHeight)
        {
            yield return waitOneSeconds;
            while (Math.Abs(characterController.height - targetHeight) > 0.05f)
            {
                characterController.height =
                    Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * 10);
                yield return null;
            }
        }

        private void PlayDashParticle()
        {
            // Only forward and backward dash, but could do left and right
            if (verticalInput > 0 && Math.Abs(horizontalInput) <= verticalInput)
            {
                forwardDashParticle.Play();
                return;
            }
            if (verticalInput < 0 && Math.Abs(horizontalInput) <= Math.Abs(verticalInput))
            {
                backwardDashParticle.Play();
                return;
            }
            forwardDashParticle.Play();
        }

        internal void SetupAnimator(Animator aniamtor)
        {
            characterAnimator = aniamtor;
        }
    }
}