using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class EvilGod : BossEnemy
    {
        [Header("Evil God")]

        [SerializeField] private float dodgeChance = 0.5f;

        private LightningMissile_evilGod lightningMissile;
        private LightningExplosion_evilGod lightningExplosion;
        private LightningStorm_evilGod lightningStorm;
        private Shield_evilGod shield;
        private Blink_evilGod blink;

        private Bullet prevBullet;

        private bool isPerforming = false;

        private void Awake()
        {
            lightningMissile = GetComponentInChildren<LightningMissile_evilGod>();
            lightningExplosion = GetComponentInChildren<LightningExplosion_evilGod>();
            lightningStorm = GetComponentInChildren<LightningStorm_evilGod>();

            shield = GetComponentInChildren<Shield_evilGod>();
            blink = GetComponentInChildren<Blink_evilGod>();

            isInvincible = true;
        }


        protected override void PerformActions()
        {
            if (isPerforming || !isBossFightTriggered)
                return;

            if (Random.value < dodgeChance)
                Dodge();

            if (shield.IsPerformingAllowed())
            {
                OpenShield();
            }

            switch (status)
            {
                case Status.Idle:
                    if (playerDetected)
                    {
                        FaceTarget(directionToPlayer);

                        if (!isAttacking)
                        {
                            if (isPlayerInAttackRange)
                            {
                                Attack();
                            }
                            else
                            {
                                MoveToPlayer();
                            }
                        }
                    }
                    else
                    {
                        Rest();
                    }
                    break;

                case Status.Moving:
                    status = Status.Idle;
                    break;

                case Status.Attacking:
                    if (isPlayerInSafeDistance)
                    {
                        Blink(transform.position + directionToPlayer * -attackRange);
                    }
                    else if (isFatigued)
                    {
                        PrepareForNextAttack();
                    }
                    status = Status.Idle;

                    break;

                case Status.Retreating:

                    break;

                case Status.Waiting:
                    if (!isFatigued)
                    {
                        Rest();
                    }
                    break;
            }
        }

        public override void TriggerBossFight()
        {
            isInvincible = true;
            animator.applyRootMotion = true;
            animator.SetTrigger("Awake");
        }

        private void AwakeAnimationComplete()
        {
            isInvincible = false;
            isBossFightTriggered = true;
            animator.applyRootMotion = false;
        }

        protected override void MoveToPlayer()
        {
            base.MoveToPlayer();

            Vector3 position = PlayerManager.Instance.Player.transform.position + 
                Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up) * directionToPlayer * Random.Range(3f, attackRange * 0.8f);

            Blink(position);
        }
        protected override IEnumerator PerformActionsOnWaiting()
        {
            Vector3 position = PlayerManager.Instance.Player.transform.position + Quaternion.AngleAxis(Random.Range(0f, 135f), Vector3.up) * directionToPlayer * attackRange;
            Blink(position);
            currentAttackNum = 0;
            yield return null;
        }

        protected override void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
        }



        private void Dodge()
        {
            bool isFired = WeaponManager.Instance.isFiring && prevBullet != WeaponManager.Instance.CarriedWeapon.bulletFired;
            prevBullet = WeaponManager.Instance.CarriedWeapon.bulletFired;

            if (!isFired || !Physics.Raycast(player.position, WeaponManager.Instance.GetShootDirection(), out RaycastHit hit, 1000))
                return;

            var offset = Vector3.right * GetComponent<Collider>().bounds.size.x;
            if (Random.value < 0.5f)
                offset = -offset;

            var position = transform.position + offset;
            Blink(position);
        }
        private void Blink(Vector3 position)
        {
            StartCoroutine(blink.Perform(position));
        }

        private void Attack()
        {
            Attack_lightningStorm();
            Attack_lightningExplosion();
            Attack_lightningMissile();

            status = Status.Attacking;
        }

        private void Attack_lightningMissile()
        {
            if (!lightningMissile.IsPerformingAllowed())
                return;

            status = Status.Attacking;
            SetAttack(0);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator LightningMissile_performed()
        {
            StartCoroutine(lightningMissile.Perform());

            yield return new WaitForSeconds(1f);
            isPerforming = false;
        }


        private void Attack_lightningExplosion()
        {
            if (!lightningExplosion.IsPerformingAllowed())
                return;

            status = Status.Attacking;
            SetAttack(1);
            currentAttackNum++;
            isPerforming = true;
        }
        private IEnumerator LightningExplosion_performed()
        {
            Vector3 position = PlayerManager.Instance.Player.transform.position + Quaternion.AngleAxis(Random.Range(0f, 135f), Vector3.up) * directionToPlayer * attackRange / 2;
            Blink(position);

            yield return new WaitForSeconds(Time.deltaTime);
            isPerforming = false;

            yield return StartCoroutine(lightningExplosion.Perform());
        }


        private void Attack_lightningStorm()
        {
            if (!lightningStorm.IsPerformingAllowed())
                return;

            status = Status.Attacking;
            SetAttack(2);
            currentAttackNum++;
            isPerforming = true;

            Vector3 position = PlayerManager.Instance.Player.transform.position + directionToPlayer * 5f;
            Blink(position);

            isInvincible = true;
        }
        private IEnumerator LightningStorm_performed()
        {
            yield return StartCoroutine(lightningStorm.Perform());
        }


        private void OpenShield()
        {
            animator.SetTrigger("Shield");
            isPerforming = true;

        }
        private IEnumerator Shield_performed()
        {
            isPerforming = false;

            yield return StartCoroutine(shield.Perform());
        }

        private void DonePerforming()
        {
            isPerforming = false;
        }
    }
}
