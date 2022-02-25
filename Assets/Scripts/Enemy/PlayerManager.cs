using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Camera playerCamera;

        private Enemy hitByBullet;

        private Enemy lastEnemyHit;

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
            playerCamera = WeaponManager.Instance.CarriedWeapon.GunCamera;
        }


        public void PerformDamage(Enemy enemy, Vector3 position)
        {
            lastEnemyHit = enemy;
            if (!enemy.IsAlive)
                return;

            Firearms weapon = WeaponManager.Instance.CarriedWeapon;

            Damage damage = new Damage(weapon.Damage, weapon.Element, PlayerStats.Instance, lastEnemyHit);
            lastEnemyHit.TakeDamage(damage);

            StartCoroutine(DamageNumber.Instance.DisplayDamageNumber(damage, position));
        }

        public void PerformDamage(Enemy enemy, Damage damage)
        {
            lastEnemyHit = enemy;
            if (!enemy.IsAlive)
                return;

            lastEnemyHit.TakeDamage(damage);

            Renderer renderer = enemy.transform.GetComponentInChildren<Renderer>();
            Vector3 position = enemy.transform.position + Vector3.up * renderer.bounds.size.y / 2;
            StartCoroutine(DamageNumber.Instance.DisplayDamageNumber(damage, position));
        }

        public GameObject Player { get { return player; } }
        public Camera PlayerCamera { get { return playerCamera; } }
        public Enemy LastEnemyHit { get { return lastEnemyHit; } }
        public Enemy HitByBullet { get { return hitByBullet; } set { hitByBullet = value; } } 
    }
}