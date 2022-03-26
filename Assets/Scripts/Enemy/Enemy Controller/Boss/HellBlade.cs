using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class HellBlade : BossEnemy
    {
        [Header("2nd Phase Prefabs")]      
        [SerializeField] private GameObject greatSword;
        [SerializeField] private GameObject blackFire;

        protected override void PerformActions()
        {
            
        }

        protected override IEnumerator PerformActionsOnWaiting()
        {
            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        public override void TriggerBossFight()
        {
            isInvincible = true;
            animator.SetTrigger("Awake");
        }

        protected override void AwakeAnimationComplete()
        {
            isInvincible = false;
            isBossFightTriggered = true;
        }
    }
}
