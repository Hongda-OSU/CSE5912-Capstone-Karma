using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Goblin_2 : RegularEnemy
    {
        private bool isAttacking = false;
        private bool isGuarding = false;

        private void Awake()
        {
            enemyName = "Shield Goblin";
            health = 100f;
            maxHealth = 100f;
        }

        void Update()
        {
            distanceToPlayer = Vector3.Distance(player.position, transform.position);
            directionToPlayer = (player.position - transform.position).normalized;

            if ((distanceToPlayer <= viewRadius && Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
                || distanceToPlayer <= closeDetectionRange || isAttackedByPlayer) { 
            
            }
        }

        public override void TakeDamage(Damage damage)
        {
            if (isGuarding)
            {
                health -= (damage.ResolvedValue / 2);
            }
            else 
            {
                health -= damage.ResolvedValue;
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
