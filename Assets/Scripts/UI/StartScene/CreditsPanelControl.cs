using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class CreditsPanelControl : UI
    {
        [TextArea(5, 10)]
        [SerializeField] private string[] credits;

        [SerializeField] private bool isDisplaying = false;
        [SerializeField] private float displayTime = 3f;

        private VisualElement panel;
        private Label credit;

        private IEnumerator display;

        private static CreditsPanelControl instance;
        public static CreditsPanelControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            panel = root.Q<VisualElement>("CreditsPanel");
            credit = panel.Q<Label>("Credit");

            panel.style.display = DisplayStyle.None;
            credit.style.display = DisplayStyle.None;
        }

        private void Update()
        {
            if (!isDisplaying)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isDisplaying = false;
                StartCoroutine(FadeTo(panel, root.Q<VisualElement>("OptionsPanel")));
                StopCoroutine(display);
            }
        }

        public void StartDisplay()
        {
            display = DisplayCredits();
            StartCoroutine(display);
        }
        private IEnumerator DisplayCredits()
        {
            isDisplaying = true;

            credit.text = "";

            yield return new WaitForSeconds(displayTime);

            panel.style.display = DisplayStyle.Flex;
            credit.style.display = DisplayStyle.None;

            for (int i = 0; i < credits.Length; i++)
            {
                yield return StartCoroutine(FadeOut(credit));

                credit.text = credits[i];

                yield return StartCoroutine(FadeIn(credit));

                yield return new WaitForSeconds(displayTime);
            }
            isDisplaying = false;
            StartCoroutine(FadeTo(panel, root.Q<VisualElement>("OptionsPanel")));
        }
    }
}
