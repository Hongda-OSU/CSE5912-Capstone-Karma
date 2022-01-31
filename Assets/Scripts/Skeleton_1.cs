using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton_1 : MonoBehaviour, IEnemy
{
    private float viewRadius = 10f;
    private float closeDetectionDistance = 1.5f;
    [Range(0, 360)]
    private float viewAngle = 135f;
    private bool foundTarget = false; // This is used for testing

    private float distance;
    private Vector3 directionToTarget;

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;

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

            ResetAttackAnimationTriggers();

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
            }
        }
        else 
        {
            foundTarget = false;
            agent.isStopped = true;
            animator.SetBool("Run", false);
        }
    }

    private void ResetAttackAnimationTriggers() {
        animator.ResetTrigger("Attack_1");
        animator.ResetTrigger("Attack_2");
        animator.ResetTrigger("Attack_3");
        animator.ResetTrigger("Attack_4");
    }

    private void AttackPlayerRandomly() {
        float random = Random.value;

        if (random >= 0f && random < 0.25f)
        {
            animator.SetTrigger("Attack_1");
        }
        else if (random >= 0.25f && random < 0.5f)
        {
            animator.SetTrigger("Attack_2");
        }
        else if (random >= 0.5f && random < 0.75f) 
        {
            animator.SetTrigger("Attack_3");
        }
        else if (random >= 0.75f && random < 1f)
        {
            animator.SetTrigger("Attack_4");
        }
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

    // These codes below are used by Eiditor for testing purpose.
    public Vector3 GetTargetPosition()
    {
        return target.position;
    }

    public Transform GetTransform() {
        return transform;
    }

    public float GetViewAngle()
    {
        return viewAngle;
    }

    public float GetViewRadius()
    {
        return viewRadius;
    }

    public float GetCloseDetectionDistance()
    {
        return closeDetectionDistance;
    }

    public bool FoundTarget() {
        return foundTarget;
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
