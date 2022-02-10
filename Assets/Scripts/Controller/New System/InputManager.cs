using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class InputManager : MonoBehaviour
    {
        private OpenMenuHandler openMenuHandler;
        private ScrollHandler scrollHandler;

        private InputActions inputSchemes;

        //private FPSMovementCC cc;
        //private FPSMouseLookNew look;

        void Awake()
        {
            inputSchemes = new InputActions();

            openMenuHandler = new OpenMenuHandler(inputSchemes, IngameMenuController.Instance);
            scrollHandler = new ScrollHandler(inputSchemes, WeaponsPanelControl.Instance);

            //cc = GetComponent<FPSMovementCC>();
            //look = GetComponentInChildren<FPSMouseLookNew>();
            //inputSchemes.PlayerActions.Jump.performed += ctx => cc.Jump();
            //inputSchemes.PlayerActions.Crouch.performed += ctx => cc.Crouch();
            //inputSchemes.PlayerActions.Sprint.started += ctx => cc.Sprint();
            //inputSchemes.PlayerActions.Sprint.canceled += ctx => cc.Sprint();
        }

        //void FixedUpdate()
        //{
        //    cc.ProcessMove(inputSchemes.PlayerActions.Move.ReadValue<Vector2>());
        //}

        //void LateUpdate()
        //{
        //    look.ProcessLook(inputSchemes.PlayerActions.Look.ReadValue<Vector2>());
        //}


        //void OnEnable()
        //{
        //    inputSchemes.PlayerActions.Enable();
        //}

        //void OnDisable()
        //{
        //    inputSchemes.PlayerActions.Disable();
        //}
    }
}