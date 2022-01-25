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
        [SerializeField] private GameObject options;

        private Label gameNameText;
        private Button startGameButton;
        private Button optionsButton;
        private Button quitGameButton;

        private void Start()
        {
            Initialize();

            gameNameText = background.Q<Label>("GameName");

            startGameButton = background.Q<Button>("StartGame");
            startGameButton.clicked += StartGameButtonPressed;

            optionsButton = background.Q<Button>("Options");
            optionsButton.clicked += OptionsButtonPressed;

            quitGameButton = background.Q<Button>("QuitGame");
            quitGameButton.clicked += QuitGameButtonPressed;
        }
        
        // start main game scene
        private void StartGameButtonPressed()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        // open option menu
        private void OptionsButtonPressed()
        {
            StartCoroutine(LoadUI(options)); 
        }

        // quit game
        private void QuitGameButtonPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
