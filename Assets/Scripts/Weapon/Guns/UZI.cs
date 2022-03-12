using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class UZI : Firearms
    {
        // bullet hole effect
        public GameObject ImpactPrefab;
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
            // not ammo in mag, return
            if (CurrentAmmo <= 0) return;
            // keep fire rate
            if (!IsAllowShooting()) return;
            MuzzleParticle.Play();
            // decrease ammo in mag
            CurrentAmmo -= 1;
            // determine which Fire animation should be play, aiming layer => 1, base layer => 0
            GunAnimator.Play("Fire", isAiming ? 1 : 0, 0);
            // play shooting audio
            ShootingAudioSource.clip = WeaponAudioData.ShootingAudio;
            ShootingAudioSource.PlayOneShot(ShootingAudioSource.clip);
            // create bullet
            CreateBullet();
            CastingParticle.Play();
            // enable different recoil between aimed and not aimed
            if (isAiming)
                fpsMouseLook.FiringWithRecoilAimed();
            else
                fpsMouseLook.FiringWithRecoil();
            // calculate IsAllowShooting
            LastFireTime = Time.time;
        }

        protected override void Reload()
        {
            GunAnimator.SetLayerWeight(2, 1);
            // if current ammo is 0, play "ReloadOutOf", else, "ReloadLeft"
            GunAnimator.SetTrigger(CurrentAmmo > 0 ? "ReloadLeft" : "ReloadOutOf");
            // reload audio
            ReloadingAudioSource.clip = CurrentAmmo > 0 ? WeaponAudioData.ReloadLeft : WeaponAudioData.ReloadOutOf;
            ReloadingAudioSource.Play();
            // coroutine for reload animation
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

        // camera leaning use gun camera(rotation) and current gameobject(rotation and position)

        protected override void StopCameraLean()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, ControllerLocalOriginalRotation, SlerpTime * Time.deltaTime);
            transform.localPosition = Vector3.Slerp(transform.localPosition, ControllerLocalOriginalPosition, SlerpTime * Time.deltaTime);
            GunCamera.transform.localRotation = Quaternion.Slerp(GunCamera.transform.localRotation, GunCameraLocalOriginalRotation, SlerpTime * Time.deltaTime);
        }

        protected override void CameraLeftLean()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, SlerpAngle), SlerpTime * Time.deltaTime);
            transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(-SlerpDistance, 0, 0), SlerpTime * Time.deltaTime);
            GunCamera.transform.localRotation = Quaternion.Slerp(GunCamera.transform.localRotation, Quaternion.Euler(90, SlerpAngle, 0), SlerpTime * Time.deltaTime);
        }

        protected override void CameraLeanRight()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, -SlerpAngle), SlerpTime * Time.deltaTime);
            transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(SlerpDistance, 0, 0), SlerpTime * Time.deltaTime);
            GunCamera.transform.localRotation = Quaternion.Slerp(GunCamera.transform.localRotation, Quaternion.Euler(90, -SlerpAngle, 0), SlerpTime * Time.deltaTime);
        }

        protected void CreateBullet()
        {
            // create bullet 
            GameObject bullet = Instantiate(BulletPrefab, MuzzlePoint.position, MuzzlePoint.rotation);
            // add scattering to bullet
            bullet.transform.eulerAngles += CalculateBulletSpreadOffset();
            // pass the needed component for bullet
            var bulletScript = bullet.AddComponent<Bullet>();
            bulletScript.ImpactPrefab = ImpactPrefab;
            bulletScript.impactAudioData = impactAudioData;
            bulletScript.BulletSpeed = 100;
            bulletScript.Penetrable = BulletPenetrable;

            bulletScript.damage = Damage;
            bulletScript.elementType = Element;
            shootEvent.Invoke();
            bulletFired = bulletScript;
            // destroy bullet in 3s
            Destroy(bullet, 3);
        }
    }
}
