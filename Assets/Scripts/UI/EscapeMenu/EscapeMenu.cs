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
            optionsPanel.style.display = DisplayStyle.Flex;
            audioPanel.style.display = DisplayStyle.None;
        }

        private void Start()
        {

            // set up buttons
            audioButton = root.Q<Button>("Audio");
            audioButton.clicked += AudioButtonPressed;

            KeybindingsButton = root.Q<Button>("Keybindings");
            KeybindingsButton.clicked += KeybindingsButtonPressed;

            exitButton = root.Q<Button>("Exit");
            exitButton.clicked += ExitButtonPressed;

        }

        public IEnumerator DisplayMenu(bool enabled)
        {
            if (!isFadingComplete)
                yield break;

            isFadingComplete = false;
            if (!enabled)
            {
                yield return StartCoroutine(FadeOut(root));
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
        private void ExitButtonPressed()
        {
            clickSound.Play();

            DontDestroy.Instance.Destroy();

            IngameAudioControl.Instance.SmoothMusicVolume(0f);

            SceneLoader.Instance.LoadLevel(startSceneIndex);
        }
        public bool IsFadingComplete { get { return isFadingComplete; } }
    }
}
