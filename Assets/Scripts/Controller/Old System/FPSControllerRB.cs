using System;
using System.Collections;
using UnityEngine;

public class FPSControllerRB : MonoBehaviour
{
    private Rigidbody characterRigidbody;
    private CapsuleCollider capsuleCollider;
    private Animator characterAnimator;

    [Header("Speed")]
    public float WalkSpeed;
    public float WalkSpeedWhenCrouched;
    public float SprintingSpeed;
    public float SprintingSpeedWhenCrouched;
    public float DashSpeed;

    [Header("Physics")]
    public float JumpHeight;
    public float CrouchHeight;
    public float Gravity;

    private Transform characterTransform;
    private Vector3 currentMovementDirection;
    private Vector3 currentVelocity;

    private float currentSpeed;
    private float originHeight;
    private float characterSpeed;
    private float horizontalInput, verticalInput;

    private bool isGrounded;
    private bool isCrouched;

    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
    [SerializeField] private ParticleSystem forwardDashParticle;
    [SerializeField] private ParticleSystem backwardDashParticle;


    private void Start()
    {
        isGrounded = true;
        characterTransform = transform;
        characterRigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        originHeight = capsuleCollider.height;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (isGrounded)
        {
            var isJumpKeyDown = Input.GetButtonDown("Jump");
            var isCrouchKeyDown = Input.GetKeyDown(KeyCode.C);
            var isDashKeyDown = Input.GetKeyDown(KeyCode.V);
            var isSprinting = Input.GetKey(KeyCode.LeftShift);

            // Handle Jump
            if (isJumpKeyDown)
            {
                characterRigidbody.velocity = new Vector3(currentVelocity.x, CalculateJumpHeightSpeed(),
                    currentVelocity.z);

                if (characterAnimator)
                    characterAnimator.SetTrigger("Jump");
            }

            // Handle Crouch
            if (isCrouchKeyDown)
            {
                var tmp_CurrentHeight = isCrouched ? originHeight : CrouchHeight;
                StartCoroutine(DoCrouch(tmp_CurrentHeight));
                isCrouched = !isCrouched;
            }

            // Handle Dash only when running
            if (isDashKeyDown && characterRigidbody.velocity.magnitude > 5.0f && Math.Abs(horizontalInput) != 1)
            {
                currentSpeed = DashSpeed;
                PlayDashParticle();
            }
            else
            {
                if (isCrouched)
                    currentSpeed = isSprinting ? SprintingSpeedWhenCrouched : WalkSpeedWhenCrouched;
                else
                    currentSpeed = isSprinting ? SprintingSpeed : WalkSpeed;
            }

            HandleAnimation();
        }
    }

    private void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        currentMovementDirection = new Vector3(horizontalInput, 0, verticalInput);
        currentMovementDirection = characterTransform.TransformDirection(currentMovementDirection);
        currentMovementDirection *= currentSpeed;

        var tmp_CurrentVelocity = characterRigidbody.velocity;
        currentVelocity = tmp_CurrentVelocity;
        var tmp_VelocityChange = currentMovementDirection - tmp_CurrentVelocity;
        tmp_VelocityChange.y = 0;
        characterRigidbody.AddForce(tmp_VelocityChange, ForceMode.VelocityChange);
    }

    private void HandleAnimation()
    {
        var tmp_Velocity = characterRigidbody.velocity;
        tmp_Velocity.y = 0;
        characterSpeed = tmp_Velocity.magnitude;
        if (Math.Abs(horizontalInput) > 0 && Math.Abs(verticalInput) > 0)
            characterSpeed /= (float)Math.Sqrt(2);
        if (characterAnimator != null)
        {
            if (Input.GetKeyDown(KeyCode.L))
                characterAnimator.SetTrigger("Inspect");
            characterAnimator.SetFloat("Velocity", characterSpeed, 0.25f, Time.deltaTime);
        }
    }

    private IEnumerator DoCrouch(float _targetHeight)
    {
        yield return waitForSeconds;
        while (Math.Abs(capsuleCollider.height - _targetHeight) > 0.05f)
        {
            capsuleCollider.height =
                Mathf.Lerp(capsuleCollider.height, _targetHeight, Time.deltaTime * 10);
            yield return null;
        }
    }

    private float CalculateJumpHeightSpeed()
    {
        return Mathf.Sqrt(2 * Gravity * JumpHeight);
    }

    private void PlayDashParticle()
    {
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

    private void OnCollisionStay(Collision _other)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision _other)
    {
        isGrounded = false;
    }
}