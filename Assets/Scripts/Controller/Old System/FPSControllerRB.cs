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
    private Vector3 movementDirection;

    private float currentSpeed;
    private float originHeight;

    private bool isGrounded = true;
    private bool isCrouched;

    private WaitForSeconds waitOneSeconds = new WaitForSeconds(0.1f);
    [SerializeField] private ParticleSystem forwardDashParticle;
    [SerializeField] private ParticleSystem backwardDashParticle;

    private float horizontalInput, verticalInput;


    private void Start()
    {
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
            var tmp_IsCrouchKeyDown = Input.GetKeyDown(KeyCode.C);
            var tmp_IsSprinting = Input.GetKey(KeyCode.LeftShift);

            currentSpeed = tmp_IsSprinting ? SprintingSpeed : WalkSpeed;

            if (Input.GetKeyDown(KeyCode.C))
            {
                var tmp_CurrentHeight = isCrouched ? originHeight : CrouchHeight;
                StartCoroutine(DoCrouch(tmp_CurrentHeight));
                isCrouched = !isCrouched;
            }

            if (isCrouched)
                currentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeedWhenCrouched : WalkSpeedWhenCrouched;
            else
                currentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeed : WalkSpeed;

            if (Input.GetButtonDown("Jump"))
            {
                characterRigidbody.velocity = new Vector3(movementDirection.x, CalculateJumpHeightSpeed(),
                    movementDirection.z);
            }
        }
    }


    private void FixedUpdate()
    {
        var tmp_Horizontal = Input.GetAxis("Horizontal");
        var tmp_Vertical = Input.GetAxis("Vertical");

        var tmp_CurrentDirection = new Vector3(tmp_Horizontal, 0, tmp_Vertical);
        tmp_CurrentDirection = characterTransform.TransformDirection(tmp_CurrentDirection);
        tmp_CurrentDirection *= currentSpeed;

        var tmp_CurrentVelocity = characterRigidbody.velocity;
        movementDirection = tmp_CurrentVelocity;
        var tmp_VelocityChange = tmp_CurrentDirection - tmp_CurrentVelocity;
        tmp_VelocityChange.y = 0;

        characterRigidbody.AddForce(tmp_VelocityChange, ForceMode.VelocityChange);
    }

    private IEnumerator DoCrouch(float _targetHeight)
    {
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

    private void OnCollisionStay(Collision _other)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision _other)
    {
        isGrounded = false;
    }
}