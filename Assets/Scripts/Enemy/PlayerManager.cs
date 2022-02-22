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


        public void PerformDamage(Enemy enemy, Vector3 position)
        {
            enemyHit = enemy;

            Firearms weapon = WeaponManager.Instance.CarriedWeapon;

            Damage damage = new Damage(weapon.Damage, weapon.Element, PlayerStats.Instance, enemyHit);
            enemyHit.TakeDamage(damage);

            StartCoroutine(DamageNumber.Instance.DisplayDamageNumber(damage, position));
        }

        public GameObject Player { get { return player; } }
        public Camera PlayerCamera { get { return playerCamera; } }
    }
}