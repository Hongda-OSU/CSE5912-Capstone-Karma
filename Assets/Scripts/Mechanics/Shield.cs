using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Shield : MonoBehaviour, IDamageable
    {
        [Header("Energy")]
        [SerializeField] private float shield_energy = 0f;
        [SerializeField] private float maxShield_energy = 20;
        [SerializeField] private float timeSinceDamaged = 0f;
        [SerializeField] private float timeToRestore = 5f;
        [SerializeField] private float restoreSpeed = 10f;
        [SerializeField] private float elementalDamageReduction = 0.75f;

        [Header("Armor")]
        [SerializeField] private float shield_armor = 0f;
        [SerializeField] private float maxShield_armor = 20;
        [SerializeField] private float physicalDamageReduction = 0.5f;
        [SerializeField] private float resistBonus = 0.1f;
        [SerializeField] private float resistGained;
        private bool isOn = false;

        private float overflow;

        private void Awake()
        {
            ResetShield();
        }

        private void Update()
        {
            UpdateEnergy();
            UpdateArmor();
        }

        public void ResetShield()
        {
            shield_energy = maxShield_energy;
            shield_armor = maxShield_armor;
        }

        private void UpdateEnergy()
        {
            var delta = Time.deltaTime;
            if (timeSinceDamaged >= timeToRestore)
            {
                var restore = restoreSpeed * delta;
                PlayerStats.Instance.Shield_energy += restore;
            }
            else
            {
                timeSinceDamaged += delta;
            }

            shield_energy = Mathf.Clamp(shield_energy, 0, maxShield_energy);
        }

        private void UpdateArmor()
        {
            if (shield_armor <= 0f && isOn)
            {
                PlayerStats.Instance.GetResist().Physical.Value -= resistGained;
                PlayerStats.Instance.GetResist().Fire.Value -= resistGained;
                PlayerStats.Instance.GetResist().Cryo.Value -= resistGained;
                PlayerStats.Instance.GetResist().Electro.Value -= resistGained;
                PlayerStats.Instance.GetResist().Venom.Value -= resistGained;

                resistGained = 0f;

                isOn = false;
            }
            else if (shield_armor > 0f && !isOn)
            {
                resistGained = shield_armor * resistBonus;

                PlayerStats.Instance.GetResist().Physical.Value += resistGained;
                PlayerStats.Instance.GetResist().Fire.Value += resistGained;  
                PlayerStats.Instance.GetResist().Cryo.Value += resistGained;
                PlayerStats.Instance.GetResist().Electro.Value += resistGained;
                PlayerStats.Instance.GetResist().Venom.Value += resistGained;

                isOn = true;
            }

            shield_armor = Mathf.Clamp(shield_armor, 0, maxShield_armor);
        }


        /// <summary>
        /// Returns overflow damage value in float.
        /// </summary>
        public void TakeDamage(Damage damage)
        {
            float value = damage.ResolvedValue;

            // calculate overflowed damage on energy shield
            float overflow_energy = shield_energy - value;
            if (damage.Element != Element.Type.Physical && shield_energy > 0)
            {
                overflow_energy -= value * (1 - elementalDamageReduction);
            }
            shield_energy = Mathf.Clamp(overflow_energy, 0, maxShield_energy);
            timeSinceDamaged = 0f;
            if (overflow_energy >= 0)
            {
                overflow = 0f;
                return;
            }

            // calculate overflowed damage on armor shield
            float overflow_armor = shield_armor + overflow_energy;
            if (damage.Element == Element.Type.Physical && shield_armor > 0)
            {
                overflow_armor -= overflow_energy * (1 - physicalDamageReduction);
            }
            shield_armor = Mathf.Clamp(overflow_armor, 0, maxShield_armor);
            if (overflow_armor >= 0)
            {
                overflow = 0f;
                return;
            }

            overflow = -overflow_armor;
        }



        public DamageFactor GetDamageFactor() { return new DamageFactor(); }
        public Resist GetResist() { return new Resist(); }
        public float ComputeExtraDamage(float baseValue) { return 0f; }

        public float TotalHealth { get { return shield_armor + shield_energy; } }
        public float Shield_armor { get { return shield_armor; } set { shield_armor = value; } }
        public float MaxShield_armor { get { return maxShield_armor; } set { maxShield_armor = value; } }
        public float Shield_energy { get { return shield_energy; } set { shield_energy = value; } }
        public float MaxShield_energy { get { return maxShield_energy; } set { maxShield_energy = value; } }
        public float Overflow { get { return overflow; } }

    }
}
