using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Golem : EliteEnemy
    {
        [Header("Shardstone Shooting")]
        [SerializeField] private GameObject prefab_1;
        [SerializeField] private Transform pivot_1;

        [Header("Shockwave")]
        [SerializeField] private GameObject prefab_2;

        [Header("Vine")]
        [SerializeField] private GameObject prefab_3;
        [SerializeField] private GameObject prefab_4;


        protected override void PerformActions()
        {
            switch (status) 
            {
                case Status.Idle:
                    if (playerDetected)
                    {
                        MoveToPlayer();
                    }
                    else
                    {
                        Rest();
                    }

                    break;

                case Status.Moving:                 
                    if (!isAttacking)
                    {
                        isPlayingAttackAnim = true;

                        if (Vector3.Distance(player.position, pivot_1.position) <= 12f && Vector3.Distance(player.position, pivot_1.position) >= 10f)
                        {
                            Attack(1);                          
                        }
                        else if (isPlayerInAttackRange && distanceToPlayer > closeDetectionRange)
                        {
                            Attack(2);
                        }
                        else if (distanceToPlayer <= 5f)
                        {
                            Attack(3);
                        }
                        else if (distanceToPlayer <= 20f && Vector3.Distance(player.position, pivot_1.position) > 12f)
                        {
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
                        
                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Attack_1") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Attack_2") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_1")) {
                            FaceTarget(directionToPlayer);
                        }
                        

                        if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_Stomp_1") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_Stomp_2") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_1") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Attack_1") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Attack_2") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_4")))
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
        

        protected override IEnumerator PerformActionsOnWaiting()
        {
            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        public void ShardstoneShooting() {
            GameObject vfx_1 = Instantiate(prefab_1);
            GameObject vfx_2 = Instantiate(prefab_1);
            GameObject vfx_3 = Instantiate(prefab_1);

            vfx_1.transform.position = pivot_1.position;
            vfx_2.transform.position = pivot_1.position;
            vfx_3.transform.position = pivot_1.position;


            Vector3 angle_1 = DirFromAngle(30f, false) * 10f;
            Vector3 angle_2 = DirFromAngle(330f, false) * 10f;

            vfx_1.transform.LookAt(new Vector3(PlayerManager.Instance.Player.transform.position.x, 0f, PlayerManager.Instance.Player.transform.position.z) + angle_1);
            vfx_2.transform.LookAt(new Vector3(PlayerManager.Instance.Player.transform.position.x, 0f, PlayerManager.Instance.Player.transform.position.z));
            vfx_3.transform.LookAt(new Vector3(PlayerManager.Instance.Player.transform.position.x, 0f, PlayerManager.Instance.Player.transform.position.z) + angle_2);

            Destroy(vfx_1, 4f);
            Destroy(vfx_2, 4f);
            Destroy(vfx_3, 4f);
        }

        public void Shockwave() {
            GameObject vfx = Instantiate(prefab_2);
            vfx.transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
            //vfx.transform.LookAt(new Vector3(PlayerManager.Instance.Player.transform.position.x, 0f, PlayerManager.Instance.Player.transform.position.z));

            Destroy(vfx, 2f);
        }

        public void Vine() {
            GameObject vfx = Instantiate(prefab_3);
            GameObject dust = Instantiate(prefab_4);
            vfx.transform.position = new Vector3(PlayerManager.Instance.Player.transform.position.x, 0f, PlayerManager.Instance.Player.transform.position.z);
            dust.transform.position = pivot_1.position;

            Destroy(vfx, 4f);
            Destroy(dust, 2f);
        }

                // These codes below are used by Eiditor for testing purpose.
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
            return closeDetectionRange;
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

        /*
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
        }
        */
    }
}
