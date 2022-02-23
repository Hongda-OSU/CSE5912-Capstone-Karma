using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class WeaponManager : MonoBehaviour
    {
        // FPS controller and Weapons
        [SerializeField] private FPSControllerCC fpsController;
        // 1st weapon in the inspector
        public Firearms MainWeapon;
        public Firearms SecondaryWeapon;
        // TODO: add three more weapon 

        [SerializeField] private GameObject weaponCollection;
        private List<Firearms> presetWeaponList;

        // current weapon player equipped
        private Firearms carriedWeapon;
        // TODO: comment

        // Weapon pick up
        public Transform EyeCameraTransform;
        // the distance for player to pickup the weapon
        public float RayCastMaxDistance;
        // weapons and attachments will be placed under Item layer
        public LayerMask ItemLayerMask;
        // a list of weapons that could be enabled when the related gun is picked up
        //public List<Firearms> Arms = new List<Firearms>(); 

        // condition checking
        internal bool isAiming;
        internal bool isFiring;

        // WeaponManager singleton
        private static WeaponManager instance;
        public static WeaponManager Instance { get { return instance; } }

        private void Awake()
        {
            // create 
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            presetWeaponList = new List<Firearms>();
        }

        void Start()
        {
            // disable all weapons on start
            foreach (Transform child in weaponCollection.transform)
            {
                var weapon = child.GetComponent<Firearms>();
                presetWeaponList.Add(weapon);
                weapon.gameObject.SetActive(false);
            }


            MainWeapon = PlayerInventory.Instance.DefaultWeapon;
            // baihua: player always has at least one defualt weapon on start
            // if main weapon exist, then set main weapon as carried weapon (By default the player has a primary weapon)
            if (MainWeapon != null)
            {
                SetupCarriedWeapon(MainWeapon);
            }
            else
            {
                Debug.LogError("Error: Default weapon not set. ");
            }

            //// if main weapon not exist and secondary weapon exist, set secondary weapon as carried weapon
            //if (MainWeapon == null && SecondaryWeapon != null)
            //    SetupCarriedWeapon(SecondaryWeapon);
        }

        void Update()
        {
            MainWeapon = PlayerInventory.Instance.PlayerWeapons[0];
            SecondaryWeapon = PlayerInventory.Instance.PlayerWeapons[1];

            // check item pick up
            CheckItem();
            // don't update if no weapon
            if (!carriedWeapon) return;
            // handle weapon swapping
            SwapWeapon();

            // hold the left mouse to shoot, no shooting on reloading
            // shooting type: continue
            if (carriedWeapon.shootingType == Firearms.ShootingType.Continued)
            {
                if (Input.GetMouseButton(0) && !carriedWeapon.isReloading)
                {
                    carriedWeapon.HoldTrigger();
                    isFiring = true;
                }
            }
            // shooting type: fixed
            if (carriedWeapon.shootingType == Firearms.ShootingType.Fixed)
            {
                if (Input.GetMouseButtonDown(0) && !carriedWeapon.isReloading)
                {
                    carriedWeapon.HoldTrigger();
                    isFiring = true;
                }
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
            }

            // stop weapon aiming by releasing the right mouse (enable crosshair when scope attached)
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
        }

        // getter method for current weapon
        public Firearms CarriedWeapon { get { return carriedWeapon; } }

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
            // using raycast to detect if the item in front of player is in Item layer
            bool isItem = Physics.Raycast(EyeCameraTransform.position,
                EyeCameraTransform.forward, out RaycastHit hit,
                RayCastMaxDistance, ItemLayerMask);
            if (isItem)
            {
                // player pick up weapon by pressing E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // get the item component of type BaseItem, if it exists
                    bool hasItem = hit.collider.TryGetComponent(out FirearmsItem firearmsItem);
                    if (hasItem)
                    {
                        PickupWeapon(firearmsItem);
                        Debug.Log(firearmsItem.Weapon.WeaponName);
                    }
                    //if (hasItem)
                    //{
                    //    // pick up the baseItem (could be FirearmsItem or Attachment)
                    //    PickupWeapon(baseItem);
                    //    PickupAttachment(baseItem);
                    //}
                }
            }
        }

        private void PickupWeapon(FirearmsItem firearmsItem)
        {
            // if the baseItem find is not FirearmsItem, return
            //if (!(firearmsItem is FirearmsItem firearmsItem)) return;
            var weapon = firearmsItem.Weapon;

            weapon.gameObject.transform.SetParent(weaponCollection.transform, false);

            PlayerInventory.Instance.AddWeapon(weapon);

            SetupCarriedWeapon(weapon);

            // loop through each arm in the Firearm list
            //foreach (Firearms arm in Arms)
            //{
            //    // compare the arm name to find the corresponding Firearm weapon
            //    if (firearmsItem.ArmsName.CompareTo(arm.name) == 0)
            //    {
            //        // equip firearm weapon base on weapon type
            //        switch (firearmsItem.CurrentFirearmsType)
            //        {
            //            case FirearmsItem.FirearmsType.AssaultRifle:
            //                MainWeapon = arm;
            //                break;
            //            case FirearmsItem.FirearmsType.HandGun:
            //                SecondaryWeapon = arm;
            //                break;
            //        }
            //        // firearmsItem.HideItem();
            //        SetupCarriedWeapon(arm);
            //    }
            //}
        }

        private void PickupAttachment(BaseItem baseItem)
        {
            // if the baseItem find is not Attachment, return
            if (!(baseItem is AttachmentItem attachmentItem)) return;
            // enable attachment on weapon base on attachment type
            //switch (attachmentItem.Type)
            //{
            //    case Attachment.AttachmentType.Scope:
            //        foreach (Firearms.ScopeInfo scopeInfo in carriedWeapon.ScopeInfos)
            //        {
            //            // find the right scope to enable
            //            if (scopeInfo.ScopeName.CompareTo(attachmentItem.) != 0)
            //            {
            //                scopeInfo.ScopeGameObject.SetActive(false);
            //                continue;
            //            }
            //            // enable the scope
            //            scopeInfo.ScopeGameObject.SetActive(true);
            //            carriedWeapon.SetupCarriedScope(scopeInfo);
            //            // enable scoping value in Firearms
            //            carriedWeapon.isAttached = true;
            //        }
            //        break;
            //    case Attachment.AttachmentType.Other:
            //        break;
            //}
        }

        // allow weapon switching during reloading
        private void ResetTriggers()
        {
            carriedWeapon.isReloading = false;
        }

        public void SetupCarriedWeapon(Firearms targetWeapon)
        {
            // disable current carried weapon if exist
            if (carriedWeapon)
                carriedWeapon.gameObject.SetActive(false);
            // swap to target weapon and set up gun animator
            carriedWeapon = targetWeapon;
            carriedWeapon.gameObject.SetActive(true);
            // set up the correct gun animator
            fpsController.SetupAnimator(carriedWeapon.GunAnimator);
        }
    }
}
   
