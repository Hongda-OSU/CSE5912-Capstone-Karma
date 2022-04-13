using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace CSE5912.PolyGamers
{
    public class EscapeMenu : UI
    {
        [SerializeField] private int startSceneIndex;

        [SerializeField] private AudioSource clickSound;

        private VisualElement optionsPanel;
        private VisualElement audioPanel;

        private Button audioButton;
        private Button KeybindingsButton;
        //private Button resolutionButton;
        //private Button languageButton;
        private Button exitButton;

        private bool isFadingComplete = true;


        private static EscapeMenu instance;
        public static EscapeMenu Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            optionsPanel = root.Q<VisualElement>("OptionsPanel");
            audioPanel = root.Q<VisualElement>("AudioPanel");

            root.style.display = DisplayStyle.None;

            ResetPanel();
        }

        private void Start()
        {

            // set up buttons
            audioButton = root.Q<Button>("Audio");
            audioButton.clicked += AudioButtonPressed;

            KeybindingsButton = root.Q<Button>("Keybindings");
            KeybindingsButton.clicked += KeybindingsButtonPressed;

            exitButton = root.Q<Button>("Exit");
            exitButton.clicked += delegate { StartCoroutine(ExitButtonPressed()); };

        }

        private void ResetPanel()
        {
            optionsPanel.style.display = DisplayStyle.Flex;
            optionsPanel.style.opacity = 1f;

            audioPanel.style.display = DisplayStyle.None;
        }

        public IEnumerator DisplayMenu(bool enabled)
        {
            if (!isFadingComplete)
                yield break;

            isFadingComplete = false;
            if (!enabled)
            {
                yield return StartCoroutine(FadeOut(root));
                ResetPanel();
            }
            else
            {
                yield return StartCoroutine(FadeIn(root));
            }
            isFadingComplete = true;
        }



        private void AudioButtonPressed()
        {
            StartCoroutine(FadeTo(optionsPanel, audioPanel));
            clickSound.Play();
        }

        private void KeybindingsButtonPressed()
        {
            // todo - load keybindings ui screen
        }

        // go back to previous UI
        private IEnumerator ExitButtonPressed()
        {
            clickSound.Play();

            yield return StartCoroutine(FadeOut(root));

            DontDestroy.Instance.Destroy();

            BgmControl.Instance.SmoothMusicVolume(0f);

            PlayerStats.Instance.IsInvincible = true;

            SceneLoader.Instance.LoadLevel(startSceneIndex, GameStateController.Instance.karmicLevel);
        }
        public bool IsFadingComplete { get { return isFadingComplete; } }
    }
}
