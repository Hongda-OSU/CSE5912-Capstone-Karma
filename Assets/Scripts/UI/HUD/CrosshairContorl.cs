
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class CrosshairContorl : UI
    {
        private VisualElement crosshair;

        public CharacterController CharacterController;
        [SerializeField] private WeaponManager weaponManager;

        public float OriginalSize; // Crosshair original size
        public float FiringSize; // Crosshair size when firing or aiming
        public float AimingSize;
        public float MaxSize; // Crosshair size when moving
        private float currentSize;
        public float speed; // how fast the size change

        private static CrosshairContorl instance;
        public static CrosshairContorl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            Initialize();

            crosshair = root.Q<VisualElement>("Crosshair");
        }

        private void Update()
        {
            bool isMoving = CharacterController.velocity.magnitude > 0;

            crosshair.style.display = DisplayStyle.Flex;

            // if player is moving without aiming or shooting, then crosshair become more bigger
            if (isMoving && !weaponManager.isAiming && !weaponManager.isFiring)
                currentSize = Mathf.Lerp(currentSize, MaxSize, Time.deltaTime * speed);

            // if player is moving without aiming but shooting, then crosshair become bigger
            else if (isMoving && !weaponManager.isAiming && !weaponManager.isFiring)
                currentSize = Mathf.Lerp(currentSize, (MaxSize + FiringSize) / 2, Time.deltaTime * speed);

            // if player is shooting without aiming, then crosshair become smaller
            else if (weaponManager.isFiring && !weaponManager.isAiming)
                currentSize = Mathf.Lerp(currentSize, FiringSize, Time.deltaTime * speed);

            // if player is aiming, then crosshair become more smaller
            else if (weaponManager.isAiming)
                crosshair.style.display = DisplayStyle.None;

            // else, change to original size
            else
                currentSize = Mathf.Lerp(currentSize, OriginalSize, Time.deltaTime * speed);
            
            crosshair.style.width = currentSize;
            crosshair.style.height = currentSize;
        }

        public override void Display(bool enabled)
        {
            if (enabled)
            {
                crosshair.style.display = DisplayStyle.Flex;
            }
            else
            {
                crosshair.style.display = DisplayStyle.None;
            }
        }

        //TODO: when hit enemy, change the color of crosshair to red


        public VisualElement Crosshair { get { return crosshair; } }
    }
}