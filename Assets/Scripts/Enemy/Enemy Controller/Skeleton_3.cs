using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class Skeleton_3 : Enemy
    {
        private bool isAttacking = false;

        protected override void Update()
        {
            distance = Vector3.Distance(target.position, transform.position);
            directionToTarget = (target.position - transform.position).normalized;

            if (hp <= 0)
            {
                HandleDeath();
                return;
            }

            if ((distance <= viewRadius && Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2) || distance <= closeDetectionRange)
            {
                agent.isStopped = false;
                animator.SetBool("FoundPlayer", true);

                FaceTarget(directionToTarget);
                agent.SetDestination(target.position);
                agent.speed = 3f;

                ResetAttackAnimationTriggers();

                if (distance < agent.stoppingDistance + 0.3 || isAttacking)
                {
                    // Inside attacking range, attack player.
                    isAttacking = true;
                    agent.isStopped = true;
                    animator.SetBool("InAttackRange", true);
                    animator.SetBool("AttackFinished", false);
                    AttackPlayerRandomly();
                    if ((animator.GetCurrentAnimatorStateInfo(0).IsName("2Hand-Axe-Attack3") ||
                        animator.GetCurrentAnimatorStateInfo(0).IsName("2Hand-Axe-Attack1")) &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        isAttacking = false;
                        animator.SetBool("AttackFinished", true);
                    }
                }
                else
                {
                    // Outside attacking range.
                    animator.SetBool("InAttackRange", false);
                }
            }
            else
            {
                agent.isStopped = true;
                animator.SetBool("FoundPlayer", false);
            }
        }

        protected override void HandleDeath()
        {
            if (!isPlayingDeathAnimation)
            {
                PlayDeathAnimation();
                isPlayingDeathAnimation = true;
            }

            agent.isStopped = true;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("2Hand-Axe-Death1") &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                Destroy(gameObject);
            }
        }

        private void PlayDeathAnimation()
        {
            float random = Random.value;

            animator.SetTrigger("Die_1");
        }

        private void ResetAttackAnimationTriggers()
        {
            animator.ResetTrigger("Attack_1");
            animator.ResetTrigger("Attack_2");
        }

        private void AttackPlayerRandomly()
        {
            float random = Random.value;

            if (random >= 0f && random < 0.5f)
            {
                animator.SetTrigger("Attack_1");
            }
            else if (random >= 0.5f && random < 1f)
            {
                animator.SetTrigger("Attack_2");
            }
        }
    }
}