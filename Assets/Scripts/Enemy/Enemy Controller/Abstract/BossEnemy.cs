using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public abstract class BossEnemy : EliteEnemy
    {
        [SerializeField] protected bool isBossFightTriggered = false;

        public abstract void TriggerBossFight();

        protected override void CalculateAggro()
        {
            base.CalculateAggro();
            playerDetected = isBossFightTriggered;
        }

        public bool IsBossFightTriggered { get { return playerDetected; } }
    }
}
