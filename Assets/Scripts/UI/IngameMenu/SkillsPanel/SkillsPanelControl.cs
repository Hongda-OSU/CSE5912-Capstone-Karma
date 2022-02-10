using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillsPanelControl : UI
    {
        private PlayerSkill playerSkill;

        private SkillTree skillTree_elements;

        private VisualElement skillsPanel;

        private VisualElement selectedSkillSlot;

        private Label skillPointsLabel;

        private VisualElement specificPanel;
        private bool isSpecificOpened = false;
        private bool isFadingFinished = true;

        private static SkillsPanelControl instance;
        public static SkillsPanelControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            Initialize();

            skillsPanel = root.Q<VisualElement>("SkillsPanel");

            skillPointsLabel = skillsPanel.Q<Label>("SkillPoints");

            specificPanel = skillsPanel.Q<VisualElement>("SkillSpecific");

            skillTree_elements = new SkillTree_elements(skillsPanel);
        }

        private void Start()
        {
            playerSkill = PlayerSkill.Instance;

            UpdateVisual();

            foreach (var skillSlot in skillTree_elements.skillSlotList)
            {
                skillSlot.slot.RegisterCallback<MouseDownEvent>(evt => SkillSlot_performed(skillSlot.slot));
            }

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
            if (slotName.Contains("Skill_") || slotName.Contains("MainSkill"))
            {
                skill = skillTree_elements.FindSkillBySlot(slot);

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
                if (skillTree_elements.LevelUpSkill(slot))
                    playerSkill.skillPoints--;
            }

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            skillPointsLabel.text = "Skill Points: " + playerSkill.skillPoints;

            foreach (var skillSlot in skillTree_elements.skillSlotList)
            {
                VisualElement slot = skillSlot.slot;
                ApplySelectedVfx(slot, slot == selectedSkillSlot);
            }
        }

        private IEnumerator PopUpSkillSpecific(VisualElement slot)
        {

            Skill skill = skillTree_elements.FindSkillBySlot(slot);

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

            isFadingFinished = true;
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
