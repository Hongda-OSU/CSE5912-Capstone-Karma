using System.Collections;
using UnityEngine;

namespace Assets.Weapon
{
    public class AssualtRifle : Firearms
    {
        private IEnumerator reloadAmmoCheckerCoroutine;
        private IEnumerator doAimingCoroutine;
        private bool isReloading;

        protected override void Start()
        {
            base.Start();
            reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
            doAimingCoroutine = DoAim();
        }

        protected override void Shooting()
        {
            if (CurrentAmmo <= 0) return;
            if (!IsAllowShooting()) return;
            MuzzleParticle.Play();
            CurrentAmmo -= 1;
            GunAnimator.Play("Fire", isAiming ? 1: 0, 0);
            FirearmsShootingAudioSource.clip = FirearmsAudioData.ShootingAudio;
            FirearmsShootingAudioSource.Play();
            CreateBullet();
            CastingParticle.Play();
            LastFireTime = Time.time;
        }

        protected override void Reload()
        {
            GunAnimator.SetLayerWeight(2, 1);
            GunAnimator.SetTrigger(CurrentAmmo > 0 ? "ReloadLeft" : "ReloadOutOf");

            FirearmsReloadingAudioSource.clip = CurrentAmmo > 0 ? FirearmsAudioData.ReloadLeft : FirearmsAudioData.ReloadOutOf;
            FirearmsReloadingAudioSource.Play();

            if (reloadAmmoCheckerCoroutine == null)
            {
                reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
                StartCoroutine(reloadAmmoCheckerCoroutine);
            }
            else
            {
                StopCoroutine(reloadAmmoCheckerCoroutine);
                reloadAmmoCheckerCoroutine = null;
                reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
                StartCoroutine(reloadAmmoCheckerCoroutine);
            }
        }

        protected override void Aim()
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

        void Update()
        {
            if (Input.GetMouseButton(0) && !isReloading)
                Attack();

            if (Input.GetKeyDown(KeyCode.R))
            {
                isReloading = true;
                Reload();
            }

            if (Input.GetMouseButtonDown(1))
            {
                isAiming = true;
                Aim();
            }

            if (Input.GetMouseButtonUp(1))
            {
                isAiming = false;
                Aim();
            }
        }

        protected void CreateBullet()
        {
            GameObject tmp_Bullet = Instantiate(BulletPrefab, MuzzlePoint.position, MuzzlePoint.rotation);
            var bullet_Rigibody = tmp_Bullet.AddComponent<Rigidbody>();
            bullet_Rigibody.velocity = tmp_Bullet.transform.forward * 200f;
        }

        IEnumerator CheckReloadAmmoAnimationEnd()
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
                        CurrentMaxAmmoCarried = tmp_RemaingAmmo < 0 ? 0 : tmp_RemaingAmmo;
                        isReloading = false;
                        yield break;
                    }
                }
            }
        }

        IEnumerator DoAim()
        {
            while (true)
            {
                yield return null;
                float tmp_CurrentFOV = 0f;
                EyeCamera.fieldOfView =
                    Mathf.SmoothDamp(EyeCamera.fieldOfView, 
                        isAiming ? FOVforDoubleMirror : OriginFOV, 
                        ref tmp_CurrentFOV, 
                        Time.deltaTime * 2);
            }
        }
    }
}
