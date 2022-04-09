using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class OnScreenTipControl : UI
    {
        [SerializeField] private bool isDisplayed = false;

        private VisualElement panel;
        private Label label;

        private Coroutine displayCoroutine;
        private Coroutine hideCoroutine;
        
        private static OnScreenTipControl instance;
        public static OnScreenTipControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            panel = root.Q<VisualElement>("Panel");
            label = panel.Q<Label>("Text");

            panel.style.display = DisplayStyle.None;
        }

        public void Display(string text)
        {
            label.text = text;
            if (!isDisplayed)
            {
                isDisplayed = true;

                if (hideCoroutine != null)
                    StopCoroutine(hideCoroutine);

                displayCoroutine = StartCoroutine(Display());
            }
        }

        public void Hide()
        {
            label.text = "";
            if (isDisplayed)
            {
                isDisplayed = false;

                if (displayCoroutine != null)
                    StopCoroutine(displayCoroutine);

                hideCoroutine = StartCoroutine(FadeOut(panel));
            }
        }

        private IEnumerator Display()
        {
            label.style.opacity = 0f;
            yield return StartCoroutine(FadeIn(panel));
            yield return StartCoroutine(FadeIn(label));
        }
    }
}
