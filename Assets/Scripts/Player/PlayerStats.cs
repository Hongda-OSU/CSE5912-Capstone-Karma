using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float shield_armor = 0f;
        [SerializeField] private float shield_energy = 0f;

        [SerializeField] private float extraMoveSpeedFactor = 0f;
        [SerializeField] private float extraReloadSpeedFactor = 0f;

        [SerializeField] private float extraDamageFactor_physic = 0f;
        [SerializeField] private float extraDamageFactor_fire = 0f;
        [SerializeField] private float extraDamageFactor_cryo = 0f;
        [SerializeField] private float extraDamageFactor_electro = 0f;
        [SerializeField] private float extraDamageFactor_venom = 0f;

        private static PlayerStats instance;
        public static PlayerStats Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;
        }

        public void ResetExtras()
        {
            extraMoveSpeedFactor = 0f;
            extraReloadSpeedFactor = 0f;

            extraDamageFactor_physic = 0f;
            extraDamageFactor_fire = 0f;
            extraDamageFactor_cryo = 0f;
            extraDamageFactor_electro = 0f;
            extraDamageFactor_venom = 0f;
        }


        public float Health { get { return health; } set { Mathf.Clamp(health += value, 0, maxHealth); } }
        public float MaxHealth { get { return maxHealth; } set {  maxHealth = value; } }
        public float Shield_armor {  get { return shield_armor; } set { shield_armor = value; } }
        public float Shield_energy { get { return shield_energy; } set { shield_energy = value; } }
        public float ExtraMoveSpeedFactor { get { return extraMoveSpeedFactor; } set { extraMoveSpeedFactor = value; } }
        public float ExtraReloadSpeedFactor { get { return extraReloadSpeedFactor; } set { extraReloadSpeedFactor = value; } }
        public float ExtraDamageFactor_physic { get { return extraDamageFactor_physic; } set { extraDamageFactor_physic = value; } }
        public float ExtraDamageFactor_fire {  get { return extraDamageFactor_fire; } set { extraDamageFactor_fire = value; } }
        public float ExtraDamageFactor_cryo {  get { return extraDamageFactor_cryo;} set { extraDamageFactor_cryo = value;} }
        public float ExtraDamageFactor_electro {  get { return extraDamageFactor_electro; } set { extraDamageFactor_electro = value;} }
        public float ExtraDamageFactor_venom { get { return extraDamageFactor_venom; } set { extraDamageFactor_venom = value;} }
    }
}
