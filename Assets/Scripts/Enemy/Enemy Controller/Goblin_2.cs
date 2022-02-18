using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Goblin_2 : Enemy
    {
        private bool isAttacking = false;
        private bool isGuarding = false;

        private void Awake()
        {
            enemyName = "Shield Goblin";
            hp = 100f;
            maxHp = 100f;
        }

        void Update()
        {
            distance = Vector3.Distance(target.position, transform.position);
            directionToTarget = (target.position - transform.position).normalized;

            if ((distance <= viewRadius && Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
                || distance <= closeDetectionRange || isAttackedByPlayer) { 
            
            }
        }

        public override void TakeDamage(float amount)
        {
            if (isGuarding)
            {
                hp -= (amount / 2);
            }
            else 
            {
                hp -= amount;
            }

            if (!isAttackedByPlayer)
            {
                isAttackedByPlayer = true;
            }
        }

        protected override void HandleDeath()
        {
            
        }

        protected override void HandleWander()
        {
            
        }

        protected override void HandlePatrol()
        {
            
        }
    }
}
