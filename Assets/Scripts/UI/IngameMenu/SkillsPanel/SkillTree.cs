using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillTree
    {
        public SkillSlot mainSkill;

        public List<SkillSlot> passiveSlotList;

        public List<SkillSlot> buffSlotList;

        public List<SkillSlot> skillSlotList;

        public VisualElement skillTreeElement;


        protected virtual void Initialize(VisualElement skillsPanel)
        {
            skillTreeElement = skillsPanel;

            passiveSlotList = new List<SkillSlot>();
            buffSlotList = new List<SkillSlot>();
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
                    for (int i = 0; i < child.childCount; i++)
                    {
                        var slot = child.Q<VisualElement>("Buff_" + i);
                        if (slot == null)
                            break;

                        SkillSlot skillSlot = new SkillSlot(slot);

                        buffSlotList.Add(skillSlot);
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

                        passiveSlotList.Add(skillSlot);
                        skillSlotList.Add(skillSlot);
                    }
                }
            }
        }

        public PlayerSkill FindSkillBySlot(VisualElement slot)
        {
            foreach (var skillSlot in skillSlotList)
            {
                if (skillSlot.slot == slot)
                    return skillSlot.skill;
            }
            return null;
        }

        public SkillSlot FindSlotBySkill(PlayerSkill skill)
        {
            foreach (var skillSlot in skillSlotList)
            {
                if (skillSlot.skill == skill)
                    return skillSlot;
            }
            return null;
        }

        public void LevelUpSkill(VisualElement slot)
        {
            foreach (var skillSlot in skillSlotList)
            {
                if (skillSlot.slot == slot)
                {
                    skillSlot.LevelUp();

                    return;
                }
            }
        }

        public void AssignPassive(int index, PlayerSkill skill)
        {
            passiveSlotList[index].skill = skill;
        }
        public void AssignBuff(int index, PlayerSkill skill)
        {
            buffSlotList[index].skill = skill;
        }

        public void AssignMain(PlayerSkill skill)
        {
            mainSkill.skill = skill;
        }

    }
}
