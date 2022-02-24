using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FireMastery : Skill
    {
        [SerializeField] private float percentageDamagePerLevel = 0.02f;

        private void Update()
        {
            if (isReady)
            {
                PlayerStats.Instance.GetDamageFactor().Fire.Value += percentageDamagePerLevel;
                isReady = false;
            }
        }

        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            if (result)
                isReady = true;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
            PlayerStats.Instance.GetDamageFactor().Fire.Value -= percentageDamagePerLevel * level;
        }
    }
}
