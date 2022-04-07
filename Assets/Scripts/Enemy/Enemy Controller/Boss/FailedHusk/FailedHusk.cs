using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FailedHusk : BossEnemy
    {
        [Header("Failed Husk")]

        private SwordZone swordZone;
        private GroundCrack groundCrack;

        private Attack_0_failedHusk attack_0;
        private Attack_1_failedHusk attack_1;
        private Attack_2_failedHusk attack_2;
        private Attack_3_failedHusk attack_3;
        private Attack_4_failedHusk attack_4;
        private Attack_5_failedHusk attack_5;

        private bool isPerforming = false;

        private void Awake()
        {
            swordZone = GetComponentInChildren<SwordZone>();
            groundCrack = GetComponentInChildren<GroundCrack>();

            attack_0 = GetComponentInChildren<Attack_0_failedHusk>();
            attack_1 = GetComponentInChildren<Attack_1_failedHusk>();
            attack_2 = GetComponentInChildren<Attack_2_failedHusk>();
            attack_3 = GetComponentInChildren<Attack_3_failedHusk>();
            attack_4 = GetComponentInChildren<Attack_4_failedHusk>();
            attack_5 = GetComponentInChildren<Attack_5_failedHusk>();

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
            //Attack_0();
            //Attack_1();
            Attack_2();
            //Attack_3();
            //Attack_4();
            //Attack_5();

            //if (attack_0.IsPerformingAllowed())
            //    Attack_0();
            //else if (attack_1.IsPerformingAllowed())
            //    Attack_1();
            //else if (attack_2.IsPerformingAllowed())
            //    Attack_2();
            //else if (attack_3.IsPerformingAllowed())
            //    Attack_3();
            //else if (attack_4.IsPerformingAllowed())
            //    Attack_4();
            //else if (attack_5.IsPerformingAllowed())
            //    Attack_5();
            //else 
            //    PrepareForNextAttack();

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
            yield return StartCoroutine(attack_0.Perform(swordZone));

            isPerforming = false;
        }

        private void Attack_1()
        {
            status = Status.Attacking;
            SetAttack(1);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator Attack_1_performed()
        {
            yield return StartCoroutine(attack_1.Perform(groundCrack));

            isPerforming = false;
        }

        private void Attack_2()
        {
            status = Status.Attacking;
            SetAttack(2);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator Attack_2_performed()
        {
            yield return StartCoroutine(attack_2.Perform(groundCrack));

            isPerforming = false;
        }

        private void Attack_3()
        {
            status = Status.Attacking;
            SetAttack(3);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator Attack_3_performed()
        {
            yield return StartCoroutine(attack_3.Perform());

            isPerforming = false;
        }

        private void Attack_4()
        {
            status = Status.Attacking;
            SetAttack(4);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator Attack_4_performed()
        {
            yield return StartCoroutine(attack_4.Perform());

            isPerforming = false;
        }

        private void Attack_5()
        {
            status = Status.Attacking;
            SetAttack(5);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator Attack_5_performed()
        {
            yield return StartCoroutine(attack_5.Perform());

            isPerforming = false;
        }

        private void DonePerforming()
        {
            isPerforming = false;
        }

    }
}
