using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class LightningMissile_evilGod : EnemySkill
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform pivot;
        [SerializeField] private float speed = 50f;
        [SerializeField] private float attackRange = 15f;

        [SerializeField] private GameObject impactPrefab;

        public IEnumerator Perform()
        {
            StartCoolingdown();

            GameObject vfx = Instantiate(prefab);
            vfx.transform.position = pivot.position;
            vfx.transform.LookAt(PlayerManager.Instance.Player.transform);

            Destroy(vfx, 15f);

            var damager = vfx.GetComponent<Damager_collision>();
            damager.Initialize(enemy);

            while (vfx != null && damager.Hit == null)
            {
                vfx.transform.position += vfx.transform.forward * speed * Time.deltaTime;

                yield return new WaitForSeconds(Time.deltaTime);
            }

            if (vfx == null)
                yield break;

            GameObject impact = Instantiate(impactPrefab);
            impact.GetComponent<Damager_collision>().Initialize(enemy);
            impact.transform.position = vfx.transform.position;
            Destroy(impact, 5f);

            Destroy(vfx);
        }

        public override bool IsPerformingAllowed()
        {
            return isReady && enemy.DistanceToPlayer <= attackRange;
        }
    }
}
