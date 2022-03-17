using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ArcherGoblin : EliteEnemy
    {
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
                            Attack(0);
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

            float randomWaitTime = Random.Range(timeBetweenWaitActions.x, timeBetweenWaitActions.y);
            yield return new WaitForSeconds(randomWaitTime);
        }

        private void Shoot()
        {

        }
    }
}