using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CSE5912.PolyGamers
{
    public class DarkSlayer : EliteEnemy
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
                    // 23
                    if (distanceToPlayer > attackRange + 5f)
                        MoveToPlayer();

                    if (!isAttacking)
                    {
                        isPlayingAttackAnim = true;
                        // 0~8
                        if (isPlayerInSafeDistance)
                        {
                            if (Random.value < 0.4f && counter <= 5)
                            {
                                Attack(1);
                                counter++;
                            }
                            else
                            {
                                Attack(2);
                                if (counter > 5)
                                    counter = 0;
                            }
                        }
                        // 10~18
                        else if (distanceToPlayer > closeDetectionRange + 2f && distanceToPlayer <= attackRange)
                        {
                            if (Random.value < 0.7f)
                                Attack(3);
                            else
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

        private void StartStormAttack()
        {
            attackFinished = false;
            StartCoroutine(HammerStorm());
        }

        private void StopStormAttack()
        {
            attackFinished = true;
        }

        private void FlameRise()
        {
            flameVFX = Instantiate(DarkFlame, transform.position + new Vector3(0, 2, 0) + Vector3.back, Quaternion.Euler(-90,0,0));
        }

        private void FlameEnd()
        {
            if (flameVFX != null)
            {
                Material m = flameVFX.GetComponent<ParticleSystemRenderer>().material;
                StartCoroutine(FadeOut(m, 10f, 1f, flameVFX));
            }
        }

        private void FirePump()
        {
            fireVFX1 = Instantiate(DarkFire, transform.position + transform.forward * 5f + new Vector3(0,-10,0), Quaternion.Euler(-90,0,0));
            Material m = fireVFX1.GetComponent<ParticleSystemRenderer>().material;
            StartCoroutine(FadeOut(m, 40f, 4f, fireVFX1));
        }

        private void TrippleFirePump()
        {
            fireVFX2 = Instantiate(DarkFire, transform.position + transform.forward * 5f + new Vector3(0, -10, 0) + 2 * transform.right, Quaternion.Euler(-90, 0, 0));
            fireVFX3 = Instantiate(DarkFire, transform.position + transform.forward * 5f + new Vector3(0, -10, 0), Quaternion.Euler(-90, 0, 0));
            fireVFX4 = Instantiate(DarkFire, transform.position + transform.forward * 5f + new Vector3(0, -10, 0) - 2 * transform.right, Quaternion.Euler(-90, 0, 0));

            Destroy(fireVFX2, 4f);
            Destroy(fireVFX3, 4f);
            Destroy(fireVFX4, 4f);
        }

        private void DoomAttack()
        {
            nova = Instantiate(Nova, transform.position + transform.forward * 5f + new Vector3(0, 0.5f, 0),
                Quaternion.Euler(-90, 0, 0));
        }

        private IEnumerator FadeOut(Material material, float divider, float slow, GameObject obj)
        {
            for (float t = 0; t < 1f; t += Time.deltaTime / slow)
            {
                Color c = new Color(material.color.r, material.color.g, material.color.b,
                    Mathf.Lerp(material.color.a, 0f, t/divider));
                material.color = c;
                yield return null;
            }
            if(obj != null)
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

        protected override IEnumerator PerformActionsOnWaiting()
        {
            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        void OnEnable()
        {
            attackFinished = false;
        }
    }
}