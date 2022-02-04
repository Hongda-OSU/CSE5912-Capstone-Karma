using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Glock : Firearms
    {
        [Header("Glock")]
        public GameObject ImpactPrefab;
        public ImpactAudioData impactAudioData;
        private IEnumerator reloadAmmoCheckerCoroutine;

        private Quaternion ControllerOriginLocalRotation;
        private Vector3 ControllerOriginLocalPosition;
        private FPSMouseLook fpsMouseLook;

        protected override void Awake()
        {
            base.Awake();
            ControllerOriginLocalRotation = transform.localRotation;
            ControllerOriginLocalPosition = transform.localPosition;
            reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
            fpsMouseLook = FindObjectOfType<FPSMouseLook>();

            weaponType = WeaponType.Handgun;
        }

        protected override void Shooting()
        {
            if (CurrentAmmo <= 0) return;
            if (!IsAllowShooting()) return;
            MuzzleParticle.Play();
            CurrentAmmo -= 1;
            GunAnimator.Play("Fire", isAiming ? 1 : 0, 0);
            FirearmsShootingAudioSource.clip = FirearmsAudioData.ShootingAudio;
            FirearmsShootingAudioSource.Play();
            CreateBullet();
            CastingParticle.Play();
            if (isAiming)
                fpsMouseLook.FiringWithRecoilAimed();
            else
                fpsMouseLook.FiringWithRecoil();
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

        protected override void StartCameraLean()
        {
            if (Input.GetKey(KeyCode.Q))
                CameraLeanLeft();

            if (Input.GetKey(KeyCode.E))
                CameraLeanRight();
        }

        protected override void StopCameraLean()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, ControllerOriginLocalRotation, SlerpTime * Time.deltaTime);
            transform.localPosition = Vector3.Slerp(transform.localPosition, ControllerOriginLocalPosition, SlerpTime * Time.deltaTime);
            EyeCamera.transform.localRotation = Quaternion.Slerp(EyeCamera.transform.localRotation, CameraLocalOriginRotation, SlerpTime * Time.deltaTime);
        }

        private void CameraLeanLeft()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, SlerpAngle), SlerpTime * Time.deltaTime);
            transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(-SlerpDistance, 0, 0), SlerpTime * Time.deltaTime);
            EyeCamera.transform.localRotation = Quaternion.Slerp(EyeCamera.transform.localRotation, Quaternion.Euler(90, SlerpAngle, 0), SlerpTime * Time.deltaTime);
        }

        private void CameraLeanRight()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, -SlerpAngle), SlerpTime * Time.deltaTime);
            transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(SlerpDistance, 0, 0), SlerpTime * Time.deltaTime);
            EyeCamera.transform.localRotation = Quaternion.Slerp(EyeCamera.transform.localRotation, Quaternion.Euler(90, -SlerpAngle, 0), SlerpTime * Time.deltaTime);
        }

        protected void CreateBullet()
        {
            GameObject tmp_Bullet = Instantiate(BulletPrefab, MuzzlePoint.position, MuzzlePoint.rotation);
            tmp_Bullet.transform.eulerAngles += CalculateSpreadOffset();
            var tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
            tmp_BulletScript.ImpactPrefab = ImpactPrefab;
            tmp_BulletScript.impactAudioData = impactAudioData;
            tmp_BulletScript.BulletSpeed = 100;
            Destroy(tmp_Bullet, 5);
        }
    }
}
