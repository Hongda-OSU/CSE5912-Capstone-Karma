using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class KeybindingsPanelControl : UI
    {
        [SerializeField] private AudioSource clickSound;

        private VisualElement panel;

        private Button back;


        private static KeybindingsPanelControl instance;
        public static KeybindingsPanelControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            Initialize();

            panel = root.Q<VisualElement>("KeybindingsPanel");
            back = panel.Q<Button>("Back");
        }

        private void Start()
        {
            back.clicked += BackButtonPressed;
        }
        private void BackButtonPressed()
        {
            StartCoroutine(FadeTo(panel, root.Q<VisualElement>("OptionsPanel")));
            clickSound.Play();
        }

    }
}