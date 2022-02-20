using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public abstract class RegularEnemy : Enemy
    {
        [Header("Path Finding")]
        [SerializeField] protected bool canWander = false;
        [SerializeField] protected float wanderAreaNumber = 0f;
        [SerializeField] protected bool canPatrol = false;
        [SerializeField] protected Transform[] waypoints;

        protected virtual void Start()
        {
            Initialize();

            agent.isStopped = true;
            animator.applyRootMotion = false;
        }

        protected abstract void HandleDeath();

        protected abstract void HandleWander();

        protected abstract void HandlePatrol();


        
        // These codes below are used by Eiditor for testing purpose.
        public Transform GetTransform()
        {
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
            return closeDetectionRange;
        }

        public bool FoundTarget()
        {
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
}
