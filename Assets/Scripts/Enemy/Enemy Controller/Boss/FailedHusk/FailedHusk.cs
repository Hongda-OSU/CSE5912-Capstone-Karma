using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FailedHusk : BossEnemy
    {
        [Header("Failed Husk")]

        private SwordZone swordZone;
        private Attack_0_failedHusk attack_0;

        private bool isPerforming = false;

        private void Awake()
        {
            swordZone = GetComponentInChildren<SwordZone>();
            attack_0 = GetComponentInChildren<Attack_0_failedHusk>();

            isInvincible = true;
        }


        protected override void PerformActions()
        {
            if (isPerforming || !isBossFightTriggered)
                return;


            switch (status)
            {
                case Status.Idle:
                    if (playerDetected)
                    {
                        FaceTarget(directionToPlayer);

                        if (!isAttacking)
                        {
                            if (isPlayerInAttackRange)
                            {
                                Attack();
                            }
                            else
                            {
                                MoveToPlayer();
                            }
                        }
                    }
                    else
                    {
                        Rest();
                    }
                    break;

                case Status.Moving:
                    status = Status.Idle;
                    break;

                case Status.Attacking:
                    if (isFatigued)
                    {
                        PrepareForNextAttack();
                    }
                    status = Status.Idle;

                    break;

                case Status.Retreating:

                    break;

                case Status.Waiting:
                    if (!isFatigued)
                    {
                        Rest();
                    }
                    break;
            }
        }

        public override void TriggerBossFight()
        {
            isInvincible = true;
            animator.applyRootMotion = true;
            animator.SetTrigger("Awake");
        }

        protected override void AwakeAnimationComplete()
        {
            isInvincible = false;
            isBossFightTriggered = true;
            animator.applyRootMotion = true;
            isPerforming = false;
        }

        public override void ResetEnemy()
        {
            base.ResetEnemy();
            isBossFightTriggered = false;

        }

        protected override void MoveToPlayer()
        {
            base.MoveToPlayer();

        }
        protected override IEnumerator PerformActionsOnWaiting()
        {
            currentAttackNum = 0;
            yield return null;
        }

        protected override void PlayDeathAnimation()
        {
            animator.applyRootMotion = true;

            animator.SetTrigger("Die");
        }


        private void Attack()
        {
            if (attack_0.IsPerformingAllowed())
                Attack_0();

            status = Status.Attacking;
        }

        private void Attack_0()
        {
            status = Status.Attacking;
            SetAttack(0);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator Attack_0_performed()
        {
            yield return StartCoroutine(attack_0.Perform());
            StartCoroutine(swordZone.Perform());

            isPerforming = false;
        }

        private void DonePerforming()
        {
            isPerforming = false;
        }

    }
}
