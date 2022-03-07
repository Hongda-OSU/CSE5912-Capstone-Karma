using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Everfrost : Skill
    {
        [Header("Everfrost")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float baseDamage = 20f;
        [SerializeField] private float damagePerLevel = 10f;

        [SerializeField] private float baseFrozenTime = 2f;
        [SerializeField] private float frozenTimePerLevel = 0.3f;

        private void Update()
        {
            if (!isLearned)
                return;

            foreach (var obj in EnemyManager.Instance.EnemyList)
            {
                var enemy = obj.GetComponent<Enemy>();
                if (enemy.Frozen.Stack < enemy.Frozen.MaxStack || enemy.IsFrozen)
                    continue;

                Perform(enemy);
            }
        }

        private void Perform(Enemy enemy)
        {
            float dmg = baseDamage + damagePerLevel * (level - 1);
            Damage damage = new Damage(dmg, Element.Type.Cryo, PlayerStats.Instance, enemy);
            PlayerManager.Instance.PerformSkillDamage(enemy, damage);

            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = enemy.transform.position;

            float frozenTime = baseFrozenTime + frozenTimePerLevel * (level - 1);

            StartCoroutine(enemy.Freeze(frozenTime));
            StartCoroutine(vfx.GetComponent<IceControl>().WaitFor(frozenTime));

            Destroy(vfx, 5f);
        }
    }
}
