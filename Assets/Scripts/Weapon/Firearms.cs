using UnityEngine;

namespace PolyGamers.Weapon
{
    public abstract class Firearms : MonoBehaviour, IWeapon
    {
        public Transform MuzzlePoint;
        public Transform CasingPoint;
        public GameObject BulletPrefab;
        public ParticleSystem MuzzleParticle;
        public ParticleSystem CastingParticle;
        

        public float FireRate;

        public int AmmoInMag = 30;
        public int MaxAmmoCarried = 120;

        protected int CurrentAmmo;
        protected int CurrentMaxAmmoCarried;
        protected float LastFireTime;
        protected Animator GunAnimator;
        protected AnimatorStateInfo GunStateInfo;
       

        public AudioSource FirearmsShootingAudioSource;
        public AudioSource FirearmsReloadingAudioSource;
        public FirearmsAudioData FirearmsAudioData;

        public Camera EyeCamera;
        public float FOVforDoubleMirror;
        protected float OriginFOV;
        protected Quaternion CameraLocalOriginRotation;
        protected bool isAiming;
        public float SlerpTime;
        public float SlerpAngle;
        public float SlerpDistance;

        public float SpreadAngle;


        protected virtual void Start()
        {
            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            GunAnimator = GetComponent<Animator>();
            OriginFOV = EyeCamera.fieldOfView;
            CameraLocalOriginRotation = EyeCamera.transform.localRotation;
        }

        public void Attack()
        {
            Shooting();
        }

        protected abstract void Shooting();
        protected abstract void Reload();
        protected abstract void Aim();
        protected abstract void CameraLean();

        protected bool IsAllowShooting()
        {
            return Time.time - LastFireTime > 1 / FireRate;
        }

        protected Vector3 CalculateSpreadOffset()
        {
            float tmp_SpreadPercent = SpreadAngle / EyeCamera.fieldOfView;
            return tmp_SpreadPercent * UnityEngine.Random.insideUnitCircle;
        }
    }
}