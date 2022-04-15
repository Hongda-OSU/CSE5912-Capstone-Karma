using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ElectroIntellection : PlayerSkill
    {
        [SerializeField] private float chancePerLevel = 0.03f;

        protected override string GetBuiltSpecific()
        {
            var value = BuildSpecific("Electrocuted chance", chancePerLevel * 100, chancePerLevel * 100, "%", "");
            return value;
        }

        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            if (result)
                PlayerStats.Instance.ElectrocutedBaseChance += chancePerLevel;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
            PlayerStats.Instance.ElectrocutedBaseChance -= chancePerLevel * level;
        }

        public float ChancePerLevel { get { return chancePerLevel; } }
    }
}
