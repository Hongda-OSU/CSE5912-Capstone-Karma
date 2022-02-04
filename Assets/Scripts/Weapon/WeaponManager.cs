using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class WeaponManager : MonoBehaviour
    {
        public Firearms MainWeapon;
        public Firearms SecondaryWeapon;
        private Firearms carriedWeapon;
        [SerializeField] private FPSControllerCC fpsController;

        void Start()
        {
            carriedWeapon = MainWeapon;
            fpsController.SetupAnimator(carriedWeapon.GunAnimator);
        }

        void Update()
        {
            if (!carriedWeapon) return;
            SwapWeapon();

            // hold the trigger to shoot, no attack on reload
            if (Input.GetMouseButton(0) && !carriedWeapon.isReloading)
                carriedWeapon.HoldTrigger();

            // release the trigger to stop shooting
            if (Input.GetMouseButtonUp(0))
                carriedWeapon.ReleaseTrigger();

            // reload ammo
            if (Input.GetKeyDown(KeyCode.R) && !carriedWeapon.isReloading)
                carriedWeapon.ReloadAmmo();

            // weapon aiming, but no aiming during reloading
            if (Input.GetMouseButtonDown(1) && !carriedWeapon.isReloading)
                carriedWeapon.StartAiming();

            // stop weapon aiming
            if (Input.GetMouseButtonUp(1))
                carriedWeapon.StopAiming();

            // start lean shooting only when aiming
            if ((Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)) && carriedWeapon.isAiming)
                carriedWeapon.StartLeanShooting();
            else
                carriedWeapon.StopLeanShooting();
        }

        private void SwapWeapon()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && carriedWeapon != MainWeapon && !carriedWeapon.isAiming) // 1-> main weapon (AK...)
            {
                ResetTriggers();
                carriedWeapon.gameObject.SetActive(false);
                carriedWeapon = MainWeapon;
                carriedWeapon.gameObject.SetActive(true);
                fpsController.SetupAnimator(carriedWeapon.GunAnimator);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && carriedWeapon != SecondaryWeapon && !carriedWeapon.isAiming) // 2-> secondary weapon (Pistol...)
            {
                ResetTriggers();
                carriedWeapon.gameObject.SetActive(false);
                carriedWeapon = SecondaryWeapon;
                carriedWeapon.gameObject.SetActive(true);
                fpsController.SetupAnimator(carriedWeapon.GunAnimator);
            }
        }

        private void ResetTriggers()
        {
            carriedWeapon.isReloading = false;
        }
    }
}
   
