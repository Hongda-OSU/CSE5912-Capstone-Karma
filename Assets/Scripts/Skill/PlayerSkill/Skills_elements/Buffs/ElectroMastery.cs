using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ElectroMastery : PlayerSkill
    {
        [SerializeField] private float reductionPerLevel = 0.06f;

        protected override string GetBuiltSpecific()
        {
            var value = BuildSpecific("Resist Reduction", reductionPerLevel * 100, reductionPerLevel * 100, "%", "");
            return value;
        }

        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            if (result)
                PlayerStats.Instance.ElectrocutedResistReductionPerStack += reductionPerLevel;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
            PlayerStats.Instance.ElectrocutedResistReductionPerStack -= reductionPerLevel * level;
        }

        public float ReductionPerLevel { get { return reductionPerLevel; } }
    }
}
