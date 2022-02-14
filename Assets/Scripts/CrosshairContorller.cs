
using UnityEngine;


namespace CSE5912.PolyGamers
{
    public class CrosshairContorller : MonoBehaviour
    {
        public CharacterController CharacterController;
        [SerializeField] private WeaponManager weaponManager;
        public RectTransform Reticle;
        public float OriginalSize; // Crosshair original size
        public float FiringSize; // Crosshair size when firing or aiming
        public float AimingSize;
        public float MaxSize; // Crosshair size when moving
        private float currentSize;
        public float speed; // how fast the size change

        void Update()
        {
            bool isMoving = CharacterController.velocity.magnitude > 0;
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
                currentSize = Mathf.Lerp(currentSize, AimingSize, Time.deltaTime * speed);
            // else, change to original size
            else
                currentSize = Mathf.Lerp(currentSize, OriginalSize, Time.deltaTime * speed);
            Reticle.sizeDelta = new Vector2(currentSize, currentSize);
        }

        //TODO: when hit enemy, change the color of crosshair to red
    }
}