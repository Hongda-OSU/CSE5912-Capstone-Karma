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
            if (isReady)
            {
                PlayerStats.Instance.GetDamageFactor().Fire.Value += damagePerLevel;
                isReady = false;
            }

            yield return null;
        }

        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            if (result)
                isReady = true;

            return result;
        }

        public override void ResetLevel()
        {
            base.ResetLevel();
            PlayerStats.Instance.GetDamageFactor().Fire.Value -= damagePerLevel * level;
        }
    }
}
