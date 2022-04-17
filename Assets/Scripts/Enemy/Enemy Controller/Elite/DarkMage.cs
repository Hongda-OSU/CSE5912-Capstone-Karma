using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class DarkMage : EliteEnemy
    {
        [SerializeField] private GameObject energyBall;
        [SerializeField] private GameObject fireBall;
        [SerializeField] private GameObject nova;
        [SerializeField] private GameObject impact;
        private GameObject fireBallVFX;
        private GameObject energyBallVFX1;
        private GameObject energyBallVFX2;
        private GameObject energyBallVFX3;
        private GameObject darkNova;
        private int counter;

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
                    if (distanceToPlayer > attackRange)
                        MoveToPlayer();

                    if (!isAttacking)
                    {
                        isPlayingAttackAnim = true;
                        if (isPlayerInSafeDistance)
                        {
                            Attack(2);
                        }
                        else if (distanceToPlayer > closeDetectionRange && distanceToPlayer < attackRange)
                        {
                            Attack(1);
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

                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2") )
                        {
                            FaceTarget(directionToPlayer);
                        }

                        if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") ||
                              animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2")))
                        {
                            isPlayingAttackAnim = false;
                            animator.ResetTrigger("Attack_1");
                            animator.ResetTrigger("Attack_2");
                        }
                    }
                    else
                    {
                        status = Status.Moving;
                    }
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
            if (counter >= 5)
            {
                MultiShoot();
            }
            else
            {
                fireBallVFX = Instantiate(fireBall, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity);
                fireBallVFX.transform.LookAt(PlayerManager.Instance.Player.transform.position);
                fireBallVFX.AddComponent<MageAttack>();
                fireBallVFX.GetComponent<MageAttack>().SetVariables(10f, attackDamage, 50f, impact, this, Element.Type.Fire);
                Destroy(fireBallVFX, 4f);
            }
        }

        private void MultiShoot()
        {
            counter = 0;
            energyBallVFX1 = Instantiate(energyBall, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity);
            energyBallVFX2 = Instantiate(energyBall, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity);
            energyBallVFX3 = Instantiate(energyBall, transform.position + transform.forward * 0.8f + new Vector3(0, 1f, 0), Quaternion.identity);

            Vector3 angle_1 = DirFromAngle(30f, false) * 10f;
            Vector3 angle_2 = DirFromAngle(-30f, false) * 10f;

            energyBallVFX1.transform.LookAt(PlayerManager.Instance.Player.transform.position + angle_1);
            energyBallVFX2.transform.LookAt(PlayerManager.Instance.Player.transform.position);
            energyBallVFX3.transform.LookAt(PlayerManager.Instance.Player.transform.position + angle_2);

            energyBallVFX1.gameObject.AddComponent<MageAttack>();
            energyBallVFX1.GetComponent<MageAttack>().SetVariables(10f, attackDamage, 60f, impact, this, Element.Type.Fire);
            energyBallVFX2.gameObject.AddComponent<MageAttack>();
            energyBallVFX2.GetComponent<MageAttack>().SetVariables(10f, attackDamage, 60f, impact, this, Element.Type.Fire);
            energyBallVFX3.gameObject.AddComponent<MageAttack>();
            energyBallVFX3.GetComponent<MageAttack>().SetVariables(10f, attackDamage, 60f, impact, this, Element.Type.Fire);

            Destroy(energyBallVFX1, 4f);
            Destroy(energyBallVFX2, 4f);
            Destroy(energyBallVFX3, 4f);
        }

        private void Protection()
        {
            darkNova = Instantiate(nova, transform.position + new Vector3(0,1,0), Quaternion.Euler(new Vector3(75, 0, 0)));
            FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 100f);
            FPSControllerCC.Instance.AddImpact(transform.up, 30f);
            Destroy(darkNova, 2f);
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