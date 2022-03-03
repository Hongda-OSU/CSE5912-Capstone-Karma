using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FireMastery : Skill
    {
        [SerializeField] private float damagePerLevel = 5f;

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
