using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerStats : MonoBehaviour, IDamageable
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float shield_armor = 0f;
        [SerializeField] private float shield_energy = 0f;

        [SerializeField] private float moveSpeedFactor = 1f;
        [SerializeField] private float reloadSpeedFactor = 1f;

        [SerializeField] private float critDamageFactor = 0.5f;
        [Range(0f, 1f)]
        [SerializeField] private float critChance = 0f;

        [SerializeField] private float physicalResist = 0f;
        [SerializeField] private float fireResist = 0f;
        [SerializeField] private float cryoResist = 0f;
        [SerializeField] private float electroResist = 0f;
        [SerializeField] private float venomResist = 0f;

        [SerializeField] private float damageFactor_physic = 1f;
        [SerializeField] private float damageFactor_fire = 1f;
        [SerializeField] private float damageFactor_cryo = 1f;
        [SerializeField] private float damageFactor_electro = 1f;
        [SerializeField] private float damageFactor_venom = 1f;

        Resist resist;

        private static PlayerStats instance;
        public static PlayerStats Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            resist = new Resist();
            resist.SetValues(physicalResist, fireResist, cryoResist, electroResist, venomResist);
        }

        public void TakeDamage(Damage damage)
        {

        }

        public Resist GetResist()
        {
            return resist;
        }

        public float ComputeExtraDamage()
        {
            float extraDamage = 0f;
            if (critChance > Random.value) // todo - or enemy vital is hit)
            {
                extraDamage = WeaponManager.Instance.CarriedWeapon.damage * critDamageFactor;
            }
            return extraDamage;
        }

        public void ResetFactors()
        {
            moveSpeedFactor = 1f;
            reloadSpeedFactor = 1f;

            damageFactor_physic = 1f;
            damageFactor_fire = 1f;
            damageFactor_cryo = 1f;
            damageFactor_electro = 1f;
            damageFactor_venom = 1f;
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
        public float Shield_armor {  get { return shield_armor; } set { shield_armor = value; } }
        public float Shield_energy { get { return shield_energy; } set { shield_energy = value; } }
        public float CritDamageFactor { get { return critDamageFactor; } set { critDamageFactor = value; } }
        public float CritChance { get { return critChance; } set { critChance = Mathf.Clamp(value, 0f, 1f); } }
        public float MoveSpeedFactor { get { return moveSpeedFactor; } set { moveSpeedFactor = value; } }
        public float ReloadSpeedFactor { get { return reloadSpeedFactor; } set { reloadSpeedFactor = value; } }
        public float DamageFactor_physic { get { return damageFactor_physic; } set { damageFactor_physic = value; } }
        public float DamageFactor_fire {  get { return damageFactor_fire; } set { damageFactor_fire = value; } }
        public float DamageFactor_cryo {  get { return damageFactor_cryo;} set { damageFactor_cryo = value;} }
        public float DamageFactor_electro {  get { return damageFactor_electro; } set { damageFactor_electro = value;} }
        public float DamageFactor_venom { get { return damageFactor_venom; } set { damageFactor_venom = value;} }
    }
}
