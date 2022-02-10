using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerProperty : MonoBehaviour
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private float shield_armor = 0f;
        [SerializeField] private float shield_energy = 0f;

        [SerializeField] private float speedFactor = 1f;
        [SerializeField] private float reloadSpeedFactor = 1f;

        [SerializeField] private float damageFactor_physic = 1f;
        [SerializeField] private float damageFactor_fire = 1f;
        [SerializeField] private float damageFactor_cryo = 1f;
        [SerializeField] private float damageFactor_electro = 1f;
        [SerializeField] private float damageFactor_venom = 1f;

        private static PlayerProperty instance;
        public static PlayerProperty Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;
        }
    }
}
