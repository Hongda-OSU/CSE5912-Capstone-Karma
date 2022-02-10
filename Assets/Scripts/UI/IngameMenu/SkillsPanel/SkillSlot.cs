using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillSlot
    {
        public VisualElement slot;
        public VisualElement icon;
        public List<VisualElement> indicatorList;
        public Skill skill;

        public SkillSlot(VisualElement skillSlot)
        {
            slot = skillSlot;

            icon = slot.Q<VisualElement>("SkillIcon");

            indicatorList = new List<VisualElement>();
            var indicators = slot.Q<VisualElement>("LevelIndicator");
            for (int i = 0; i < indicators.childCount; i++)
            {
                indicatorList.Add(indicators.Q<VisualElement>("Indicator_" + i));
            }

            SetSlotActive(false);
        }

        public void SetIndicators(int num)
        {
            for (int i = 0; i < indicatorList.Count;i++)
            {
                if (i < num)
                    indicatorList[i].style.display = DisplayStyle.Flex;
                else
                    indicatorList[i].style.display = DisplayStyle.None;
            }
        }

        public void SetSlotActive(bool isActive)
        {
            if (isActive)
            {
                icon.style.opacity = 1f;
            }
            else
            {
                icon.style.opacity = 0.5f;
                foreach (var indicator in indicatorList)
                    indicator.style.display = DisplayStyle.None;
            }
        }

        public bool LevelUp()
        {
            bool result = skill.LevelUp();

            if (skill.Level > 0)
            {
                SetSlotActive(true);

                for (int i = 0; i < indicatorList.Count; i++)
                {
                    var indicator = indicatorList[i];
                    if (i < skill.Level)
                    {
                        indicator.style.display = DisplayStyle.Flex;
                    }
                    else
                    {
                        indicator.style.display = DisplayStyle.None;
                    }
                }
            }
            return result;
        }
    }
}
