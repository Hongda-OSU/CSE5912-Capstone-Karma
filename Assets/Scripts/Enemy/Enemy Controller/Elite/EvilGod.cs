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

        [Header("Lightning Explosion")]
        [SerializeField] private GameObject lightningExplosionPrefab;
        [SerializeField] private int lightningExplosionNumber = 20;
        [SerializeField] private float lightningExplosionSpacing = 2f;
        [SerializeField] private float lightningExplosionInterval = 0.1f;
        [SerializeField] private float lightningExplosionCooldown = 5f;
        private bool isLightningExplosionReady = true;

        [Header("Shield")]
        [SerializeField] private GameObject shieldPrefab;
        private bool isShieldOn = false;
        private bool isShieldReady = true;

        private bool isPerforming = false;

        protected override void Start()
        {
            base.Start();

            animator.applyRootMotion = false;
        }

        protected override void PerformActions()
        {
            if (isPerforming)
                return;


            if (health < maxHealth * 0.5f && isShieldReady)
            {
                OpenShield();
                isShieldReady = false;
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
                        StartCoroutine(Blink(transform.position + directionToPlayer * -attackRange, 1f));
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
            StartCoroutine(Blink(PlayerManager.Instance.Player.transform.position - directionToPlayer * attackRange * 0.8f, 0.3f));
        }
        protected override IEnumerator PerformActionsOnWaiting()
        {
            Vector3 position = PlayerManager.Instance.Player.transform.position + directionToPlayer * attackRange;
            yield return StartCoroutine(Blink(position, 1f));
            currentAttackNum = 0;
        }

        protected override void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
        }

        private IEnumerator Blink(Vector3 position, float delay)
        {
            isPerforming = true;

            GameObject portals = new GameObject("Portals");

            var origin = Instantiate(portalPrefab, portals.transform);
            origin.transform.position = portalPivot.position;
            origin.transform.rotation = Quaternion.Euler(portalPrefab.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            Destroy(origin, delay + 1);

            yield return new WaitForSeconds(delay);

            transform.position = position;

            yield return new WaitForSeconds(Time.deltaTime);

            var target = Instantiate(portalPrefab, portals.transform);
            target.transform.position = portalPivot.position;
            target.transform.rotation = Quaternion.Euler(portalPrefab.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            Destroy(target, delay + 1);

            yield return new WaitForSeconds(delay);

            Destroy(portals, 5f);

            isPerforming = false;
        }

        private void Attack()
        {
            Attack_LightningExplosion();

            status = Status.Attacking;
        }

        private void Attack_LightningExplosion()
        {
            if (!isLightningExplosionReady)
                return;

            status = Status.Attacking;
            SetAttack(0);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator LightningExplosion_performed()
        {
            isPerforming = false;

            isLightningExplosionReady = false;

            var lightningExplosions = new GameObject("LightningExplosions");

            float offset = lightningExplosionSpacing;

            float damage = lightningExplosionPrefab.GetComponent<Damager_collision>().BaseDamage;

            for (int i = 0; i < lightningExplosionNumber; i++)
            {
                GameObject vfx = Instantiate(lightningExplosionPrefab, lightningExplosions.transform);

                vfx.GetComponent<Damager_collision>().Source = this;
                vfx.GetComponent<Damager_collision>().BaseDamage = damage;

                vfx.transform.position = transform.position + transform.forward * offset * (i + 1);

                Destroy(vfx, 5f);

                yield return new WaitForSeconds(lightningExplosionInterval);

                if (vfx.GetComponent<Damager_collision>().IsPlayerHit)
                {
                    damage = 0f;
                }
            }
            Destroy(lightningExplosions, 5f);

            yield return new WaitForSeconds(lightningExplosionCooldown);
            isLightningExplosionReady = true;
        }

        private void OpenShield()
        {
            if (!isShieldOn)
            {
                animator.SetTrigger("Shield");
                isPerforming = true;
            }
        }
        private IEnumerator Shield_performed()
        {
            if (isShieldOn)
                yield break;

            isShieldOn = true;
            isPerforming = false;

            GameObject energyShield = Instantiate(shieldPrefab, transform);
            Shield shield = energyShield.GetComponent<Shield>();

            while (isShieldOn)
            {
                yield return new WaitForSeconds(Time.deltaTime);

                if (shield.TotalHealth <= 0)
                {
                    isShieldOn = false;
                    Destroy(energyShield);
                }
            }

        }
    }
}
