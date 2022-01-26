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
        [SerializeField] private UI options;

        private Button startGameButton;
        private Button optionsButton;
        private Button quitGameButton;

        private void Start()
        {
            Initialize();

            // set up buttons
            startGameButton = root.Q<Button>("StartGame");
            startGameButton.clicked += StartGameButtonPressed;

            optionsButton = root.Q<Button>("Options");
            optionsButton.clicked += OptionsButtonPressed;

            quitGameButton = root.Q<Button>("QuitGame");
            quitGameButton.clicked += QuitGameButtonPressed;
        }
        
        // load the main game scene
        private void StartGameButtonPressed()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        // open option menu
        private void OptionsButtonPressed()
        {
            LoadUI(this, options); 
        }

        // quit the game
        private void QuitGameButtonPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
