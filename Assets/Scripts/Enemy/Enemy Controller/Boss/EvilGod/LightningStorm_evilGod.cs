using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class LightningStorm_evilGod : EnemySkill
    {
        [SerializeField] private GameObject lightningPrefab;
        [SerializeField] private GameObject electricityPrefab;
        [SerializeField] private GameObject missilePrefab;

        [SerializeField] private float triggerHealthPercentage = 0.7f;

        [SerializeField] private int numberOfWaves = 5;
        [SerializeField] private float timeBetweenWaves = 2f;

        [SerializeField] private float warningTime = 1f;
        [SerializeField] private int number = 30;
        [SerializeField] private float radius = 30f;

        private List<GameObject> missileList = new List<GameObject>();


        public IEnumerator Perform()
        {
            StartCoolingdown();
            for (int j = 0; j < numberOfWaves; j++)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
                for (int i = 0; i < number; i++)
                {
                    StartCoroutine(SummonLightning());
                }
            }
        }

        private IEnumerator SummonLightning()
        {
            float offsetX = Random.Range(-radius, radius);
            float offsetZ = Random.Range(-radius, radius);
            var position = enemy.transform.position + Vector3.right * offsetX + Vector3.forward * offsetZ;

            GameObject electricity = Instantiate(electricityPrefab);
            electricity.transform.position = position;

            yield return new WaitForSeconds(warningTime);
            Destroy(electricity);

            GameObject lightning = Instantiate(lightningPrefab);

            lightning.transform.position = position;

            Destroy(lightning, 3f);

            var damager = lightning.GetComponent<Damager_collision>();
            damager.Initialize(enemy);


            GameObject missile = Instantiate(missilePrefab);

            missile.transform.position = position;

            damager = missile.GetComponent<Damager_collision>();
            damager.Initialize(enemy);

            missileList.Add(missile);
        }

        public override bool IsPerformingAllowed()
        {
            return isReady && enemy.playerDetected && enemy.Health <= enemy.MaxHealth * triggerHealthPercentage;
        }

        public List<GameObject> MissileList { get { return missileList; } }
    }
}
