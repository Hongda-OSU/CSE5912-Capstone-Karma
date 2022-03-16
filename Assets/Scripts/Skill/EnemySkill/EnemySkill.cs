using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public abstract class EnemySkill : Skill
    {
        [SerializeField] protected Enemy enemy;

        private void Awake()
        {
            StartCoolingdown();
        }

        public abstract bool IsPerformingAllowed();

    }
}
