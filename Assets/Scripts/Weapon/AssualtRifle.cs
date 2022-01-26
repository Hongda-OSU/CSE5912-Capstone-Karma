using System.Collections;
using Assets.Scripts.Weapon;
using UnityEngine;

namespace Assets.Weapon
{
    public class AssualtRifle : Firearms
    {
        private IEnumerator reloadAmmoCheckerCoroutine;
        private IEnumerator doAimingCoroutine;
        private bool isReloading;
        private Quaternion ControllerOriginLocalRotation;
        private Vector3 ControllerQriginLocalPosition;

        protected override void Start()
        {
            base.Start();
            ControllerOriginLocalRotation = transform.localRotation;
            ControllerQriginLocalPosition = transform.localPosition;
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

        protected override void CameraLean()
        {
            if (Input.GetKey(KeyCode.Q))
                CameraLeanLeft();

            if (Input.GetKey(KeyCode.E))
                CameraLeanRight();
        }

        private void CameraLeanLeft()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0,0,SlerpAngle), SlerpTime);
            transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(-SlerpDistance, 0, 0), SlerpTime);
            EyeCamera.transform.localRotation = Quaternion.Slerp(EyeCamera.transform.localRotation, Quaternion.Euler(90, SlerpAngle, 0), SlerpTime);
        }

        private void CameraLeanRight()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, -SlerpAngle), SlerpTime);
            transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(SlerpDistance, 0, 0), SlerpTime);
            EyeCamera.transform.localRotation = Quaternion.Slerp(EyeCamera.transform.localRotation, Quaternion.Euler(90, -SlerpAngle, 0), SlerpTime);
        }

        void Update()
        {
            if (Input.GetMouseButton(0) && !isReloading)
                Attack();

            if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            {
                isReloading = true;
                isAiming = false;
                Reload();
            }

            if (Input.GetMouseButtonDown(1) && !isReloading)
            {
                isAiming = true;
                Aim();
            }
            if (Input.GetMouseButtonUp(1))
            {
                isAiming = false;
                Aim();
            }

            //do localRoatation on camera lean
            if ((Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)) && isAiming)
            {
                CameraLean();
            }
            else
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, ControllerOriginLocalRotation, SlerpTime);
                transform.localPosition = Vector3.Slerp(transform.localPosition, ControllerQriginLocalPosition, SlerpTime);
                EyeCamera.transform.localRotation = Quaternion.Slerp(EyeCamera.transform.localRotation, CameraLocalOriginRotation, SlerpTime);
            }
        }

        protected void CreateBullet()
        {
            GameObject tmp_Bullet = Instantiate(BulletPrefab, MuzzlePoint.position, MuzzlePoint.rotation);
            tmp_Bullet.AddComponent<Rigidbody>();
            var tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
            tmp_BulletScript.BulletSpeed = 100;

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
