using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillsPanelControl : UI
    {
        [SerializeField] SkillSlotsControl skillSlotsControl;

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            foreach (var skillSlot in skillSlotsControl.electroSkillSlotList)
            {
                skillSlot.slot.RegisterCallback<MouseDownEvent>(evt => SkillSlot_performed(skillSlot.slot));
            }
        }

        private void SkillSlot_performed(VisualElement slot)
        {
            skillSlotsControl.LevelUpSkill(slot);
            Debug.Log(slot.name);
        }
    }
}
