using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillTree_elements : SkillTree
    {

        public SkillTree_elements(VisualElement skillsPanel)
        {
            Initialize(skillsPanel.Q<VisualElement>("SkillTree_elements"));
        }

        protected override void Initialize(VisualElement skillTreePanel)
        {
            base.Initialize(skillTreePanel);


            mainSkill.skill = new Skill();

            // test
            // empty skills
            foreach (var kvp in indexToSkillSlotChain)
            {
                var slotChain = kvp.Value;
                for (int i = 0; i < 4; i++)
                {
                    slotChain[i].skill = new Skill();

                    if (i != 0)
                        slotChain[i].skill.RequiredSkill = slotChain[i - 1].skill;
                }
            }
            foreach (var skillSlot in buffSlotList)
                skillSlot.skill = new Skill();


            // todo - assign skills
            // main
            // chain
            buffSlotList[0].skill = new FireMastery();
        }
    }
}
