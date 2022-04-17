using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class SkeletonWarrior : RegularEnemy
    {
        private bool isAttacking = false;


        protected override void PerformActions()
        {

            if ((distanceToPlayer <= viewRadius && Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
                || distanceToPlayer <= closeDetectionRange || isAttackedByPlayer || isAggroOn)
            {
                agent.isStopped = false;
                animator.SetBool("FoundPlayer", true);

                FaceTarget(directionToPlayer);
                agent.SetDestination(player.position);

                ResetAttackAnimationTriggers();

                if (distanceToPlayer < agent.stoppingDistance + 0.3 || isAttacking)
                {
                    // Inside attacking range, attack player.
                    isAttacking = true;
                    agent.isStopped = true;
                    animator.SetBool("InAttackRange", true);
                    animator.SetBool("AttackFinished", false);
                    AttackPlayerRandomly();
                    if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3") ||
                        animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 4")) &&
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

        protected override void Hit()
        {
            float damageAmount;
            if (distanceToPlayer <= attackRange)
            {
                damageAmount = attackDamage + Mathf.RoundToInt(Random.Range(-5f, 2f));
                Damage damage = new Damage(damageAmount, Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 5f);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 5f);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 50f);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 4"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 50f);
                    }
                }
            }
        }

        void Kick()
        {
            if (distanceToPlayer <= attackRange)
            {
                Damage damage = new Damage(attackDamage + Mathf.RoundToInt(Random.Range(0f, 6f)), Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
            }
        }

        void HeavyHit()
        {
            if (distanceToPlayer <= attackRange)
            {
                Damage damage = new Damage(Mathf.RoundToInt(Random.Range(9f, 12f)), Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
            }
        }

        protected override void PlayDeathAnimation()
        {
            animator.SetTrigger("Die_1");

            transform.Find("Audio Sources").Find("Death").GetComponent<AudioSource>().Play();
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

        protected override void HandleWander()
        {

        }

        protected override void HandlePatrol()
        {

        }
    }
}