using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class BigGoblin : EliteEnemy
    {
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
                    if (distanceToPlayer > 15f)
                        MoveToPlayer();

                    if (!isAttacking)
                    {
                        isPlayingAttackAnim = true;
                        // 3~8
                        if (isPlayerInAttackRange)
                            Attack(1);
                        // 8~15
                        else if (distanceToPlayer <= 15f && distanceToPlayer > attackRange)
                            Attack(2);
                        else
                            isPlayingAttackAnim = false;
                    }

                    break;

                case Status.Attacking:
                    if (distanceToPlayer <= 20f && distanceToPlayer > 15f)
                    {
                        animator.SetTrigger("RollForward");
                    }

                    if (distanceToPlayer < closeDetectionRange)
                    {
                        if (Random.value < 0.5f)
                            animator.SetTrigger("DodgeLeft");
                        else
                            animator.SetTrigger("DodgeRight");
                    }

                    status = Status.Moving;
                    break;


                case Status.Waiting:
                    if (isReadyToAttack)
                    {
                        MoveToPlayer();
                    }

                    break;
            }
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