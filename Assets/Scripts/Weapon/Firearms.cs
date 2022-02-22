using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSE5912.PolyGamers
{
    public abstract class Firearms : MonoBehaviour, IWeapon
    {
        [Header("WeaponEffect")]
        // position where bullet generate
        public Transform MuzzlePoint;
        // position where bullet pops
        public Transform CasingPoint; 
        public GameObject BulletPrefab;
        public ParticleSystem MuzzleParticle;
        public ParticleSystem CastingParticle;

        [Header("WeaponInfo")]  
        // weapon damage value
        public float damage = 0f;
        // weapon elemental type
        public Element.Type element;
        // predefined ammo per mag (AK: 30, Glock: 8)
        public int AmmoInMag;
        // predefined ammo total (AK: 120, Glock: 32)
        public int MaxAmmoCarried;
        public float FireRate;
        // current ammo in mag (per)
        protected int CurrentAmmo;
        // total current ammo left in mag (all)
        protected int CurrentMaxAmmoCarried;
        protected float LastFireTime;
        // current gun animator
        internal Animator GunAnimator; 
        protected AnimatorStateInfo GunStateInfo;
        
        // Get current ammo and ammo left in mag 
        public int GetCurrentAmmo => CurrentAmmo;
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

        public enum WeaponType { Rifle, Handgun };
        public enum ShootingType { Fixed, Continued }
        public ShootingType shootingType;

        // UI related
        [Header("UI related")]
        public WeaponType weaponType;
        public Sprite iconImage;
        public string description;

        // Attachments
        [Header("Attachments")]
        [SerializeField] protected Attachment[] attachments;
        public Attachment[] Attachments { get { return attachments; } }

        [Header("Attachment info")]
        // holder of different scopes in this gun
        public List<ScopeInfo> ScopeInfos = new List<ScopeInfo>();
        // store the information of scopes, like scope gameobject, fov
        protected ScopeInfo scopeInfo;
        [System.Serializable]
        public class ScopeInfo
        {
            public string ScopeName;
            public GameObject ScopeGameObject;
            public float GunCameraFovWhenAttached;
            public Vector3 GunCameraPosition;
        }
        internal bool isAttached;

        // Firearms singleton
        public static Firearms Instance { get; private set; }

        protected virtual void Awake()
        {
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
            // define how many attachments one gun could have (4)
            attachments = new Attachment[PlayerInventory.NumOfAttachmentsPerWeapon];
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
        protected abstract void StartCameraLean();
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
                if (isAttached)
                {
                    //float tmp_GunCurrentFOV = 0f;
                    //GunCamera.fieldOfView = Mathf.SmoothDamp(GunCamera.fieldOfView,
                    //    isAiming ? scopeInfo.GunFov : GunOriginFOV,
                    //    ref tmp_GunCurrentFOV,
                    //    Time.deltaTime * 2);

                    // smooth transit to aiming pos
                    Vector3 tmp_RefGunCameraPosition = Vector3.zero;
                    GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                        isAiming ? scopeInfo.GunCameraPosition : GunCameraLocalOriginalPosition,
                        ref tmp_RefGunCameraPosition,
                        Time.deltaTime * 2);
                }
            }
        }

        internal void SetupCarriedScope(ScopeInfo scopeInfo)
        {
            this.scopeInfo = scopeInfo;
        }

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
        internal void StartLeanShooting()
        {
            StartCameraLean();
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
            attachment.attachedTo = this;
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
                    target.attachedTo = null;
                }
            }
        }

        public string BuildDescription()
        {
            description =
                "Name: " + gameObject.name +
                "\nType: " + weaponType;

            // test
            description += "\n\n-----------------------------------------\n" +
                "Test\n";
            for (int i = 0; i < attachments.Length; i++)
            {
                description += "\nAttachment_" + i + ": ";
                Attachment attachment = attachments[i];
                if (attachment != null)
                    description += attachment.attachmentName;
            }
            return description;
        }
    }
}