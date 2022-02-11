using UnityEngine;
using System.Collections;

namespace PolyGamers.Weapon
{
    public abstract class Firearms : MonoBehaviour, IWeapon
    {
        // WeaponEffect
        public Transform MuzzlePoint;
        public Transform CasingPoint;
        public GameObject BulletPrefab;
        public ParticleSystem MuzzleParticle;
        public ParticleSystem CastingParticle;
        
        // WeaponInfo
        public int AmmoInMag = 30;
        public int MaxAmmoCarried = 120;
        public float FireRate;
        protected int CurrentAmmo;
        protected int CurrentMaxAmmoCarried;
        protected float LastFireTime;
        internal Animator GunAnimator;
        protected AnimatorStateInfo GunStateInfo;
       
        // WeaponAudioInfo
        public AudioSource FirearmsShootingAudioSource;
        public AudioSource FirearmsReloadingAudioSource;
        public FirearmsAudioData FirearmsAudioData;

        // WeaponAiming
        public Camera EyeCamera;
        public float FOVforDoubleMirror;
        internal bool isAiming;
        protected float OriginFOV;
        private IEnumerator doAimingCoroutine;

        // WeaponReloading
        internal bool isReloading;

        // WeaponLeaning
        protected Quaternion CameraLocalOriginRotation;
        public float SlerpTime;
        public float SlerpAngle;
        public float SlerpDistance;

        // CameraShaking
        public float SpreadAngle;

        // check Shooting 
        internal bool IsHoldingTrigger;


        protected virtual void Awake()
        {
            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            GunAnimator = GetComponent<Animator>();
            OriginFOV = EyeCamera.fieldOfView;
            CameraLocalOriginRotation = EyeCamera.transform.localRotation;
            doAimingCoroutine = DoAim();
        }

        public void Attack()
        {
            Shooting();
        }

        protected abstract void Shooting();
        protected abstract void Reload();
        protected abstract void StartCameraLean();
        protected abstract void StopCameraLean();

        protected Vector3 CalculateSpreadOffset()
        {
            float tmp_SpreadPercent = SpreadAngle / EyeCamera.fieldOfView;
            return tmp_SpreadPercent * UnityEngine.Random.insideUnitCircle;
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

        protected bool IsAllowShooting()
        {
            return Time.time - LastFireTime > 1 / FireRate;
        }

        protected IEnumerator DoAim()
        {
            while (true)
            {
                yield return null;
                float tmp_CurrentFOV = 0f;
                EyeCamera.fieldOfView = Mathf.SmoothDamp(EyeCamera.fieldOfView,
                        isAiming ? FOVforDoubleMirror : OriginFOV,
                        ref tmp_CurrentFOV,
                        Time.deltaTime * 2);
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
    }
}