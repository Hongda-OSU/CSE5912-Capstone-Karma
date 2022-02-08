using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonKnightControl : MonoBehaviour
{
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private int maxContinuousAttackNum = 3;

    [SerializeField] private float safeDistance = 3f;
    [SerializeField] private float aggroDistance = 20f;

    [SerializeField] private float timeBetweenAttack = 3f;
    private float timeSinceAttack = 0f;

    [SerializeField] Vector2 timeRangeBetweenWaitActions = new Vector2(1, 2);
    float timeSinceWaitAction = 1f;

    int waitAction = -1;

    private int currentAttackNum = 0;

    private bool isAttacking = false;

    private bool playerDetected = false;
    private bool playerLocked = false;

    private bool isPlayerInAttackRange;
    private bool isPlayerInSafeDistance;

    [SerializeField] private float aggro = 0f;
    [SerializeField] private float aggroThreshold = 2f;


    private float viewRadius = 15f;
    [Range(0, 360)]
    private float viewAngle = 135f;

    private float distanceToPlayer;
    private Vector3 directionToTarget;

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;

    private Status status = Status.Idle;
    private enum Status
    {
        Idle,
        Moving,
        Retreating,
        Attacking,
        Wait,
    }

    private enum Direction
    {
        forward,
        backward,
        left,
        right,
        none = -1,
    }
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        //
        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;
        //

    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(target.position, transform.position);

        directionToTarget = (target.position - transform.position).normalized;

        isPlayerInAttackRange = distanceToPlayer < attackRange; 

        isPlayerInSafeDistance = distanceToPlayer < safeDistance;

        CalculateAggro();

        Debug.Log("Status: " + status);
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
                if (distanceToPlayer < safeDistance)
                {
                    Retreat();
                }

                if (!isAttacking)
                {
                    if (isPlayerInAttackRange) {
                        Attack();
                    }
                    
                    else
                    {
                        MoveToPlayer();
                    }
                }
                break;

            case Status.Retreating:
                if (playerLocked || !isPlayerInAttackRange)
                {
                    if (currentAttackNum >= maxContinuousAttackNum)
                        PrepareForNextAttack();
                    else 
                        MoveToPlayer();
                }

                break;
            case Status.Attacking:
                if (currentAttackNum >= maxContinuousAttackNum)
                {
                    Retreat();
                }
                else 
                    status = Status.Moving;

                break;

            case Status.Wait:
                if (timeSinceAttack < timeBetweenAttack)
                {
                    PrepareForNextAttack();

                    timeSinceAttack += Time.deltaTime;
                }
                else
                {
                    Rest();
                    timeSinceAttack = 0f;
                }
                //if (timeSinceAttack == 0f)
                //{
                //    StartCoroutine(WaitForNextAction());
                //}

                break;
        }
    }



    /*
     *  enemy actions
     */

    private void Rest()
    {
        status = Status.Idle;


        SetMove(Direction.none);
        SetAttack(-1);
        SetRoll(Direction.none);
    }
    private void MoveToPlayer()
    {
        status = Status.Moving;

        SetMove(Direction.forward);

        FaceTarget(directionToTarget);
        agent.SetDestination(target.position);
    }

    private void Attack()
    {
        status = Status.Attacking;

        SetMove(Direction.none);

        SetAttack(Random.Range(0, 1));
    }

    private void Retreat()
    {
        status = Status.Retreating;

        SetAttack(-1);
        SetMove(Direction.backward);

        agent.destination = directionToTarget * -5f;

    }

    private void PrepareForNextAttack()
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
     * 
     */
    private void CalculateAggro()
    {
        playerDetected = distanceToPlayer <= viewRadius && Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2 || distanceToPlayer <= safeDistance;

        if (isPlayerInSafeDistance && !playerLocked)
        {
            aggro += Time.deltaTime;
        }

        else if (!isPlayerInSafeDistance && aggro > 0f)
        {
            aggro -= Time.deltaTime;
        }


        playerLocked = aggro >= aggroThreshold;
    }
    private void FaceTarget(Vector3 direction) { 
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void TakeDamage(float amount)
    {
        // TODO
    }

    public float GetHP()
    {
        // TODO
        return 0f;
    }



    /*
     *  animation control
     */

    private void SetMove(Direction dir)
    {

        animator.SetInteger("Move", (int)dir);
        agent.isStopped = dir == Direction.none;
    }

    private void SetAttack(int index)
    {
        /*
         * 4 attacks in total
         * -1 means stop attacking
         */
        animator.SetInteger("Attack", index);
    }

    private void SetRoll(Direction dir)
    {

        animator.SetInteger("Roll", (int)dir);
    }


    /*
     * used by animation event
     */
    private void Hit()
    {

    }
    private void FootL()
    {

    }
    private void FootR()
    {

    }
    private void StartAttack()
    {
        isAttacking = true;
    }
    private void FinishAttack()
    {
        isAttacking = false;
        currentAttackNum++;
    }

}
