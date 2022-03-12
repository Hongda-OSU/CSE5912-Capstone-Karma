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

        [SerializeField] private AudioSource bgm;
        [SerializeField] private AudioSource clickSound;

        private VisualElement mainMenuPanel;
        private VisualElement audioPanel;

        private Button startGameButton;
        private Button optionsButton;
        private Button quitGameButton;


        private VisualElement optionsPanel;

        private Button audioButton;
        private Button KeybindingsButton;
        private Button creditsButton;
        //private Button resolutionButton;
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

            mainMenuPanel.style.display = DisplayStyle.Flex;
            optionsPanel.style.display = DisplayStyle.None;
            audioPanel.style.display = DisplayStyle.None;
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

            backButton = root.Q<Button>("Back");
            backButton.clicked += BackButtonPressed;

            // not showing at default
            optionsPanel.style.display = DisplayStyle.None;
        }
        
        // load the main game scene
        private void StartGameButtonPressed()
        {
            StartCoroutine(FadeOutBgm());
            SceneLoader.Instance.LoadLevel(gameSceneIndex);
            clickSound.Play();
            root.style.display = DisplayStyle.None;
        }
        private IEnumerator FadeOutBgm()
        {
            float timeSince = 0f;
            float fadeoutTime = 0.5f;
            while (timeSince < fadeoutTime)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

                bgm.volume = fadeoutTime - timeSince;
            }
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
            // todo - load keybindings ui screen
        }

        private void CreditsButtonPressed()
        {
            // todo - load credits screen
        }

        // go back to previous UI
        private void BackButtonPressed()
        {
            StartCoroutine(FadeTo(optionsPanel, mainMenuPanel));
            clickSound.Play();
        }
    }
}
