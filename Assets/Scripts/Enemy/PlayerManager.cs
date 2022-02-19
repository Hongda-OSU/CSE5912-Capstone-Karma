using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Camera playerCamera;

        private Enemy enemyHit;

        private static PlayerManager instance;
        public static PlayerManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;
        }

        private void Update()
        {
        }


        public void PerformDamage(Enemy enemy)
        {
            enemyHit = enemy;
            Firearms weapon = WeaponManager.Instance.CarriedWeapon;

            float critDamageFactor = 1f;
            if (PlayerStats.Instance.CritChance > Random.value) // todo - or enemy vital is hit)
            {
                critDamageFactor = PlayerStats.Instance.CritDamageFactor;
            }

            Damage damage = new Damage(weapon.damage * critDamageFactor, weapon.element, PlayerStats.Instance, enemyHit);
            enemyHit.TakeDamage(damage);

            Debug.Log(enemyHit.Health);
        }

        public GameObject Player { get { return player; } }
        public Camera PlayerCamera { get { return playerCamera; } }
    }
}