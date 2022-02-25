using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class SkeletonKnight : EliteEnemy
    {

        protected override void PerformActions()
        {
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

                case Status.Retreating:
                    if (aggro >= aggroThreshold || !isPlayerInAttackRange)
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
                    if (timeSinceAttack < timeBetweenAttack)
                    {
                        PrepareForNextAttack();

                        timeSinceAttack += Time.deltaTime;
                    }
                    else
                    {
                        Rest();
                        timeSinceAttack = 0f;
                    }

                    break;
            }
        }

        protected override void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
        }
    }
}
