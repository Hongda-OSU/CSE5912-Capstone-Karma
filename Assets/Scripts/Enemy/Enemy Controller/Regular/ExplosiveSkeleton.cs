using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ExplosiveSkeleton : RegularEnemy
    {
        [Header("Explosion")]
        [SerializeField] private GameObject effect;
        [SerializeField] private GameObject barrel;

        bool exploded = false;

        protected override void PerformActions()
        {
            if ((distanceToPlayer <= viewRadius && Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
                   || distanceToPlayer <= closeDetectionRange || isAttackedByPlayer)
            {
                foundTarget = true;
                agent.isStopped = false;
                animator.SetBool("PlayerDetected", true);

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Sit-Idle")) 
                {
                    FaceTarget(directionToPlayer);

                    if (distanceToPlayer < closeDetectionRange)
                    {
                        if (!exploded) 
                        {
                            Explode();
                        }
                    }
                }

                if (distanceToPlayer < attackRange)
                {
                    // Inside attacking range, attack player.                
                    animator.SetBool("PlayerInAttackRange", true);
                    agent.speed = agentSpeed * 3f;
                }
                else
                {
                    // Outside attacking range.
                    animator.SetBool("PlayerInAttackRange", false);
                    agent.SetDestination(player.position);
                    agent.speed = agentSpeed;
                }
            }
            else
            {
                foundTarget = false;
                agent.isStopped = true;
                animator.SetBool("Run", false);
            }
        }

        protected override void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
        }

        protected override void HandlePatrol()
        {
            // Do Nothing
        }

        protected override void HandleWander()
        {
            // Do Nothing
        }

        private void Explode() {
            GameObject vfx = Instantiate(effect);
            vfx.transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);

            if (DistanceToPlayer < 5f) 
            {
                Damage damage = new Damage(attackDamage, Element.Type.Fire, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
            }
            health = 0;
            Die();

            Destroy(barrel);
            Destroy(vfx, 10f);
            exploded = true;
        }
    }
}
