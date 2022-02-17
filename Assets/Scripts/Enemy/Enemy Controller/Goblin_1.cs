using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class Goblin_1 : MonoBehaviour, IEnemy
    {
        private float viewRadius = 15f;
        private float closeDetectionDistance = 3f;
        [Range(0, 360)]
        private float viewAngle = 135f;
        private bool foundTarget = false; // This is used for testing

        private float distance;
        private Vector3 directionToTarget;

        private Transform target;
        private NavMeshAgent agent;
        private Animator animator;

        private bool isPlayingDeathAnimation = false;
        private bool isMovingBack = false;
        private bool isMovingAroundPlayer = false;
        private bool isAttacking = false;

        private bool readyToAttack = false;
        private float attackCoolDown = 5f;

        [SerializeField] protected float hp = 100f;
        [SerializeField] protected float maxHp = 100f;

        void Start()
        {
            target = PlayerManager.Instance.Player.transform;
            agent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
            animator.applyRootMotion = false;
            agent.speed = 1.5f;
        }

        void Update()
        {

            distance = Vector3.Distance(target.position, transform.position);
            directionToTarget = (target.position - transform.position).normalized;

            Debug.DrawRay(transform.position, directionToTarget, Color.red);

            if (hp <= 0)
            {
                HandleDeath();
                return;
            }

            if ((distance <= viewRadius && Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2) ||
                distance <= closeDetectionDistance)
            {
                foundTarget = true;
                agent.isStopped = false;
                animator.SetBool("FoundPlayer", true);

                FaceTarget(directionToTarget);
                agent.SetDestination(target.position);
                agent.speed = 3f;

                if (distance < agent.stoppingDistance + 0.1f)
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
                        if (distance < agent.stoppingDistance)
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

        private void HandleDeath()
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
                if (distance > 2.5f)
                {
                    agent.Move(4f * directionToTarget * Time.deltaTime);
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
                        agent.Move(1f * Tangent(directionToTarget) * Time.deltaTime);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("2Hand-Spear-Strafe-Right"))
                    {
                        agent.Move(-1f * Tangent(directionToTarget) * Time.deltaTime);
                    }
                }
                else
                {
                    agent.Move(-2f * directionToTarget * Time.deltaTime);
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

        private void FaceTarget(Vector3 direction)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        public void TakeDamage(float amount)
        {
            hp -= amount;
            //Debug.Log("Goblin hit");
        }

        public float GetHealth()
        {
            return hp;
        }

        public float GetMaxHealth()
        {
            return maxHp;
        }
        void Hit()
        {

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