using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Detonation : Skill
    {
        [Header("Lightning Chain")]
        [SerializeField] private GameObject detonationPrefab;

        [SerializeField] private float triggerTime = 5f;

        [SerializeField] private float baseDamageRadius = 3f;
        [SerializeField] private float radiusPerLevel = 0.5f;

        [SerializeField] private float baseDamage = 200f;
        [SerializeField] private float damagePerLevel = 50f;
        

        private void Update()
        {
            var target = PlayerManager.Instance.HitByBullet;
            if (!isLearned || target == null || !target.IsAlive)
                return;

            if (target.Electrocuted.Stack != target.Electrocuted.MaxStack)
                return;

            StartCoroutine(Perform(target));
        }

        private IEnumerator Perform(Enemy target)
        {
            if (!isReady)
                yield break;
            isReady = false;

            yield return new WaitForSeconds(triggerTime);

            GameObject vfx = Instantiate(detonationPrefab);
            vfx.transform.localScale = Vector3.one * (1 + radiusPerLevel * (level - 1) / baseDamageRadius);
            vfx.transform.position = target.transform.position + Vector3.up * target.GetComponentInChildren<Renderer>().bounds.size.y / 2;
            Destroy(vfx, 5f);

            float dmg = baseDamage + damagePerLevel * (level - 1);
            Damage damage = new Damage(dmg, Element.Type.Electro, PlayerStats.Instance, target);
            PlayerManager.Instance.PerformSkillDamage(target, damage);

            foreach (var enemyObj in EnemyManager.Instance.EnemyList)
            {
                var enemy = enemyObj.GetComponent<Enemy>();
                if (!enemy.IsAlive)
                    continue;

                float distance = Vector3.Distance(target.gameObject.transform.position, enemyObj.transform.position);
                float radius = baseDamageRadius + radiusPerLevel * (level - 1);
                if (distance < radius && enemyObj != target.gameObject)
                {
                    Debug.Log(enemyObj.name);
                    damage = new Damage(dmg, Element.Type.Electro, PlayerStats.Instance, enemy);
                    PlayerManager.Instance.PerformSkillDamage(enemy, damage);
                }
            }

            isReady = true;
            target.Electrocuted.Stack = 0;
        }
    }
}
