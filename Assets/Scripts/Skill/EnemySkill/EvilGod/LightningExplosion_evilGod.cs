using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class LightningExplosion_evilGod : EnemySkill
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int number = 20;
        [SerializeField] private float spacing = 1f;
        [SerializeField] private float timeInterval = 0.1f;
        [SerializeField] private float attackRange = 15f;


        public IEnumerator Perform()
        {
            StartCoolingdown();

            var lightningExplosions = new GameObject("LightningExplosions");

            float offset = spacing;

            float damage = prefab.GetComponent<Damager_collision>().BaseDamage;

            Vector3 position = transform.position;
            Vector3 direction = transform.forward;
            for (int i = 0; i < number; i++)
            {
                GameObject vfx = Instantiate(prefab, lightningExplosions.transform);
                vfx.GetComponent<Damager_collision>().Initialize(enemy);

                vfx.GetComponent<Damager_collision>().BaseDamage = damage;

                vfx.transform.position = position + direction * offset * (i + 1);

                Destroy(vfx, 5f);

                yield return new WaitForSeconds(timeInterval);

                if (vfx.GetComponent<Damager_collision>().IsPlayerHit)
                {
                    damage = 0f;
                }
            }
            Destroy(lightningExplosions, 5f);
        }

        public override bool IsPerformingAllowed()
        {
            return isReady && enemy.DistanceToPlayer <= attackRange;
        }
    }
}
