using System;
using System.Collections;
using UnityEngine;

public class FPSControllerCC : MonoBehaviour
{
    private CharacterController characterController;
    private Animator characterAnimator;

    [Header("Speed")]
    public float SprintingSpeed;
    public float WalkSpeed;
    public float SprintingSpeedWhenCrouched;
    public float WalkSpeedWhenCrouched;

    [Header("Physics")]
    public float Gravity;
    public float JumpHeight;
    public float CrouchHeight;

    private Transform characterTransform;
    private Vector3 movementDirection;
    private float velocity;
    private bool isCrouched;
    private float controlerHeight;

    private WaitForSeconds ws = new WaitForSeconds(0.1f);

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterAnimator = GetComponentInChildren<Animator>();
        characterTransform = transform;
        controlerHeight = characterController.height;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float tmp_CurrentSpeed = WalkSpeed;
        if (characterController.isGrounded)
        {
            var tmp_Horizontal = Input.GetAxis("Horizontal");
            var tmp_Vertical = Input.GetAxis("Vertical");
            movementDirection =
                characterTransform.TransformDirection(new Vector3(tmp_Horizontal, 0, tmp_Vertical));
            // Handle Jump
            if (Input.GetButtonDown("Jump"))
            {
                movementDirection.y = JumpHeight;
            }
            // Handle Crouch
            if (Input.GetKeyDown(KeyCode.C))
            {
                var tmp_CurrentHeight = isCrouched ? controlerHeight : CrouchHeight;
                StartCoroutine(DoCrouch(tmp_CurrentHeight));
                isCrouched = !isCrouched;
            }
            // Handle Speed change
            if (isCrouched)
                tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeedWhenCrouched : WalkSpeedWhenCrouched;
            else
                tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeed : WalkSpeed;

            HandleAnimation();
        }

        movementDirection.y -= Gravity * Time.deltaTime * 0.6f;
        characterController.Move(tmp_CurrentSpeed * Time.deltaTime * movementDirection);
    }

    private void HandleAnimation()
    {
        var tmp_Velocity = characterController.velocity;
        tmp_Velocity.y = 0;
        velocity = tmp_Velocity.magnitude;
        if (Input.GetKeyDown(KeyCode.L))
            characterAnimator.SetTrigger("Inspect");
        characterAnimator.SetFloat("Velocity", velocity, 0.25f, Time.deltaTime);
    }

    IEnumerator DoCrouch(float targetHeight)
    {
        yield return ws;
        while (Math.Abs(characterController.height - targetHeight) > 0.05f)
        {
            characterController.height =
                Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * 20);
            yield return null;
        }
    }
}