using System;
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
                    //if (distanceToPlayer < closeDetectionRange)
                    //{
                    //    Retreat();
                    //}

                    if (!isAttacking)
                    {
                        isPlayingAttackAnim = true;
                        
                        if (isPlayerInAttackRange && distanceToPlayer > closeDetectionRange)
                        {
                            Attack(1);
                        }
                        else if (distanceToPlayer <= 15f && distanceToPlayer > attackRange)
                        {
                            Attack(2);
                        }
                        else if (distanceToPlayer > 15f)
                        {
                            isPlayingAttackAnim = false;
                            MoveToPlayer();
                        }
                    }
                    break;

                case Status.Attacking:
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                    {
                        Debug.Log("Finished");
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
            if (counter == 3)
                TrippleShooting();
            else
            {
                GameObject vfx = Instantiate(arrowVFX, transform.position + transform.forward + new Vector3(0, 0.8f, 0), Quaternion.identity);
                // arrow follow player direction
                vfx.transform.LookAt(PlayerManager.Instance.Player.transform.position);
                Destroy(vfx, 5f);
            }
            counter++;
        }

        private void TrippleShooting()
        {
            GameObject vfx1 = Instantiate(arrowVFX, transform.position + transform.forward + new Vector3(0, 0.8f, 0), Quaternion.identity);
            GameObject vfx2 = Instantiate(arrowVFX, transform.position + transform.forward + new Vector3(0, 0.8f, 0), Quaternion.identity);
            GameObject vfx3 = Instantiate(arrowVFX, transform.position + transform.forward + new Vector3(0, 0.8f, 0), Quaternion.identity);

            Vector3 angle_1 = DirFromAngle(30f, false) * 10f;
            Vector3 angle_2 = DirFromAngle(330f, false) * 10f;

            vfx1.transform.LookAt(PlayerManager.Instance.Player.transform.position + angle_1);
            vfx2.transform.LookAt(PlayerManager.Instance.Player.transform.position);
            vfx3.transform.LookAt(PlayerManager.Instance.Player.transform.position + angle_2);
            Destroy(vfx1, 5f);
            Destroy(vfx2, 5f);
            Destroy(vfx3, 5f);
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