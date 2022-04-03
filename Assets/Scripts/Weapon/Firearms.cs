using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace CSE5912.PolyGamers
{
    public abstract class Firearms : MonoBehaviour, IWeapon
    {
        // weapon stats
        [Header("WeaponStats")]
        [SerializeField] private string weaponName;
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private WeaponRarity rarity = WeaponRarity.Common;
        [SerializeField] private float damage = 0f;
        [SerializeField] private Element.Type element;
        public bool BulletPenetrable = false;
        private WeaponBonus weaponBonus;

        public enum WeaponRarity
        {
            Common = 0,
            Rare = 1,
            Epic = 2,
            Legendary = 3,
            Divine = 4,
        }
        public enum WeaponType
        {
            AK47,
            Glock18,
            M16,
            SCAR,
            BERETTA,
            P85,
            HKP7,
            MP7,
            VZ61,
            UZI,
            P90,
            MP5,
            ShotGun
        }

        [Header("WeaponEffect")]
        // position where bullet generate
        public Transform MuzzlePoint;
        // position where bullet pops
        public Transform CasingPoint; 
        public GameObject BulletPrefab;
        public ParticleSystem MuzzleParticle;
        public ParticleSystem CastingParticle;
        public GameObject ImpactPrefab;
        [SerializeField] private GameObject ironSights;
        private GameObject arms;


        [Header("WeaponInfo")]
        // current ammo in mag (per)
        public int CurrentAmmo;
        // predefined ammo per mag (AK: 30, Glock: 8)
        public int MaxAmmoPerMag;
        // total current ammo left in mag (all)
        public int CurrentMaxAmmoCarried;
        // predefined ammo total (AK: 120, Glock: 32)
        public int MaxAmmoCarried;

        public float FireRate;
        protected float LastFireTime;
        // current gun animator
        internal Animator GunAnimator; 
        protected AnimatorStateInfo GunStateInfo;

        public UnityEvent shootEvent;
        public Bullet bulletFired;
        
        // Get current ammo and ammo left in mag 
        //public int GetCurrentAmmo => CurrentAmmo;
        public int GetCurrentMaxAmmo => CurrentMaxAmmoCarried;

        [Header("WeaponAudioInfo")]
        public AudioSource ShootingAudioSource;
        public AudioSource ReloadingAudioSource;
        public FirearmsAudioData WeaponAudioData;

        [Header("WeaponAiming")]
        public Camera GunCamera;
        protected float GunCameraOriginFOV;
        // the updated field of view for gun camera when aimed
        public float FOVWhenAimed;
        internal bool isAiming;
        private IEnumerator doAimingCoroutine;

        // Weapon Shooting & Reloading
        internal bool IsHoldingTrigger;
        internal bool isReloading;

        [Header("WeaponLeaning")]
        // camera local rotation and position
        protected Quaternion GunCameraLocalOriginalRotation;
        protected Vector3 GunCameraLocalOriginalPosition;
        // camera slerping value
        public float SlerpTime;
        public float SlerpAngle;
        public float SlerpDistance;

        [Header("CameraShaking")] 
        public float SpreadAngle;

        public enum ShootingType { Fixed, Continued }
        public ShootingType shootingType;

        // UI related
        [Header("UI related")]
        public Sprite iconImage;

        // Attachments
        [Header("Attachments")]
        [SerializeField] protected Attachment[] attachments;
        public Attachment[] Attachments { get { return attachments; } set { attachments = value; } }

        public FirearmsItem firearmsItem;

        //[Header("Attachment info")]
        //// holder of different scopes in this gun
        //public List<ScopeInfo> ScopeInfos = new List<ScopeInfo>();
        //// store the information of scopes, like scope gameobject, fov
        //protected ScopeInfo scopeInfo;
        //[System.Serializable]
        //public class ScopeInfo
        //{
        //    public string ScopeName;
        //    public GameObject ScopeGameObject;
        //    public float GunCameraFovWhenAttached;
        //    public Vector3 GunCameraPosition;
        //}
        public bool wasBulletFiredThisFrame = false;
        // scope attached
        internal bool isAttached;
        private Attachment currentAttachment;
        private Bullet prevBullet;

        protected virtual void Awake()
        {
            // set up current ammo
            CurrentAmmo = MaxAmmoPerMag; 
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            // set up the gun animator
            GunAnimator = GetComponent<Animator>(); 
            // save the original value for gun camera FOV, localPos and localRot
            GunCameraOriginFOV = GunCamera.fieldOfView; 
            GunCameraLocalOriginalRotation = GunCamera.transform.localRotation;
            GunCameraLocalOriginalPosition = GunCamera.transform.localPosition;
            doAimingCoroutine = DoAim();

            // define how many attachments one gun could have (4)
            attachments = new Attachment[4];

            foreach (Transform child in transform)
            {
                if (child.name == "arms")
                {
                    arms = child.gameObject;
                }
            }
            if (arms == null)
                Debug.LogError("arms not found");

            var go = Instantiate(BulletPrefab);
            Destroy(go);

            weaponBonus = new WeaponBonus(rarity);
            shootEvent = new UnityEvent();
        }
       

        private void OnEnable()
        {
            PerformBonus(true);
        }
        private void OnDisable()
        {
            PerformBonus(false);
        }
        public void PerformBonus(bool enabled)
        {
            weaponBonus.Perform(enabled);
            foreach (var attachment in attachments)
                if (attachment != null)
                    attachment.PerformBonus(enabled);
        }

        private void PerformMeleeAttack()
        {
            MeleeAttack.Instance.PerformDamage();
        }


        public bool UpdateBulletFired()
        {
            bool shoot = WeaponManager.Instance.isFiring && prevBullet != bulletFired;

            prevBullet = bulletFired;

            return shoot;
        }

        private void Update()
        {
            wasBulletFiredThisFrame = UpdateBulletFired();
        }

        public void Display(bool enabled)
        {
            arms.SetActive(enabled);
            if (isAttached)
            {
                this.gameObject.transform.Find("Armature/weapon/" + currentAttachment.AttachmentRealName).gameObject.SetActive(enabled);
            }
        }

        public void Attack()
        {
            Shoot();
        }

        protected bool IsAllowShooting()
        {
            // check if able to shoot
            return Time.unscaledTime - LastFireTime > 1 / FireRate * PlayerStats.Instance.FireRateFactor;
        }

        protected abstract void Shoot();
        protected abstract void Reload();
        protected abstract void CameraLeftLean();
        protected abstract void CameraLeanRight();
        protected abstract void StopCameraLean();

        public Vector3 CalculateBulletSpreadOffset()
        {
            // aimed => less bullet spread
            float spreadPercentage = SpreadAngle / (100 - GunCamera.fieldOfView);
            return spreadPercentage * UnityEngine.Random.insideUnitCircle;
        }

        protected IEnumerator CheckReloadAmmoAnimationEnd()
        {
            while (true)
            {
                yield return null;
                GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(2);
                if (GunStateInfo.IsTag("ReloadAmmo"))
                {
                    if (GunStateInfo.normalizedTime >= 0.9f)
                    {
                        // calculate how many ammo have been used and how much ammo left
                        int ammoUsed = MaxAmmoPerMag - CurrentAmmo;
                        int remaingAmmo = CurrentMaxAmmoCarried - ammoUsed;
                        if (remaingAmmo <= 0)
                            CurrentAmmo += CurrentMaxAmmoCarried;
                        else
                            CurrentAmmo = MaxAmmoPerMag;
                        // update how many ammo left 
                        CurrentMaxAmmoCarried = remaingAmmo <= 0 ? 0 : remaingAmmo;
                        // reloading finished
                        isReloading = false;
                        yield break;
                    }
                }
            }
        }

        // weapon aiming
        protected void Aiming()
        {
            GunAnimator.SetBool("Aim", isAiming);
            if (doAimingCoroutine == null)
            {
                doAimingCoroutine = DoAim();
                StartCoroutine(doAimingCoroutine);
            }
            else
            {
                StopCoroutine(doAimingCoroutine);
                doAimingCoroutine = null;
                doAimingCoroutine = DoAim();
                StartCoroutine(doAimingCoroutine);
            }
        }

        protected IEnumerator DoAim()
        {
            while (true)
            {
                yield return null;
                float tmp_RefGunCameraFOV = 0f;
                // smooth transit to aiming fov
                GunCamera.fieldOfView = Mathf.SmoothDamp(GunCamera.fieldOfView,
                        isAiming ? FOVWhenAimed : GunCameraOriginFOV,
                        ref tmp_RefGunCameraFOV,
                        Time.unscaledDeltaTime * 2);

                if (isAttached)
                {
                    Vector3 tmp_RefGunCameraPosition = Vector3.zero;
                    Vector3 aimingTo = Vector3.zero;
                    switch (weaponType)
                    {
                        case WeaponType.AK47:
                            if (currentAttachment.AttachmentRealName == "Scope Carbine A")
                                aimingTo = new Vector3(0, -0.2f, 0.0251f);
                            else if (currentAttachment.AttachmentRealName == "Scope Holo A")
                                aimingTo = new Vector3(0, -0.2f, 0.0281f);
                            else if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.2f, 0.03f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.M16:
                            if (currentAttachment.AttachmentRealName == "Scope Carbine A")
                                aimingTo = new Vector3(0, -0.02f, -0.01f);
                            else if (currentAttachment.AttachmentRealName == "Scope Holo A")
                                aimingTo = new Vector3(0, -0.02f, -0.005f);
                            else if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.02f, 0.01f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.SCAR:
                            if (currentAttachment.AttachmentRealName == "Scope Carbine A")
                                aimingTo = new Vector3(0, -0.02f, -0.01f);
                            else if (currentAttachment.AttachmentRealName == "Scope Holo A")
                                aimingTo = new Vector3(0, -0.02f, -0.005f);
                            else if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.02f, 0.005f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.Glock18:
                            if (currentAttachment.AttachmentRealName == "Scope Carbine A")
                                aimingTo = new Vector3(0, -0.2f, 0.018f);
                            else if (currentAttachment.AttachmentRealName == "Scope Holo A")
                                aimingTo = new Vector3(0, -0.2f, 0.02f);
                            else if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.2f, 0.025f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.BERETTA:
                            if (currentAttachment.AttachmentRealName == "Scope Carbine A")
                                aimingTo = new Vector3(0, -0.25f, 0.008f);
                            else if (currentAttachment.AttachmentRealName == "Scope Holo A")
                                aimingTo = new Vector3(0, -0.25f, 0.01f);
                            else if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.25f, 0.018f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.P85:
                            if (currentAttachment.AttachmentRealName == "Scope Carbine A")
                                aimingTo = new Vector3(0, -0.25f, 0.008f);
                            else if (currentAttachment.AttachmentRealName == "Scope Holo A")
                                aimingTo = new Vector3(0, -0.25f, 0.015f);
                            else if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.25f, 0.018f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.HKP7:
                            if (currentAttachment.AttachmentRealName == "Scope Carbine A")
                                aimingTo = new Vector3(0, -0.25f, 0.008f);
                            else if (currentAttachment.AttachmentRealName == "Scope Holo A")
                                aimingTo = new Vector3(0, -0.25f, 0.015f);
                            else if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.25f, 0.018f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.MP7:
                            if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.02f, 0.03f);
                            else
                                aimingTo = new Vector3(0, -0.02f, 0.015f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.VZ61:
                            if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.02f, 0.03f);
                            else
                                aimingTo = new Vector3(0, -0.015f, 0.015f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.UZI:
                            if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.2f, 0.035f);
                            else
                                aimingTo = new Vector3(0, -0.2f, 0.02f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.P90:
                            if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.2f, 0.058f);
                            else
                                aimingTo = new Vector3(0, -0.2f, 0.042f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.MP5:
                            if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.02f, 0.03f);
                            else
                                aimingTo = new Vector3(0, -0.015f, 0.015f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                        case WeaponType.ShotGun:
                            if (currentAttachment.AttachmentRealName == "Scope RedDotOuter A")
                                aimingTo = new Vector3(0, -0.02f, 0.03f);
                            else
                                aimingTo = new Vector3(0, -0.015f, 0.02f);
                            GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                                isAiming ? aimingTo : GunCameraLocalOriginalPosition,
                                ref tmp_RefGunCameraPosition,
                                Time.unscaledDeltaTime * 2);
                            break;
                    }
                }

                //TODO: check attachment, if true, smooth transit gun camera position to (0,-0.2f,0.03f)
                //if (isAttached)
                //{
                //    //float tmp_GunCurrentFOV = 0f;
                //    //GunCamera.fieldOfView = Mathf.SmoothDamp(GunCamera.fieldOfView,
                //    //    isAiming ? scopeInfo.GunFov : GunOriginFOV,
                //    //    ref tmp_GunCurrentFOV,
                //    //    Time.deltaTime * 2);

            }
        }

        //internal void SetupCarriedScope(ScopeInfo scopeInfo)
        //{
        //    this.scopeInfo = scopeInfo;
        //}

        // for weapon shooting
        internal void HoldTrigger()
        {
            Attack();
            IsHoldingTrigger = true;
        }

        internal void ReleaseTrigger()
        {
            IsHoldingTrigger = false;
        }

        // for weapon aiming
        internal void StartAiming()
        {
            isAiming = true;
            if(isAttached && weaponType != WeaponType.M16 && weaponType != WeaponType.SCAR && weaponType != WeaponType.P85 && weaponType != WeaponType.UZI)
                this.ironSights.SetActive(false);
            Aiming();
        }

        internal void StopAiming()
        {
            isAiming = false;
            if (isAttached && weaponType != WeaponType.M16 && weaponType != WeaponType.SCAR && weaponType != WeaponType.P85 && weaponType != WeaponType.UZI)
                this.ironSights.SetActive(true);
            Aiming();
        }

        // for weapon reloading
        internal void ReloadAmmo()
        {
            if (CurrentAmmo == MaxAmmoPerMag) return;
            isReloading = true;
            isAiming = false;
            GunAnimator.SetBool("Aim", isAiming);
            Reload();
        }

        // for weapon lean shooting
        internal void StartLeftLeanShooting()
        {
            CameraLeftLean();
        }
        internal void StartRightLeanShooting()
        {
            CameraLeanRight();
        }

        internal void StopLeanShooting()
        {
            StopCameraLean();
        }

        /*
         * Ui related
         */
        public void SetAttachment(Attachment attachment, int index)
        {
            RemoveAttachment(attachments[index]);

            attachments[index] = attachment;
            attachment.AttachTo(this);

            if (attachment.Type == Attachment.AttachmentType.Scope)
            {
                this.gameObject.transform.Find("Armature/weapon/" + attachment.AttachmentRealName).gameObject.SetActive(true);
                if (this.weaponType is WeaponType.M16 || this.weaponType is WeaponType.SCAR || this.weaponType is WeaponType.P85 || this.weaponType is WeaponType.UZI)
                    ironSights.SetActive(false);
                isAttached = true;
                currentAttachment = attachment;
            }
        }

        public void RemoveAttachment(Attachment target)
        {
            if (target == null)
                return;

            for (int i = 0; i < attachments.Length; i++)
            {
                if (attachments[i] == target)
                {
                    isAttached = false;

                    if (target.Type == Attachment.AttachmentType.Scope)
                    {
                        this.gameObject.transform.Find("Armature/weapon/" + attachments[i].AttachmentRealName).gameObject.SetActive(false);
                        if (this.weaponType is WeaponType.M16 || this.weaponType is WeaponType.SCAR || this.weaponType is WeaponType.P85 || this.weaponType is WeaponType.UZI)
                            ironSights.SetActive(true);
                    }

                    attachments[i] = null;
                    target.AttachTo(null);
                }
            }
        }

        public string WeaponName { get { return weaponName; } set { weaponName = value; } }
        public WeaponType Type { get { return weaponType; } } 
        public WeaponRarity Rarity { get { return rarity; } set { rarity = value; } }
        public float Damage { get { return damage; } set { damage = value; } }
        public Element.Type Element { get { return element; } set { element = value; } }
        public WeaponBonus Bonus { get { return weaponBonus; } set { weaponBonus = value; } }
        public Sprite IconImage { get { return iconImage; } }

    }
}