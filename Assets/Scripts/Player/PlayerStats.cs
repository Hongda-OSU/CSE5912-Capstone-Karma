using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private Shield shield;

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

        [Header("Debuff")]
        [SerializeField] private float burnedBaseChance = 0f;
        [SerializeField] private float frozenBaseChance = 0f;
        [SerializeField] private float electrocutedBaseChance = 0f;
        [SerializeField] private float infectedBaseChance = 0f;

        [SerializeField] private float burnedDamagePerStack = 10f;
        [SerializeField] private float frozenSlowdownPerStack = 0.05f;
        [SerializeField] private float electrocutedResistReductionPerStack = 0.05f;
        [SerializeField] private float infectedCurrentHealthDamagePerStack = 0.01f;

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

        private UnityEvent takeDamageEvent;

        private static PlayerStats instance;
        public static PlayerStats Instance { get { return instance; } }


        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            shield = GetComponent<Shield>();

            damageFactor = new DamageFactor();
            damageFactor.SetDamageValues(damageFactor_physical, damageFactor_fire, damageFactor_cryo, damageFactor_electro, damageFactor_venom);

            resist = new Resist();
            resist.SetValues(physicalResist, fireResist, cryoResist, electroResist, venomResist);

            takeDamageEvent = new UnityEvent();
        }

        private void Update()
        {
            CheckAndUpgrade();
        }

        public void TakeDamage(Damage damage)
        {
            shield.TakeDamage(damage);

            Health -= shield.Overflow;

            takeDamageEvent.Invoke();
        }


        public float ComputeExtraDamage(float baseValue)
        {
            float extraDamage = 0f;
            if (critRate > Random.value) // todo - or enemy vital is hit)
            {
                extraDamage = baseValue * critDamageFactor;
            }
            return extraDamage;
        }

        public bool DebuffStacks(Element.Type type)
        {
            switch (type)
            {
                case Element.Type.Physical:
                    return false;

                case Element.Type.Fire:
                    if (Random.value < burnedBaseChance)
                        return true;
                    return false;

                case Element.Type.Cryo:
                    if (Random.value < frozenBaseChance)
                        return true;
                    return false;

                case Element.Type.Electro:
                    if (Random.value < electrocutedBaseChance)
                        return true;
                    return false;

                case Element.Type.Venom:
                    if (Random.value < infectedBaseChance)
                        return true;
                    return false;
            }
            return false;
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
                    shield.Shield_armor += shieldUp;
                    shield.MaxShield_armor += shieldUp;
                    break;
                case "Shield_energy":
                    shield.Shield_energy += shieldUp;
                    shield.MaxShield_energy += shieldUp;
                    break;
                case "CritRate":
                    critRate += critRateUp;
                    break;
                case "CritDamage":
                    critDamageFactor += critDamageUp;
                    break;
                case "PhysicalDamage":
                    damageFactor.Physical.Value += damageUp;
                    break;
                case "FireDamage":
                    damageFactor.Fire.Value += damageUp;
                    break;
                case "CryoDamage":
                    damageFactor.Cryo.Value += damageUp;
                    break;
                case "ElectroDamage":
                    damageFactor.Electro.Value += damageUp;
                    break;
                case "VenomDamage":
                    damageFactor.Venom.Value += damageUp;
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
                health = Mathf.Clamp(value, 0, maxHealth);
                PlayerHealthBarControl.Instance.UpdateHealthBar();
            } 
        }
        public float MaxHealth { get { return maxHealth; } 
            set 
            {  
                maxHealth = value;
                if (maxHealth < 0f)
                    maxHealth = 0f;
                PlayerHealthBarControl.Instance.UpdateHealthBar();
            }
        }
        public float Shield_armor { get { return shield.Shield_armor; } set { shield.Shield_armor = value; } }
        public float MaxShield_armor { get { return shield.MaxShield_armor; } set { shield.MaxShield_armor = value; } }
        public float Shield_energy { get { return shield.Shield_energy; } set { shield.Shield_energy = value; } }
        public float MaxShield_energy { get { return shield.MaxShield_energy; } set { shield.Shield_energy = value; } }

        public int StatPoint { get { return statPoint; } }
        public int Level { get { return level; } }
        public float Experience { get { return experience; } set { experience = value; } }
        public float ExperienceToUpgrade { get { return experienceToUpgrade; } set { experienceToUpgrade = value; } }
        public float CritDamageFactor { get { return critDamageFactor; } set { critDamageFactor = value; } }
        public float CritRate { get { return critRate; } set { critRate = Mathf.Clamp(value, 0f, 1f); } }
        public float MoveSpeedFactor { get { return moveSpeedFactor; } set { moveSpeedFactor = value; } }
        public float ReloadSpeedFactor { get { return reloadSpeedFactor; } set { reloadSpeedFactor = value; } }

        public float BurnedBaseChance { get { return burnedBaseChance; } set { burnedBaseChance = value; } }
        public float FrozenBaseChance { get { return frozenBaseChance; } set { frozenBaseChance = value; } }
        public float ElectrocutedBaseChance { get { return electrocutedBaseChance; } set { electrocutedBaseChance = value; } }
        public float InfectedBaseChance { get { return infectedBaseChance; } set { infectedBaseChance = value; } }

        public float BurnedDamagePerStack { get { return burnedDamagePerStack; } set { burnedDamagePerStack = value; } }
        public float FrozenSlowdownPerStack { get { return frozenSlowdownPerStack;} set { frozenSlowdownPerStack = value; } }
        public float ElectrocutedResistReductionPerStack { get { return electrocutedResistReductionPerStack; } set { electrocutedResistReductionPerStack = value; } }
        public float InfectedCurrentHealthDamagePerStack { get { return infectedCurrentHealthDamagePerStack;} set { infectedCurrentHealthDamagePerStack = value; } }

        public float HealthUp { get { return healthUp; } }
        public float ShieldUp { get { return shieldUp; } }
        public float CritRateUp { get { return critRateUp; } }
        public float CritDamageUp { get { return critDamageUp; } }
        public float DamageUp { get { return damageUp; } }
        public float ResistUp { get { return resistUp; } }

        public UnityEvent TakeDamageEvent { get { return takeDamageEvent; } }
    }
}
