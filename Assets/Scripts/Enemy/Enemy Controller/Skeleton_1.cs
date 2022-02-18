using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class Skeleton_1 : Enemy
    {
        private bool isAttacking = false;

        private bool wandering = false;
        private float wanderCounter = 10f;

        private void Awake()
        {
            enemyName = "Skeleton";
            hp = 100f;
            maxHp = 100f;
        }

        void Update()
        {
            distance = Vector3.Distance(target.position, transform.position);
            directionToTarget = (target.position - transform.position).normalized;
         
            if (hp <= 0)
            {
                HandleDeath();
                return;
            }

            if (canWander) {
                HandleWander();
            }
            

            if ((distance <= viewRadius && Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2) 
                || distance <= closeDetectionRange || isAttackedByPlayer)
            {
                foundTarget = true;
                agent.isStopped = false;
                agent.speed = 3f;
                agent.stoppingDistance = 2f;
                animator.SetBool("FoundPlayer", true);

                FaceTarget(directionToTarget);
                agent.SetDestination(target.position);

                ResetAttackAnimationTriggers();

                if (distance < agent.stoppingDistance + 0.3)
                {
                    // Inside attacking range, attack player.
                    animator.SetBool("InAttackRange", true);
                    AttackPlayerRandomly();
                    isAttacking = true;
                }
                else
                {
                    // Outside attacking range.
                    animator.SetBool("InAttackRange", false);
                }

                if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Mace-Attack-L1") ||
                    animator.GetCurrentAnimatorStateInfo(0).IsName("Dagger-Attack-L1") ||
                    animator.GetCurrentAnimatorStateInfo(0).IsName("Mace-Attack-R1") ||
                    animator.GetCurrentAnimatorStateInfo(0).IsName("Item-Attack-R2")))
                {
                    isAttacking = false;
                }

                if (isAttacking)
                {
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                }
            }
            else
            {
                foundTarget = false;
                animator.SetBool("FoundPlayer", false);
                if (!canWander) {
                    agent.isStopped = true;
                }
            }     
        }

        protected override void HandleWander() {
            if (!foundTarget)
            {
                agent.isStopped = false;
                agent.speed = 1f;
                agent.stoppingDistance = 0f;

                if (!agent.hasPath)
                {
                    agent.SetDestination(GetPoint.Instance.GetRandomPoint());
                }

                if (Vector3.Distance(agent.destination, transform.position) <= 0.2f) {
                    animator.SetBool("Wander", false);
                }
                else 
                {
                    animator.SetBool("Wander", true);
                }
            }
            else
            {
                animator.SetBool("Wander", false);
                animator.SetBool("FoundPlayer", true);
                canWander = false;
            }
        }

        protected override void HandlePatrol()
        {

        }

        protected override void HandleDeath()
        {
            if (!isPlayingDeathAnimation)
            {
                PlayDeathAnimation();
                isPlayingDeathAnimation = true;
            }

            agent.isStopped = true;
            
            if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Armed-Death1") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-Death1")) &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                Destroy(gameObject);
            }
            
        }

        private void PlayDeathAnimation()
        {
            float random = Random.value;

            if (random >= 0f && random < 0.5f)
            {
                animator.SetTrigger("Die_1");
            }
            else if (random >= 0.5f && random < 1f)
            {
                animator.SetTrigger("Die_2");
            }
        }

        private void ResetAttackAnimationTriggers()
        {
            animator.ResetTrigger("Attack_1");
            animator.ResetTrigger("Attack_2");
            animator.ResetTrigger("Attack_3");
            animator.ResetTrigger("Attack_4");
        }

        private void AttackPlayerRandomly()
        {
            float random = Random.value;

            if (random >= 0f && random < 0.25f)
            {
                animator.SetTrigger("Attack_1");
            }
            else if (random >= 0.25f && random < 0.5f)
            {
                animator.SetTrigger("Attack_2");
            }
            else if (random >= 0.5f && random < 0.75f)
            {
                animator.SetTrigger("Attack_3");
            }
            else if (random >= 0.75f && random < 1f)
            {
                animator.SetTrigger("Attack_4");
            }
        }
    }
}