using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Goblin_2 : MonoBehaviour, IEnemy
    {
        private float viewRadius = 15f;
        private float closeDetectionDistance = 3f;
        [Range(0, 360)]
        private float viewAngle = 135f;
        private bool foundTarget = false; // This is used for testing

        private float distance;
        private Vector3 directionToTarget;

        private Transform target;
        private UnityEngine.AI.NavMeshAgent agent;
        private Animator animator;

        private bool isAttacking = false;
        private bool isPlayingDeathAnimation = false;

        [SerializeField] protected float HP = 100f;
        [SerializeField] protected float maxHp = 100f;

        void Start()
        {
        
        }

        void Update()
        {
        
        }
        public bool FoundTarget()
        {
            throw new System.NotImplementedException();
        }

        public float GetHealth()
        {
            throw new System.NotImplementedException();
        }

        public float GetMaxHealth()
        {
            return maxHp;
        }

        public void TakeDamage(float amount)
        {
            throw new System.NotImplementedException();
        }
    }
}
