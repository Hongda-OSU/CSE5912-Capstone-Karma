using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    public class StartSceneMenu : UI
    {
        [SerializeField] private int gameSceneIndex;

        [SerializeField] private float displayDelay = 2f;
        private float timeSince = 0f;
        [SerializeField] private bool isDisplayedOnStart = false;
        [SerializeField] private float comeoutTime = 2f;

        [SerializeField] private bool isGameEnding = false;

        public AudioSource audioSource;
        public AudioClip menuBgm;
        public AudioClip creditsBgm;
        public AudioSource clickSound;

        private VisualElement mainMenuPanel;
        private VisualElement audioPanel;

        private Button startGameButton;
        private Button optionsButton;
        private Button quitGameButton;


        private VisualElement optionsPanel;
        private VisualElement creditsPanel;
        private VisualElement profilesPanel;
        private VisualElement keybindingsPanel;
        private VisualElement videoPanel;

        private Button audioButton;
        private Button KeybindingsButton;
        private Button creditsButton;
        private Button videoButton;
        //private Button languageButton;
        private Button backButton;


        private static StartSceneMenu instance;
        public static StartSceneMenu Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            mainMenuPanel = root.Q<VisualElement>("MainMenuPanel");
            optionsPanel = root.Q<VisualElement>("OptionsPanel");
            audioPanel = root.Q<VisualElement>("AudioPanel");
            creditsPanel = root.Q<VisualElement>("CreditsPanel");
            profilesPanel = root.Q<VisualElement>("ProfilesPanel");
            keybindingsPanel = root.Q<VisualElement>("KeybindingsPanel");
            videoPanel = root.Q<VisualElement>("VideoPanel");

            mainMenuPanel.style.display = DisplayStyle.None;
            optionsPanel.style.display = DisplayStyle.None;
            audioPanel.style.display = DisplayStyle.None;
            creditsPanel.style.display = DisplayStyle.None;
            profilesPanel.style.display = DisplayStyle.None;
            keybindingsPanel.style.display = DisplayStyle.None;
            videoPanel.style.display = DisplayStyle.None;

            audioSource.clip = menuBgm;
            audioSource.Play();
        }

        private void Start()
        {
            // set up buttons
            startGameButton = root.Q<Button>("StartGame");
            startGameButton.clicked += StartGameButtonPressed;

            optionsButton = root.Q<Button>("Options");
            optionsButton.clicked += OptionsButtonPressed;

            quitGameButton = root.Q<Button>("QuitGame");
            quitGameButton.clicked += QuitGameButtonPressed;


            // set up buttons
            audioButton = root.Q<Button>("Audio");
            audioButton.clicked += AudioButtonPressed;

            KeybindingsButton = root.Q<Button>("Keybindings");
            KeybindingsButton.clicked += KeybindingsButtonPressed;

            creditsButton = root.Q<Button>("Credits");
            creditsButton.clicked += CreditsButtonPressed;

            videoButton = root.Q<Button>("Video");
            videoButton.clicked += VideoButtonPressed;

            backButton = root.Q<Button>("Back");
            backButton.clicked += BackButtonPressed;

            // not showing at default
            optionsPanel.style.display = DisplayStyle.None;
        }

        private void Update()
        {
            if (!isGameEnding && !isDisplayedOnStart)
            {
                timeSince += Time.deltaTime;
                if (timeSince > displayDelay)
                {
                    isDisplayedOnStart = true;
                    StartCoroutine(FadeIn(mainMenuPanel, comeoutTime));
                }
            }
        }
        // load the main game scene
        private void StartGameButtonPressed()
        {
            StartCoroutine(FadeTo(mainMenuPanel, profilesPanel));
            clickSound.Play();
        }


        // open option menu
        private void OptionsButtonPressed()
        {
            StartCoroutine(FadeTo(mainMenuPanel, optionsPanel));
            clickSound.Play();
        }

        // quit the game
        private void QuitGameButtonPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            clickSound.Play();
            Application.Quit();
        }

        private void AudioButtonPressed()
        {
            StartCoroutine(FadeTo(optionsPanel, audioPanel));
            clickSound.Play();
        }

        private void KeybindingsButtonPressed()
        {
            StartCoroutine(FadeTo(optionsPanel, keybindingsPanel));
            clickSound.Play();
        }
        private void VideoButtonPressed()
        {
            StartCoroutine(FadeTo(optionsPanel, videoPanel));
            clickSound.Play();
        }

        private void CreditsButtonPressed()
        {
            audioSource.clip = creditsBgm;
            audioSource.Play();

            StartCoroutine(FadeTo(optionsPanel, creditsPanel));
            clickSound.Play();

            CreditsPanelControl.Instance.StartDisplay(false);
        }

        // go back to previous UI
        private void BackButtonPressed()
        {
            StartCoroutine(FadeTo(optionsPanel, mainMenuPanel));
            clickSound.Play();
        }

        public void PlayGameEnding()
        {
            isGameEnding = true;

            audioSource.clip = creditsBgm; 
            audioSource.loop = false;
            audioSource.Play();

            mainMenuPanel.style.display = DisplayStyle.None;
            optionsPanel.style.display = DisplayStyle.None;
            audioPanel.style.display = DisplayStyle.None;
            creditsPanel.style.display = DisplayStyle.None;
            profilesPanel.style.display = DisplayStyle.None;
            keybindingsPanel.style.display = DisplayStyle.None;
            videoPanel.style.display = DisplayStyle.None;

            CreditsPanelControl.Instance.StartDisplay(true);
        }
    }
}
