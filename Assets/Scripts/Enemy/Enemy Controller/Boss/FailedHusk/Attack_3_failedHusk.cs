using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Attack_3_failedHusk : EnemySkill
    {
        [SerializeField] private bool isUpgraded = false;

        [SerializeField] private GameObject swordPrefab;

        [SerializeField] private int number;
        [SerializeField] private float range;

        public IEnumerator Perform()
        {
            StartCoolingdown();

            yield return null;
        }

        public override bool IsPerformingAllowed()
        {
            return isReady && enemy.DistanceToPlayer < range;
        }
    }
}
