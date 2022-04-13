using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class Golem : BossEnemy
    {
        [Header("Shardstone Shooting")]
        [SerializeField] private GameObject prefab_1;
        [SerializeField] private Transform pivot_1;

        [Header("Shockwave")]
        [SerializeField] private GameObject prefab_2;

        [Header("Vine")]
        [SerializeField] private GameObject prefab_3;
        [SerializeField] private GameObject prefab_4;

        [Header("Landing")]
        [SerializeField] private GameObject prefab_5;

        bool isFalling = false;
        bool slowdownPlayer = false;
        bool playerSlowed = false;
        float slowdownCounter = 3.5f;
        float originSpeedFactor;

        private void OnEnable()
        {
            transform.position = new Vector3(transform.position.x, 40f, transform.position.z);
        }

        protected override void Start()
        {
            base.Start();
            agent.enabled = false;
            transform.position = new Vector3(transform.position.x, 40f, transform.position.z);
            originSpeedFactor = PlayerStats.Instance.MoveSpeedFactor;
        }
        protected override void PerformActions()
        {
            if (isFalling)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 50f * Time.deltaTime, transform.position.z);

                //var target = new Vector3(transform.position.x, 4f, transform.position.z);
                //transform.position = Vector3.MoveTowards(transform.position, target, 500f * Time.deltaTime);

                if (transform.position.y <= 4f)
                {
                    isFalling = false;
                    agent.enabled = true;
                    animator.SetTrigger("Land");
                }
            }

            if (!isBossFightTriggered)
                return;

            if (slowdownPlayer)
            {
                if (!playerSlowed)
                {
                    PlayerStats.Instance.MoveSpeedFactor -= 0.8f;
                    playerSlowed = true;
                }

                slowdownCounter -= Time.deltaTime;

                if (slowdownCounter <= 0f)
                {
                    PlayerStats.Instance.MoveSpeedFactor = originSpeedFactor;
                    slowdownCounter = 3.5f;
                    slowdownPlayer = false;
                    playerSlowed = false;
                }
            }

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

                        if (Vector3.Distance(player.position, pivot_1.position) <= 30f && Vector3.Distance(player.position, pivot_1.position) >= 22f)
                        {
                            FaceTarget(directionToPlayer);
                            Attack(1);
                        }
                        else if (isPlayerInAttackRange && distanceToPlayer > closeDetectionRange)
                        {
                            FaceTarget(directionToPlayer);
                            Attack(2);
                        }
                        else if (distanceToPlayer <= 15f)
                        {
                            FaceTarget(directionToPlayer);
                            Attack(3);
                        }
                        else if (distanceToPlayer <= 70f && Vector3.Distance(player.position, pivot_1.position) > 30f)
                        {
                            FaceTarget(directionToPlayer);
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
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_1"))
                        {
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

        protected override void Die()
        {
            PlayerStats.Instance.MoveSpeedFactor = originSpeedFactor;
            base.Die();

            GameStateController.Instance.bossToAlive["Golem"] = false;
        }

        public override void TriggerBossFight()
        {
            isInvincible = true;
            agent.enabled = false;
            animator.SetTrigger("Awake");
            isFalling = true;
        }

        protected override void AwakeAnimationComplete()
        {
            isInvincible = false;
            agent.enabled = true;
            isBossFightTriggered = true;
            isFalling = false;
        }

        protected override IEnumerator PerformActionsOnWaiting()
        {
            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        public void ShardstoneShooting()
        {
            GameObject vfx_1 = Instantiate(prefab_1, pivot_1.position, Quaternion.identity);
            GameObject vfx_2 = Instantiate(prefab_1, pivot_1.position, Quaternion.identity);
            GameObject vfx_3 = Instantiate(prefab_1, pivot_1.position, Quaternion.identity);

            Vector3 angle_1 = DirFromAngle(30f, false) * 10f;
            Vector3 angle_2 = DirFromAngle(330f, false) * 10f;

            vfx_1.transform.LookAt(new Vector3(PlayerManager.Instance.Player.transform.position.x, PlayerManager.Instance.Player.transform.position.y - 1.5f, PlayerManager.Instance.Player.transform.position.z) + angle_1);
            vfx_2.transform.LookAt(new Vector3(PlayerManager.Instance.Player.transform.position.x, PlayerManager.Instance.Player.transform.position.y - 1.5f, PlayerManager.Instance.Player.transform.position.z));
            vfx_3.transform.LookAt(new Vector3(PlayerManager.Instance.Player.transform.position.x, PlayerManager.Instance.Player.transform.position.y - 1.5f, PlayerManager.Instance.Player.transform.position.z) + angle_2);

            if (Vector3.Distance(player.position, pivot_1.position) <= 30f &&
                Vector3.Angle(transform.forward, directionToPlayer) < 60 / 2 &&
                (PlayerManager.Instance.Player.transform.position.y <= vfx_2.transform.position.y + 2f))
            {
                Damage damage = new Damage(Random.Range(10f, 20f), Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);

                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 100f);
                    FPSControllerCC.Instance.AddImpact(Vector3.up, 800f);
                }
            }

            Destroy(vfx_1, 4f);
            Destroy(vfx_2, 4f);
            Destroy(vfx_3, 4f);
        }

        public void Shockwave()
        {
            GameObject vfx = Instantiate(prefab_2);
            vfx.transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
            //vfx.transform.LookAt(new Vector3(PlayerManager.Instance.Player.transform.position.x, 0f, PlayerManager.Instance.Player.transform.position.z));

            Damage damage = new Damage(Random.Range(3f, 5f), Element.Type.Physical, this, PlayerStats.Instance);
            PlayerStats.Instance.TakeDamage(damage);

            if (PlayerStats.Instance.Health > 0f)
            {
                FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 200f);
            }

            Destroy(vfx, 2f);
        }

        public void Vine()
        {
            GameObject vfx = Instantiate(prefab_3);
            GameObject dust = Instantiate(prefab_4);
            vfx.transform.position = new Vector3(PlayerManager.Instance.Player.transform.position.x, PlayerManager.Instance.Player.transform.position.y - 2f, PlayerManager.Instance.Player.transform.position.z);
            dust.transform.position = pivot_1.position;

            if (PlayerManager.Instance.Player.transform.position.y < vfx.transform.position.y + 3f)
            {
                Damage damage = new Damage(Random.Range(1f, 5f), Element.Type.Venom, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                slowdownPlayer = true;
            }

            Destroy(vfx, 4f);
            Destroy(dust, 2f);
        }

        protected override void Hit()
        {
            float damageAmount;
            if (distanceToPlayer <= attackRange)
            {
                damageAmount = attackDamage + Mathf.RoundToInt(Random.Range(-10f, 5f));
                Damage damage = new Damage(damageAmount, Element.Type.Physical, this, PlayerStats.Instance);
                PlayerStats.Instance.TakeDamage(damage);
                if (PlayerStats.Instance.Health > 0f)
                {
                    FPSControllerCC.Instance.AddImpact(this.gameObject.transform.TransformDirection(Vector3.forward), 200f);
                }
            }
        }

        public void Landing()
        {
            GameObject vfx = Instantiate(prefab_5);
            vfx.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

            Destroy(vfx, 4f);
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
