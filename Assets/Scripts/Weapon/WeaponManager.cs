using System.Collections.Generic;
using UnityEngine;

namespace PolyGamers.Weapon
{
    public class WeaponManager : MonoBehaviour
    {
        public Firearms MainWeapon;
        public Firearms SecondaryWeapon;
        private Firearms carriedWeapon;
        [SerializeField] private FPSControllerCC fpsController;
        public Transform WorldCameraTransform;
        public float RayCastMaxDistance = 2f;
        public LayerMask CheckItemLayerMask;
        public List<Firearms> Arms = new List<Firearms>();
        public Camera Tmp_Camera;

        void Start()
        {
            // check null weapon
            if (MainWeapon != null)
                SetupCarriedWeapon(MainWeapon);

            if (MainWeapon == null && SecondaryWeapon != null)
                SetupCarriedWeapon(SecondaryWeapon);
        }

        void Update()
        {
            CheckItem();

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
            if (Input.GetKeyDown(KeyCode.Alpha1) && carriedWeapon != MainWeapon && !carriedWeapon.isAiming && MainWeapon != null) // 1-> main weapon (AK...)
            {
                ResetTriggers();
                carriedWeapon.gameObject.SetActive(false);
                carriedWeapon = MainWeapon;
                carriedWeapon.gameObject.SetActive(true);
                fpsController.SetupAnimator(carriedWeapon.GunAnimator);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && carriedWeapon != SecondaryWeapon && !carriedWeapon.isAiming && SecondaryWeapon != null) // 2-> secondary weapon (Pistol...)
            {
                ResetTriggers();
                carriedWeapon.gameObject.SetActive(false);
                carriedWeapon = SecondaryWeapon;
                carriedWeapon.gameObject.SetActive(true);
                fpsController.SetupAnimator(carriedWeapon.GunAnimator);
            }
        }

        private void CheckItem()
        {
            bool tmp_IsItem = Physics.Raycast(WorldCameraTransform.position,
                WorldCameraTransform.forward, out RaycastHit hit,
                RayCastMaxDistance, CheckItemLayerMask);

            // Need to check if the item already exist

            if (tmp_IsItem)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    bool tmp_HasItem = hit.collider.TryGetComponent(out BaseItem tmp_BaseItem);
                    if (tmp_HasItem)
                    {
                        if (tmp_BaseItem is FirearmsItem tmp_FirearmsItem)
                        {
                            foreach (Firearms tmp_FireArms in Arms)
                            {
                                if (tmp_FirearmsItem.ArmsName.CompareTo(tmp_FireArms.name) == 0)
                                {
                                    switch (tmp_FirearmsItem.CurrentFirearmsType)
                                    {
                                        case FirearmsItem.FirearmsType.AssaultRifle: 
                                            MainWeapon = tmp_FireArms;
                                            break;

                                        case FirearmsItem.FirearmsType.HandGun:
                                            SecondaryWeapon = tmp_FireArms;
                                            break;
                                    }
                                    tmp_FirearmsItem.HideItem();
                                    SetupCarriedWeapon(tmp_FireArms);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ResetTriggers()
        {
            carriedWeapon.isReloading = false;
        }

        private void SetupCarriedWeapon(Firearms targetWeapon)
        {
            if (carriedWeapon)
                carriedWeapon.gameObject.SetActive(false);
            carriedWeapon = targetWeapon;
            carriedWeapon.gameObject.SetActive(true);
            fpsController.SetupAnimator(carriedWeapon.GunAnimator);
            Tmp_Camera.gameObject.SetActive(false);
        }

    }
}
   
