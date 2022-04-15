using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ElectroAscension : PlayerSkill
    {
        [SerializeField] private float percentageDamagePerLevel = 0.02f;

        protected override string GetBuiltSpecific()
        {
            var value = BuildSpecific("Electro damage", percentageDamagePerLevel * 100, percentageDamagePerLevel * 100, "%", "");
            return value;
        }

        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            if (result)
                PlayerStats.Instance.GetDamageFactor().Electro.Value += percentageDamagePerLevel;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
            PlayerStats.Instance.GetDamageFactor().Electro.Value -= percentageDamagePerLevel * level;
        }

        public float PercentageDamagePerLevel { get { return percentageDamagePerLevel;} }
    }
}
