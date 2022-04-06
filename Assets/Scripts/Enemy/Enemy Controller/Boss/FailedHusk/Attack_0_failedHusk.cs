using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Attack_0_failedHusk : EnemySkill
    {
        [SerializeField] private bool isUpgraded = false;

        [SerializeField] private GameObject swordPrefab;

        [SerializeField] private int number;
        [SerializeField] private float range;

        public IEnumerator Perform()
        {
            StartCoolingdown();

            yield return StartCoroutine(SwordBarrage());
        }

        private IEnumerator SwordBarrage()
        {
            if (!isUpgraded)
                yield break;

            //for (int i = 0; i < number; i++)
            //{
            //    GameObject sword = Instantiate(swordPrefab);

            //    var rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
            //    var distance = Random.Range(range, 2 * range);
            //    var position = rotation * () * distance + enemy.transform.position;

            //    sword.transform.position = position;

            //    sword.transform.LookAt(enemy.transform);

            //    var degree = 45 + ((distance - range) / range) * 30;
            //    var angles = sword.transform.eulerAngles;
            //    sword.transform.eulerAngles = new Vector3(-degree, angles.y, angles.z);

            //    yield return new WaitForSeconds(0.01f);
            //}
        }

        public override bool IsPerformingAllowed()
        {
            return isReady && enemy.DistanceToPlayer < range;
        }
    }
}
