using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FireMastery : Skill
    {
        private float damagePerLevel = 0.02f;

        public FireMastery()
        {
            name = "Fire Mastery";

            requiredSkill = null;

            description = "Increase Fire damage";

            type = SkillType.buff;
        }

        public override IEnumerator PerformEffect()
        {
            PlayerStats.Instance.ExtraDamageFactor_fire += damagePerLevel * level;

            yield return null;
        }
    }
}
