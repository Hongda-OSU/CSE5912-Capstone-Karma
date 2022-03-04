using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Crystallization : Skill
    {
        [Header("Crystallization")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float baseDamage = 20f;
        [SerializeField] private float damagePerLevel = 10f;

        [SerializeField] private float baseFrozenTime = 2f;
        [SerializeField] private float frozenTimePerLevel = 0.3f;

        private void Update()
        {
            var target = PlayerManager.Instance.HitByBullet;
            if (!isLearned || !isReady || target == null || !target.IsAlive)
                return;

            var frozen = target.GetComponentInChildren<Frozen>();
            if (frozen.Stack < frozen.MaxStack)
                return;

            StartCoroutine(Perform(target.GetComponent<Enemy>()));
        }

        private IEnumerator Perform(Enemy enemy)
        {
            isReady = false;

            enemy.Freeze(true);

            float dmg = baseDamage + damagePerLevel * (level - 1);
            Damage damage = new Damage(dmg, Element.Type.Electro, PlayerStats.Instance, enemy);
            PlayerManager.Instance.PerformSkillDamage(enemy, damage);

            GameObject vfx = Instantiate(vfxPrefab);
            vfx.transform.position = enemy.transform.position;

            float frozenTime = baseFrozenTime + frozenTimePerLevel * (level - 1);

            yield return StartCoroutine(vfx.GetComponent<IceControl>().WaitFor(frozenTime));

            Destroy(vfx, 5f);
            enemy.Freeze(false);
            enemy.Frozen.Stack = 0;
            isReady = true;
        }
    }
}
