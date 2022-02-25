using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class SkeletonSlave : RegularEnemy
    {
        protected override void Update()
        {
            if (!isAlive)
                return;

            base.Update();

            if ((distanceToPlayer <= viewRadius && Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
                || distanceToPlayer <= closeDetectionRange || isAttackedByPlayer)
            {
                foundTarget = true;
                agent.isStopped = false;
                animator.SetBool("Run", true);
                agent.speed = 7f;

                FaceTarget(directionToPlayer);

                if (distanceToPlayer < agent.stoppingDistance + 0.3)
                {
                    // Inside attacking range, attack player.                
                    animator.SetBool("InAttackRange", true);

                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Backward"))
                    {
                        agent.isStopped = true;
                        AttackPlayerRandomly();
                        animator.SetBool("AttackFinished", true);
                        agent.stoppingDistance = Random.Range(7f, 10f);
                    }
                    else
                    {
                        agent.Move(-1f * directionToPlayer * Time.deltaTime);
                        if (distanceToPlayer >= agent.stoppingDistance)
                        {
                            animator.SetBool("AttackFinished", false);
                            animator.SetBool("InAttackRange", false);
                            agent.stoppingDistance = 2f;
                            ResetAttackAnimationTriggers();
                        }
                    }
                }
                else
                {
                    // Outside attacking range.
                    animator.SetBool("InAttackRange", false);
                    agent.SetDestination(player.position);
                }
            }
            else
            {
                foundTarget = false;
                agent.isStopped = true;
                animator.SetBool("Run", false);
                agent.speed = 2f;
            }
        }

        protected override void Hit()
        {
            float damageAmount;
            if (distanceToPlayer <= attackRange)
            {
                damageAmount = attackDamage + Mathf.RoundToInt(Random.Range(-3f, 2f));
                Damage damage = new Damage(damageAmount, Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
            }
        }

        void Scratch() {
            if (distanceToPlayer <= attackRange)
            {
                Damage damage = new Damage(Mathf.RoundToInt(Random.Range(8f, 10f)), Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
            }
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

        protected override void HandleWander()
        {

        }

        protected override void HandlePatrol()
        {

        }
    }
}