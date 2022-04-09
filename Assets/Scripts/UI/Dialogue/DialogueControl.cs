using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class DialogueControl : UI
    {
        [SerializeField] private bool isDisplayed = false;

        [SerializeField] private bool isInterrupted = false;
        [SerializeField] private float timeToReDisplay = 3f;
        [SerializeField] private float timeSinceInterrupted = 0f;

        private VisualElement panel;
        private Label label;

        private Coroutine displayCoroutine;
        private Coroutine hideCoroutine;
        
        private static DialogueControl instance;
        public static DialogueControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            timeSinceInterrupted = timeToReDisplay;

            panel = root.Q<VisualElement>("Panel");
            label = panel.Q<Label>("Text");

            panel.style.display = DisplayStyle.None;
        }

        private void Update()
        {
            if (isInterrupted && !isDisplayed)
            {
                timeSinceInterrupted += Time.deltaTime;
                if (timeSinceInterrupted >= timeToReDisplay)
                {
                    isInterrupted = false;
                }
            }

            if (WeaponManager.Instance.CarriedWeapon.wasBulletFiredThisFrame)
            {
                HideImmediate();
            }
        }

        public void Display(string text)
        {
            if (!isDisplayed && !isInterrupted)
            {
                isDisplayed = true;

                label.text = text;

                if (hideCoroutine != null)
                    StopCoroutine(hideCoroutine);

                displayCoroutine = StartCoroutine(Display());
            }
        }

        public void Hide()
        {
            if (isDisplayed && !isInterrupted)
            {
                isDisplayed = false;

                if (displayCoroutine != null)
                    StopCoroutine(displayCoroutine);

                hideCoroutine = StartCoroutine(FadeOut(panel));
            }
        }

        public void HideImmediate()
        {
            isInterrupted = true;
            timeSinceInterrupted = 0f;

            isDisplayed = false;

            if (displayCoroutine != null)
                StopCoroutine(displayCoroutine);

            if (hideCoroutine != null)
                StopCoroutine(hideCoroutine);

            panel.style.display = DisplayStyle.None;
        }

        private IEnumerator Display()
        {
            label.style.opacity = 0f;
            yield return StartCoroutine(FadeIn(panel));
            yield return StartCoroutine(FadeIn(label));
        }

    }
}
