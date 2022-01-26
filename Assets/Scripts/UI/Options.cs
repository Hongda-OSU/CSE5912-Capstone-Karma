using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class Options : UI
    {
        [SerializeField] UI parentUI;

        private Button audioButton;
        private Button KeybindingsButton;
        private Button creditsButton;
        //private Button resolutionButton;
        //private Button languageButton;

        private Button backButton;

        private void Start()
        {
            Initialize();

            audioButton = root.Q<Button>("Audio");
            audioButton.clicked += AudioButtonPressed;

            KeybindingsButton = root.Q<Button>("Keybindings");
            KeybindingsButton.clicked += KeybindingsButtonPressed;

            creditsButton = root.Q<Button>("Credits");
            creditsButton.clicked += CreditsButtonPressed;

            backButton = root.Q<Button>("Back");
            backButton.clicked += BackButtonPressed;

            root.style.display = DisplayStyle.None;
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

        private void BackButtonPressed()
        {
            LoadUI(this, parentUI);
        }
    }
}
