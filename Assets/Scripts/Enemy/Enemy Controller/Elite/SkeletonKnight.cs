using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class SkeletonKnight : EliteEnemy
    {
        //[Header("Skeleton Knight")]
        //[SerializeField] private GameObject fireBlade;
        //[SerializeField] private float vfxSpeed = 10f;
        //[SerializeField] private float vfxScale = 1f;

        protected override void PerformActions()
        {
            if (playerDetected)
                FaceTarget(directionToPlayer);

            switch (status)
            {
                case Status.Idle:
                    if (playerDetected)
                    {
                        FaceTarget(directionToPlayer);
                        MoveToPlayer();
                    }
                    else
                    {
                        Rest();
                    }
                    break;

                case Status.Moving:

                    if (distanceToPlayer < closeDetectionRange)
                    {
                        Retreat();
                    }

                    if (!isAttacking)
                    {
                        if (isPlayerInAttackRange)
                        {
                            Attack(Random.Range(0, 1));
                        }
                        else
                        {
                            MoveToPlayer();
                        }
                    }
                    break;

                case Status.Attacking:
                    if (isFatigued)
                    {
                        PrepareForNextAttack();
                    }
                    else
                    {
                        status = Status.Moving;
                    }

                    break;

                case Status.Retreating:
                    if (!isRetreatFinished)
                        break;

                    else
                    {
                        if (isFatigued)
                        {
                            PrepareForNextAttack();
                        }
                        else
                        {
                            MoveToPlayer();
                        }
                    }

                    break;

                case Status.Waiting:
                    if (isReadyToAttack && !isFatigued)
                    {
                        MoveToPlayer();
                    }

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

            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            currentAttackNum = 0;
        }

        private IEnumerator RandomAction()
        {
            waitAction = Random.Range(-2, 4);

            bool roll = Random.value < 0.7f;
            if (roll)
                SetRoll((Direction)waitAction);

            yield return new WaitForSeconds(Time.deltaTime);

            SetMove((Direction)waitAction);

            float randomWaitTime = Random.Range(timeBetweenWaitActions.x, timeBetweenWaitActions.y);
            yield return new WaitForSeconds(randomWaitTime);
        }

        protected override void Hit()
        {
            float damageAmount;
            if (distanceToPlayer <= agent.stoppingDistance + 0.3)
            {
                damageAmount = attackDamage + Mathf.RoundToInt(Random.Range(-2f, 4f));
                Damage damage = new Damage(damageAmount, Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_1"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 30f);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_0"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 5f);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordAttack_2"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 40f);
                    }
                }
            }
        }

        private void StopAgent()
        {
            agent.speed = 0;
        }

        private void StartAgent()
        {
            agent.speed = agentSpeed;
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
