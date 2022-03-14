using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class EvilGod : BossEnemy
    {
        [Header("Evil God")]
        [Header("Portal")]
        [SerializeField] private GameObject portalPrefab;
        [SerializeField] private Transform portalPivot;

        private LightningMissile_evilGod lightningMissile;
        private LightningExplosion_evilGod lightningExplosion;
        private Shield_evilGod shield;
        private Blink_evilGod blink;
        
        private bool isPerforming = false;

        private void Awake()
        {
            lightningMissile = GetComponentInChildren<LightningMissile_evilGod>();
            lightningExplosion = GetComponentInChildren<LightningExplosion_evilGod>();
            shield = GetComponentInChildren<Shield_evilGod>();
            blink = GetComponentInChildren<Blink_evilGod>();
        }


        protected override void PerformActions()
        {
            if (isPerforming)
                return;


            if (shield.IsPerformingAllowed())
            {
                OpenShield();
            }

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
                    if (isPlayerInSafeDistance)
                    {
                        Blink(transform.position + directionToPlayer * -attackRange);
                    }
                    else if (isFatigued)
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

        private void AwakeAnimationComplete()
        {
            isInvincible = false;
            isBossFightTriggered = true;
            animator.applyRootMotion = false;
        }

        protected override void MoveToPlayer()
        {
            base.MoveToPlayer();
            Blink(PlayerManager.Instance.Player.transform.position - directionToPlayer * attackRange * 0.8f);
        }
        protected override IEnumerator PerformActionsOnWaiting()
        {
            Vector3 position = PlayerManager.Instance.Player.transform.position + directionToPlayer * attackRange;
            Blink(position);
            currentAttackNum = 0;
            yield return null;
        }

        protected override void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
        }




        private void Blink(Vector3 position)
        {
            StartCoroutine(blink.Perform(position));
        }

        private void Attack()
        {
            Attack_lightningExplosion();
            Attack_lightningMissile();

            status = Status.Attacking;
        }

        private void Attack_lightningMissile()
        {
            if (!lightningMissile.IsPerformingAllowed())
                return;

            status = Status.Attacking;
            SetAttack(0);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator LightningMissile_performed()
        {
            isPerforming = false;

            yield return StartCoroutine(lightningMissile.Perform());
        }


        private void Attack_lightningExplosion()
        {
            if (!lightningExplosion.IsPerformingAllowed())
                return;

            status = Status.Attacking;
            SetAttack(1);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator LightningExplosion_performed()
        {
            isPerforming = false;

            Vector3 position = PlayerManager.Instance.Player.transform.position + directionToPlayer * attackRange;
            Blink(position);

            yield return new WaitForSeconds(Time.deltaTime);
            yield return StartCoroutine(lightningExplosion.Perform());
        }



        private void OpenShield()
        {
            animator.SetTrigger("Shield");
            isPerforming = true;

        }
        private IEnumerator Shield_performed()
        {
            isPerforming = false;

            yield return StartCoroutine(shield.Perform());
        }
    }
}
