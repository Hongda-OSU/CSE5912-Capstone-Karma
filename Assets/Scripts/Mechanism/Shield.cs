using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Shield : MonoBehaviour, IDamageable
    {
        [SerializeField] private float shield_armor = 0f;
        [SerializeField] private float maxShield_armor = 100f;
        [SerializeField] private float shield_energy = 0f;
        [SerializeField] private float maxShield_energy = 100f;
        private float overflow;

        /// <summary>
        /// Returns overflow damage value in float.
        /// </summary>
        public void TakeDamage(Damage damage)
        {
            float value = damage.ResolvedValue;

            // calculate overflowed damage on energy shield
            float overflow_energy = shield_energy - value;
            if (damage.Element == Element.Type.Electro && shield_energy > 0)
            {
                overflow_energy -= value * 0.5f;
            }
            shield_energy = Mathf.Clamp(overflow_energy, 0, maxShield_energy);
            if (overflow_energy >= 0)
                return;

            // calculate overflowed damage on armor shield
            float overflow_armor = shield_armor + overflow_energy;
            if (damage.Element == Element.Type.Venom && shield_armor > 0)
            {
                overflow_armor -= overflow_energy * 0.5f;
            }
            shield_armor = Mathf.Clamp(overflow_armor, 0, maxShield_armor);
            if (overflow_armor >= 0)
                return;

            overflow = -overflow_armor;
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    collision.gameObject.TryGetComponent(out Bullet bullet);
        //    if (bullet == null)
        //        return;

        //    Destroy(bullet.gameObject);

        //    Damage damage = new Damage(bullet.damage, bullet.elementType, PlayerStats.Instance, this);
        //    TakeDamage(damage);

        //    StartCoroutine(DamageNumberControl.Instance.DisplayDamageNumber(damage, position));
        //}

        public DamageFactor GetDamageFactor() { return new DamageFactor(); }
        public Resist GetResist() { return new Resist(); }
        public float ComputeExtraDamage(float baseValue) { return 0f; }

        public float TotalHealth { get { return shield_armor + shield_energy; } }
        public float Shield_armor { get { return shield_armor; } set { shield_armor = value; } }
        public float MaxShield_armor { get { return maxShield_armor; } set { maxShield_armor = value; } }
        public float Shield_energy { get { return shield_energy; } set { shield_energy = value; } }
        public float MaxShield_energy { get { return maxShield_energy; } set { shield_energy = value; } }
        public float Overflow { get { return overflow; } }

    }
}
