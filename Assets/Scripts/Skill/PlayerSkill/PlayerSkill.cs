using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public abstract class PlayerSkill : Skill
    {
        [SerializeField] protected string skillName;

        [SerializeField] protected PlayerSkill requiredSkill;
        [SerializeField] protected int requiredSkillLevel;

        [SerializeField] protected bool isLearned = false;

        [SerializeField] protected int level = 0;
        [SerializeField] protected int maxLevel = 5;

        [SerializeField] protected int learnCost = 1;
        [SerializeField] protected int levelupCost = 1;

        [SerializeField] protected Sprite icon;

        [SerializeField] protected SkillType type;

        [TextArea(20, 30)]
        [SerializeField] protected string description;

        public enum SkillType
        {
            Passive,
            Main,
            Buff,
            SetSkill,
        }

        public virtual bool LevelUp()
        {
            PlayerSkillManager playerSkill = PlayerSkillManager.Instance;

            if (level >= maxLevel)
                return false;

            if (requiredSkill != null && requiredSkill.level < requiredSkillLevel)
                return false;


            if (level == 0 && playerSkill.SkillPoints >= learnCost)
            {
                isLearned = true;
                isReady = true;

                playerSkill.SkillPoints -= learnCost;
                level++;
            }
            else if (level > 0 && playerSkill.SkillPoints >= levelupCost)
            {
                playerSkill.SkillPoints -= levelupCost;
                level++;
            }
            return true;
        }
        public void SetLevel(int level)
        {
            isLearned = level > 0;
            isReady = level > 0;
            this.level = Mathf.Clamp(level, 0, maxLevel);
        }

        public virtual void ResetLevel()
        {
            PlayerSkillManager.Instance.SkillPoints += learnCost + levelupCost * (level - 1);
            isReady = false;
            isLearned = false;
            level = 0;
        }

        protected string BuildSpecific(string tag, float baseValue, float valuePerLevel, string unit, string suffix)
        {
            string result = "\n\n";

            result += tag + ":\n";

            for (int i = 0; i < maxLevel; i++)
            {
                string value = baseValue + valuePerLevel * i + unit;
                if (i < maxLevel - 1)
                {
                    value += "/";
                }
                else
                {
                    value += " " + suffix;
                }
                result += value;
            }

            return result;
        }

        protected string GetRequirements()
        {
            var learn = "";
            if (learnCost > 0)
                learn = "\nLearn Cost: " + learnCost;

            var levelup = "";
            if (levelupCost > 0)
                levelup = "\nUpgrade Cost: " + levelupCost;

            var require = "";
            if (requiredSkill != null)
            {
                require += "\nRequired: " + requiredSkill.Name + " Lv. " + requiredSkillLevel;
            }

            return "\n" + learn + levelup + require;
        } 

        abstract protected string GetBuiltSpecific();

        public string Name { get { return skillName; } }
        public PlayerSkill RequiredSkill { get { return requiredSkill; } set { requiredSkill = value; } }
        public bool IsLeanred { get { return isLearned; } }
        public int Level { get { return level; } }
        public SkillType Type { get { return type; } }
        public Sprite Icon { get { return icon; } }
        public string Description { get { return description + GetBuiltSpecific() + GetRequirements(); } }
    }
}
