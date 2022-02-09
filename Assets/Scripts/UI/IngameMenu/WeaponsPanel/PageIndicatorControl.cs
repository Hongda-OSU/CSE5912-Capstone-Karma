using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class PageIndicatorControl : UI
    {
        private List<VisualElement> indicatorList;

        private VisualElement pageIndicators;

        private void Awake()
        {
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

        public void SetIndicatorNum(int num)
        {
            for (int i = 0; i < indicatorList.Count; i++)
            {
                if (i < num)
                    indicatorList[i].style.display = DisplayStyle.Flex;

                else
                    indicatorList[i].style.display = DisplayStyle.None;
            }
        }

    }
}
