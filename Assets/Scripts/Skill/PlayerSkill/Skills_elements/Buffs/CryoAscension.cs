using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class CryoAscension : PlayerSkill
    {
        [SerializeField] private float percentageDamagePerLevel = 0.02f;

        protected override string GetBuiltSpecific()
        {
            var value = BuildSpecific("Cryo Damage", percentageDamagePerLevel * 100, percentageDamagePerLevel * 100, "%", "");
            return value;
        }

        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            if (result)
                PlayerStats.Instance.GetDamageFactor().Cryo.Value += percentageDamagePerLevel;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
            PlayerStats.Instance.GetDamageFactor().Cryo.Value -= percentageDamagePerLevel * level;
        }

        public float PercentageDamagePerLevel { get { return percentageDamagePerLevel; } }
    }
}
