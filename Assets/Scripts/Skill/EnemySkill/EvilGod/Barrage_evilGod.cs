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
      

        [SerializeField] private float timeBetweenAttack = 0.5f;
        [SerializeField] private float directionBias = 2f;

        [SerializeField] private GameObject impactPrefab;

        [SerializeField] private float triggerHealthPercentage = 0.7f;

        public IEnumerator Perform(List<GameObject> list)
        {
            var missileList = new List<GameObject>(list);
            list.Clear();

            var position = pivot.position;
            for (int i = 0; i < missileList.Count; i++)
            {

                var startPosition = position + new Vector3(
                    Random.Range(-pivotBias, pivotBias),
                    Random.Range(0, pivotBias),
                    Random.Range(-pivotBias, pivotBias)
                    );

                StartCoroutine(Grab(missileList[i], startPosition, 0.4f));

                yield return new WaitForSeconds(timeBetweenAttack);
            }

            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < missileList.Count; i++)
            {
                StartCoroutine(Fire(missileList[i]));

                yield return new WaitForSeconds(timeBetweenAttack);
            }

            StartCoolingdown();
        }

        private IEnumerator Grab(GameObject missile, Vector3 position, float time)
        {
            float grabSpeed = Vector3.Distance(enemy.transform.position, missile.transform.position) / time;
            float timeSince = 0f;
            while (timeSince < time)
            {
                missile.transform.position = Vector3.MoveTowards(missile.transform.position, position, grabSpeed * Time.deltaTime);

                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

        }

        private IEnumerator Fire(GameObject missile)
        {
            missile.transform.LookAt(PlayerManager.Instance.Player.transform);
            var rotation = missile.transform.rotation.eulerAngles;
            missile.transform.rotation = Quaternion.Euler(
                rotation.x + Random.Range(-directionBias, directionBias),
                rotation.y + Random.Range(-directionBias, directionBias),
                rotation.z + Random.Range(-directionBias, directionBias)
                );

            Destroy(missile, 15f);

            var damager = missile.GetComponent<Damager_collision>();
            //damager.Initialize(enemy);
            damager.Hit = null;

            while (missile != null && damager.Hit == null)
            {
                missile.transform.position += missile.transform.forward * speed * Time.deltaTime;

                yield return new WaitForSeconds(Time.deltaTime);
            }

            if (missile == null)
                yield break;

            GameObject impact = Instantiate(impactPrefab);
            impact.GetComponent<Damager_collision>().Initialize(enemy);
            impact.transform.position = missile.transform.position;
            Destroy(impact, 5f);

            Destroy(missile);
        }

        public override bool IsPerformingAllowed()
        {
            return isReady && enemy.playerDetected && enemy.Health <= enemy.MaxHealth * triggerHealthPercentage;
        }
    }
}
