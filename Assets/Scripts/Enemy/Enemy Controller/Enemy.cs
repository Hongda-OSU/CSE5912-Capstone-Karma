using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public abstract class Enemy : MonoBehaviour, IEnemy
    {
        [Header("Enemy Properties")]
        [SerializeField] protected string enemyName;
        [SerializeField] protected string enemyLevel;
        [SerializeField] protected float experience;
        [SerializeField] protected float hp;
        [SerializeField] protected float maxHp;

        [Header("Detection Range")]
        [SerializeField] protected float viewRadius = 15f;
        [Range(0, 360)]
        [SerializeField] protected float viewAngle = 135f;
        [SerializeField] protected float closeDetectionRange = 3f;

        [Header("Path Finding")]
        [SerializeField] protected bool canWander = false;
        [SerializeField] protected float wanderAreaNumber = 0f;
        [SerializeField] protected bool canPatrol = false;
        [SerializeField] protected float patrolRouteNumber = 0f;

        protected bool foundTarget = false;
        protected bool isPlayingDeathAnimation = false;
        protected bool isAttackedByPlayer = false;

        protected float distance;
        protected Vector3 directionToTarget;

        protected Transform target;
        protected NavMeshAgent agent;
        protected Animator animator;

        protected virtual void Start()
        {
            target = PlayerManager.Instance.Player.transform;
            agent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
            animator.applyRootMotion = false;
        }

        protected abstract void HandleDeath();

        protected abstract void HandleWander();

        protected abstract void HandlePatrol();

        protected virtual void FaceTarget(Vector3 direction)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // IEnemy interface methods
        public float GetHealth()
        {
            return hp;
        }

        public float GetMaxHealth()
        {
            return maxHp;
        }

        public virtual void TakeDamage(float amount)
        {
            hp -= amount;
            if (!isAttackedByPlayer) {
                isAttackedByPlayer = true;
            }
        }


        /*
        // These codes below are used by Eiditor for testing purpose.
        public Vector3 GetTargetPosition()
        {
            return target.position;
        }

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
        */
    }
}
