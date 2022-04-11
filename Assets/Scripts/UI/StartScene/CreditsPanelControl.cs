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
        [TextArea(5, 10)]
        [SerializeField] private string gameEndingClaim;
        private List<string> creditsList = new List<string>();

        [SerializeField] private bool isDisplaying = false;
        [SerializeField] private float displayTime = 3f;

        private VisualElement panel;
        private Label credit;

        private VisualElement previousPanel;

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

            previousPanel = root.Q<VisualElement>("OptionsPanel");

            creditsList = new List<string>(credits);
        }

        private void Update()
        {
            if (!isDisplaying)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isDisplaying = false;
                StartCoroutine(FadeTo(panel, previousPanel));
                StopCoroutine(display);
            }
        }

        public void StartDisplay(bool isGameEnding)
        {
            if (isDisplaying)
                return;

            display = DisplayCredits(isGameEnding);
            StartCoroutine(display);
        }

        private IEnumerator DisplayCredits(bool isGameEnding)
        {
            isDisplaying = true;

            credit.text = "";

            if (isGameEnding)
            {
                creditsList.Add(gameEndingClaim);
                previousPanel = root.Q<VisualElement>("MainMenuPanel");
            }
            else
            {
                previousPanel = root.Q<VisualElement>("OptionsPanel");
            }

            yield return new WaitForSeconds(displayTime);

            panel.style.display = DisplayStyle.Flex;
            credit.style.display = DisplayStyle.None;

            for (int i = 0; i < creditsList.Count; i++)
            {
                yield return StartCoroutine(FadeOut(credit));

                credit.text = creditsList[i];

                yield return StartCoroutine(FadeIn(credit));

                yield return new WaitForSeconds(displayTime);
            }

            yield return new WaitForSeconds(displayTime);
            yield return StartCoroutine(FadeOut(credit));
            isDisplaying = false;

            if (isGameEnding)
            {
                creditsList.Remove(gameEndingClaim);
            }
            StartCoroutine(FadeTo(panel, previousPanel));

            StartSceneMenu.Instance.audioSource.clip = StartSceneMenu.Instance.menuBgm;
            StartSceneMenu.Instance.audioSource.Play();
            StartSceneMenu.Instance.audioSource.loop = true;
        }
    }
}
