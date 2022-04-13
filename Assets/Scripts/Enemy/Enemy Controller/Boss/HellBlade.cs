using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class HellBlade : BossEnemy
    {
        [Header("Effect Prefabs")]
        [SerializeField] private GameObject explosion_1;
        [SerializeField] private GameObject explosion_2;
        [SerializeField] private GameObject swirl;
        [SerializeField] private GameObject flame;
        [SerializeField] private GameObject flamePivot;

        [Header("2nd Phase GameObjects")]      
        [SerializeField] private GameObject greatSword;
        [SerializeField] private GameObject blackFire;

        private bool inPhase2 = false;
        private bool switchingPhase = false;
        private float flameCoolDown = 0f;

        protected override void PerformActions()
        {
            if (!isBossFightTriggered) 
            {
                return;
            }

            /*
            if (health <= MaxHealth / 2f) 
            {
                if (!inPhase2) {
                    animator.SetTrigger("SecondPhase");
                }

                switchingPhase = true;
                inPhase2 = true;
            }
            */

            if (switchingPhase)
            {             
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("dead2")) 
                {
                    animator.ResetTrigger("SecondPhase");
                }
                
                return;
            }
            else 
            {
                switch (status)
                {
                    case Status.Idle:
                        if (playerDetected)
                        {
                            FaceTarget(directionToPlayer);
                            RandomTeleport();
                            MoveToPlayer();
                            isAttackedByPlayer = false;
                            animator.SetBool("FastMove", false);
                        }
                        else
                        {
                            Rest();
                        }
                        break;

                    case Status.Moving:

                        if (!isAttacking)
                        {
                            if (isAttackedByPlayer || distanceToPlayer >= 12f)
                            {
                                if (Random.Range(0f, 1f) < 0.5f)
                                {
                                    RandomTeleport();
                                }
                                else
                                {
                                    animator.SetBool("FastMove", true);
                                }

                                isAttackedByPlayer = false;
                            }

                            flameCoolDown -= Time.deltaTime;

                            if (flameCoolDown <= 0f)
                            {
                                Attack(4);
                                flameCoolDown = Random.Range(5f, 15f);
                            }

                            if (isPlayerInAttackRange)
                            {
                                if (animator.GetBool("FastMove"))
                                {
                                    if (Random.Range(0f, 1f) < 0.33f)
                                    {
                                        Attack(1);
                                    }
                                    else if (Random.Range(0f, 1f) >= 0.33f && Random.Range(0f, 1f) < 0.66f)
                                    {
                                        Attack(2);
                                    }
                                    else
                                    {
                                        Attack(3);
                                    }
                                }
                                else
                                {
                                    Attack(1);
                                }
                            }
                            else
                            {
                                MoveToPlayer();
                            }
                        }
                        break;

                    case Status.Attacking:

                        FaceTarget(directionToPlayer);

                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                        {
                            status = Status.Idle;
                            animator.ResetTrigger("Attack_1");
                            animator.ResetTrigger("Attack_2");
                            animator.ResetTrigger("Attack_3");
                            animator.ResetTrigger("Attack_4");
                        }

                        break;
                }
            }
        }

        public void RandomSlashStart() 
        { 
        
        }

        public void RandomSlashFinish()
        {

        }

        private void RandomTeleport() 
        {
            Vector3 position = PlayerManager.Instance.Player.transform.position + Quaternion.AngleAxis(Random.Range(0f, 135f), Vector3.up) * directionToPlayer * 20f / 2;
            GameObject vfx_1 = Instantiate(explosion_1, transform.position + new Vector3(0f, 0.6f, 0f), Quaternion.identity);
            transform.position = (position);
            Destroy(vfx_1, 3f);
        }

        protected override IEnumerator PerformActionsOnWaiting()
        {
            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        public override void TriggerBossFight()
        {
            isInvincible = true;
            animator.SetTrigger("Awake");

            GameObject vfx = Instantiate(explosion_2, transform.position + new Vector3(0f, 0.6f, 0f), Quaternion.identity);
            Destroy(vfx, 3f);
        }

        protected override void AwakeAnimationComplete()
        {
            isInvincible = false;
            isBossFightTriggered = true;
        }

        public void Shoot() 
        {
            GameObject vfx = Instantiate(explosion_2, flamePivot.transform.position, Quaternion.identity);
            Destroy(vfx, 3f);

            if (distanceToPlayer <= attackRange + 1f) 
            {
                Damage damage = new Damage(Random.Range(17.5f, 22.5f), Element.Type.Fire, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);

                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 100f);
                    FPSControllerCC.Instance.AddImpact(Vector3.up, 100f);
                }
            }
        }

        public void ThrowFlame()
        {
            GameObject vfx_1 = Instantiate(flame, flamePivot.transform.position, Quaternion.identity);
            vfx_1.transform.LookAt(new Vector3(PlayerManager.Instance.Player.transform.position.x, PlayerManager.Instance.Player.transform.position.y, PlayerManager.Instance.Player.transform.position.z));
            Destroy(vfx_1, 6f);
        }

        public void Swirl()
        {          
            GameObject vfx_1 = Instantiate(swirl, transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
            Destroy(vfx_1, 3f);

            if (distanceToPlayer <= attackRange + 0.2f)
            {
                Damage damage = new Damage(Random.Range(20f, 30f), Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);

                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 50f);
                    FPSControllerCC.Instance.AddImpact(Vector3.up, 200f);
                }
            }
        }

        public void Hit_1() 
        {
            if (distanceToPlayer <= attackRange + 0.2f)
            {
                Damage damage = new Damage(Random.Range(12f, 18f), Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);

                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 50f);
                }
            }
        }

        public void PhaseTransition() 
        {
            greatSword.SetActive(true);
            blackFire.SetActive(true);
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
