using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillTree
    {
        public SkillSlot mainSkill;
        public Dictionary<int, List<SkillSlot>> indexToSkillSlotChain;

        public List<SkillSlot> buffList;

        public List<SkillSlot> skillSlotList;

        public VisualElement skillTreeElement;


        protected virtual void Initialize(VisualElement skillsPanel)
        {
            skillTreeElement = skillsPanel;

            indexToSkillSlotChain = new Dictionary<int, List<SkillSlot>>();
            buffList = new List<SkillSlot>();
            skillSlotList = new List<SkillSlot>();

            foreach (var child in skillTreeElement.Children())
            {
                string name = child.name;
                if (name == "MainSkill")
                {
                    mainSkill = new SkillSlot(child);
                    skillSlotList.Add(mainSkill);
                }

                else if (name == "Buffs")
                {
                    foreach (var skill in child.Children())
                    {
                        SkillSlot skillSlot = new SkillSlot(skill);

                        buffList.Add(skillSlot);
                        skillSlotList.Add(skillSlot);
                    }
                }
                else if (name.Contains("SkillChain_"))
                {
                    int index = int.Parse(name.Substring(name.Length - 1));
                    List<SkillSlot> skillChain = new List<SkillSlot>();
                    for (int i = 0; i < child.childCount; i++)
                    {
                        var slot = child.Q<VisualElement>("Skill_" + i);
                        if (slot == null)
                            break;

                        SkillSlot skillSlot = new SkillSlot(slot);

                        skillChain.Add(skillSlot);
                        skillSlotList.Add(skillSlot);
                    }
                    indexToSkillSlotChain.Add(index, skillChain);
                }
            }
        }

        public Skill FindSkillBySlot(VisualElement slot)
        {
            foreach (var skillSlot in skillSlotList)
            {
                if (skillSlot.slot == slot)
                    return skillSlot.skill;
            }
            return null;
        }

        public bool LevelUpSkill(VisualElement slot)
        {
            foreach (var skillSlot in skillSlotList)
            {
                if (skillSlot.slot == slot)
                    return skillSlot.LevelUp();
            }
            return false;
        }

    }
}
