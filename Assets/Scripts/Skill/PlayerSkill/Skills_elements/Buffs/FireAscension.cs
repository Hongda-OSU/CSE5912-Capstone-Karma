using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FireAscension : PlayerSkill
    {
        [SerializeField] private float percentageDamagePerLevel = 0.02f;

        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            if (result)
                PlayerStats.Instance.GetDamageFactor().Fire.Value += percentageDamagePerLevel;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
            PlayerStats.Instance.GetDamageFactor().Fire.Value -= percentageDamagePerLevel * level;
        }

        public float PercentageDamagePerLevel { get { return percentageDamagePerLevel; } }
    }
}
