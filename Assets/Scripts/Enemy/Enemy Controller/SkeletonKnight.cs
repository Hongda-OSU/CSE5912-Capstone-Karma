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
                            Attack(Random.Range(0, 2));
                        }

                        else
                        {
                            MoveToPlayer();
                        }
                    }
                    break;

                case Status.Retreating:
                    if (!isRetreatFinished)
                        break;

                    else
                    {
                        if (currentAttackNum >= maxContinuousAttackNum)
                            PrepareForNextAttack();
                        else
                            MoveToPlayer();
                    }

                    break;
                case Status.Attacking:
                    if (currentAttackNum >= maxContinuousAttackNum)
                    {
                        Retreat();
                    }
                    else
                        status = Status.Moving;

                    break;

                case Status.Wait:
                    if (isReadyToAttack)
                    {
                        Rest();
                    }

                    break;
            }
        }


        protected override IEnumerator PerformActionsOnWaiting()
        {
            StartCoroutine(CoolDown(timeBetweenAttack));

            while (!isReadyToAttack)
                yield return StartCoroutine(RandomAction());

            SetRoll(Direction.None);

            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            currentAttackNum = 0;
        }

        private IEnumerator RandomAction()
        {
            float randomWaitTime = Random.Range(timeBetweenWaitActions.x, timeBetweenWaitActions.y);

            yield return new WaitForSeconds(randomWaitTime);

            waitAction = Random.Range(-2, 4);

            SetMove((Direction)waitAction);
            bool roll = Random.value < 0.5f;
            if (roll)
                SetRoll((Direction)waitAction);
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
