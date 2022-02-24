using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class InputManager : MonoBehaviour
    {
        private OpenMenuHandler openMenuHandler;
        private ScrollHandler scrollHandler;

        private InputActions inputSchemes;
        private FPSControllerCC fpsController;
        private FPSMouseLook fpsMouseLook;

        void Awake()
        {
            inputSchemes = new InputActions();
            fpsController = FindObjectOfType<FPSControllerCC>();
            fpsMouseLook = FindObjectOfType<FPSMouseLook>();

            inputSchemes.PlayerActions.Jump.performed += ctx => fpsController.PerformJump();
            inputSchemes.PlayerActions.Crouch.performed += ctx => fpsController.PerformCrouch();
            inputSchemes.PlayerActions.Dash.performed += ctx => fpsController.PerformDash();
            inputSchemes.PlayerActions.Inspect.performed += ctx => fpsController.PerformInspect();
            inputSchemes.PlayerActions.Sprint.started += ctx => fpsController.DoSprint();
            inputSchemes.PlayerActions.Sprint.canceled += ctx => fpsController.DoSprint();
        }

        void Start()
        {
            openMenuHandler = new OpenMenuHandler(inputSchemes, IngameMenuController.Instance);
            scrollHandler = new ScrollHandler(inputSchemes, WeaponsPanelControl.Instance);
        }

        void Update()
        {
            fpsController.ProcessMove(inputSchemes.PlayerActions.Move.ReadValue<Vector2>());
        }

        void LateUpdate()
        {
            fpsMouseLook.ProcessLook(inputSchemes.PlayerActions.Look.ReadValue<Vector2>());
        }

        void OnEnable()
        {
            inputSchemes.PlayerActions.Enable();
            inputSchemes.FPSActions.Enable();
        }

        void OnDisable()
        {
            inputSchemes.PlayerActions.Disable();
            inputSchemes.FPSActions.Disable();
        }
    }
}