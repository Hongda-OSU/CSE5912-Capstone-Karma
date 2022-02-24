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
        private WeaponManager weaponManager;

        private static InputManager instance;
        public static InputManager Instance { get { return instance; } }
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            inputSchemes = new InputActions();
            fpsController = FindObjectOfType<FPSControllerCC>();
            fpsMouseLook = FindObjectOfType<FPSMouseLook>();
            weaponManager = FindObjectOfType<WeaponManager>();
            weaponManager.inputSchemes = inputSchemes;

            inputSchemes.PlayerActions.Jump.performed += ctx => fpsController.PerformJump();
            inputSchemes.PlayerActions.Crouch.performed += ctx => fpsController.PerformCrouch();
            inputSchemes.PlayerActions.Dash.performed += ctx => fpsController.PerformDash();
            inputSchemes.PlayerActions.Inspect.performed += ctx => fpsController.PerformInspect();
            inputSchemes.PlayerActions.Sprint.performed += ctx => fpsController.DoSprint();
            inputSchemes.PlayerActions.Sprint.canceled += ctx => fpsController.DoSprint();
            inputSchemes.FPSActions.Reload.performed += ctx => weaponManager.StartReloadAmmo();
            inputSchemes.FPSActions.Aim.performed += ctx => weaponManager.StartAiming();
            inputSchemes.FPSActions.Aim.canceled += ctx => weaponManager.StopAiming();
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