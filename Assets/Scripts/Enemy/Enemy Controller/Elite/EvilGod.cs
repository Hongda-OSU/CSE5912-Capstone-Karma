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

        [Header("Lightning")]
        [SerializeField] private GameObject lightningExplosionPrefab;
        [SerializeField] private int lightningExplosionNumber = 20;
        [SerializeField] private float lightningExplosionSpacing = 1f;
        [SerializeField] private float lightningExplosionInterval = 0.1f;

        [Header("Shield")]
        [SerializeField] private GameObject shieldPrefab;
        private bool isShieldOn = false;

        private bool isPerforming = false;


        protected override void PerformActions()
        {
            if (isPerforming)
                return;

            if (playerDetected)
                FaceTarget(directionToPlayer);

            switch (status)
            {
                case Status.Idle:
                    if (playerDetected)
                    {
                        MoveToPlayer();
                    }
                    else
                    {
                        Rest();
                    }
                    break;

                case Status.Moving:


                    if (!isAttacking)
                    {
                        if (isPlayerInAttackRange)
                        {
                            //OpenShield();
                            Attack_LightningExplosion();
                            //StartCoroutine(Blink(transform.position + directionToPlayer * -5f, 1f));
                        }
                        else
                        {
                            MoveToPlayer();
                        }
                    }
                    break;

                case Status.Attacking:

                    break;

                case Status.Retreating:

                    break;

                case Status.Waiting:
                    break;
            }
        }


        protected override IEnumerator PerformActionsOnWaiting()
        {
            StartCoroutine(CoolDown(timeBetweenAttack));

            while (!isReadyToAttack)
            {
                yield return StartCoroutine(RandomAction());
            }

            SetRoll(Direction.None);

            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            currentAttackNum = 0;
            Debug.Log(currentAttackNum);
        }

        private IEnumerator RandomAction()
        {
            waitAction = Random.Range(-2, 4);

            bool roll = Random.value < 0.5f;
            if (roll)
                SetRoll((Direction)waitAction);

            yield return new WaitForSeconds(Time.deltaTime);

            SetMove((Direction)waitAction);
            SetRoll(Direction.None);

            float randomWaitTime = Random.Range(timeBetweenWaitActions.x, timeBetweenWaitActions.y);
            yield return new WaitForSeconds(randomWaitTime);
        }


        protected override void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
        }

        private IEnumerator Blink(Vector3 position, float delay)
        {
            isPerforming = true;

            GameObject portals = new GameObject("Portals");

            Rest();
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

        private void Attack_LightningExplosion()
        {
            SetAttack(0);
            isPerforming = true;
        }
        private IEnumerator LightningExplosion_performed()
        {
            isPerforming = false;
            agent.speed = 0f;

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

            agent.speed = agentSpeed;
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
        //protected override void StartAttack()
        //{
        //    base.StartAttack();

        //    StartCoroutine(DisplayVfx());
        //}
        //private IEnumerator DisplayVfx()
        //{
        //    Vector3 forward = transform.forward;

        //    GameObject vfx = Instantiate(fireBlade);
        //    vfx.transform.position = transform.position + Vector3.up * 2;
        //    vfx.transform.LookAt(forward);
        //    vfx.transform.localScale = Vector3.one * vfxScale;

        //    float timeSince = 0f;
        //    while (timeSince < 10f)
        //    {
        //        timeSince += Time.deltaTime;
        //        yield return new WaitForSeconds(Time.deltaTime);

        //        vfx.transform.position += forward * vfxSpeed * Time.deltaTime;
        //    }
        //}
    }
}
