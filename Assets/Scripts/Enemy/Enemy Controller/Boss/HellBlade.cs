using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class HellBlade : EliteEnemy
    {
        [Header("Great Sword")]      
        [SerializeField] private GameObject greatSword;

        protected override void PerformActions()
        {
            
        }

        protected override IEnumerator PerformActionsOnWaiting()
        {
            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
