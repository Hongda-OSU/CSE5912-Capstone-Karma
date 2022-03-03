using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class ElectroMastery : Skill
    {
        [SerializeField] private float percentageDamagePerLevel = 0.02f;

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
    }
}
