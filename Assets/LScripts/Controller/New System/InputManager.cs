using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private MenuOpener menuOpener;
        private PlayerInputs inputSchemes;
        private FPSMovementCC cc;
        private FPSMouseLookNew look;

        private InputActions inputActions;
        private MenuOpenHandler menuOpenHandler;

        void Awake()
        {
            inputSchemes = new PlayerInputs();
            cc = GetComponent<FPSMovementCC>();
            look = GetComponentInChildren<FPSMouseLookNew>();
            //inputSchemes.PlayerActions.Jump.performed += ctx => cc.Jump();
            //inputSchemes.PlayerActions.Crouch.performed += ctx => cc.Crouch();
            //inputSchemes.PlayerActions.Sprint.started += ctx => cc.Sprint();
            //inputSchemes.PlayerActions.Sprint.canceled += ctx => cc.Sprint();

            inputActions = new InputActions();
            menuOpenHandler = new MenuOpenHandler(inputSchemes.PlayerActions.Menu, menuOpener);
        }



        void OnEnable()
        {
            inputSchemes.PlayerActions.Enable();
        }

        void OnDisable()
        {
            inputSchemes.PlayerActions.Disable();
        }
    }
}