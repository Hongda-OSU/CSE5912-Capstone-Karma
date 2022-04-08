using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class DarkSlayer : EliteEnemy
    {
        public Vector3 Direction;
        private bool attackFinished;

        protected override void PerformActions()
        {

            if (playerDetected)
                FaceTarget(directionToPlayer);

            switch (status)
            {
                case Status.Idle:
                    if (playerDetected)
                        MoveToPlayer();
                    else
                        Rest();
                    break;

                case Status.Moving:
                    // 20
                    if (distanceToPlayer > attackRange + 5f)
                        MoveToPlayer();

                    if (!isAttacking)
                    {
                        isPlayingAttackAnim = true;
                        // 0~6
                        if (isPlayerInSafeDistance)
                        {
                            if (Random.value < 0.5f)
                                Attack(1);
                            else 
                                Attack(2);
                        }
                        // 8~15
                        else if (distanceToPlayer > closeDetectionRange + 2f && distanceToPlayer <= attackRange)
                        {
                            if (Random.value < 0.6f)
                                Attack(3);
                            else
                                Attack(4);
                        }
                        else
                        {
                            isPlayingAttackAnim = false;
                            MoveToPlayer();
                        }

                    }
                    break;

                case Status.Attacking:
                    if (isPlayingAttackAnim)
                    {
                        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") ||
                        //    animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2"))
                        //{
                        //    FaceTarget(directionToPlayer);
                        //}

                        if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") ||
                              animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2") ||
                              animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3") ||
                              animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 4")))
                        {
                            isPlayingAttackAnim = false;
                            animator.ResetTrigger("Attack_1");
                            animator.ResetTrigger("Attack_2");
                            animator.ResetTrigger("Attack_3");
                            animator.ResetTrigger("Attack_4");
                        }
                    }
                    else
                    {
                        if (isFatigued)
                        {
                            PrepareForNextAttack();
                        }
                        else
                        {
                            status = Status.Moving;
                        }
                    }
                    break;


                case Status.Waiting:
                    if (isReadyToAttack && !isFatigued)
                    {
                        MoveToPlayer();
                    }
                    break;
            }
        }

        private void StartStormAttack()
        {
            attackFinished = false;
            StartCoroutine(HammerStorm());
        }

        private void StopStormAttack()
        {
            attackFinished = true;
        }

        private IEnumerator HammerStorm()
        {
            while (!attackFinished)
            {
                Direction = transform.forward;
                Direction += (player.transform.position - transform.position);
                Direction.Normalize();
                transform.position += Direction * 4f * Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return null;
        }

        protected override IEnumerator PerformActionsOnWaiting()
        {
            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            yield return new WaitForSeconds(Time.deltaTime);
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