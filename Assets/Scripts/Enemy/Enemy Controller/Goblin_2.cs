using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Goblin_2 : Enemy
    {
        private void Awake()
        {
            enemyName = "Shield Goblin";
            hp = 100f;
            maxHp = 100f;
        }

        void Update()
        {
            //TODO
        }

        public override void TakeDamage(float amount)
        {
            //TODO
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
