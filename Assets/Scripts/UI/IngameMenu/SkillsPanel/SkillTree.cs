using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillTree
    {
        public List<SkillSlot> skillSlotList;

        public VisualElement skillTreeElement;


        public SkillTree(VisualElement skillsPanel)
        {
            skillTreeElement = skillsPanel.Q<VisualElement>("SkillTree_Electro");
            skillSlotList = InitializeSkillSlot(skillTreeElement);
        }

        private List<SkillSlot> InitializeSkillSlot(VisualElement skillTree)
        {
            List<SkillSlot> skillSlotList = new List<SkillSlot>();
            foreach (var child in skillTree.Children())
            {
                for (int i = 0; i < child.childCount; i++)
                {
                    var skillSlot = child.Q<VisualElement>("Skill_" + i);
                    if (skillSlot != null)
                    {
                        skillSlotList.Add(new SkillSlot(skillSlot));
                    }
                }
            }
            return skillSlotList;
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

        public void AssignSkillToSkillSlot(List<Skill> skillList)
        {
            for (int i = 0; i < skillSlotList.Count; i++)
            {
                skillSlotList[i].skill = skillList[i];
            }
        }
    }
}
