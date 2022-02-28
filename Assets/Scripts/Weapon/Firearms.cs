using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
            BENELLI
        }

        [Header("WeaponEffect")]
        // position where bullet generate
        public Transform MuzzlePoint;
        // position where bullet pops
        public Transform CasingPoint; 
        public GameObject BulletPrefab;
        public ParticleSystem MuzzleParticle;
        public ParticleSystem CastingParticle;


        [Header("WeaponInfo")]
        // predefined ammo per mag (AK: 30, Glock: 8)
        public int AmmoInMag;
        // predefined ammo total (AK: 120, Glock: 32)
        public int MaxAmmoCarried;
        public float FireRate;
        // current ammo in mag (per)
        public int CurrentAmmo;
        // total current ammo left in mag (all)
        protected int CurrentMaxAmmoCarried;
        protected float LastFireTime;
        // current gun animator
        internal Animator GunAnimator; 
        protected AnimatorStateInfo GunStateInfo;
        
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
        public Attachment[] Attachments { get { return attachments; } }

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
        internal bool isAttached;

        // Firearms singleton
        //public static Firearms Instance { get; private set; }

        protected virtual void Awake()
        {
            weaponBonus = new WeaponBonus(this);
            // set up current ammo
            CurrentAmmo = AmmoInMag; 
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            // set up the gun animator
            GunAnimator = GetComponent<Animator>(); 
            // save the original value for gun camera FOV, localPos and localRot
            GunCameraOriginFOV = GunCamera.fieldOfView; 
            GunCameraLocalOriginalRotation = GunCamera.transform.localRotation;
            GunCameraLocalOriginalPosition = GunCamera.transform.localPosition;
            doAimingCoroutine = DoAim();
        }

        protected void Start()
        {
            // define how many attachments one gun could have (4)
            attachments = new Attachment[PlayerInventory.Instance.MaxNumOfAttachmentsPerWeapon];
        }

        protected virtual void Update()
        {
            weaponBonus.Perform(true);
        }
        private void OnDisable()
        {
            weaponBonus.Perform(false);
        }

        public void Attack()
        {
            Shoot();
        }

        protected bool IsAllowShooting()
        {
            // check if able to shoot
            return Time.time - LastFireTime > 1 / FireRate;
        }

        protected abstract void Shoot();
        protected abstract void Reload();
        protected abstract void CameraLeftLean();
        protected abstract void CameraLeanRight();
        protected abstract void StopCameraLean();

        protected Vector3 CalculateBulletSpreadOffset()
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
                        int ammoUsed = AmmoInMag - CurrentAmmo;
                        int remaingAmmo = CurrentMaxAmmoCarried - ammoUsed;
                        if (remaingAmmo <= 0)
                            CurrentAmmo += CurrentMaxAmmoCarried;
                        else
                            CurrentAmmo = AmmoInMag;
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
                        Time.deltaTime * 2);

                //TODO: check attachment, if true, smooth transit gun camera position to (0,-0.2f,0.03f)
                //if (isAttached)
                //{
                //    //float tmp_GunCurrentFOV = 0f;
                //    //GunCamera.fieldOfView = Mathf.SmoothDamp(GunCamera.fieldOfView,
                //    //    isAiming ? scopeInfo.GunFov : GunOriginFOV,
                //    //    ref tmp_GunCurrentFOV,
                //    //    Time.deltaTime * 2);

                //    // smooth transit to aiming pos
                //    Vector3 tmp_RefGunCameraPosition = Vector3.zero;
                //    GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                //        isAiming ? scopeInfo.GunCameraPosition : GunCameraLocalOriginalPosition,
                //        ref tmp_RefGunCameraPosition,
                //        Time.deltaTime * 2);
                //}
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
            Aiming();
        }

        internal void StopAiming()
        {
            isAiming = false;
            Aiming();
        }

        // for weapon reloading
        internal void ReloadAmmo()
        {
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
            attachment.AttachedTo = this;
        }

        public void RemoveAttachment(Attachment target)
        {
            if (target == null)
                return;

            for (int i = 0; i < attachments.Length; i++)
            {
                if (attachments[i] == target)
                {
                    attachments[i] = null;
                    target.AttachedTo = null;
                }
            }
        }

        //protected virtual void CreateBullet()
        //{

        //}

        public string WeaponName { get { return weaponName; } set { weaponName = value; } }
        public WeaponType Type { get { return weaponType; } } 
        public WeaponRarity Rarity { get { return rarity; } set { rarity = value; } }
        public float Damage { get { return damage; } }
        public Element.Type Element { get { return element; } }
        public WeaponBonus Bonus { get { return weaponBonus; } set { weaponBonus = value; } }
        public Sprite IconImage { get { return iconImage; } }
    }
}