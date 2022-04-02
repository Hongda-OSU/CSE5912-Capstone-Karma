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
        [SerializeField] private float experienceMultiplier = 1f;

        [Header("Durability")]
        [SerializeField] private bool isAlive = true;
        [SerializeField] private float health = 100f;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private Shield shield;
        [SerializeField] private float bulletVamp = 0f;
        [SerializeField] private float takeDamageFactor = 1f;

        [Header("Agility")]
        [SerializeField] private float moveSpeedFactor = 1f;
        [SerializeField] private float reloadSpeedFactor = 1f;
        [SerializeField] private float fireRateFactor = 1f;

        [Header("Melee")]
        [SerializeField] private float meleeDamageFactor = 1f;
        [SerializeField] private float meleeSpeedFactor = 1f;

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

        [Header("Audio")]
        [SerializeField] private AudioSource levelUpAudio;
        [SerializeField] private AudioSource statUpAudio;

        [Header("VFX")]
        [SerializeField] private GameObject levelUpVfxPrefab;


        private Dictionary<string, int> nameToStatsLevel = new Dictionary<string, int>();

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
            damage.ResolvedValue *= takeDamageFactor;

            shield.TakeDamage(damage);

            Health -= shield.Overflow;

            //Health -= damage.ResolvedValue;

            takeDamageEvent.Invoke();

            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (!isAlive)
                return;

            isAlive = false;
            RespawnManager.Instance.RespawnPlayerToLast();
        }

        public void Respawn()
        {
            isAlive = true;

            health = maxHealth;
            shield.ResetShield();
        }

        public void HitBack(Vector3 dir, float force)
        {
            FPSControllerCC.Instance.AddImpact(dir, force);
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

                levelUpAudio.Play();

                GameObject vfx = Instantiate(levelUpVfxPrefab, PlayerManager.Instance.Player.transform);
                Destroy(vfx, 10f);
            }
        }

        public void GetExperience(float value)
        {
            experience += value * experienceMultiplier;
        }

        public void ResetStats()
        {
            foreach (var kvp in nameToStatsLevel)
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    AddStat(kvp.Key, false);
                }
            }
            nameToStatsLevel.Clear();
        }


        public void LevelUpStat(string stat)
        {
            if (statPoint == 0)
                return;

            statUpAudio.Play();

            if (nameToStatsLevel.ContainsKey(stat))
            {
                nameToStatsLevel[stat]++;
            }
            else
            {
                nameToStatsLevel.Add(stat, 1);
            }

            AddStat(stat, true);
            statPoint--;
        }

        public void AddStat(string stat, bool levelUp)
        {
            int multiplier = levelUp ? 1 : -1;
            switch (stat)
            {
                case "Health":
                    health += healthUp * multiplier;
                    maxHealth += healthUp * multiplier;
                    break;
                case "Shield_armor":
                    shield.Shield_armor += shieldUp * multiplier;
                    shield.MaxShield_armor += shieldUp * multiplier;
                    break;
                case "Shield_energy":
                    shield.Shield_energy += shieldUp * multiplier;
                    shield.MaxShield_energy += shieldUp * multiplier;
                    break;
                case "CritRate":
                    critRate += critRateUp * multiplier;
                    break;
                case "CritDamage":
                    critDamageFactor += critDamageUp * multiplier;
                    break;
                case "PhysicalDamage":
                    damageFactor.Physical.Value += damageUp * multiplier;
                    break;
                case "FireDamage":
                    damageFactor.Fire.Value += damageUp * multiplier;
                    break;
                case "CryoDamage":
                    damageFactor.Cryo.Value += damageUp * multiplier;
                    break;
                case "ElectroDamage":
                    damageFactor.Electro.Value += damageUp * multiplier;
                    break;
                case "VenomDamage":
                    damageFactor.Venom.Value += damageUp * multiplier;
                    break;
                case "PhysicalResist":
                    resist.Physical.Value += resistUp * multiplier;
                    break;
                case "FireResist":
                    resist.Fire.Value += resistUp * multiplier;
                    break;
                case "CryoResist":
                    resist.Cryo.Value += resistUp * multiplier;
                    break;
                case "ElectroResist":
                    resist.Electro.Value += resistUp * multiplier;
                    break;
                case "VenomResist":
                    resist.Venom.Value += resistUp * multiplier;
                    break;
            }
        }


        public DamageFactor GetDamageFactor()
        {
            return damageFactor;
        }
        public DamageFactor PlayerDamageFactor { set { damageFactor = value; } }

        public Resist GetResist()
        {
            return resist;
        }
        public Resist PlayerResist { set { resist = value; } }

        public float Health { get { return health; } 
            set 
            {
                health = Mathf.Clamp(value, 0, maxHealth);
            } 
        }
        public float MaxHealth { get { return maxHealth; } 
            set 
            {  
                maxHealth = value;
                if (maxHealth < 0f)
                    maxHealth = 0f;
            }
        }

        public float Shield_armor { get { return shield.Shield_armor; } set { shield.Shield_armor = Mathf.Clamp(value, 0f, MaxShield_armor); } }
        public float MaxShield_armor { get { return shield.MaxShield_armor; } set { shield.MaxShield_armor = value; } }
        public float Shield_energy { get { return shield.Shield_energy; } set { shield.Shield_energy = Mathf.Clamp(value, 0f, MaxShield_energy); } }
        public float MaxShield_energy { get { return shield.MaxShield_energy; } set { shield.MaxShield_energy = value; } }
        public float BulletVamp { get { return bulletVamp; } set { bulletVamp = value; } }
        public float TakeDamageFactor { get { return takeDamageFactor; } set { takeDamageFactor = value; } }

        public int StatPoint { get { return statPoint; } set { statPoint = value; } }
        public int Level { get { return level; } set { level = value; } }
        public float Experience { get { return experience; } set { experience = value; } }
        public float ExperienceToUpgrade { get { return experienceToUpgrade; } set { experienceToUpgrade = value; } }
        public float ExperienceMultiplier { get { return experienceMultiplier; } set { experienceMultiplier = value; } }
        public float CritDamageFactor { get { return critDamageFactor; } set { critDamageFactor = value; } }
        public float CritRate { get { return critRate; } set { critRate = Mathf.Clamp(value, 0f, 1f); } }

        public float MoveSpeedFactor { get { return moveSpeedFactor; } set { moveSpeedFactor = value; } }
        public float ReloadSpeedFactor { get { return reloadSpeedFactor; } set { reloadSpeedFactor = value; } }
        public float FireRateFactor { get { return fireRateFactor; } set { fireRateFactor = value; } }

        public float MeleeDamageFactor { get { return meleeDamageFactor; } set { meleeDamageFactor = value; } }
        public float MeleeSpeedFactor { get { return meleeSpeedFactor; } set { meleeSpeedFactor = value; } }

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

        public Dictionary<string, int> NameToStatsLevel { get { return nameToStatsLevel; } set { nameToStatsLevel = value; } }

        public UnityEvent TakeDamageEvent { get { return takeDamageEvent; } }
    }
}
