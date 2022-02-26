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
            Debug.Log(status);
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
