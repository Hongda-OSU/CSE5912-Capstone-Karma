using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Skill
    {
        public Skill requiredSkill;

        public Skill nextSkill;

        private bool isLearned = false;

        private int level = 0;
        private int maxLevel = 5;

        public bool LevelUp()
        {
            if (level >= maxLevel)
                return false;

            if (requiredSkill != null && !requiredSkill.IsLeanred)
                return false;

            if (level == 0)
                isLearned = true;

            level++;

            return true;
        }

        public void ResetLevel()
        {
            isLearned = false;
            level = 0;
        }

        public bool IsLeanred { get { return isLearned; } }
        public int Level { get { return level; } }
    }
}
