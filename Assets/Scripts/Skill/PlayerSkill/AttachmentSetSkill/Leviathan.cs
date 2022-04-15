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
        [SerializeField] private float onGroundOffsetY;

        [SerializeField] private float jumpHeight;
        [SerializeField] private bool isDoubleJumped = false;

        [SerializeField] private bool wasJumpingLastFrame = false;

        [SerializeField] private LayerMask layerMask;

        protected override string GetBuiltSpecific()
        {
            return "";
        }

        private void Update()
        {
            if (!isLearned)
                return;

            if (InputManager.Instance.InputSchemes.PlayerActions.Jump.WasPressedThisFrame() && FPSControllerCC.Instance.IsJumping && wasJumpingLastFrame)
            {
                if (isDoubleJumped)
                    return;

                FPSControllerCC.Instance.Jump(jumpHeight);
                isDoubleJumped = false;

                if (!isReady)
                    return;

                StartCoolingdown();

                var player = PlayerManager.Instance.Player;

                var position = player.transform.position - 2 * Vector3.up * FPSControllerCC.Instance.CharacterController.height;
                if (Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, layerMask)) 
                {
                    position = hit.point + Vector3.up * onGroundOffsetY;
                }

                GameObject blackHole = Instantiate(blackHolePrefab);
                blackHole.transform.position = position;

                blackHole.GetComponent<BlackHole>().Initialize(pullSpeed, radius);

                Destroy(blackHole, duration);

            }
            wasJumpingLastFrame = FPSControllerCC.Instance.IsJumping;
        }


    }
}
