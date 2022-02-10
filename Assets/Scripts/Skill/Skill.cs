using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Skill
    {
        private string name;

        private Skill requiredSkill;

        private bool isLearned = false;

        private int level = 0;
        private int maxLevel = 5;

        private string description;
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

        public string BuildSpecific()
        {
            string specific =
                "Name: " + name +
                "\nDescription" + description;

            if (requiredSkill != null)
                specific += "\nRequire: " + requiredSkill.name;

            return specific;
        }


        public string Name { get { return name; } }
        public Skill RequiredSkill { get { return requiredSkill; } set { requiredSkill = value; } }
        public bool IsLeanred { get { return isLearned; } }
        public int Level { get { return level; } }

       
    }
}
