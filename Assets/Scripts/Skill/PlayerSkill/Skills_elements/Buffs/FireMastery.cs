using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FireMastery : PlayerSkill
    {
        [SerializeField] private float damagePerLevel = 1f;

        protected override string GetBuiltSpecific()
        {
            var value = BuildSpecific("Damage", damagePerLevel, damagePerLevel, "", "Fire damage per stack");
            var curr = "\n\nBase damage per stack: \n" + PlayerStats.Instance.BurnedDamagePerStack;
            return value + curr;
        }

        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            if (result)
                PlayerStats.Instance.BurnedDamagePerStack += damagePerLevel;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
            PlayerStats.Instance.BurnedDamagePerStack -= damagePerLevel * level;
        }

        public float DamagePerLevel { get { return damagePerLevel; } }
    }
}
