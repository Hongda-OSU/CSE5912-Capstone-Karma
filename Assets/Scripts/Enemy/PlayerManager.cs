using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private GameObject playerArms;

        private Enemy hitByBullet;

        private Enemy lastEnemyHit;

        private static PlayerManager instance;
        public static PlayerManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void Update()
        {
            playerCamera = WeaponManager.Instance.CarriedWeapon.GunCamera;

        }


        public void PerformBulletDamage(Enemy enemy, Vector3 position)
        {
            lastEnemyHit = enemy;
            if (!enemy.IsAlive || enemy.isInvincible)
                return;

            Firearms weapon = WeaponManager.Instance.CarriedWeapon;

            Damage damage = new Damage(weapon.Damage, weapon.Element, PlayerStats.Instance, enemy);
            enemy.TakeDamage(damage);

            PlayerStats.Instance.Health += damage.ResolvedValue * PlayerStats.Instance.BulletVamp;

            DialogueControl.Instance.HideImmediate();

            StartCoroutine(DamageNumberControl.Instance.DisplayDamageNumber(damage, position));
        }
        public void PerformBulletDamage(Shield shield, Damage damage, Vector3 position)
        {
            shield.TakeDamage(damage);

            PlayerStats.Instance.Health += damage.ResolvedValue * PlayerStats.Instance.BulletVamp;

            DialogueControl.Instance.HideImmediate();

            StartCoroutine(DamageNumberControl.Instance.DisplayDamageNumber(damage, position));
        }

        public void PerformSkillDamage(Enemy enemy, Damage damage)
        {
            lastEnemyHit = enemy;
            if (!enemy.IsAlive || enemy.isInvincible)
                return;

            enemy.TakeDamage(damage);

            DialogueControl.Instance.HideImmediate();

            Renderer renderer = enemy.transform.GetComponentInChildren<Renderer>();
            Vector3 position = enemy.transform.position + Vector3.up * renderer.bounds.size.y / 2;
            StartCoroutine(DamageNumberControl.Instance.DisplayDamageNumber(damage, position));
        }

        public void StackDebuff(Element.Type type, Enemy enemy)
        {
            if (PlayerStats.Instance.DebuffStacks(type))
                enemy.StackDebuff(type);
        }

        public GameObject Player { get { return player; } }
        public GameObject PlayerArms { get { return playerArms; } }
        public Camera PlayerCamera { get { return playerCamera; } }
        public Enemy LastEnemyHit { get { return lastEnemyHit; } }
        public Enemy HitByBullet { get { return hitByBullet; } set { hitByBullet = value; } } 
    }
}