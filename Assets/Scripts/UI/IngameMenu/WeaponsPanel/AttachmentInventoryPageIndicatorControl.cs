using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class AttachmentInventoryPageIndicatorControl : UI
    {
        private List<VisualElement> indicatorList;

        private VisualElement pageIndicators;

        private static AttachmentInventoryPageIndicatorControl instance;
        public static AttachmentInventoryPageIndicatorControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            Initialize();

            pageIndicators = root.Q<VisualElement>("AttachmentInventoryPageIndicators");

            indicatorList = new List<VisualElement>();

            for (int i = 0; i < pageIndicators.childCount; i++)
            {
                indicatorList.Add(pageIndicators.Q<VisualElement>("Page_" + i));

                if (i > 0)
                    indicatorList[i].style.display = DisplayStyle.None;
            }

        }

        public void SetIndicators(int index, int totalNum)
        {
            for (int i = 0; i < indicatorList.Count; i++)
            {
                var indicator = indicatorList[i];

                if (i < totalNum)
                {
                    indicator.style.display = DisplayStyle.Flex;

                    SetIndicatorActive(i, i == index);
                }

                else
                    indicator.style.display = DisplayStyle.None;

            }
        }

        private void SetIndicatorActive(int index, bool isActive)
        {
            if (isActive)
            {
                indicatorList[index].style.opacity = 1;
            }
            else
            {
                indicatorList[index].style.opacity = 0.3f;
            }
        }

    }
}
