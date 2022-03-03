using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ElectroMastery : Skill
    {
        [SerializeField] private float reductionPerLevel = 0.06f;

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
