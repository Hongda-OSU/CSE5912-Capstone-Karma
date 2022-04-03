using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ArcherGoblin : EliteEnemy
    {
        [SerializeField] private GameObject arrowVFX;
        private int counter = 0;
        private GameObject vfx;
        private GameObject vfx1;
        private GameObject vfx2;
        private GameObject vfx3;

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
                        if(Random.value < 0.5f)
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

        private void Shoot()
        {
            counter++;
            if (counter == 3)
                TrippleShooting();
            else
            {
                vfx = Instantiate(arrowVFX, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity);
                vfx.gameObject.AddComponent<RangeShootingResolve>();
                vfx.GetComponent<RangeShootingResolve>().SetVariables(attackDamage, 50f, this, "back");
                // arrow follow player direction
                vfx.transform.LookAt(PlayerManager.Instance.Player.transform.position);
                Destroy(vfx, 4f);
            }
        }

        private void TrippleShooting()
        {
            vfx1 = Instantiate(arrowVFX, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity); 
            vfx2 = Instantiate(arrowVFX, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity);
            vfx3 = Instantiate(arrowVFX, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity);

            vfx1.gameObject.AddComponent<RangeShootingResolve>();
            vfx1.GetComponent<RangeShootingResolve>().SetVariables(attackDamage, 50f, this, "back");
            vfx2.gameObject.AddComponent<RangeShootingResolve>();
            vfx2.GetComponent<RangeShootingResolve>().SetVariables(attackDamage, 50f, this, "back");
            vfx3.gameObject.AddComponent<RangeShootingResolve>();
            vfx3.GetComponent<RangeShootingResolve>().SetVariables(attackDamage, 50f, this, "back");

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