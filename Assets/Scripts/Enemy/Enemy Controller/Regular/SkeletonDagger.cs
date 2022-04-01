using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class SkeletonDagger : RegularEnemy
    {
        private bool isAttacking = false;
        private bool getToInitialPosition = false;
        private float waitTime = 2f;
        private float patrolCounter = 0f;
        private bool patrolling = false;
        int m_CurrentWaypointIndex;

        protected override void PerformActions()
        {
            if (canPatrol)
            {
                HandlePatrol();
            }
            distanceToPlayer = Vector3.Distance(player.position, transform.position);
            directionToPlayer = (player.position - transform.position).normalized;

            if ((distanceToPlayer <= viewRadius && Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
                || distanceToPlayer <= closeDetectionRange || isAttackedByPlayer)
            {

                foundTarget = true;
                agent.isStopped = false;
                animator.SetBool("FoundPlayer", true);

                FaceTarget(directionToPlayer);
                agent.SetDestination(player.position);
                agent.stoppingDistance = 2f;

                ResetAttackAnimationTriggers();

                if (distanceToPlayer < agent.stoppingDistance + 0.3)
                {
                    animator.SetBool("InAttackRange", true);
                    AttackPlayerRandomly();
                    isAttacking = true;
                }
                else
                {
                    animator.SetBool("InAttackRange", false);
                }
                if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") ||
                      animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2")||
                      animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3")))
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
                if (!canPatrol)
                {
                    agent.isStopped = true;
                }
            }
        }

        protected override void Hit()
        {
            float damageAmount;
            if (distanceToPlayer <= agent.stoppingDistance + 0.3)
            {
                damageAmount = attackDamage + Mathf.RoundToInt(Random.Range(-2f, 4f));
                Damage damage = new Damage(damageAmount, Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 3f);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 20f);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 30f);
                    }
                }
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

        private void ResetAttackAnimationTriggers()
        {
            animator.ResetTrigger("Attack_1");
            animator.ResetTrigger("Attack_2");
        }

        private void AttackPlayerRandomly()
        {
            float random = Random.value;

            if (random >= 0f && random < 0.7f)
            {
                animator.SetTrigger("Attack_1");
            }
            else if (random >= 0.7f && random < 1f)
            {
                animator.SetTrigger("Attack_2");
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

        protected override void HandleWander()
        {

        }

        protected override void HandlePatrol()
        {
            if (!foundTarget)
            {
                if (!getToInitialPosition)
                {
                    agent.SetDestination(waypoints[0].position);
                    agent.isStopped = false;
                    agent.stoppingDistance = 0f;
                    patrolling = true;

                    if (Vector3.Distance(waypoints[0].position, transform.position) <= 0.1f)
                    {
                        getToInitialPosition = true;
                        patrolling = false;
                    }
                }
                else
                {
                    if (patrolCounter <= 0f && !patrolling)
                    {
                        agent.isStopped = false;
                        patrolling = true;
                        patrolCounter = waitTime;
                    }
                    if (patrolCounter > 0f && !patrolling)
                    {
                        agent.isStopped = true;
                        patrolCounter -= 1f * Time.deltaTime;
                    }
                    if (patrolling)
                    {
                        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
                        {
                            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                            agent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                            patrolling = false;
                        }
                    }
                }

                if (patrolling)
                {
                    animator.SetBool("Patrol", true);
                }
                else
                {
                    animator.SetBool("Patrol", false);
                }
            }
            else
            {
                animator.SetBool("Patrol", false);
                animator.SetBool("FoundPlayer", true);
                canPatrol = false;
            }
        }
    }
}
