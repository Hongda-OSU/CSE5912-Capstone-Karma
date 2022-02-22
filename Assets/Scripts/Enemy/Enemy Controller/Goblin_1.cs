using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class Goblin_1 : RegularEnemy
    {
        private bool isMovingBack = false;
        private bool isMovingAroundPlayer = false;
        private bool isAttacking = false;

        private bool readyToAttack = false;
        private float attackCoolDown = 5f;

        void Update()
        {
            distanceToPlayer = Vector3.Distance(player.position, transform.position);
            directionToPlayer = (player.position - transform.position).normalized;

            Debug.DrawRay(transform.position, directionToPlayer, Color.red);

            if (health <= 0)
            {
                HandleDeath();
                return;
            }

            if ((distanceToPlayer <= viewRadius && Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2) ||
                distanceToPlayer <= closeDetectionRange || isAttackedByPlayer)
            {
                foundTarget = true;
                agent.isStopped = false;
                animator.SetBool("FoundPlayer", true);

                FaceTarget(directionToPlayer);
                agent.SetDestination(player.position);
                agent.speed = 3f;

                if (distanceToPlayer < agent.stoppingDistance + 0.1f)
                {
                    // Inside attacking range, attack player.
                    agent.isStopped = true;
                    animator.SetBool("InAttackRange", true);

                    //Need further changes
                    if (!isMovingBack)
                    {
                        SetReadyToAttack();
                    }

                    if (!readyToAttack)
                    {
                        Move();
                        animator.SetBool("ReadyToAttack", false);
                    }
                    else
                    {
                        animator.SetBool("ReadyToAttack", true);
                        isAttacking = true;
                    }

                    if (isAttacking)
                    {
                        AttackPlayerRandomly();
                    }
                    else
                    {
                        if (distanceToPlayer < agent.stoppingDistance)
                        {
                            animator.SetBool("MoveBack", true);
                            isMovingBack = true;
                        }
                        else
                        {
                            animator.SetBool("MoveBack", false);
                            isMovingBack = false;
                        }

                        ResetAttackAnimationTriggers();
                    }

                    RandomlySetMoveLeftOrRightBoolean();
                }
                else
                {
                    // Outside attacking range.
                    animator.SetBool("InAttackRange", false);
                    isMovingAroundPlayer = false;
                    ResetMoveLeftOrRightTrigger();
                }
            }
            else
            {
                foundTarget = false;
                agent.isStopped = true;
                animator.SetBool("FoundPlayer", false);
            }
        }

        protected override void Hit()
        {
            float damageAmount;
            if (distanceToPlayer <= attackRange)
            {
                damageAmount = attackDamage + Mathf.RoundToInt(Random.Range(-3f, 3f));
                Damage damage = new Damage(damageAmount, Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
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

            if ((animator.GetCurrentAnimatorStateInfo(0).IsName("2Hand-Spear-Death1") ||
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

        private void SetReadyToAttack()
        {
            float random = Random.value;

            if (attackCoolDown <= 0)
            {
                if (random >= 0f && random < 0.05f)
                {
                    readyToAttack = true;
                    attackCoolDown = 5f;
                }
            }
            else
            {
                readyToAttack = false;
                attackCoolDown -= Time.deltaTime;
            }
        }

        private void RandomlySetMoveLeftOrRightBoolean()
        {
            float random = Random.value;

            if (random >= 0f && random < 0.5f)
            {
                animator.SetBool("MoveLeft", true);
            }
            else if (random >= 0.5 && random < 1f)
            {
                animator.SetBool("MoveLeft", false);
            }
        }

        private void ResetMoveLeftOrRightTrigger()
        {
            animator.ResetTrigger("MoveLeft");
            animator.ResetTrigger("MoveRight");
        }

        private void Move()
        {
            if (isAttacking)
            {
                if (distanceToPlayer > 2.5f)
                {
                    agent.Move(4f * directionToPlayer * Time.deltaTime);
                }

                if ((animator.GetCurrentAnimatorStateInfo(0).IsName("2Hand-Spear-Attack9") ||
                         animator.GetCurrentAnimatorStateInfo(0).IsName("2Hand-Spear-Attack1") ||
                         animator.GetCurrentAnimatorStateInfo(0).IsName("Spear-Attack-R4")) &&
                         animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    isAttacking = false;
                    isMovingBack = true;
                    animator.SetBool("MoveBack", true);
                }
            }
            else
            {
                if (!isMovingBack)
                {
                    if (!isMovingAroundPlayer)
                    {
                        RandomlySetMoveLeftOrRightBoolean();
                        isMovingAroundPlayer = true;
                    }

                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("2Hand-Spear-Strafe-Left"))
                    {
                        agent.Move(1f * Tangent(directionToPlayer) * Time.deltaTime);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("2Hand-Spear-Strafe-Right"))
                    {
                        agent.Move(-1f * Tangent(directionToPlayer) * Time.deltaTime);
                    }
                }
                else
                {
                    agent.Move(-2f * directionToPlayer * Time.deltaTime);
                }
            }
        }

        private Vector3 Tangent(Vector3 direction)
        {
            Vector3 tangent = Vector3.Cross(direction, Vector3.up);

            if (tangent.magnitude == 0)
            {
                tangent = Vector3.Cross(direction, Vector3.right);
            }

            return tangent;
        }

        private void ResetAttackAnimationTriggers()
        {
            animator.ResetTrigger("Attack_1");
            animator.ResetTrigger("Attack_2");
            animator.ResetTrigger("Attack_3");
        }

        private void AttackPlayerRandomly()
        {
            float random = Random.value;

            if (random >= 0f && random < 0.33f)
            {
                animator.SetTrigger("Attack_1");
            }
            else if (random >= 0.33f && random < 0.67f)
            {
                animator.SetTrigger("Attack_2");
            }
            else if (random >= 0.67f && random < 1f)
            {
                animator.SetTrigger("Attack_3");
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