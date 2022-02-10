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

        private VisualElement selectedSkillSlot;

        private Label skillPointsLabel;

        private VisualElement specificPanel;
        private bool isSpecificOpened = false;
        private bool isFadingFinished = true;

        private void Awake()
        {
            Initialize();

            skillsPanel = root.Q<VisualElement>("SkillsPanel");

            skillPointsLabel = skillsPanel.Q<Label>("SkillPoints");

            specificPanel = skillsPanel.Q<VisualElement>("SkillSpecific");

            skillTree_electro = new SkillTree(skillsPanel);
        }

        private void Start()
        {
            UpdateVisual();

            foreach (var skillSlot in skillTree_electro.skillSlotList)
            {
                skillSlot.slot.RegisterCallback<MouseDownEvent>(evt => SkillSlot_performed(skillSlot.slot));
            }

            skillTree_electro.AssignSkillToSkillSlot(playerSkill.skillList_electro);
        }


        private void SelectSlot(VisualElement slot)
        {
            var skill = SelectSkillSlot(slot);

            UpdateVisual();
        }

        private Skill SelectSkillSlot(VisualElement slot)
        {
            Skill skill = null;

            string slotName = slot.name;
            if (slotName.Substring(0, slotName.Length - 1) == "Skill_")
            {
                skill = skillTree_electro.FindSkillBySlot(slot);

                selectedSkillSlot = slot;
            }

            return skill;
        }

        private void SkillSlot_performed(VisualElement slot)
        {
            if (selectedSkillSlot != slot)
            {
                isFadingFinished = false;

                StartCoroutine(PopUpSkillSpecific(slot));

                SelectSlot(slot);

                return;
            }
            else if (isFadingFinished && playerSkill.skillPoints > 0)
            {
                if (skillTree_electro.LevelUpSkill(slot))
                    playerSkill.skillPoints--;
            }

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            skillPointsLabel.text = "Skill Points: " + playerSkill.skillPoints;

            foreach (var skillSlot in skillTree_electro.skillSlotList)
            {
                VisualElement slot = skillSlot.slot;
                if (slot == selectedSkillSlot)
                {
                    slot.style.backgroundColor = Color.red;
                }
                else
                {
                    slot.style.backgroundColor = Color.clear;
                }
            }
        }

        private IEnumerator PopUpSkillSpecific(VisualElement slot)
        {

            Skill skill = skillTree_electro.FindSkillBySlot(slot);

            if (skill == null)
            {
                yield return StartCoroutine(PopOffSpecific());
            }
            else if (selectedSkillSlot != slot)
            {
                if (isSpecificOpened)
                    yield return StartCoroutine(PopOffSpecific());

                yield return StartCoroutine(PopUpSpecific(skill.BuildSpecific()));
            }
        }

        private IEnumerator PopUpSpecific(string specific)
        {
            specificPanel.Q<Label>("Specific").text = specific;

            yield return StartCoroutine(FadeIn(specificPanel));

            isSpecificOpened = true;
        }

        private IEnumerator PopOffSpecific()
        {

            selectedSkillSlot = null;

            yield return StartCoroutine(FadeOut(specificPanel));

            isSpecificOpened = false;

            isFadingFinished = true;
        }
    }
}
