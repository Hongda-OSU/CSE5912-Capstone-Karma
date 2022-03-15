using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Golem : EliteEnemy
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
                    if (!isAttacking)
                    {
                        if (isPlayerInAttackRange && distanceToPlayer > closeDetectionRange)
                        {
                            int attackNum = Random.Range(1, 3);
                            Attack(attackNum);
                            isPlayingAttackAnim = true;
                        }
                        else if (distanceToPlayer <= closeDetectionRange)
                        {
                            Attack(3);
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
                        
                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Attack_1") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Attack_2")) {
                            FaceTarget(directionToPlayer);
                        }
                        

                        if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_Stomp_1") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_Stomp_2") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_1") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Attack_1") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Attack_2")))
                        {
                            isPlayingAttackAnim = false;
                            animator.ResetTrigger("Attack_1");
                            animator.ResetTrigger("Attack_2");
                            animator.ResetTrigger("Attack_3");
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
