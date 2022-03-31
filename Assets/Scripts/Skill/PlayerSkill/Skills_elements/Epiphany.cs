using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class Epiphany : PlayerSkill
    {

        [Header("Epiphany")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float duration = 15f;

        [SerializeField] private float damageUp = 0.25f;
        [SerializeField] private float chanceUp = 0.25f;

        [SerializeField] private GameObject buffVfxPrefab;
        [SerializeField] private string buffPositionPath;

        [SerializeField] private AudioSource sfx;

        private InputAction action;

        private Firearms buffedWeapon;

        private void Start()
        {
            action = InputManager.Instance.InputSchemes.PlayerActions.MainSkill;
        }

        private void Update()
        {
            bool isTriggered = action.WasPressedThisFrame();
            if (isTriggered)
                StartCoroutine(Perform());
        }
        private IEnumerator Perform()
        {
            if (!isLearned || !isReady)
                yield break;

            isReady = false;

            buffedWeapon = WeaponManager.Instance.CarriedWeapon;

            sfx.Play();

            GameObject vfx = Instantiate(vfxPrefab, PlayerManager.Instance.Player.transform);
            Destroy(vfx, 5f);

            BuffPlayer(true);
            GameObject buffVfx = Instantiate(buffVfxPrefab, WeaponManager.Instance.CarriedWeapon.transform.Find(buffPositionPath), false);

            float timeSince = 0f;
            while (timeSince < duration)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

                if (WeaponManager.Instance.CarriedWeapon != buffedWeapon)
                {
                    timeSince = duration;
                }
            }
            buffedWeapon = null;

            BuffPlayer(false);
            Destroy(buffVfx);

            StartCoolingdown();
        }


        private void BuffPlayer(bool enabled)
        {
            float damage = damageUp;
            float chance = chanceUp;
            if (!enabled)
            {
                damage = -damage;
                chance = -chance;
            }


            PlayerStats.Instance.GetDamageFactor().Fire.Value += damage;
            PlayerStats.Instance.GetDamageFactor().Cryo.Value += damage;
            PlayerStats.Instance.GetDamageFactor().Electro.Value += damage;
            PlayerStats.Instance.GetDamageFactor().Venom.Value += damage;

            PlayerStats.Instance.BurnedBaseChance += chance;
            PlayerStats.Instance.FrozenBaseChance += chance;
            PlayerStats.Instance.ElectrocutedBaseChance += chance;
            PlayerStats.Instance.InfectedBaseChance += chance;
        }

        public override bool LevelUp()
        {
            var result = base.LevelUp();

            if (result)
            {
                PlayerSkillManager.Instance.SetMainSkill(this);
            }

            return result;
        }
    }
}
