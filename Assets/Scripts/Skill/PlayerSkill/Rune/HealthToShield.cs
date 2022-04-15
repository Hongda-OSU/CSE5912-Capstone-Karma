using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class HealthToShield : PlayerSkill
    {
        [Header("HealthToShield")]
        [SerializeField] private float health;
        [SerializeField] private float maxHealth;


        protected override string GetBuiltSpecific()
        {
            return "";
        }

        private void Update()
        {
            if (!isLearned || !isReady)
                return;

            Convert();
        }

        private void Convert()
        {
            if (PlayerStats.Instance.Health <= 1f && PlayerStats.Instance.MaxHealth <= 1f)
                return;

            var max = PlayerStats.Instance.MaxHealth - 1f;
            var amount = PlayerStats.Instance.Health - 1f;

            PlayerStats.Instance.MaxHealth -= max;
            PlayerStats.Instance.Health -= amount;

            PlayerStats.Instance.MaxShield_energy += max * 0.5f;
            PlayerStats.Instance.Shield_energy += amount * 0.5f;

            PlayerStats.Instance.MaxShield_armor += max * 0.5f;
            PlayerStats.Instance.Shield_armor += amount * 0.5f;

            health += amount;
            maxHealth += max;
        }

        public override bool LevelUp()
        {
            var result = base.LevelUp();
            if (result)
                Convert();
            return result;
        }

        public override void ResetLevel()
        {
            if (level <= 0)
                return;

            base.ResetLevel();

            PlayerStats.Instance.MaxHealth += maxHealth;
            PlayerStats.Instance.Health += health;

            PlayerStats.Instance.MaxShield_energy -= maxHealth * 0.5f;
            PlayerStats.Instance.Shield_energy -= health * 0.5f;

            PlayerStats.Instance.MaxShield_armor -= maxHealth * 0.5f;
            PlayerStats.Instance.Shield_armor -= health * 0.5f;

            health = 0;
            maxHealth = 0;
        }
    }
}
