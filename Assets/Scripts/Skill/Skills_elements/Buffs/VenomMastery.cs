using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class VenomMastery : Skill
    {
        [SerializeField] private float damagePerLevel = 0.002f;

        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            if (result)
                PlayerStats.Instance.InfectedCurrentHealthDamagePerStack += damagePerLevel;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
            PlayerStats.Instance.InfectedCurrentHealthDamagePerStack -= damagePerLevel * level;
        }

        public float DamagePerLevel { get { return damagePerLevel; } }
    }
}
