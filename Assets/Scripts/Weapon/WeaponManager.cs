using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class WeaponManager : MonoBehaviour
    {
        public InputActions inputSchemes;
        // FPS controller and Weapons
        [SerializeField] private FPSControllerCC fpsController;
        // 1st weapon in the inspector
        public Firearms MainWeapon;
        public Firearms SecondaryWeapon;
        public Firearms TertiaryWeapon;
        public Firearms QuaternaryWeapon;
        public Firearms QuinaryWeapon;

        [SerializeField] private GameObject weaponCollection;
        private List<Firearms> presetWeaponList;

        // current weapon player equipped
        private Firearms carriedWeapon;

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

        [SerializeField] private AudioSource pickupSound;

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

            // set up default weapon on game start
            MainWeapon = PlayerInventory.Instance.DefaultWeapon;
            if (MainWeapon != null)
            {
                SetupCarriedWeapon(MainWeapon);
            }
            else
            {
                Debug.LogError("Error: Default weapon not set. ");
            }

        }

        void Update()
        {
            MainWeapon = PlayerInventory.Instance.PlayerWeapons[0];
            SecondaryWeapon = PlayerInventory.Instance.PlayerWeapons[1];
            TertiaryWeapon = PlayerInventory.Instance.PlayerWeapons[2];
            QuaternaryWeapon = PlayerInventory.Instance.PlayerWeapons[3];
            QuinaryWeapon = PlayerInventory.Instance.PlayerWeapons[4];

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
                if (inputSchemes.FPSActions.Shoot.ContinuedShooting() && !carriedWeapon.isReloading)
                {
                    carriedWeapon.HoldTrigger();
                    isFiring = true;
                }
            }
            // shooting type: fixed
            if (carriedWeapon.shootingType == Firearms.ShootingType.Fixed)
            {
                if (inputSchemes.FPSActions.Shoot.FixedShooting() && !carriedWeapon.isReloading)
                {
                    carriedWeapon.HoldTrigger();
                    isFiring = true;
                }
            }
            // release the left mouse to stop shooting
            if (inputSchemes.FPSActions.Shoot.StopShooting())
            {
                carriedWeapon.ReleaseTrigger();
                isFiring = false;
            }

            if (inputSchemes.FPSActions.SwitchBetweenWeapon.ReadValue<float>() > 0 && !carriedWeapon.isAiming)
            {
                //Debug.Log("Switch to previous");
                SwitchToPreviousWeapon();
            } 
            else if (inputSchemes.FPSActions.SwitchBetweenWeapon.ReadValue<float>() < 0 && !carriedWeapon.isAiming)
            {
                //Debug.Log("Switch to next");
                SwitchToNextWeapon();
            }

            // start lean shooting only when aiming, press Q for left lean, E for right lean
            if (inputSchemes.FPSActions.LeanLeft.IsPressed() && carriedWeapon.isAiming)
                carriedWeapon.StartLeftLeanShooting();
            else if (inputSchemes.FPSActions.LeanRight.IsPressed() && carriedWeapon.isAiming)
                carriedWeapon.StartRightLeanShooting();
            else
                carriedWeapon.StopLeanShooting();

        }

        // reload ammo by pressing R
        public void StartReloadAmmo()
        {
            if (!carriedWeapon.isReloading)
                carriedWeapon.ReloadAmmo();
        }

        // weapon aiming by holding the right mouse, no aiming during reloading (disable crosshair when scope attached)
        public void StartAiming()
        {
            if (!carriedWeapon.isReloading)
            {
                carriedWeapon.StartAiming();
                isAiming = true;
            }
        }

        // stop weapon aiming by releasing the right mouse (enable crosshair when scope attached)
        public void StopAiming()
        {
            carriedWeapon.StopAiming();
            isAiming = false;
        }

        // getter method for current weapon
        public Firearms CarriedWeapon { get { return carriedWeapon; } }

        private void SwapWeapon()
        {
            // 1. switch to main weapon by pressing Alpha1
            // 2. Cannot switch to itself or main weapon is not exist
            // 3. Cannot switch weapon when aiming
            if (inputSchemes.FPSActions.SwitchToMainWeapon.triggered 
                && carriedWeapon != MainWeapon
                && MainWeapon != null
                && !carriedWeapon.isAiming)
            {
                ResetTriggers();
                // active main weapon and set up the corresponding gun animator for fps controller
                carriedWeapon.gameObject.SetActive(false);
                carriedWeapon = MainWeapon;
                carriedWeapon.gameObject.SetActive(true);
                fpsController.SetupAnimator(carriedWeapon.GunAnimator);

                PlayerSkillManager.Instance.TryActivateSetSkill();
            }
            // 1. switch to secondary weapon by pressing Alpha2
            else if (inputSchemes.FPSActions.SwitchToSecondaryWeapon.triggered
                     && carriedWeapon != SecondaryWeapon
                     && SecondaryWeapon != null
                     && !carriedWeapon.isAiming)
            {
                ResetTriggers();
                carriedWeapon.gameObject.SetActive(false);
                carriedWeapon = SecondaryWeapon;
                carriedWeapon.gameObject.SetActive(true);
                fpsController.SetupAnimator(carriedWeapon.GunAnimator);

                PlayerSkillManager.Instance.TryActivateSetSkill();
            }
            // switch to 3rd weapon
            else if (inputSchemes.FPSActions.SwitchToTertiaryWeapon.triggered
                     && carriedWeapon != TertiaryWeapon
                     && TertiaryWeapon != null
                     && !carriedWeapon.isAiming)
            {
                ResetTriggers();
                carriedWeapon.gameObject.SetActive(false);
                carriedWeapon = TertiaryWeapon;
                carriedWeapon.gameObject.SetActive(true);
                fpsController.SetupAnimator(carriedWeapon.GunAnimator);

                PlayerSkillManager.Instance.TryActivateSetSkill();
            }
            // switch to 4th weapon
            else if (inputSchemes.FPSActions.SwitchToQuaternaryWeapon.triggered
                     && carriedWeapon != QuaternaryWeapon
                     && QuaternaryWeapon != null
                     && !carriedWeapon.isAiming)
            {
                ResetTriggers();
                carriedWeapon.gameObject.SetActive(false);
                carriedWeapon = QuaternaryWeapon;
                carriedWeapon.gameObject.SetActive(true);
                fpsController.SetupAnimator(carriedWeapon.GunAnimator);

                PlayerSkillManager.Instance.TryActivateSetSkill();
            }
            // switch to 5th weapon
            else if (inputSchemes.FPSActions.SwitchToQuinaryWeapon.triggered
                     && carriedWeapon != QuinaryWeapon
                     && QuinaryWeapon != null
                     && !carriedWeapon.isAiming)
            {
                ResetTriggers();
                carriedWeapon.gameObject.SetActive(false);
                carriedWeapon = QuinaryWeapon;
                carriedWeapon.gameObject.SetActive(true);
                fpsController.SetupAnimator(carriedWeapon.GunAnimator);

                PlayerSkillManager.Instance.TryActivateSetSkill();
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
                bool hasItem = hit.collider.TryGetComponent(out BaseItem item);

                ItemPeekControl.Instance.PeekItem(item);

                //TipsControl.Instance.PopUpTip("T", "Pick up");

                // player pick up weapon by pressing T
                if (inputSchemes.PlayerActions.PickUp.triggered)
                {
                    // get the item component of type BaseItem, if it exists
                    if (hasItem)
                    {
                        pickupSound.Play();

                        PickupWeapon(item);
                        PickupAttachment(item);
                    }
                }
            }
            else
            {
                ItemPeekControl.Instance.Clear();
                //TipsControl.Instance.PopOffTip();
            }
        }

        private void SwitchToPreviousWeapon()
        {
            int numWeaponPossessed = PlayerInventory.Instance.PlayerWeapons.Where(element => element != null).Count();
            int currentWeaponIndex = Array.IndexOf(PlayerInventory.Instance.PlayerWeapons, carriedWeapon);
            if (numWeaponPossessed == 1) return;
            Firearms weaponToSwitch = null;
            if (currentWeaponIndex == 0)
                weaponToSwitch = PlayerInventory.Instance.PlayerWeapons[numWeaponPossessed - 1];
            else
                weaponToSwitch = PlayerInventory.Instance.PlayerWeapons[currentWeaponIndex - 1];
            ResetTriggers();
            carriedWeapon.gameObject.SetActive(false);
            carriedWeapon = weaponToSwitch;
            carriedWeapon.gameObject.SetActive(true);
            fpsController.SetupAnimator(carriedWeapon.GunAnimator);

            PlayerSkillManager.Instance.TryActivateSetSkill();

        }

        private void SwitchToNextWeapon()
        {
            int numWeaponPossessed = PlayerInventory.Instance.PlayerWeapons.Where(element => element != null).Count();
            int currentWeaponIndex = Array.IndexOf(PlayerInventory.Instance.PlayerWeapons, carriedWeapon);
            if (numWeaponPossessed == 1) return;
            Firearms weaponToSwitch = null;
            if (currentWeaponIndex == numWeaponPossessed - 1)
                weaponToSwitch = PlayerInventory.Instance.PlayerWeapons[0];
            else
                weaponToSwitch = PlayerInventory.Instance.PlayerWeapons[currentWeaponIndex + 1];
            ResetTriggers();
            carriedWeapon.gameObject.SetActive(false);
            carriedWeapon = weaponToSwitch;
            carriedWeapon.gameObject.SetActive(true);
            fpsController.SetupAnimator(carriedWeapon.GunAnimator);

            PlayerSkillManager.Instance.TryActivateSetSkill();
        }

        private void PickupWeapon(BaseItem baseItem)
        {
            // if the baseItem find is not FirearmsItem, return
            if (!(baseItem is FirearmsItem firearmsItem)) return;

            var weapon = firearmsItem.Weapon;

            if (PlayerInventory.Instance.IsWeaponInventoryFull())
            {
                DropWeapon(carriedWeapon, baseItem.transform.position);
            }

            PlayerInventory.Instance.AddWeapon(weapon);

            //baseItem.gameObject.SetActive(false);
            Destroy(baseItem.gameObject);

            weapon.gameObject.transform.SetParent(weaponCollection.transform, false);

            SetupCarriedWeapon(weapon);
        }

        private void PickupAttachment(BaseItem baseItem)
        {
            // if the baseItem find is not Attachment, return
            if (!(baseItem is AttachmentItem attachmentItem)) return;

            var attachment = attachmentItem.Attachment;

            PlayerInventory.Instance.AddAttachment(attachment);

            baseItem.gameObject.SetActive(false);
        }

        private void DropWeapon(Firearms weapon, Vector3 position)
        {
            DropoffManager.Instance.CreateDropoffFromWeapon(weapon, position);

            PlayerInventory.Instance.RemoveWeapon(weapon);
        }

        // allow weapon switching during reloading or shooting
        private void ResetTriggers()
        {
            carriedWeapon.isReloading = false;
            carriedWeapon.IsHoldingTrigger = false;
        }

        public void SetupCarriedWeapon(Firearms targetWeapon)
        {
            // disable current carried weapon if exist
            if (carriedWeapon)
            {
                carriedWeapon.isAiming = false;
                carriedWeapon.IsHoldingTrigger = false;
                carriedWeapon.isReloading = false;
                carriedWeapon.gameObject.SetActive(false);
            }
            // swap to target weapon and set up gun animator
            targetWeapon.isAiming = false;
            targetWeapon.IsHoldingTrigger = false;
            targetWeapon.isReloading = false;
            carriedWeapon = targetWeapon;
            carriedWeapon.gameObject.SetActive(true);
            // set up the correct gun animator
            fpsController.SetupAnimator(carriedWeapon.GunAnimator);
        }


        public Vector3 GetShootDirection()
        {
            return carriedWeapon.transform.forward.normalized;
        }


        public GameObject WeaponCollection { get { return weaponCollection; } }
    }
}
   
