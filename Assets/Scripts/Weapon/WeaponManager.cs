using UnityEngine;

namespace PolyGamers.Weapon
{
    public class WeaponManager : MonoBehaviour
    {
        public Firearms MainWeapon;
        public Firearms SecondaryWeapon;
        private Firearms carriedWeapon;

        void Start()
        {
            carriedWeapon = MainWeapon;
        }

        void Update()
        {
            if (!carriedWeapon) return;

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
            if (Input.GetMouseButtonUp(1) )
                carriedWeapon.StopAiming();

            // start lean shooting only when aiming
            if ((Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)) && carriedWeapon.isAiming)
                carriedWeapon.StartLeanShooting();
            else
                carriedWeapon.StopLeanShooting();
        }
    }
}
