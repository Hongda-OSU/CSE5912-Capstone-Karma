using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class BERETTA : Firearms
    {
        // bullet hole effect
        //public GameObject ImpactPrefab;
        public ImpactAudioData impactAudioData;
        private IEnumerator reloadAmmoCheckerCoroutine;

        private Quaternion ControllerLocalOriginalRotation;
        private Vector3 ControllerLocalOriginalPosition;
        private FPSMouseLook fpsMouseLook;

        protected override void Awake()
        {
            base.Awake();
            // get current gameobject local rotation and local position, used in lean shooting
            ControllerLocalOriginalRotation = transform.localRotation;
            ControllerLocalOriginalPosition = transform.localPosition;
            reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
            fpsMouseLook = FindObjectOfType<FPSMouseLook>();
        }

        protected override void Shoot()
        {
            // same logic in AK47
            if (CurrentAmmo <= 0) return;
            if (!IsAllowShooting()) return;
            MuzzleParticle.Play();
            CurrentAmmo -= 1;
            GunAnimator.Play("Fire", isAiming ? 1 : 0, 0);
            ShootingAudioSource.clip = WeaponAudioData.ShootingAudio;
            ShootingAudioSource.PlayOneShot(ShootingAudioSource.clip);
            CreateBullet();
            CastingParticle.Play();
            if (isAiming)
                fpsMouseLook.FiringWithRecoilAimed();
            else
                fpsMouseLook.FiringWithRecoil();
            LastFireTime = Time.unscaledTime;
        }

        protected override void Reload()
        {
            GunAnimator.SetLayerWeight(2, 1);
            GunAnimator.SetTrigger(CurrentAmmo > 0 ? "ReloadLeft" : "ReloadOutOf");

            ReloadingAudioSource.clip = CurrentAmmo > 0 ? WeaponAudioData.ReloadLeft : WeaponAudioData.ReloadOutOf;
            ReloadingAudioSource.Play();

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

        protected override void StopCameraLean()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, ControllerLocalOriginalRotation, SlerpTime * Time.unscaledDeltaTime);
            transform.localPosition = Vector3.Slerp(transform.localPosition, ControllerLocalOriginalPosition, SlerpTime * Time.unscaledDeltaTime);
            GunCamera.transform.localRotation = Quaternion.Slerp(GunCamera.transform.localRotation, GunCameraLocalOriginalRotation, SlerpTime * Time.unscaledDeltaTime);
        }

        protected override void CameraLeftLean()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, SlerpAngle), SlerpTime * Time.unscaledDeltaTime);
            transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(-SlerpDistance, 0, 0), SlerpTime * Time.unscaledDeltaTime);
            GunCamera.transform.localRotation = Quaternion.Slerp(GunCamera.transform.localRotation, Quaternion.Euler(90, SlerpAngle, 0), SlerpTime * Time.unscaledDeltaTime);
        }

        protected override void CameraLeanRight()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, -SlerpAngle), SlerpTime * Time.unscaledDeltaTime);
            transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(SlerpDistance, 0, 0), SlerpTime * Time.unscaledDeltaTime);
            GunCamera.transform.localRotation = Quaternion.Slerp(GunCamera.transform.localRotation, Quaternion.Euler(90, -SlerpAngle, 0), SlerpTime * Time.unscaledDeltaTime);
        }

        protected void CreateBullet()
        {
            Quaternion bulletRot = MuzzlePoint.rotation;
            Vector3 bulletPos = MuzzlePoint.position;
            // fake bullet when running
            if (FPSControllerCC.Instance.IsSprint && !WeaponManager.Instance.isAiming)
            {
                Quaternion playerRot = PlayerManager.Instance.Player.transform.rotation;
                Quaternion mouseRot = PlayerManager.Instance.PlayerArms.transform.localRotation;
                bulletRot = playerRot * mouseRot;
                bulletPos = PlayerManager.Instance.Player.transform.position;
            }
            GameObject bullet = Instantiate(BulletPrefab, bulletPos, bulletRot);
            bullet.transform.eulerAngles += CalculateBulletSpreadOffset();
            var bulletScript = bullet.AddComponent<Bullet>();
            bulletScript.ImpactPrefab = ImpactPrefab;
            bulletScript.impactAudioData = impactAudioData;
            bulletScript.BulletSpeed = 100;
            bulletScript.Penetrable = BulletPenetrable;

            bulletScript.damage = Damage;
            bulletScript.elementType = Element;
            shootEvent.Invoke();
            bulletFired = bulletScript;
            Destroy(bullet, 3);
        }
    }
}
