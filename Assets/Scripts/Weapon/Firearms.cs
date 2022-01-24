using Assets.Scripts.Weapon;
using UnityEngine;

namespace Assets.Weapon
{
    public abstract class Firearms : MonoBehaviour, IWeapon
    {
        public Transform MuzzlePoint;
        public Transform CasingPoint;
        public GameObject BulletPrefab;
        public ParticleSystem MuzzleParticle;
        public ParticleSystem CastingParticle;
        public float FOVforDoubleMirror;

        public float FireRate;

        public int AmmoInMag = 30;
        public int MaxAmmoCarried = 120;

        protected int CurrentAmmo;
        protected int CurrentMaxAmmoCarried;
        protected float LastFireTime;
        protected Animator GunAnimator;
        protected AnimatorStateInfo GunStateInfo;
        protected float OriginFOV;
        protected bool isAiming;

        public AudioSource FirearmsShootingAudioSource;
        public AudioSource FirearmsReloadingAudioSource;
        public FirearmsAudioData FirearmsAudioData;

        public Camera EyeCamera;

        protected virtual void Start()
        {
            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            GunAnimator = GetComponent<Animator>();
            OriginFOV = EyeCamera.fieldOfView;
        }

        public void Attack()
        {
            Shooting();
        }

        protected abstract void Shooting();
        protected abstract void Reload();
        protected abstract void Aim();

        protected bool IsAllowShooting()
        {
            return Time.time - LastFireTime > 1 / FireRate;
        }
    }
}