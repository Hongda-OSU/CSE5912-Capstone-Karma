using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class SkeletonCreep : RegularEnemy
    {
        private bool isAttacking = false;

        private bool wandering = false;
        private float wanderCounter = 10f;

        protected override void PerformActions()
        {
            if (canWander)
            {
                HandleWander();
                return;
            }

            if ((distanceToPlayer <= viewRadius && Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
                || distanceToPlayer <= closeDetectionRange || isAttackedByPlayer)
            {
                foundTarget = true;
                agent.isStopped = false;
                agent.stoppingDistance = 2f;
                animator.SetBool("FoundPlayer", true);

                FaceTarget(directionToPlayer);
                agent.SetDestination(player.position);

                ResetAttackAnimationTriggers();

                if (distanceToPlayer < agent.stoppingDistance + 0.3)
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

                if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") ||
                      animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2") ||
                      animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3") ||
                      animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 4")))
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
                agent.speed = agentSpeed;
                animator.SetBool("FoundPlayer", false);
                if (!canWander)
                {
                    agent.isStopped = true;
                }
            }
        }


        protected override void Hit() {
            float damageAmount;           
            if (distanceToPlayer <= agent.stoppingDistance + 0.3) 
            {
                damageAmount = attackDamage + Mathf.RoundToInt(Random.Range(-1f, 2f));
                Damage damage = new Damage(damageAmount, Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 15f);
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


        protected override void PlayDeathAnimation()
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