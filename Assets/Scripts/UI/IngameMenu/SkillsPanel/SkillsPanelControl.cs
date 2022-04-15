using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static CSE5912.PolyGamers.PlayerSkill;

namespace CSE5912.PolyGamers
{
    public class SkillsPanelControl : UI
    {
        [SerializeField] private AudioSource levelUpAudio;

        private PlayerSkillManager playerSkill;

        private SkillTree currentSkillTree;

        private SkillTree skillTree_element;


        private List<SkillTree> skillTreeList;
        public List<SkillTree> SkillTreeList { get { return skillTreeList; } }

        private VisualElement skillsPanel_elements;

        private VisualElement selectedSkillSlot;

        private Label skillPointsLabel;

        private VisualElement specificPanel;


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

            skillsPanel_elements = root.Q<VisualElement>("SkillsPanel");

            skillPointsLabel = skillsPanel_elements.Q<Label>("SkillPoints");

            specificPanel = skillsPanel_elements.Q<VisualElement>("SkillSpecific");

            skillTree_element = new SkillTree_element(skillsPanel_elements);

            skillTreeList = new List<SkillTree>();
            skillTreeList.Add(skillTree_element);
        }

        private void Start()
        {
            playerSkill = PlayerSkillManager.Instance;

            UpdateVisual();

            foreach (var skillSlot in skillTree_element.skillSlotList)
            {
                skillSlot.slot.RegisterCallback<MouseDownEvent>(evt => SkillSlot_performed(skillSlot.slot));
            }

            ResetPanel();
            //
            currentSkillTree = skillTree_element;
        }

        public void ResetPanel()
        {
            selectedSkillSlot = null;
            UpdateVisual();
            StartCoroutine(PopOffSpecific());
        }

        private void SelectSlot(VisualElement slot)
        {
            var skill = SelectSkillSlot(slot);

            UpdateVisual();
        }

        private PlayerSkill SelectSkillSlot(VisualElement slot)
        {
            PlayerSkill skill = null;

            string slotName = slot.name;
            if (slotName.Contains("MainSkill") || slotName.Contains("Skill_") || slotName.Contains("Buff_"))
            {
                skill = skillTree_element.FindSkillBySlot(slot);

                selectedSkillSlot = slot;
            }

            return skill;
        }

        private void SkillSlot_performed(VisualElement slot)
        {
            if (selectedSkillSlot != slot)
            {
                StartCoroutine(PopUpSkillSpecific(slot));

                SelectSlot(slot);

                return;
            }
            else 
            {
                var result = skillTree_element.LevelUpSkill(slot);
                if (result)
                    levelUpAudio.Play();
            }

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            skillPointsLabel.text = "Skill Points: " + playerSkill.SkillPoints;

            foreach (var skillSlot in skillTree_element.skillSlotList)
            {
                VisualElement slot = skillSlot.slot;
                ApplySelectedVfx(slot, slot == selectedSkillSlot);
            }
        }

        private IEnumerator PopUpSkillSpecific(VisualElement slot)
        {

            PlayerSkill skill = skillTree_element.FindSkillBySlot(slot);

            if (skill == null)
            {
                yield return StartCoroutine(PopOffSpecific());
            }
            else if (selectedSkillSlot != slot)
            {
                PopUpSpecific(skill);
            }
        }

        private void PopUpSpecific(PlayerSkill skill)
        {
            specificPanel.Q<Label>("Name").text = skill.Name;
            specificPanel.Q<Label>("Type").text = skill.Type.ToString();
            specificPanel.Q<Label>("Description").text = skill.Description;
            specificPanel.Q<Label>("Citation").text = "";

            specificPanel.style.opacity = 1f;
            specificPanel.style.display = DisplayStyle.Flex;
        }

        private IEnumerator PopOffSpecific()
        {

            selectedSkillSlot = null;

            specificPanel.style.opacity = 0f;
            yield return null;
        }


        public SkillTree CurrentSkillTree { get { return currentSkillTree; } }
        public SkillTree SkillTree_element { get { return skillTree_element; } }
    }
}
