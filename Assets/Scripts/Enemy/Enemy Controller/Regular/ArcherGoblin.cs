using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ArcherGoblin : EliteEnemy
    {
        [SerializeField] private GameObject arrowVFX;
        [SerializeField] private GameObject arrow;
        private int counter = 0;

        protected override void PerformActions()
        {
            if (playerDetected)
                FaceTarget(directionToPlayer);
            switch (status)
            {
                case Status.Idle:
                    if (playerDetected)
                    {
                        FaceTarget(directionToPlayer);
                        MoveToPlayer();
                    }
                    else
                    {
                        Rest();
                    }
                    break;

                case Status.Moving:
                    if (distanceToPlayer > 15f)
                        MoveToPlayer();

                    if (!isAttacking)
                    {
                        isPlayingAttackAnim = true;
                        // 3~8
                        if (isPlayerInAttackRange && distanceToPlayer > closeDetectionRange)
                        {
                            Attack(1);
                        }
                        // 8~15
                        else if (distanceToPlayer <= 15f && distanceToPlayer > attackRange)
                        {
                            Attack(2);
                        }
                        else
                        {
                            isPlayingAttackAnim = false;
                        }
                    }
                    break;

                case Status.Attacking:
                    if (distanceToPlayer <= 5f)
                    {
                        animator.SetTrigger("RollBackward");
                        if (counter == 0)
                        {
                            animator.SetTrigger("DodgeLeft");
                        }
                        else if (counter > 0)
                        {
                            animator.SetTrigger("DodgeRight");
                        }
                    }
                    else if (distanceToPlayer <= 12f && distanceToPlayer > attackRange)
                    {
                        if (counter == 0)
                        {
                            if(Random.value > 0.3f)
                                animator.SetTrigger("RollForward");
                        }
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

        private void Shoot()
        {
            counter++;
            if (counter == 3)
                TrippleShooting();
            else
            {
                GameObject vfx = Instantiate(arrowVFX, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity);
                // arrow follow player direction
                vfx.transform.LookAt(PlayerManager.Instance.Player.transform.position);
                Destroy(vfx, 4f);
            }
        }

        private void Reload()
        {
            arrow.SetActive(true);
        }

        private void ReloadFinish()
        {
            arrow.SetActive(false);
        }

        private void TrippleShooting()
        {
            GameObject vfx1 = Instantiate(arrowVFX, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity);
            GameObject vfx2 = Instantiate(arrowVFX, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity);
            GameObject vfx3 = Instantiate(arrowVFX, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity);

            Vector3 angle_1 = DirFromAngle(15f, false) * 10f;
            Vector3 angle_2 = DirFromAngle(315f, false) * 10f;

            vfx1.transform.LookAt(PlayerManager.Instance.Player.transform.position + angle_1);
            vfx2.transform.LookAt(PlayerManager.Instance.Player.transform.position);
            vfx3.transform.LookAt(PlayerManager.Instance.Player.transform.position + angle_2);
            Destroy(vfx1, 4f);
            Destroy(vfx2, 4f);
            Destroy(vfx3, 4f);
            counter = 0;
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