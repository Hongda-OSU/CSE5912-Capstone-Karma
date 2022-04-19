using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class TipsControl : UI
    {
        [SerializeField] private bool isDisplayed = false;

        private VisualElement tips;

        private Label key;
        private Label action;

        private static TipsControl instance;
        public static TipsControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            Initialize();

            tips = root.Q<VisualElement>("Tips");
            key = tips.Q<Label>("Key");
            action = tips.Q<Label>("Action");

            tips.style.display = DisplayStyle.None;
        }

        public void PopUp(string keyTip, string actionTip)
        {
            key.text = keyTip;
            action.text = actionTip;

            if (isDisplayed)
                return;

            isDisplayed = true;

            StartCoroutine(FadeIn(tips));
        }
        public void PopOff()
        {
            if (!isDisplayed)
                return;

            isDisplayed = false;

            StartCoroutine(FadeOut(tips));
        }
    }
}
