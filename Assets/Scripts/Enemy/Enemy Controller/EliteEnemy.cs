using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public abstract class EliteEnemy : Enemy
    {
        [Header("Behavior Parameters")]
        // max number of continuous attacks
        [SerializeField] protected int maxContinuousAttackNum = 3;
        [SerializeField] protected int currentAttackNum = 0;

        // time interval between each attack
        [SerializeField] protected float timeBetweenAttack = 3f;
        protected bool isReadyToAttack = true;
        protected bool isFatigued = false;

        // time interval range between each action when waiting for next attack
        [SerializeField] protected Vector2 timeBetweenWaitActions = new Vector2(1, 2);

        // if aggro > aggroThreshold, attack player anyway
        [SerializeField] protected float aggro = 0f;
        [SerializeField] protected float aggroThreshold = 2f;

        [SerializeField] protected float maxRetreatTime = 3f;
        protected bool isRetreatFinished = true;

        protected int waitAction = -1;

        protected bool isAttacking = false;

        protected bool playerDetected = false;

        protected bool isPlayerInAttackRange;
        protected bool isPlayerInSafeDistance;

        protected bool isPlayingAttackAnim = false;

        // enemy status
        protected Status status = Status.Idle;
        protected enum Status
        {
            Idle,
            Moving,
            Retreating,
            Attacking,
            Waiting,
        }

        // animation direction
        protected enum Direction
        {
            Forward,
            Backward,
            Left,
            Right,
            None = -1,
        }

        protected virtual void Start()
        {
            Initialize();

            agent.isStopped = true;
            animator.applyRootMotion = true;
        }

        protected override void Update()
        {
            if (!isAlive)
                return;

            base.Update();

            isPlayerInAttackRange = distanceToPlayer < attackRange;

            isPlayerInSafeDistance = distanceToPlayer < closeDetectionRange;

            isFatigued = currentAttackNum >= maxContinuousAttackNum;

            CalculateAggro();

            PerformActions();
        }



        /*
         *  enemy actions
         */

        // perform the whole action logic
        protected abstract void PerformActions();

        // actions
        protected virtual void Rest()
        {
            status = Status.Idle;

            SetMove(Direction.None);
            SetAttack(-1);
            SetRoll(Direction.None);
        }
        protected virtual void MoveToPlayer()
        {
            status = Status.Moving;

            SetMove(Direction.Forward);
            SetRoll(Direction.None);

            FaceTarget(directionToPlayer);
            agent.SetDestination(player.position);
        }

        protected virtual void Attack(int index)
        {
            status = Status.Attacking;

            SetMove(Direction.None);

            SetAttack(index);
        }

        protected virtual void Retreat()
        {
            status = Status.Retreating;

            SetAttack(-1);
            SetMove(Direction.Backward);

            StartCoroutine(MoveBack());
        }

        private IEnumerator MoveBack()
        {
            isRetreatFinished = false;

            float timeSince = 0f;
            while (timeSince < maxRetreatTime && isPlayerInAttackRange && status == Status.Retreating)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

                agent.destination = directionToPlayer * -5f;
            }

            isRetreatFinished = true;
            status = Status.Idle;
        }

        protected virtual void PrepareForNextAttack()
        {
            status = Status.Waiting;

            StartCoroutine(PerformActionsOnWaiting());
        }

        protected abstract IEnumerator PerformActionsOnWaiting();




        /*
         * generals
         */

        protected virtual void CalculateAggro()
        {
            // check if player is detected
            bool isInViewDistance = distanceToPlayer <= viewRadius;
            bool isInViewAngle = Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2;
            bool isInSafeDistance = distanceToPlayer <= closeDetectionRange;

            playerDetected = isInViewDistance && isInViewAngle || isInSafeDistance || isAttackedByPlayer;

            // increase aggro if player stays in safe distance
            if (isPlayerInSafeDistance && aggro < aggroThreshold)
            {
                aggro += Time.deltaTime;
            }
            else if (!isPlayerInSafeDistance && aggro > 0f)
            {
                aggro -= Time.deltaTime;
            }
        }

        /*
         *  animation control
         */

        protected virtual void SetMove(Direction dir)
        {
            animator.SetInteger("Move", (int)dir);
            agent.isStopped = dir == Direction.None;
        }

        protected virtual void SetAttack(int index)
        {
            /*
             * 4 attacks in total
             * -1 means stop attacking
             */
            animator.SetInteger("Attack", index);
        }

        protected virtual void SetRoll(Direction dir)
        {
            animator.SetInteger("Roll", (int)dir);
        }


        private void AnimAttackStart()
        {

            isAttacking = true;
            currentAttackNum++;
        }

        private void AnimAttackFinish()
        {

            isAttacking = false;
            SetAttack(-1);
        }

        protected virtual IEnumerator CoolDown(float cooldown)
        {
            isReadyToAttack = false;

            yield return new WaitForSeconds(cooldown);

            isReadyToAttack = true;
        }


    }
}
