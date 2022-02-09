using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillsPanelControl : UI
    {
        [SerializeField] PlayerSkill playerSkill;
        SkillTree skillTree_electro;

        //SkillTree skillTree_pyro;
        //SkillTree skillTree_cryo;
        //SkillTree skillTree_venom;

        private VisualElement skillsPanel;

        private Label skillPointsLabel;

        private void Awake()
        {
            Initialize();

            skillsPanel = root.Q<VisualElement>("SkillsPanel");

            skillPointsLabel = skillsPanel.Q<Label>("SkillPoints");

            skillTree_electro = new SkillTree(skillsPanel);
        }

        private void Start()
        {
            UpdateSkillPointsLabel();

            foreach (var skillSlot in skillTree_electro.skillSlotList)
            {
                skillSlot.slot.RegisterCallback<MouseDownEvent>(evt => SkillSlot_performed(skillSlot.slot));
            }

            skillTree_electro.AssignSkillToSkillSlot(playerSkill.skillList_electro);
        }

        private void SkillSlot_performed(VisualElement slot)
        {
            if (playerSkill.skillPoints > 0)
            {
                if (skillTree_electro.LevelUpSkill(slot))
                    playerSkill.skillPoints--;
            }

            UpdateSkillPointsLabel();
        }

        private void UpdateSkillPointsLabel()
        {
            skillPointsLabel.text = "Skill Points: " + playerSkill.skillPoints;
        }
    }
}
