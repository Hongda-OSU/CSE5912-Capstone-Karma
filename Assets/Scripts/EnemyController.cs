using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float viewRadius = 10f;
    public float closeDetectionDistance = 1.5f;
    [Range(0, 360)]
    public float viewAngle = 135f;
    public bool foundTarget = false; // This is used for testing

    private float distance;
    private Vector3 directionToTarget;

    Transform target;
    NavMeshAgent agent;
    Animator animator;

    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    void Update()
    {
        distance = Vector3.Distance(target.position, transform.position);
        directionToTarget = (target.position - transform.position).normalized;

        if ((distance <= viewRadius && Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2) || distance <= closeDetectionDistance)
        {
            foundTarget = true;
            agent.isStopped = false;
            animator.SetBool("Run", true);

            FaceTarget(directionToTarget);
            agent.SetDestination(target.position);

            if (distance < agent.stoppingDistance + 0.3)
            {
                // Inside attacking range, attack player.
                agent.isStopped = true;
                animator.SetBool("InAttackRange", true);
                AttackPlayerRandomly();
            }
            else 
            {
                // Outside attacking range.
                animator.SetBool("InAttackRange", false);
                animator.ResetTrigger("Attack_1");
                animator.ResetTrigger("Attack_2");
            }
        }
        else 
        {
            foundTarget = false;
            agent.isStopped = true;
            animator.SetBool("Run", false);
        }


    }

    private void AttackPlayerRandomly() {
        float random = Random.Range(0f, 2f);

        if (random <= 1f) 
        {
            animator.SetTrigger("Attack_1");
        }
        else
        {
            animator.SetTrigger("Attack_2");
        }
    }

    private void FaceTarget(Vector3 direction) { 
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }


    public Vector3 GetTargetPosition() {
        return target.position;
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
    */
}
