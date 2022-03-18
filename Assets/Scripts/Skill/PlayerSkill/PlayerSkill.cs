using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PlayerSkill : Skill
    {
        [SerializeField] protected string skillName;

        [SerializeField] protected PlayerSkill requiredSkill;

        [SerializeField] protected bool isLearned = false;

        [SerializeField] protected int level = 0;
        [SerializeField] protected int maxLevel = 5;

        [SerializeField] protected int learnCost = 1;
        [SerializeField] protected int levelupCost = 1;

        [SerializeField] protected Color uiColor;

        protected string description;


        [SerializeField] protected SkillType type;
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

            if (requiredSkill != null && !requiredSkill.IsLeanred)
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

        public virtual void ResetLevel()
        {
            PlayerSkillManager.Instance.SkillPoints += learnCost + levelupCost * (level - 1);
            isReady = false;
            isLearned = false;
            level = 0;
        }

        // todo - change to abstract
        public virtual string GetSpecific()
        {
            string specific =
                "Name: " + skillName +
                "\nType: " + type.ToString() +
                "\nDescription: " + description;

            if (requiredSkill != null)
                specific += "\nRequire: " + requiredSkill.skillName;

            return specific;
        }


        public string Name { get { return skillName; } }
        public PlayerSkill RequiredSkill { get { return requiredSkill; } set { requiredSkill = value; } }
        public bool IsLeanred { get { return isLearned; } }
        public int Level { get { return level; } }
        public SkillType Type { get { return type; } }
        public Color UiColor { get { return UiColor; } }

    }
}
