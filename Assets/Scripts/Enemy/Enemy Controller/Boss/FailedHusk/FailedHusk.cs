using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FailedHusk : BossEnemy
    {
        [Header("Failed Husk")]
        [SerializeField] private bool isUnleashed = false;

        [SerializeField] private float leashedSpeed = 0.5f;
        [SerializeField] private float unleashedSpeed = 1f;

        [SerializeField] private float blinkCastSpeed = 0.25f;
        [SerializeField] private float blinkCastTime = 0.5f;

        [SerializeField] private Damager_collision sword;

        private SwordZone swordZone;
        private GroundCrack groundCrack;
        private Slash slash;
        private Blink blink;

        private List<EnemySkill> skillList = new List<EnemySkill>();

        private Attack_0_failedHusk attack_0;
        private Attack_1_failedHusk attack_1;
        private Attack_2_failedHusk attack_2;
        private Attack_3_failedHusk attack_3;
        private Attack_4_failedHusk attack_4;

        [SerializeField] private bool isPerforming = false;

        private void Awake()
        {
            sword.Initialize(this);
            sword.BaseDamage = AttackDamage;

            swordZone = GetComponentInChildren<SwordZone>();
            groundCrack = GetComponentInChildren<GroundCrack>();
            slash = GetComponentInChildren<Slash>();
            blink = GetComponentInChildren<Blink>();

            attack_0 = GetComponentInChildren<Attack_0_failedHusk>();
            attack_1 = GetComponentInChildren<Attack_1_failedHusk>();
            attack_2 = GetComponentInChildren<Attack_2_failedHusk>();
            attack_3 = GetComponentInChildren<Attack_3_failedHusk>();
            attack_4 = GetComponentInChildren<Attack_4_failedHusk>();

            skillList.Add(attack_0);
            skillList.Add(attack_1);
            skillList.Add(attack_2);
            skillList.Add(attack_3);
            skillList.Add(attack_4);

            isInvincible = true;
        }


        protected override void PerformActions()
        {
            //animator.speed = isUnleashed ? unleashedSpeed : leashedSpeed;

            //test
            PlayerStats.Instance.IsInvincible = true;
            isUnleashed = true;

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
            var usable = new List<int>();
            for (int i = 0; i < skillList.Count; i++) 
            {
                var skill = skillList[i];
                if (skill.IsPerformingAllowed())
                    usable.Add(i);
            }

            if (usable.Count > 0)
                SkillAttack(usable[Random.Range(0, usable.Count)]);
            else
                PrepareForNextAttack();

            status = Status.Attacking;
        }

        private void SkillAttack(int index)
        {
            status = Status.Attacking;
            SetAttack(index);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator Attack_performed(int index)
        {
            yield return StartCoroutine(skillList[index].Perform());

            isPerforming = false;
        }

        private IEnumerator BaseSwordAttack_started()
        {
            sword.IsPlayerHit = false;

            yield return null;
        }
        private IEnumerator BaseSwordAttack_finished()
        {
            sword.IsPlayerHit = true;

            yield return null;
        }

        private IEnumerator SwordZone_performed()
        {
            if (!isUnleashed)
                yield break;

            StartCoroutine(swordZone.Perform());
        }

        private IEnumerator GroundCrack_performed()
        {
            if (!isUnleashed)
                yield break;

            StartCoroutine(groundCrack.Perform());
        }
        
        /*
         *  0 = right, 
         *  1 = down,
         *  2 = left,
         *  3 = top,
         *  4 = prick,
         */
        private IEnumerator Slash_performed(int flag)
        {
            if (!isUnleashed)
                yield break;

            slash.direction = flag;
            StartCoroutine(slash.Perform());
        }
        private IEnumerator Blink_performed()
        {
            if (!isUnleashed)
                yield break;

            var position = player.position - directionToPlayer * 5f;

            animator.speed = blinkCastSpeed;
            yield return new WaitForSeconds(blinkCastTime);
            yield return StartCoroutine(blink.Perform(position));
            animator.speed = unleashedSpeed;
        }

        private void DonePerforming()
        {
            isPerforming = false;
        }

    }
}
