using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Smite : Skill
    {
        [Header("Smite")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float baseDamage = 200f;
        [SerializeField] private float damagePerLevel = 50f;

        [SerializeField] private int baseTargetNumber = 1;
        [SerializeField] private int targetNumberPerLevel = 1;

        [SerializeField] private float baseRadius = 10f;
        [SerializeField] private float radiusPerLevel = 3f;


        private void Update()
        {
            if (!isLearned || !isReady)
                return;

            int num = 0;
            int max = baseTargetNumber + targetNumberPerLevel * (level - 1);
            foreach (var target in EnemyManager.Instance.EnemyList)
            {
                if (num >= max)
                    break;

                var enemy = target.GetComponent<Enemy>();
                if (!enemy.IsAlive || enemy.Electrocuted.Stack == 0)
                    continue;

                float distance = Vector3.Distance(PlayerManager.Instance.Player.transform.position, enemy.transform.position);
                float effectDistance = baseRadius + radiusPerLevel * (level - 1);
                if (distance < effectDistance)
                {
                    Perform(enemy);
                    num++;
                }
            }
            StartCoolingdown();
        }

        private void Perform(Enemy enemy)
        {
            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = enemy.transform.position;
            Destroy(vfx, 5f);

            float dmg = baseDamage + damagePerLevel * (level - 1);
            Damage damage = new Damage(dmg, Element.Type.Electro, PlayerStats.Instance, enemy);
            PlayerManager.Instance.PerformSkillDamage(enemy, damage);
        }
    }
}
