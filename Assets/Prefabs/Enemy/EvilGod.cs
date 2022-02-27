using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class EvilGod : EliteEnemy
    {
        [Header("Evil God")]
        [SerializeField] private GameObject portalVfx;
        [SerializeField] private Transform portalPivot;

        private bool isPerforming = false;


        protected override void PerformActions()
        {
            if (isPerforming)
                return;

            if (playerDetected)
                FaceTarget(directionToPlayer);

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
                        if (isPlayerInAttackRange)
                        {
                            StartCoroutine(Blink(transform.position + directionToPlayer * -5f, 1f));
                        }
                        else
                        {
                            MoveToPlayer();
                        }
                    }
                    break;

                case Status.Attacking:

                    break;

                case Status.Retreating:

                    break;

                case Status.Waiting:
                    break;
            }
        }


        protected override IEnumerator PerformActionsOnWaiting()
        {
            StartCoroutine(CoolDown(timeBetweenAttack));

            while (!isReadyToAttack)
            {
                yield return StartCoroutine(RandomAction());
            }

            SetRoll(Direction.None);

            FaceTarget(directionToPlayer);
            agent.isStopped = false;

            currentAttackNum = 0;
            Debug.Log(currentAttackNum);
        }

        private IEnumerator RandomAction()
        {
            waitAction = Random.Range(-2, 4);

            bool roll = Random.value < 0.5f;
            if (roll)
                SetRoll((Direction)waitAction);

            yield return new WaitForSeconds(Time.deltaTime);

            SetMove((Direction)waitAction);
            SetRoll(Direction.None);

            float randomWaitTime = Random.Range(timeBetweenWaitActions.x, timeBetweenWaitActions.y);
            yield return new WaitForSeconds(randomWaitTime);
        }


        protected override void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
        }

        private IEnumerator Blink(Vector3 position, float delay)
        {
            isPerforming = true;


            var origin = Instantiate(portalVfx);
            origin.transform.position = portalPivot.position;
            origin.transform.rotation = Quaternion.Euler(portalVfx.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            Destroy(origin, delay + 1);

            Debug.Log(transform.rotation + " " + transform.localRotation);

            yield return new WaitForSeconds(delay);

            transform.position = position;

            yield return new WaitForSeconds(Time.deltaTime);

            var target = Instantiate(portalVfx);
            target.transform.position = portalPivot.position;
            target.transform.rotation = Quaternion.Euler(portalVfx.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            Destroy(target, delay + 1);


            isPerforming = false;
        }
        //protected override void StartAttack()
        //{
        //    base.StartAttack();

        //    StartCoroutine(DisplayVfx());
        //}
        //private IEnumerator DisplayVfx()
        //{
        //    Vector3 forward = transform.forward;

        //    GameObject vfx = Instantiate(fireBlade);
        //    vfx.transform.position = transform.position + Vector3.up * 2;
        //    vfx.transform.LookAt(forward);
        //    vfx.transform.localScale = Vector3.one * vfxScale;

        //    float timeSince = 0f;
        //    while (timeSince < 10f)
        //    {
        //        timeSince += Time.deltaTime;
        //        yield return new WaitForSeconds(Time.deltaTime);

        //        vfx.transform.position += forward * vfxSpeed * Time.deltaTime;
        //    }
        //}
    }
}
