using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Barrage_evilGod : EnemySkill
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform pivot;
        [SerializeField] private float pivotBias = 1f;

        [SerializeField] private int number = 15;
        [SerializeField] private float speed = 50f;
        [SerializeField] private float attackRange = 50f;

        [SerializeField] private float timeBetweenAttack = 0.5f;
        [SerializeField] private float directionBias = 2f;

        [SerializeField] private GameObject impactPrefab;

        [SerializeField] private float triggerHealthPercentage = 0.7f;

        public IEnumerator Perform()
        {
            var position = pivot.position;
            for (int i = 0; i < number; i++)
            {
                var startPosition = position + new Vector3(
                    Random.Range(-pivotBias, pivotBias),
                    Random.Range(0, pivotBias),
                    Random.Range(-pivotBias, pivotBias)
                    );

                StartCoroutine(Fire(startPosition, 0.5f));

                yield return new WaitForSeconds(timeBetweenAttack);
            }

            StartCoolingdown();
        }
        private IEnumerator Fire(Vector3 position, float delay)
        {
            GameObject vfx = Instantiate(prefab);

            vfx.transform.position = position;

            vfx.transform.LookAt(PlayerManager.Instance.Player.transform);
            var rotation = vfx.transform.rotation.eulerAngles;
            vfx.transform.rotation = Quaternion.Euler(
                rotation.x + Random.Range(-directionBias, directionBias),
                rotation.y + Random.Range(-directionBias, directionBias),
                rotation.z + Random.Range(-directionBias, directionBias)
                );

            yield return new WaitForSeconds(delay);

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
            return isReady && enemy.playerDetected && enemy.Health <= enemy.MaxHealth * triggerHealthPercentage;
        }
    }
}
