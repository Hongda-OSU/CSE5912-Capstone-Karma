using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillSlotsControl: UI
    {
        public List<SkillSlot> electroSkillSlotList;

        public VisualElement electroSkillTree;


        private void Awake()
        {
            Initialize();

            electroSkillTree = root.Q<VisualElement>("SkillTree_Electro");
            electroSkillSlotList = InitializeSkillSlot(electroSkillTree);


            // test
            for (int i = 0; i < electroSkillSlotList.Count; i++)
            {
                var slot = electroSkillSlotList[i];
                slot.skill = new Skill();

                if (i != 0)
                    slot.skill.requiredSkill = electroSkillSlotList[i - 1].skill;
            }
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

        //public Skill FindSkillBySlot(VisualElement slot)
        //{
        //    foreach (var skillSlot in electroSkillSlotList)
        //    {
        //        if (skillSlot.slot == slot)
        //            return skillSlot.skill;
        //    }
        //    return null;
        //}

        public void LevelUpSkill(VisualElement slot)
        {
            foreach (var skillSlot in electroSkillSlotList)
            {
                if (skillSlot.slot == slot)
                    skillSlot.LevelUp();
            }

        }
    }
}
