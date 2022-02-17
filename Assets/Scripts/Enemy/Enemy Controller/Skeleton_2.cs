using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class Skeleton_2 : MonoBehaviour, IEnemy
    {
        private float viewRadius = 15f;
        private float closeDetectionDistance = 3f;
        [Range(0, 360)]
        private float viewAngle = 120f;
        private bool foundTarget = false; // This is used for testing

        private float distance;
        private Vector3 directionToTarget;

        private Transform target;
        private NavMeshAgent agent;
        private Animator animator;

        private bool isPlayingDeathAnimation = false;

        [SerializeField] protected float HP = 100f;

        void Start()
        {
            target = PlayerManager.instance.player.transform;
            agent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
            animator.applyRootMotion = false;
            agent.speed = 2f;
        }

        void Update()
        {
            distance = Vector3.Distance(target.position, transform.position);
            directionToTarget = (target.position - transform.position).normalized;

            if (HP <= 0)
            {
                HandleDeath();
                return;
            }

            if ((distance <= viewRadius && Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2) || distance <= closeDetectionDistance)
            {
                foundTarget = true;
                agent.isStopped = false;
                animator.SetBool("Run", true);
                agent.speed = 7f;

                FaceTarget(directionToTarget);

                if (distance < agent.stoppingDistance + 0.3)
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
                        agent.Move(-1f * directionToTarget * Time.deltaTime);
                        if (distance >= agent.stoppingDistance)
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
                    agent.SetDestination(target.position);
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

        private void HandleDeath()
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

        private void FaceTarget(Vector3 direction)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        public void TakeDamage(float amount)
        {
            HP -= amount;
        }

        public float GetHP()
        {
            return HP;
        }

        // These codes below are used by Eiditor for testing purpose.
        public Vector3 GetTargetPosition()
        {
            return target.position;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public float GetViewAngle()
        {
            return viewAngle;
        }

        public float GetViewRadius()
        {
            return viewRadius;
        }

        public float GetCloseDetectionDistance()
        {
            return closeDetectionDistance;
        }

        public bool FoundTarget()
        {
            return foundTarget;
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}