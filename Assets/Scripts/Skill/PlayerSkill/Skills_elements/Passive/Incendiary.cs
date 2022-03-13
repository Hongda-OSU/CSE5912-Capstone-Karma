using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Incendiary : PlayerSkill
    {

        [Header("Incendiary")]
        [SerializeField] public GameObject vfxPrefab;

        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float damagePerLevel = 5f;

        public float baseTime = 5f;
        public float timePerLevel = 1f;

        [SerializeField] private bool spreadToEnemy = false;

        public List<Enemy> burningEnemyList;

        public Dictionary<Enemy, int> enemyToStack;

        private Bullet prevBullet;
        private LivingFlame livingFlame;

        private void Awake()
        {
            burningEnemyList = new List<Enemy>();
            enemyToStack = new Dictionary<Enemy, int>();
        }

        private void Update()
        {
            if (!isLearned)
                return;

            var bullet = WeaponManager.Instance.CarriedWeapon.bulletFired;
            if (bullet == null)
            {
                prevBullet = null;
                return;
            }

            if (bullet != prevBullet)
            {
                bullet.hitEvent.AddListener(delegate { CreateFlame(bullet); });
                prevBullet = bullet;
            }
        }

        private void CreateFlame(Bullet bullet)
        {
            bullet.targetHit.TryGetComponent(out Enemy enemy);
            if (enemy != null)
                return;

            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = bullet.hitPosition;

            vfx.GetComponent<IncendiaryFireDamager>().livingFlame = livingFlame;
            vfx.GetComponent<IncendiaryFireDamager>().baseTime = livingFlame.Time;

            float time = baseTime + timePerLevel * (level - 1);
            StartCoroutine(vfx.GetComponent<IncendiaryFireDamager>().Perform(this, time, spreadToEnemy, null));

        }
        public void CreateFlame(Enemy enemy)
        {
            GameObject vfx = Instantiate(vfxPrefab, enemy.transform);

            float time = baseTime + timePerLevel * (level - 1);
            StartCoroutine(vfx.GetComponent<IncendiaryFireDamager>().Perform(this, time, spreadToEnemy, enemy));
        }

        public void PerformDamage(Enemy enemy)
        {
            float dmg = baseDamage + damagePerLevel * (level - 1);
            Damage damage = new Damage(dmg, Element.Type.Fire, PlayerStats.Instance, enemy);
            PlayerManager.Instance.PerformSkillDamage(enemy, damage);
        }

        public void AllowSpreadToEnemy()
        {
            spreadToEnemy = true;
        }
        public void AllowEnemyTracking(LivingFlame livingFlame)
        {
            this.livingFlame = livingFlame;
        }
    }
}
