using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class EvilGod : EliteEnemy
    {
        [Header("Evil God")]
        [Header("Portal")]
        [SerializeField] private GameObject portalPrefab;
        [SerializeField] private Transform portalPivot;

        private LightningMissile_evilGod lightningMissile;
        private LightningExplosion_evilGod lightningExplosion;
        private Shield_evilGod shield;


        private bool isPerforming = false;

        private void Awake()
        {
            lightningMissile = GetComponentInChildren<LightningMissile_evilGod>();
            lightningExplosion = GetComponentInChildren<LightningExplosion_evilGod>();
            shield = GetComponentInChildren<Shield_evilGod>();
        }

        protected override void Start()
        {
            base.Start();

            animator.applyRootMotion = false;
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
                        StartCoroutine(Blink(transform.position + directionToPlayer * -attackRange));
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

        protected override void MoveToPlayer()
        {
            base.MoveToPlayer();
            StartCoroutine(Blink(PlayerManager.Instance.Player.transform.position - directionToPlayer * attackRange * 0.8f));
        }
        protected override IEnumerator PerformActionsOnWaiting()
        {
            Vector3 position = PlayerManager.Instance.Player.transform.position + directionToPlayer * attackRange;
            yield return StartCoroutine(Blink(position));
            currentAttackNum = 0;
        }

        protected override void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
        }

        private IEnumerator Blink(Vector3 position)
        {
            isPerforming = true;

            GameObject portals = new GameObject("Portals");

            var origin = Instantiate(portalPrefab, portals.transform);
            origin.transform.position = portalPivot.position;
            Destroy(origin, 5f);

            transform.position = position;

            yield return new WaitForSeconds(Time.deltaTime);

            var target = Instantiate(portalPrefab, portals.transform);
            target.transform.position = portalPivot.position;
            Destroy(target, 5f);

            Destroy(portals, 5f);

            isPerforming = false;
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
            //currentAttackNum++;
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
