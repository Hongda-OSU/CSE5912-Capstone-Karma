using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Attack_0_failedHusk : EnemySkill
    {
        [SerializeField] private float range;

        public override IEnumerator Perform()
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
