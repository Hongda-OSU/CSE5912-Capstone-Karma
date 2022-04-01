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

                FaceTarget(directionToPlayer);

                if (distanceToPlayer < attackRange)
                {
                    // Inside attacking range, attack player.                
                    animator.SetBool("PlayerInAttackRange", true);
                    agent.speed = agentSpeed * 4f;
                }
                else
                {
                    // Outside attacking range.
                    animator.SetBool("PlayerInAttackRange", false);
                    agent.speed = agentSpeed;
                }

                agent.SetDestination(player.position);

                if (distanceToPlayer < closeDetectionRange)
                {
                    if (!exploded)
                    {
                        Explode();
                    }
                }
            }
            else
            {
                foundTarget = false;
                agent.isStopped = true;
                animator.SetBool("PlayerDetected", false);
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

            if (DistanceToPlayer < closeDetectionRange) 
            {
                Damage damage = new Damage(attackDamage, Element.Type.Fire, this, PlayerStats.Instance);
                FPSControllerCC.Instance.AddImpact(Vector3.up, 100f);
                FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 300f);
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
