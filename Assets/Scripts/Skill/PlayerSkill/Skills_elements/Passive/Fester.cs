using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Fester : PlayerSkill
    {

        [Header("Fester")]
        [SerializeField] private GameObject vfxPrefab;

        [SerializeField] private float triggerTime = 20f;
        [SerializeField] private float timePerLevel = 1f;

        [SerializeField] private float baseDamage = 0.1f;
        [SerializeField] private float damagePerLevel = 0.01f;

        private void Update()
        {

            foreach (var obj in EnemyManager.Instance.EnemyList)
            {
                var enemy = obj.GetComponent<Enemy>();
                bool isTriggered = enemy.Infected.timeLasted >= triggerTime - timePerLevel * (level - 1);
                if (!isLearned || !isTriggered)
                    continue;

                Perform(enemy);
            }
        }

        private void Perform(Enemy enemy)
        {
            GameObject vfx = Instantiate(vfxPrefab);
            Destroy(vfx, 5f);

            var position = enemy.transform.position;
            vfx.transform.position = position + Vector3.up * enemy.GetComponentInChildren<Renderer>().bounds.size.y / 2;

            var rotation = enemy.transform.eulerAngles;
            vfx.transform.rotation = Quaternion.Euler(rotation.x, Random.Range(0, 360), rotation.z);

            float dmg = baseDamage + damagePerLevel * (level - 1);
            dmg *= enemy.MaxHealth;

            Damage damage = new Damage(dmg, Element.Type.Venom, PlayerStats.Instance, enemy);
            PlayerManager.Instance.PerformSkillDamage(enemy, damage);

            enemy.Infected.timeLasted = 0f;
            enemy.Infected.Stack = 0;
        }
    }
}
