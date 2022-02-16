using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSE5912.PolyGamers
{
    public abstract class Firearms : MonoBehaviour, IWeapon
    {
        // WeaponEffect 
        public Transform MuzzlePoint; // position where bullet fire
        public Transform CasingPoint; // position where bullet pops
        public GameObject BulletPrefab;
        public ParticleSystem MuzzleParticle;
        public ParticleSystem CastingParticle;
        
        // WeaponInfo (might need external reference from Player Controller)
        public int AmmoInMag = 30; // the amount of ammo per mag
        public int MaxAmmoCarried = 120; // the amount of ammo total
        public float FireRate; // gun fire rate
        protected int CurrentAmmo;
        protected int CurrentMaxAmmoCarried;
        protected float LastFireTime;
        // for fps controller to play the correct animation
        internal Animator GunAnimator; 
        protected AnimatorStateInfo GunStateInfo;
        
        // Get current ammo and ammo left in mag
        public int GetCurrentAmmo => CurrentAmmo;
        public int GetCurrentMaxAmmo => CurrentMaxAmmoCarried;

        // WeaponAudioInfo
        public AudioSource ShootingAudioSource;
        public AudioSource ReloadingAudioSource;
        public FirearmsAudioData WeaponAudioData;

        // WeaponAiming
        public Camera GunCamera;
        public float FOVWhenAimed; // the updated field of view for gun camera when aimed
        protected float GunCameraOriginFOV;
        internal bool isAiming;
        private IEnumerator doAimingCoroutine;

        // WeaponReloading
        internal bool isReloading;

        // WeaponLeaning
        protected Quaternion GunCameraLocalOriginalRotation;
        protected Vector3 GunCameraLocalOriginalPosition;
        public float SlerpTime;
        public float SlerpAngle;
        public float SlerpDistance;

        // CameraShaking
        public float SpreadAngle;

        // check Shoot 
        internal bool IsHoldingTrigger;

        // UI related
        [Header("UI related")]
        public WeaponType weaponType;
        public Sprite iconImage;
        public string description;

        // Attachments
        [Header("Attachments")]
        [SerializeField] protected Attachment[] attachments;
        public Attachment[] Attachments { get { return attachments; } }

        public enum WeaponType { Rifle, Handgun };

        // Attachment info
        public List<ScopeInfo> ScopeInfos = new List<ScopeInfo>();
        protected ScopeInfo scopeInfo;
        internal bool isAttached;
        [System.Serializable]
        public class ScopeInfo
        {
            public string ScopeName;
            public GameObject ScopeGameObject;
            public float GunCameraFovWhenAttached;
            public Vector3 GunCameraPosition;
        }

        protected virtual void Awake()
        {
            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            GunAnimator = GetComponent<Animator>();
            GunCameraOriginFOV = GunCamera.fieldOfView;
            GunCameraLocalOriginalRotation = GunCamera.transform.localRotation;
            GunCameraLocalOriginalPosition = GunCamera.transform.localPosition;
            doAimingCoroutine = DoAim();

            attachments = new Attachment[PlayerInventory.NumOfAttachmentsPerWeapon];
        }


        public void Attack()
        {
            Shoot();
        }

        // check if able to shoot (shoot with fire rate)
        protected bool IsAllowShooting()
        {
            return Time.time - LastFireTime > 1 / FireRate;
        }

        protected abstract void Shoot();
        protected abstract void Reload();
        protected abstract void StartCameraLean();
        protected abstract void StopCameraLean();

        protected Vector3 CalculateBulletSpreadOffset()
        {
            // field of view smaller (aimed), less bullet spread
            float spreadPercentage = SpreadAngle / GunCamera.fieldOfView;
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
                        int tmp_HeadAmmoCount = AmmoInMag - CurrentAmmo;
                        int tmp_RemaingAmmo = CurrentMaxAmmoCarried - tmp_HeadAmmoCount;
                        if (tmp_RemaingAmmo <= 0)
                            CurrentAmmo += CurrentMaxAmmoCarried;
                        else
                            CurrentAmmo = AmmoInMag;
                        CurrentMaxAmmoCarried = tmp_RemaingAmmo <= 0 ? 0 : tmp_RemaingAmmo;

                        isReloading = false;
                        yield break;
                    }
                }
            }
        }

        protected IEnumerator DoAim()
        {
            while (true)
            {
                yield return null;
                float tmp_RefGunCameraFOV = 0f;
                GunCamera.fieldOfView = Mathf.SmoothDamp(GunCamera.fieldOfView,
                        isAiming ? FOVWhenAimed : GunCameraOriginFOV,
                        ref tmp_RefGunCameraFOV,
                        Time.deltaTime * 2);

                //TODO: check attachment, if true, disable crosshair when aiming, damp Main camera position to (x,-0.2f,0.03f)
                if (isAttached)
                {
                    //float tmp_GunCurrentFOV = 0f;
                    //GunCamera.fieldOfView = Mathf.SmoothDamp(GunCamera.fieldOfView,
                    //    isAiming ? scopeInfo.GunFov : GunOriginFOV,
                    //    ref tmp_GunCurrentFOV,
                    //    Time.deltaTime * 2);

                    Vector3 tmp_RefGunCameraPosition = Vector3.zero;
                    GunCamera.transform.localPosition = Vector3.SmoothDamp(GunCamera.transform.localPosition,
                        isAiming ? scopeInfo.GunCameraPosition : GunCameraLocalOriginalPosition,
                        ref tmp_RefGunCameraPosition,
                        Time.deltaTime * 2);
                }
            }
        }

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


        internal void HoldTrigger()
        {
            Attack();
            IsHoldingTrigger = true;
        }

        internal void ReleaseTrigger()
        {
            IsHoldingTrigger = false;
        }

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

        internal void ReloadAmmo()
        {
            isReloading = true;
            isAiming = false;
            GunAnimator.SetBool("Aim", isAiming);
            Reload();
        }

        internal void StartLeanShooting()
        {
            StartCameraLean();
        }

        internal void StopLeanShooting()
        {
            StopCameraLean();
        }

        internal void SetupCarriedScope(ScopeInfo scopeInfo)
        {
            this.scopeInfo = scopeInfo;
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