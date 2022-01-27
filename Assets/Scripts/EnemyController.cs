using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 180f;
    public bool foundTarget = false;

    private float distance;
    private Vector3 directionToTarget;

    Transform target;
    NavMeshAgent agent;

    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        distance = Vector3.Distance(target.position, transform.position);
        directionToTarget = (target.position - transform.position).normalized;

        if (distance <= viewRadius)
        {
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                foundTarget = true; //Currently not used anywhere.

                FaceTarget(directionToTarget);
                agent.SetDestination(target.position);

                if (distance <= agent.stoppingDistance)
                {
                    // Attack player.
                }
            }
            else 
            {
                foundTarget = false;
            }
        }
        else 
        {
            foundTarget = false;
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
