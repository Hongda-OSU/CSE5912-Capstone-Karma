using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillInformationControl : UI
    {
        [SerializeField] private float mainSkillIconSize = 100f;

        private PlayerSkill mainSkill;
        private VisualElement mainSkillIcon;
        private VisualElement mainSkillIconShade;


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

            mainSkillIcon.style.backgroundImage = null;
            mainSkillIconShade.style.height = mainSkillIconSize;
        }

        private void Update()
        {
            if (mainSkill == null)
                return;

            mainSkillIconShade.style.height = Mathf.Lerp(mainSkillIconSize, 0f, mainSkill.TimeSince / mainSkill.Cooldown);
        }

        // need to be re-written if new skill trees are added to the game
        public void SetupMainSkill(PlayerSkill mainSkill)
        {
            this.mainSkill = mainSkill;

            var skillSlot = SkillsPanelControl.Instance.CurrentSkillTree.FindSlotBySkill(mainSkill);

            mainSkillIcon.style.backgroundImage = new StyleBackground(skillSlot.icon.resolvedStyle.backgroundImage);
            mainSkillIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }
    }
}
