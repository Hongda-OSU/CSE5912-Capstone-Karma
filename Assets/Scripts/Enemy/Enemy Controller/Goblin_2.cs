using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Goblin_2 : RegularEnemy
    {
        private bool isAttacking = false;
        private bool isGuarding = false;

        private bool getToInitialPosition = false;

        int m_CurrentWaypointIndex;

        private void Awake()
        {
            enemyName = "Shield Goblin";
            health = 100f;
            maxHealth = 100f;
        }



        void Update()
        {
            if (health <= 0)
            {
                HandleDeath();
                return;
            }

            if (canPatrol) {
                HandlePatrol();
            }

            distanceToPlayer = Vector3.Distance(player.position, transform.position);
            directionToPlayer = (player.position - transform.position).normalized;

            if ((distanceToPlayer <= viewRadius && Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
                || distanceToPlayer <= closeDetectionRange || isAttackedByPlayer) { 
            
            }
        }

        public override void TakeDamage(Damage damage)
        {
            if (isGuarding)
            {
                health -= (damage.ResolvedValue / 2);
            }
            else 
            {
                health -= damage.ResolvedValue;
            }

            if (!isAttackedByPlayer)
            {
                isAttackedByPlayer = true;
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

            if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Shield-Death1") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("Shooting-Death1")) &&
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

        protected override void HandleWander()
        {
            
        }

        protected override void HandlePatrol()
        {
            if (!getToInitialPosition) {
                agent.SetDestination(waypoints[0].position);
                getToInitialPosition = true;
            }

            if (agent.remainingDistance < agent.stoppingDistance)
            {
                m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                agent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
        }
    }
}
