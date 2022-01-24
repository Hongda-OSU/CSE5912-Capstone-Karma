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

        protected virtual void Start()
        {
            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            GunAnimator = GetComponent<Animator>();
        }

        public void Attack()
        {
            Shooting();
        }

        protected abstract void Shooting();
        protected abstract void Reload();

        protected bool IsAllowShooting()
        {
            return Time.time - LastFireTime > 1 / FireRate;
        }


    }
}