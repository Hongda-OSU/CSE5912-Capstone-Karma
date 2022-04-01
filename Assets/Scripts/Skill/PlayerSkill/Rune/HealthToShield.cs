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

            var amount = PlayerStats.Instance.Health - 1f;
            var max = PlayerStats.Instance.MaxHealth - 1f;

            PlayerStats.Instance.Health -= amount;
            PlayerStats.Instance.MaxHealth -= max;

            PlayerStats.Instance.Shield_energy += amount * 0.5f;
            PlayerStats.Instance.MaxShield_energy += max * 0.5f;

            PlayerStats.Instance.Shield_armor += amount * 0.5f;
            PlayerStats.Instance.MaxShield_armor += max * 0.5f;

            health += amount;
            maxHealth += max;

            Debug.Log(health + ", " + maxHealth);
        }


        public override void ResetLevel()
        {
            base.ResetLevel();

            PlayerStats.Instance.Health += health;
            PlayerStats.Instance.MaxHealth += maxHealth;

            PlayerStats.Instance.Shield_energy -= (health + 1) * 0.5f;
            PlayerStats.Instance.MaxShield_energy -= (maxHealth + 1) * 0.5f;

            PlayerStats.Instance.Shield_armor -= (health + 1) * 0.5f;
            PlayerStats.Instance.MaxShield_armor -= (maxHealth + 1) * 0.5f;

            health = 0;
            maxHealth = 0;
        }
    }
}
