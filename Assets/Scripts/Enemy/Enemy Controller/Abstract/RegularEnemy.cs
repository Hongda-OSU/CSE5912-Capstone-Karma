using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public abstract class RegularEnemy : Enemy
    {
        [Header("Regular Enemy")]
        [Header("Path Finding")]
        [SerializeField] protected bool canWander = false;
        [SerializeField] protected float wanderAreaNumber = 0f;
        [SerializeField] protected bool canPatrol = false;
        [SerializeField] protected Transform[] waypoints;

        [SerializeField] protected bool isAggroOn = false;
        [SerializeField] private float aggro;
        [SerializeField] private float aggroDuration = 5f;
        [SerializeField] private float aggroRange = 10f;
        private bool prevFound;

        protected virtual void Start()
        {
            Initialize();

            agent.isStopped = true;
            animator.applyRootMotion = false;
        }

        protected override void Update() 
        {
            base.Update();
            if (!isAlive || isFrozen)
                return;

            CalculateAggro();

            PerformActions();
        }
        abstract protected void PerformActions();

        protected abstract void HandleWander();

        protected abstract void HandlePatrol();

        private void CalculateAggro()
        {
            if (!prevFound && foundTarget)
                aggro = aggroDuration;

            isAggroOn = aggro > 0f;

            if (isAggroOn)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, aggroRange);

                bool isPlayerSeen = false;
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "Player")
                        isPlayerSeen = true;
                }
                if (!isPlayerSeen)
                    aggro -= Time.deltaTime;
                else
                    aggro = aggroDuration;
            }

            prevFound = foundTarget;
        }

        public override void ResetEnemy()
        {
            base.ResetEnemy();

            aggro = 0f;
        }

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
