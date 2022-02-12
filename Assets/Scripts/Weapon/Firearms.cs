using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        protected float EyeOriginFOV;
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

        public List<ScopeInfo> ScopeInfos = new List<ScopeInfo>();
        protected ScopeInfo rigoutScopeInfo;
        public Camera GunCamera;
        protected Transform GunCameraTransform;
        protected float GunOriginFOV;
        private Vector3 originalEyePosition;


        public CrosshairContorller crosshairContorller;

        protected virtual void Awake()
        {
            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            GunAnimator = GetComponent<Animator>();
            EyeOriginFOV = EyeCamera.fieldOfView;
            GunOriginFOV = GunCamera.fieldOfView;
            CameraLocalOriginRotation = EyeCamera.transform.localRotation;
            doAimingCoroutine = DoAim();

            GunCameraTransform = GunCamera.transform;
            originalEyePosition = GunCameraTransform.localPosition;
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
                float tmp_EyeCurrentFOV = 0f;
                EyeCamera.fieldOfView = Mathf.SmoothDamp(EyeCamera.fieldOfView,
                        isAiming ? FOVforDoubleMirror : EyeOriginFOV,
                        ref tmp_EyeCurrentFOV,
                        Time.deltaTime * 2);

                //float tmp_GunCurrentFOV = 0f;
                //GunCamera.fieldOfView = Mathf.SmoothDamp(GunCamera.fieldOfView,
                //    isAiming ? rigoutScopeInfo.GunFov : GunOriginFOV,
                //    ref tmp_GunCurrentFOV,
                //    Time.deltaTime * 2);

                //Vector3 tmp_RefPosition = Vector3.zero;
                //GunCameraTransform.localPosition = Vector3.SmoothDamp(GunCameraTransform.localPosition,
                //    isAiming ? rigoutScopeInfo.GunCameraPosition : originalEyePosition,
                //    ref tmp_RefPosition,
                //    Time.deltaTime * 2);
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

        [System.Serializable]
        public class ScopeInfo
        {
            public string ScopeName;
            public GameObject ScopeGameObject;
            public float GunFov;
            public Vector3 GunCameraPosition;
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
            //crosshairContorller.gameObject.SetActive(false);
            Aiming();
        }

        internal void StopAiming()
        {
            isAiming = false;
            //crosshairContorller.gameObject.SetActive(true);
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
            rigoutScopeInfo = scopeInfo;
        }
    }
}