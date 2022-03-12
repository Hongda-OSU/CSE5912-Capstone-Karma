using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class InputManager : MonoBehaviour
    {
        private OpenIngameMenuHandler openIngameMenuHandler;
        private OpenEscapeMenuHandler openEscapeMenuHandler;
        private ScrollHandler scrollHandler;

        private static InputActions inputSchemes;

        private static InputManager instance;
        public static InputManager Instance { get { return instance; } }
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            if (inputSchemes == null)
                inputSchemes = new InputActions();

            
        }

        void Start()
        {
            WeaponManager.Instance.inputSchemes = inputSchemes;

            inputSchemes.PlayerActions.Jump.performed += ctx => FPSControllerCC.Instance.PerformJump();
            inputSchemes.PlayerActions.Crouch.performed += ctx => FPSControllerCC.Instance.PerformCrouch();
            inputSchemes.PlayerActions.Dash.performed += ctx => FPSControllerCC.Instance.PerformDash();
            inputSchemes.PlayerActions.Inspect.performed += ctx => FPSControllerCC.Instance.PerformInspect();
            inputSchemes.PlayerActions.Sprint.performed += ctx => FPSControllerCC.Instance.DoSprint();
            inputSchemes.PlayerActions.Sprint.canceled += ctx => FPSControllerCC.Instance.DoSprint();
            inputSchemes.FPSActions.Reload.performed += ctx => WeaponManager.Instance.StartReloadAmmo();
            inputSchemes.FPSActions.Aim.performed += ctx => WeaponManager.Instance.StartAiming();
            inputSchemes.FPSActions.Aim.canceled += ctx => WeaponManager.Instance.StopAiming();

            openIngameMenuHandler = new OpenIngameMenuHandler(inputSchemes);
            openEscapeMenuHandler = new OpenEscapeMenuHandler(inputSchemes);
            scrollHandler = new ScrollHandler(inputSchemes, WeaponsPanelControl.Instance);
        }

        void Update()
        {
            FPSControllerCC.Instance.ProcessMove(inputSchemes.PlayerActions.Move.ReadValue<Vector2>());
        }

        void LateUpdate()
        {
            FPSMouseLook.Instance.ProcessLook(inputSchemes.PlayerActions.Look.ReadValue<Vector2>());
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


        public InputActions InputSchemes { get { return inputSchemes; } }
    }
}