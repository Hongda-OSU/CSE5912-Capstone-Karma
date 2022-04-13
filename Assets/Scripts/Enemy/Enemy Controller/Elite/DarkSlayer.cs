using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CSE5912.PolyGamers
{
    public class DarkSlayer : BossEnemy
    {
        public Vector3 Direction;
        private bool attackFinished;
        [SerializeField] private GameObject DarkFlame;
        [SerializeField] private GameObject DarkFire;
        [SerializeField] private GameObject Nova;

        private GameObject flameVFX;

        private GameObject fireVFX1;
        private GameObject fireVFX2;
        private GameObject fireVFX3;
        private GameObject fireVFX4;

        private GameObject nova;

        private bool isAbleToAttack = true;

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
                    // 23
                    if (distanceToPlayer > attackRange + 5f)
                        MoveToPlayer();

                    if (!isAttacking)
                    {
                        isPlayingAttackAnim = true;
                        // 0~8
                        if (isPlayerInSafeDistance)
                        {
                            if (Random.value < 0.4f)
                            {
                                Attack(2);
                            }
                            else
                            {
                                Attack(2);
                            }
                        }
                        // 10~18
                        else if (distanceToPlayer > closeDetectionRange + 2f && distanceToPlayer <= attackRange)
                        {
                            if (Random.value < 0.6f)
                            {
                                Attack(3);
                            }
                            else
                            {
                                if (isAbleToAttack)
                                    Attack(4);
                                else
                                    Attack(3);
                            }
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
                        status = Status.Moving;
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

        private void FlameRise()
        {
            flameVFX = Instantiate(DarkFlame, transform.position + new Vector3(0, 2, 0) + Vector3.back, Quaternion.Euler(-90, 0, 0));
        }

        private void FlameEnd()
        {
            if (flameVFX != null)
            {
                Material m = flameVFX.GetComponent<ParticleSystemRenderer>().material;
                StartCoroutine(FadeOut(m, 10f, 1f, flameVFX));
            }
        }

        private IEnumerator FadeOut(Material material, float divider, float slow, GameObject obj)
        {
            for (float t = 0; t < 1f; t += Time.deltaTime / slow)
            {
                Color c = new Color(material.color.r, material.color.g, material.color.b,
                    Mathf.Lerp(material.color.a, 0f, t / divider));
                material.color = c;
                yield return null;
            }
            if (obj != null)
                Destroy(obj);
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

        private void Hit()
        {
            if (distanceToPlayer <= agent.stoppingDistance + 0.2f)
            {
                Damage damage = new Damage(attackDamage, Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward),
                            30f);
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1_1"))
                    {
                        FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward),
                            120f);
                    }
                }
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

        private void StormHit()
        {
            if (distanceToPlayer <= agent.stoppingDistance / 2f)
            {
                Damage damage = new Damage(attackDamage, Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward),
                        30f);
                }
            }
        }

        private void HammerHit1()
        {
            if (distanceToPlayer <= agent.stoppingDistance)
            {
                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward),
                        75f);
                }
            }
        }

        private void HammerHit2()
        {
            if (distanceToPlayer <= agent.stoppingDistance)
            {
                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward),
                        75f);
                }
            }
        }

        private void FirePump()
        {
            fireVFX1 = Instantiate(DarkFire, transform.position + transform.forward * 5f + new Vector3(0,-10,0), Quaternion.Euler(-90,0,0));
            fireVFX1.AddComponent<FlameCollider>();
            fireVFX1.GetComponent<FlameCollider>().SetVariables(attackDamage, 50f, this);
            Material m = fireVFX1.GetComponent<ParticleSystemRenderer>().material;
            StartCoroutine(FadeOut(m, 40f, 3f, fireVFX1));
        }

        private void TrippleFirePump()
        {
            fireVFX2 = Instantiate(DarkFire, transform.position + transform.forward * 5f + new Vector3(0, -10, 0) + 2 * transform.right, Quaternion.Euler(-90, 0, 0));
            fireVFX3 = Instantiate(DarkFire, transform.position + transform.forward * 5f + new Vector3(0, -10, 0), Quaternion.Euler(-90, 0, 0));
            fireVFX4 = Instantiate(DarkFire, transform.position + transform.forward * 5f + new Vector3(0, -10, 0) - 2 * transform.right, Quaternion.Euler(-90, 0, 0));

            fireVFX2.AddComponent<FlameCollider>();
            fireVFX3.AddComponent<FlameCollider>();
            fireVFX4.AddComponent<FlameCollider>();

            fireVFX2.GetComponent<FlameCollider>().SetVariables(attackDamage, 80f, this);
            fireVFX3.GetComponent<FlameCollider>().SetVariables(attackDamage, 80f, this);
            fireVFX4.GetComponent<FlameCollider>().SetVariables(attackDamage, 80f, this);

            Destroy(fireVFX2, 4f);
            Destroy(fireVFX3, 4f);
            Destroy(fireVFX4, 4f);
        }

        private void DoomAttack()
        {
            nova = Instantiate(Nova, transform.position + transform.forward * 5f + new Vector3(0, 0.5f, 0),
                Quaternion.Euler(-90, 0, 0));
            Destroy(nova, 1f);
            if (distanceToPlayer > attackRange - 2f)
            {
                FPSControllerCC.Instance.AddImpact(Vector3.up, 200f);
                FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.back), 150f);
            }

            StartCoroutine(Doom(transform.position));
        }

        private IEnumerator Doom(Vector3 position)
        {
            for (int i = 0; i < 8; i++)
            {
                yield return new WaitForSeconds(0.1f);
                Vector3 randomPos = RandomCircle(position, Random.Range(5f, 10f));
                GameObject fireVFX = Instantiate(DarkFire, randomPos + new Vector3(0, -10f, 2f), Quaternion.Euler(-90, 0, 0));
                fireVFX.AddComponent<FlameCollider>();
                fireVFX.GetComponent<FlameCollider>().SetVariables(attackDamage, 60f, this);
                Destroy(fireVFX, 3f);
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

        private IEnumerator SkillCoolDown()
        {
            isAbleToAttack = false;
            yield return new WaitForSeconds(12f);
            isAbleToAttack = true;
        }

        protected override IEnumerator PerformActionsOnWaiting()
        {
            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        void OnEnable()
        {
            attackFinished = false;
        }

        public override void TriggerBossFight()
        {
            isInvincible = true;
            animator.SetTrigger("Awake");
        }

        protected override void AwakeAnimationComplete()
        {
            isInvincible = false;
            isBossFightTriggered = true;
        }
    }
}