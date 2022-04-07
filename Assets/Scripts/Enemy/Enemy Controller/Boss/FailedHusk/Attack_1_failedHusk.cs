using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Attack_1_failedHusk : EnemySkill
    {
        [SerializeField] private bool isUpgraded = false;

        [SerializeField] private float range;
        [SerializeField] private float size;

        public IEnumerator Perform(GroundCrack crack)
        {
            StartCoolingdown();

            if (isUpgraded)
            {
                StartCoroutine(crack.Perform(enemy, size));
            }

            yield return null;
        }

        public override bool IsPerformingAllowed()
        {
            return isReady && enemy.DistanceToPlayer < range;
        }
    }
}
