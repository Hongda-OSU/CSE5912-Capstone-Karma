using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class Leviathan : PlayerSkill
    {
        [Header("Leviathan")]
        [SerializeField] private GameObject blackHolePrefab;
        [SerializeField] private float pullSpeed;
        [SerializeField] private float radius;
        [SerializeField] private float duration = 5f;

        [SerializeField] private float jumpHeight;

        [SerializeField] private bool wasJumpingLastFrame = false;


        private void Update()
        {
            if (InputManager.Instance.InputSchemes.PlayerActions.Jump.WasPressedThisFrame() && FPSControllerCC.Instance.IsJumping && wasJumpingLastFrame)
            {
                FPSControllerCC.Instance.Jump(jumpHeight);

                if (isReady)
                {
                    StartCoolingdown();

                    var player = PlayerManager.Instance.Player;

                    GameObject blackHole = Instantiate(blackHolePrefab);
                    blackHole.transform.position = player.transform.position - 2 * Vector3.up * FPSControllerCC.Instance.CharacterController.height;

                    blackHole.GetComponent<BlackHole>().Initialize(pullSpeed, radius);

                    Destroy(blackHole, duration);
                }
            }
            wasJumpingLastFrame = FPSControllerCC.Instance.IsJumping;
        }


    }
}
