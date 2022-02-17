using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class BossEnemy : MonoBehaviour, IEnemy
    {
        [Header("Enemy Properties")]
        [SerializeField] protected string enemyName;

        [SerializeField] protected float experience;

        [SerializeField] protected float health;
        [SerializeField] protected float maxHealth;

        [SerializeField] protected float attackDamage;

        [SerializeField] protected float attackRange = 5f;

        [Header("Behavior Parameters")]
        // max number of continuous attacks
        [SerializeField] protected int maxContinuousAttackNum = 3;
        protected int currentAttackNum = 0;

        // if player is detected immediately if in safe distance
        [SerializeField] protected float safeDistance = 3f;

        // if player is out of giveup distance, stop chasing
        [SerializeField] protected float chasingDistance = 20f;

        // time interval between each attack
        [SerializeField] protected float timeBetweenAttack = 3f;
        protected float timeSinceAttack = 0f;

        // time interval range between each action when waiting for next attack
        [SerializeField] protected Vector2 timeRangeBetweenWaitActions = new Vector2(1, 2);
        protected float timeSinceWaitAction = 1f;

        // if aggro > aggroThreshold, attack player anyway
        [SerializeField] protected float aggro = 0f;
        [SerializeField] protected float aggroThreshold = 2f;


        protected int waitAction = -1;

        protected bool isAttacking = false;

        protected bool playerDetected = false;

        protected bool isPlayerInAttackRange;
        protected bool isPlayerInSafeDistance;


        [SerializeField] protected float viewRadius = 15f;
        [Range(0, 360)]
        [SerializeField] protected float viewAngle = 135f;

        protected float distanceToPlayer;
        protected Vector3 directionToTarget;

        protected Transform target;
        protected NavMeshAgent agent;
        protected Animator animator;

        // enemy status
        protected Status status = Status.Idle;
        protected enum Status
        {
            Idle,
            Moving,
            Retreating,
            Attacking,
            Wait,
        }

        // animation direction
        protected enum Direction
        {
            forward,
            backward,
            left,
            right,
            none = -1,
        }

        protected virtual void Start()
        {
            target = PlayerManager.Instance.Player.transform;

            agent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;

            animator = GetComponent<Animator>();
            animator.applyRootMotion = true;
        }

        protected virtual void Update()
        {
            distanceToPlayer = Vector3.Distance(target.position, transform.position);

            directionToTarget = (target.position - transform.position).normalized;

            isPlayerInAttackRange = distanceToPlayer < attackRange;

            isPlayerInSafeDistance = distanceToPlayer < safeDistance;

            CalculateAggro();

            PerformActions();
        }



        /*
         *  enemy actions
         */

        // perform the whole action logic
        protected virtual void PerformActions()
        {

        }

        // actions
        protected virtual void Rest()
        {
            status = Status.Idle;

            SetMove(Direction.none);
            SetAttack(-1);
            SetRoll(Direction.none);
        }
        protected virtual void MoveToPlayer()
        {
            status = Status.Moving;

            SetMove(Direction.forward);

            FaceTarget(directionToTarget);
            agent.SetDestination(target.position);
        }

        protected virtual void Attack(int index)
        {
            status = Status.Attacking;

            SetMove(Direction.none);

            SetAttack(index);
        }

        protected virtual void Retreat()
        {
            status = Status.Retreating;

            SetAttack(-1);
            SetMove(Direction.backward);

            agent.destination = directionToTarget * -5f;

        }

        protected virtual void PrepareForNextAttack()
        {
            status = Status.Wait;

            timeSinceWaitAction += Time.deltaTime;

            float randomWaitTime = Random.Range(timeRangeBetweenWaitActions.x, timeRangeBetweenWaitActions.y);
            if (timeSinceWaitAction >= randomWaitTime)
            {
                waitAction = Random.Range(-2, 4);
                timeSinceWaitAction = 0f;

                SetMove((Direction)waitAction);
                bool roll = Random.value < 0.5f;
                if (roll)
                    SetRoll((Direction)waitAction);

                return;
            }

            SetRoll(Direction.none);

            FaceTarget(directionToTarget);
            agent.isStopped = false;

            currentAttackNum = 0;

        }




        /*
         * generals
         */

        protected virtual void CalculateAggro()
        {
            // check if player is detected
            bool isInViewDistance = distanceToPlayer <= viewRadius;
            bool isInViewAngle = Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2;
            bool isInSafeDistance = distanceToPlayer <= safeDistance;
            bool isInChasingDistance = distanceToPlayer < chasingDistance;

            playerDetected = (isInViewDistance && isInViewAngle || isInSafeDistance) && isInChasingDistance;


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

        protected virtual void FaceTarget(Vector3 direction)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }


        public virtual string GetName()
        {
            return enemyName;
        }
        public virtual float GetHealth()
        {
            return health;
        }
        public float GetMaxHealth()
        {
            return maxHealth;
        }
        public virtual float GetExperience()
        {
            return experience;
        }
        public virtual float GetAttackDamage()
        {
            return attackDamage;
        }
        public virtual void TakeDamage(float amount)
        {
            health -= amount;
        }

        /*
         *  animation control
         */

        protected virtual void SetMove(Direction dir)
        {
            animator.SetInteger("Move", (int)dir);
            agent.isStopped = dir == Direction.none;
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


        /*
         * used by animation event
         */
        protected virtual void Hit()
        {

        }
        protected virtual void FootL()
        {

        }
        protected virtual void FootR()
        {

        }
        protected virtual void StartAttack()
        {
            isAttacking = true;
        }
        protected virtual void FinishAttack()
        {
            isAttacking = false;
            currentAttackNum++;
        }


    }
}
