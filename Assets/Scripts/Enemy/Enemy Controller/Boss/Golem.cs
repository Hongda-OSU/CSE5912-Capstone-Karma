using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Golem : EliteEnemy
    {
        protected override void PerformActions()
        {
            /*
            if (playerDetected && !isPlayingAttackAnim)
                FaceTarget(directionToPlayer);
            */
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
                    //isPlayingAttackAnim = false;                   

                    if (!isAttacking)
                    {
                        if (isPlayerInAttackRange && distanceToPlayer > closeDetectionRange)
                        {
                            Attack(1);                          
                        }
                        else if (distanceToPlayer <= closeDetectionRange)
                        {
                            Attack(2);
                        }
                        else 
                        {
                            MoveToPlayer();
                        }
                    }

                    break;

                case Status.Attacking:
                    //isPlayingAttackAnim = true;

                    if (isFatigued)
                    {
                        PrepareForNextAttack();
                    }
                    else
                    {
                        status = Status.Moving;
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
            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        /*
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
        }
        */
    }
}
