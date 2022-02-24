using System;
using System.Collections;
using UnityEngine;

public class FPSMovementCC : MonoBehaviour
{
    public float SprintingSpeed;
    public float WalkSpeed;
    public float SprintingSpeedWhenCrouched;
    public float WalkSpeedWhenCrouched;
    private float currentSpeed;

    public float Gravity;
    public float JumpHeight;
    public float CrouchHeight;

    private Vector3 movementDirection;
    private Vector3 playerVelocity;
    private Transform characterTransform;

    private CharacterController characterController;

    private bool isCrouched;
    private bool isGrounded;
    private bool isSprinted;
    private float originHeight;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterTransform = transform;
        originHeight = characterController.height;
        currentSpeed = WalkSpeed;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;
        if (isCrouched)
            currentSpeed = isSprinted ? SprintingSpeedWhenCrouched : WalkSpeedWhenCrouched;
        else
            currentSpeed = isSprinted ? SprintingSpeed : WalkSpeed;
    }

    public void ProcessMove(Vector2 movementInput)
    {
        movementDirection =
            characterTransform.TransformDirection(new Vector3(movementInput.x, 0, movementInput.y));
        characterController.Move(currentSpeed * Time.deltaTime * movementDirection);
        playerVelocity.y -= Gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0f)
            playerVelocity.y = -2f;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(JumpHeight * 2.0f * Gravity);
        }
    }

    public void Crouch()
    {
        if (isGrounded)
        {
            var tmp_CurrentHeight = isCrouched ? originHeight : CrouchHeight;
            StartCoroutine(DoCrouch(tmp_CurrentHeight));
            isCrouched = !isCrouched;
        }
    }

    public void Sprint()
    {
        isSprinted = !isSprinted;
    }

    IEnumerator DoCrouch(float _targetHeight)
    {
        while (Math.Abs(characterController.height - _targetHeight) > 0.05f)
        {
            characterController.height =
                Mathf.Lerp(characterController.height, _targetHeight, Time.deltaTime * 10);
            yield return null;
        }
    }
}