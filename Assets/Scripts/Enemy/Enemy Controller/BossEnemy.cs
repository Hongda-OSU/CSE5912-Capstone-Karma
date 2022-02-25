using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PolyGamers
{
    public class BossEnemy : EliteEnemy
    {
        [SerializeField] private float triggerDistance = 10f;

        private void TriggerFighting()
        {

        }

        protected override void PlayDeathAnimation()
        {
            throw new System.NotImplementedException();
        }
    }
}
