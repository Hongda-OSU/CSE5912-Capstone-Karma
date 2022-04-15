using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class VenomMastery : PlayerSkill
    {
        [SerializeField] private float damagePerLevel = 0.0004f;

        protected override string GetBuiltSpecific()
        {
            var value = BuildSpecific("Current Health Damage", damagePerLevel * 100, damagePerLevel * 100, "%", "of target's current health per second as Venom damage per stack");
            var curr = "\n\nBase damage per stack: \n" + PlayerStats.Instance.InfectedCurrentHealthDamagePerStack * 100 + "%";
            return value + curr;
        }

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
