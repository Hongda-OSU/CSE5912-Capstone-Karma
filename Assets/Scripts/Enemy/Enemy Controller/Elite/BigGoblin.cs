using System.Collections;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class BigGoblin : EliteEnemy
    {
        [SerializeField] private GameObject IceBall;
        [SerializeField] private GameObject IceBump;
        [SerializeField] private GameObject IceRange;
        [SerializeField] private GameObject IceImpact;

        private GameObject iceBumpVFX;
        private GameObject iceRangeVFX;

        private bool isAbleToAttack = true;

        protected override void PerformActions()
        {
            if (playerDetected)
            {
                FaceTarget(directionToPlayer);
                animator.SetTrigger("Boost");
            }
                
            switch (status)
            {
                case Status.Idle:
                    if (playerDetected)
                        MoveToPlayer();
                    else
                        Rest();
                    break;

                case Status.Moving:
                    // 18
                    if (distanceToPlayer > attackRange + 3f)
                        MoveToPlayer();

                    if (!isAttacking)
                    {
                        isPlayingAttackAnim = true;
                        // 0 ~ 6
                        if (isPlayerInSafeDistance)
                        {
                            if (Random.value > 0.5)
                                Attack(2);
                            else
                                Attack(2);
                        }
                        // 8 ~ 15
                        else if (distanceToPlayer > closeDetectionRange + 2f && distanceToPlayer < attackRange && isAbleToAttack)
                        {
                            Attack(3);
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
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3"))
                        {
                            FaceTarget(directionToPlayer);
                        }

                        if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") ||
                              animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2") ||
                              animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3")))
                        {
                            isPlayingAttackAnim = false;
                            animator.ResetTrigger("Attack_1");
                            animator.ResetTrigger("Attack_2");
                            animator.ResetTrigger("Attack_3");
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

        private void IceRainAttack()
        {
            iceBumpVFX = Instantiate(IceBump, transform.position, Quaternion.Euler(-90,0,0));
            Destroy(iceBumpVFX, 1f);
            iceRangeVFX = Instantiate(IceRange, FPSControllerCC.Instance.transform.position + new Vector3(0, 6f,0),
                Quaternion.identity);
            StartCoroutine(IceRain(iceRangeVFX.transform.position));
            Destroy(iceRangeVFX, 3f);
        }

        private IEnumerator IceRain(Vector3 position) 
        {
            for (int i = 0; i < 12; i++)
            {
                yield return new WaitForSeconds(0.2f);
                Vector3 randomPos = RandomCircle(position, Random.Range(1f, 3f));
                GameObject iceBallVFX = Instantiate(IceBall, randomPos, Quaternion.Euler(-90, 0, 0));
                iceBallVFX.transform.LookAt(PlayerManager.Instance.Player.transform.position);
                iceBallVFX.AddComponent<IceRainControl>();
                iceBallVFX.GetComponent<IceRainControl>().SetVariables(6f, attackDamage, 10f, this, Element.Type.Cryo, IceImpact, transform.position.y);
                Destroy(iceBallVFX, 3f);
            }
            
            StartCoroutine(SkillCoolDown());
        }

        private Vector3 RandomCircle(Vector3 center, float radius)
        {
            float ang = Random.value * 360;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y;
            pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            return pos;
        }

        private void Hit()
        {
            if (distanceToPlayer <= agent.stoppingDistance - 0.2f)
            {
                Damage damage = new Damage(attackDamage, Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward),
                            20f);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1_1"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward),
                            50f);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward),
                            20f);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2_1"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward),
                            70f);
                    }
                }
            }
        }

        public void DeathSound()
        {
            transform.Find("Audio Sources").Find("Death").GetComponent<AudioSource>().Play();
        }

        private IEnumerator SkillCoolDown()
        {
            isAbleToAttack = false;
            yield return new WaitForSeconds(8f);
            isAbleToAttack = true;
        }
    }
}