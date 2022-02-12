using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace PolyGamers.Weapon
{
    public class WeaponManager : MonoBehaviour
    {
        // FPS controller and Weapons
        [SerializeField] private FPSControllerCC fpsController;
        public Firearms MainWeapon;
        public Firearms SecondaryWeapon;
        private Firearms carriedWeapon;

        // Weapon pick up
        public Transform WorldCameraTransform;
        public float RayCastMaxDistance = 2f;
        public LayerMask CheckItemLayerMask;
        public List<Firearms> Arms = new List<Firearms>();

        public Camera Tmp_Camera;
        internal bool isAiming;
        internal bool isFiring;
        public GameObject AmmoCount;
        private TMPro.TextMeshProUGUI AmmoText;

        void Start()
        {
            // if main weapon exist, then set main weapon as carried weapon
            if (MainWeapon != null)
                SetupCarriedWeapon(MainWeapon);
            // if main weapon not exist and secondary weapon exist, set secondary weapon as carried weapon
            if (MainWeapon == null && SecondaryWeapon != null)
                SetupCarriedWeapon(SecondaryWeapon);

            AmmoText = AmmoCount.GetComponent<TMPro.TextMeshProUGUI>();
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

            // weapon aiming by holding the right mouse, no aiming during reloading
            if (Input.GetMouseButtonDown(1) && !carriedWeapon.isReloading)
            {
                carriedWeapon.StartAiming();
                isAiming = true;
            }

            // stop weapon aiming by releasing the right mouse
            if (Input.GetMouseButtonUp(1))
            {
                carriedWeapon.StopAiming();
                isAiming = false;
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
            // Tmp_Camera is a camera if player doesn't have any weapon
            Tmp_Camera.gameObject.SetActive(false);
        }

    }
}
   
