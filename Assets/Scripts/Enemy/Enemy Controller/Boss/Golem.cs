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
                            isPlayingAttackAnim = true;
                        }
                        else if (distanceToPlayer <= closeDetectionRange)
                        {
                            Attack(2);
                            isPlayingAttackAnim = true;
                        }
                        else 
                        {
                            MoveToPlayer();
                        }
                    }

                    break;

                case Status.Attacking:                   
                    if (isPlayingAttackAnim)
                    {
                        if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_Stomp_1") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_Stomp_2") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_1")))
                        {
                            isPlayingAttackAnim = false;
                            animator.ResetTrigger("Attack_1");
                            animator.ResetTrigger("Attack_2");
                        }
                    }
                    else 
                    {
                        if (isFatigued)
                        {
                            PrepareForNextAttack();
                        }
                        else
                        {
                            status = Status.Moving;
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
