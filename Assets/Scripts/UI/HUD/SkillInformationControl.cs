using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillInformationControl : UI
    {
        [SerializeField] private float mainSkillIconSize = 100f;
        [SerializeField] private float setSkillIconSize = 66f;

        private PlayerSkill mainSkill;
        private VisualElement mainSkillIcon;
        private VisualElement mainSkillIconShade;

        private PlayerSkill setSkill;
        private VisualElement setSkillIcon;
        private VisualElement setSkillIconShade;

        private static SkillInformationControl instance;
        public static SkillInformationControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            mainSkillIcon = root.Q<VisualElement>("MainSkill");
            mainSkillIconShade = mainSkillIcon.Q<VisualElement>("Shade");

            setSkillIcon = root.Q<VisualElement>("SetSkill");
            setSkillIconShade = setSkillIcon.Q<VisualElement>("Shade");


            mainSkillIcon.style.backgroundImage = null;
            mainSkillIconShade.style.height = mainSkillIconSize;

            setSkillIcon.style.backgroundImage = null;
            setSkillIconShade.style.height = setSkillIconSize;
        }

        private void Update()
        {
            UpdateMainSkillDisplay();

            UpdateSetSkillDisplay();
        }

        private void UpdateMainSkillDisplay()
        {
            if (mainSkill == null)
                return;

            mainSkillIconShade.style.height = Mathf.Lerp(mainSkillIconSize, 0f, mainSkill.TimeSincePerformed / mainSkill.Cooldown);
        }
        // need to be re-written if new skill trees are added to the game
        public void SetupMainSkill(PlayerSkill mainSkill)
        {
            this.mainSkill = mainSkill;

            if (mainSkill == null)
            {
                mainSkillIcon.style.backgroundImage = null;
            }
            else
            {
                var skillSlot = SkillsPanelControl.Instance.CurrentSkillTree.FindSlotBySkill(mainSkill);

                mainSkillIcon.style.backgroundImage = new StyleBackground(skillSlot.icon.resolvedStyle.backgroundImage);
            }
            mainSkillIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }


        private void UpdateSetSkillDisplay()
        {
            if (setSkill == null)
                return;

            setSkillIconShade.style.height = Mathf.Lerp(setSkillIconSize, 0f, setSkill.TimeSincePerformed / setSkill.Cooldown);
        }
        public void SetupSetSkill(PlayerSkill setSkill)
        {
            this.setSkill = setSkill;

            if (setSkill == null)
                return;

            setSkillIcon.style.backgroundImage = new StyleBackground(setSkill.Icon);
            setSkillIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }



        public override void Display(bool enabled)
        {
            if (enabled)
            {
                mainSkillIcon.style.display = DisplayStyle.Flex;
                setSkillIcon.style.display = DisplayStyle.Flex;
            }
            else
            {
                mainSkillIcon.style.display = DisplayStyle.None;
                setSkillIcon.style.display = DisplayStyle.None;
            }
        }
    }
}
