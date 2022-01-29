using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    public class MainMenu : UI
    {
        [SerializeField] private string gameSceneName;

        private VisualElement mainMenuPanel;

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

        private void Awake()
        {
            Initialize();

            mainMenuPanel = root.Q<VisualElement>("MainMenuPanel");
            optionsPanel = root.Q<VisualElement>("OptionsPanel");
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
            SceneManager.LoadScene(gameSceneName);
        }

        // open option menu
        private void OptionsButtonPressed()
        {
            StartCoroutine(FadeTo(mainMenuPanel, optionsPanel));
        }

        // quit the game
        private void QuitGameButtonPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        private void AudioButtonPressed()
        {
            // todo - load audio ui screen
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
        }
    }
}
