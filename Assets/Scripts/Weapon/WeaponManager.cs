using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PolyGamers.Weapon
{
    public class WeaponManager : MonoBehaviour
    {
        // FPS controller and Weapons
        [SerializeField] private FPSControllerRB fpsController;
        public Firearms MainWeapon;
        public Firearms SecondaryWeapon;
        private Firearms carriedWeapon;

        // Weapon pick up
        public Transform EyeCameraTransform;
        public float RayCastMaxDistance; // for item pickup
        public LayerMask ItemLayerMask; // item will be placed under Item layer
        public List<Firearms> Arms = new List<Firearms>(); // the firearms (under player controller) enabled when the related gun is picked up

        internal bool isAiming;
        internal bool isFiring;
        internal bool isAttached;
        public GameObject AmmoCount;
        public Image WeaponIcon;
        private TMPro.TextMeshProUGUI AmmoText;
        public GameObject crosshair;

        void Start()
        {
            // if main weapon exist, then set main weapon as carried weapon
            if (MainWeapon != null)
                SetupCarriedWeapon(MainWeapon);
            // if main weapon not exist and secondary weapon exist, set secondary weapon as carried weapon
            if (MainWeapon == null && SecondaryWeapon != null)
                SetupCarriedWeapon(SecondaryWeapon);

            if (carriedWeapon)
            {
                AmmoText = AmmoCount.GetComponent<TMPro.TextMeshProUGUI>();
                WeaponIcon.sprite = carriedWeapon.GunIcon;
            }
        }

        void Update()
        {
            // method to pick up item
            CheckItem();
            // don't update if no weapon
            if (!carriedWeapon) return;

            // handle weapon swapping
            SwapWeapon();

            // hold the left mouse to shoot, no shooting on reloading
            if (Input.GetMouseButton(0) && !carriedWeapon.isReloading)
            {
                carriedWeapon.HoldTrigger();
                isFiring = true;
            }
            
            // release the left mouse to stop shooting
            if (Input.GetMouseButtonUp(0))
            {
                carriedWeapon.ReleaseTrigger();
                isFiring = false;
            }

            // reload ammo by pressing R
            if (Input.GetKeyDown(KeyCode.R) && !carriedWeapon.isReloading)
                carriedWeapon.ReloadAmmo();

            // weapon aiming by holding the right mouse, no aiming during reloading (disable crosshair when scope attached)
            if (Input.GetMouseButtonDown(1) && !carriedWeapon.isReloading)
            {
                carriedWeapon.StartAiming();
                isAiming = true;
                if (isAttached)
                    crosshair.gameObject.SetActive(false);
            }

            // stop weapon aiming by releasing the right mouse (enable crosshair when scope attached)
            if (Input.GetMouseButtonUp(1))
            {
                carriedWeapon.StopAiming();
                isAiming = false;
                if (isAttached)
                    crosshair.gameObject.SetActive(true);
            }
            
            // start lean shooting only when aiming, press Q for left lean, E for right lean
            if ((Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)) && carriedWeapon.isAiming)
                carriedWeapon.StartLeanShooting();
            else
                carriedWeapon.StopLeanShooting();

            UpdateAmmoInfo(carriedWeapon.GetCurrentAmmo, carriedWeapon.GetCurrentMaxAmmo);
        }

        private void UpdateAmmoInfo(int ammo, int remainingAmmo)
        {
            AmmoText.SetText(ammo + "/" + remainingAmmo);
        }

        private void SwapWeapon()
        {
            // 1. switch to main weapon by pressing Alpha1
            // 2. Cannot switch to itself or main weapon is not exist
            // 3. Cannot switch weapon when aiming or shooting
            if (Input.GetKeyDown(KeyCode.Alpha1) 
                && carriedWeapon != MainWeapon
                && MainWeapon != null
                && !carriedWeapon.isAiming
                && !carriedWeapon.IsHoldingTrigger) 
            {
                ResetTriggers();
                // active main weapon and set up the corresponding gun animator for fps controller
                carriedWeapon.gameObject.SetActive(false);
                carriedWeapon = MainWeapon;
                carriedWeapon.gameObject.SetActive(true);
                fpsController.SetupAnimator(carriedWeapon.GunAnimator);
            }
            // 1. switch to secondary weapon by pressing Alpha2
            else if (Input.GetKeyDown(KeyCode.Alpha2) 
                     && carriedWeapon != SecondaryWeapon 
                     && !carriedWeapon.isAiming 
                     && SecondaryWeapon != null &&
                     !carriedWeapon.IsHoldingTrigger)
            {
                ResetTriggers();
                carriedWeapon.gameObject.SetActive(false);
                carriedWeapon = SecondaryWeapon;
                carriedWeapon.gameObject.SetActive(true);
                fpsController.SetupAnimator(carriedWeapon.GunAnimator);
            }
            WeaponIcon.sprite = carriedWeapon.GunIcon;
        }

        private void CheckItem()
        {
            
            bool tmp_IsItem = Physics.Raycast(EyeCameraTransform.position,
                EyeCameraTransform.forward, out RaycastHit hit,
                RayCastMaxDistance, ItemLayerMask);
            // Need to check if the item already exist
            if (tmp_IsItem)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    bool tmp_HasItem = hit.collider.TryGetComponent(out BaseItem tmp_BaseItem);
                    if (tmp_HasItem)
                    {
                        PickupWeapon(tmp_BaseItem);
                        PickupAttachment(tmp_BaseItem);
                    }
                }
            }
        }

        private void PickupWeapon(BaseItem baseItem)
        {
            if (!(baseItem is FirearmsItem tmp_FirearmsItem)) return;
            foreach (Firearms tmp_Arm in Arms)
            {
                if (tmp_FirearmsItem.ArmsName.CompareTo(tmp_Arm.name) == 0)
                {
                    switch (tmp_FirearmsItem.CurrentFirearmsType)
                    {
                        case FirearmsItem.FirearmsType.AssaultRifle:
                            MainWeapon = tmp_Arm;
                            break;
                        case FirearmsItem.FirearmsType.HandGun:
                            SecondaryWeapon = tmp_Arm;
                            break;
                    }
                    tmp_FirearmsItem.HideItem();
                    SetupCarriedWeapon(tmp_Arm);
                }
            }
        }

        private void PickupAttachment(BaseItem baseItem)
        {
            if (!(baseItem is Attachment tmp_AttachmentItem)) return;

            switch (tmp_AttachmentItem.CurrentAttachmentType)
            {
                case Attachment.AttachmentType.Scope:
                    foreach (Firearms.ScopeInfo tmp_ScopeInfo in carriedWeapon.ScopeInfos)
                    {
                        if (tmp_ScopeInfo.ScopeName.CompareTo(tmp_AttachmentItem.ItemName) != 0)
                        {
                            tmp_ScopeInfo.ScopeGameObject.SetActive(false);
                            continue;
                        }
                        tmp_ScopeInfo.ScopeGameObject.SetActive(true);
                        carriedWeapon.SetupCarriedScope(tmp_ScopeInfo);
                        //
                        carriedWeapon.isAttached = true;
                        isAttached = true;
                    }
                    break;
                case Attachment.AttachmentType.Other:
                    break;
            }
        }

        // allow weapon switching during reloading
        private void ResetTriggers()
        {
            carriedWeapon.isReloading = false;
        }

        private void SetupCarriedWeapon(Firearms targetWeapon)
        {
            // disable current carried weapon if exist
            if (carriedWeapon)
                carriedWeapon.gameObject.SetActive(false);
            // swap to target weapon and set up gun animator
            carriedWeapon = targetWeapon;
            carriedWeapon.gameObject.SetActive(true);
            fpsController.SetupAnimator(carriedWeapon.GunAnimator);
        }

    }
}
   
