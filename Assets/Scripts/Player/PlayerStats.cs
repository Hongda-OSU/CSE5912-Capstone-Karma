using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerStats : MonoBehaviour, IDamageable
    {
        [Header("Basic")]
        [SerializeField] private int statPoint = 1;
        [SerializeField] private int level = 1;
        [SerializeField] private float experience = 0;
        [SerializeField] private float experienceToUpgrade = 668;

        [Header("Durability")]
        [SerializeField] private float health = 100f;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float shield_armor = 0f;
        [SerializeField] private float maxShield_armor = 100f;
        [SerializeField] private float shield_energy = 0f;
        [SerializeField] private float maxShield_energy = 100f;

        [Header("Agility")]
        [SerializeField] private float moveSpeedFactor = 1f;
        [SerializeField] private float reloadSpeedFactor = 1f;

        [Header("Critical")]
        [Range(0f, 1f)]
        [SerializeField] private float critRate = 0f;
        [SerializeField] private float critDamageFactor = 0.5f;

        [Header("Damage")]
        [SerializeField] private float damageFactor_physical = 1f;
        [SerializeField] private float damageFactor_fire = 1f;
        [SerializeField] private float damageFactor_cryo = 1f;
        [SerializeField] private float damageFactor_electro = 1f;
        [SerializeField] private float damageFactor_venom = 1f;
        private DamageFactor damageFactor;

        [Header("Resist")]
        [SerializeField] private float physicalResist = 0f;
        [SerializeField] private float fireResist = 0f;
        [SerializeField] private float cryoResist = 0f;
        [SerializeField] private float electroResist = 0f;
        [SerializeField] private float venomResist = 0f;
        private Resist resist;

        [Header("Stats Up per Level")]
        [SerializeField] private float healthUp = 10f;
        [SerializeField] private float shieldUp = 10f;
        [SerializeField] private float critRateUp = 0.02f;
        [SerializeField] private float critDamageUp = 0.05f;
        [SerializeField] private float damageUp = 0.03f;
        [SerializeField] private float resistUp = 30f;

        private static PlayerStats instance;
        public static PlayerStats Instance { get { return instance; } }


        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            damageFactor = new DamageFactor();
            damageFactor.SetValues(damageFactor_physical, damageFactor_fire, damageFactor_cryo, damageFactor_electro, damageFactor_venom);

            resist = new Resist();
            resist.SetValues(physicalResist, fireResist, cryoResist, electroResist, venomResist);
        }

        private void Update()
        {
            CheckAndUpgrade();
        }

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

            // calculate final damage on health
            health += overflow_armor;
        }


        public float ComputeExtraDamage()
        {
            float extraDamage = 0f;
            if (critRate > Random.value) // todo - or enemy vital is hit)
            {
                extraDamage = WeaponManager.Instance.CarriedWeapon.Damage * critDamageFactor;
            }
            return extraDamage;
        }

        private void CheckAndUpgrade()
        {
            if (experience >= experienceToUpgrade)
            {
                experience -= experienceToUpgrade;

                experienceToUpgrade = experienceToUpgrade * 1.1f;

                level++;

                statPoint++;
            }
        }

        public void ResetFactors()
        {
            moveSpeedFactor = 1f;
            reloadSpeedFactor = 1f;

            damageFactor_physical = 1f;
            damageFactor_fire = 1f;
            damageFactor_cryo = 1f;
            damageFactor_electro = 1f;
            damageFactor_venom = 1f;
        }


        public void LevelUpStat(string stat)
        {
            if (statPoint == 0)
                return;

            switch (stat)
            {
                case "Health":
                    health += healthUp;
                    maxHealth += healthUp;
                    break;
                case "Shield_armor":
                    shield_armor += shieldUp;
                    maxShield_armor += shieldUp;
                    break;
                case "Shield_energy":
                    shield_energy += shieldUp;
                    maxShield_energy += shieldUp;
                    break;
                case "CritRate":
                    critRate += critRateUp;
                    break;
                case "CritDamage":
                    critDamageFactor += critDamageUp;
                    break;
                case "PhysicalDamage":
                    damageFactor_physical += damageUp;
                    break;
                case "FireDamage":
                    damageFactor_fire += damageUp;
                    break;
                case "CryoDamage":
                    damageFactor_cryo += damageUp;
                    break;
                case "ElectroDamage":
                    damageFactor_electro += damageUp;
                    break;
                case "VenomDamage":
                    damageFactor_venom += damageUp;
                    break;
                case "PhysicalResist":
                    resist.Physical.Value += resistUp;
                    break;
                case "FireResist":
                    resist.Fire.Value += resistUp;
                    break;
                case "CryoResist":
                    resist.Cryo.Value += resistUp;
                    break;
                case "ElectroResist":
                    resist.Electro.Value += resistUp;
                    break;
                case "VenomResist":
                    resist.Venom.Value += resistUp;
                    break;
            }
            statPoint--;
        }

        

        public DamageFactor GetDamageFactor()
        {
            return damageFactor;
        }
        public Resist GetResist()
        {
            return resist;
        }
        public float Health { get { return health; } 
            set 
            { 
                Mathf.Clamp(health += value, 0, maxHealth);
                PlayerHealthBarControl.Instance.UpdateHealthBar();
            } 
        }
        public float MaxHealth { get { return maxHealth; } 
            set 
            {  
                maxHealth += value;
                if (maxHealth < 0f)
                    maxHealth = 0f;
                PlayerHealthBarControl.Instance.UpdateHealthBar();
            } 
        }
        public int StatPoint { get { return statPoint; } }
        public int Level { get { return level; } }
        public float Experience { get { return experience; } set { experience = value; } }
        public float ExperienceToUpgrade { get { return experienceToUpgrade; } set { experienceToUpgrade = value; } }
        public float Shield_armor {  get { return shield_armor; } set { shield_armor = value; } }
        public float MaxShield_armor { get { return maxShield_armor; } set { maxShield_armor = value; } }
        public float Shield_energy { get { return shield_energy; } set { shield_energy = value; } }
        public float MaxShield_energy { get { return maxShield_energy; } set { shield_energy = value; } }
        public float CritDamageFactor { get { return critDamageFactor; } set { critDamageFactor = value; } }
        public float CritRate { get { return critRate; } set { critRate = Mathf.Clamp(value, 0f, 1f); } }
        public float MoveSpeedFactor { get { return moveSpeedFactor; } set { moveSpeedFactor = value; } }
        public float ReloadSpeedFactor { get { return reloadSpeedFactor; } set { reloadSpeedFactor = value; } }

        public float HealthUp { get { return healthUp; } }
        public float ShieldUp { get { return shieldUp; } }
        public float CritRateUp { get { return critRateUp; } }
        public float CritDamageUp { get { return critDamageUp; } }
        public float DamageUp { get { return damageUp; } }
        public float ResistUp { get { return resistUp; } }
    }
}
