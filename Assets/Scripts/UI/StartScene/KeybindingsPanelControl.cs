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

        public AudioPanelControl.SlideControl mouseSensitivity;

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

            mouseSensitivity = new AudioPanelControl.SlideControl(panel.Q<VisualElement>("Mouse"));
        }

        private void Start()
        {
            AssignCallback(mouseSensitivity);
            SetSensitivity(mouseSensitivity, DataManager.Instance.mouseSensitivity);

            back.clicked += BackButtonPressed;
        }

        private void AssignCallback(AudioPanelControl.SlideControl slideControl)
        {
            var thresholdList = slideControl.thresholdList;
            for (int i = 0; i < thresholdList.Count; i++)
            {
                int magnitude = i;
                thresholdList[i].RegisterCallback<MouseOverEvent>(evt => StartCoroutine(OnMouseHover(slideControl, magnitude)));
                thresholdList[i].RegisterCallback<MouseOutEvent>(evt => slideControl.isMouseHovering = false);
            }
        }

        private IEnumerator OnMouseHover(AudioPanelControl.SlideControl slideControl, int magnitude)
        {
            if (slideControl.isMouseHovering)
                yield break;

            slideControl.isMouseHovering = true;

            while (slideControl.isMouseHovering)
            {
                if (Input.GetMouseButton(0))
                {
                    SetSensitivity(slideControl, magnitude);
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        public void SetSensitivity(AudioPanelControl.SlideControl slideControl, int magnitude)
        {
            var value = Mathf.Clamp(magnitude / 10f, 0.01f, 1f);
            var volume = slideControl.volume;

            volume.style.width = value * 400f;

            slideControl.magnitude.text = magnitude.ToString();

            if (FPSMouseLook.Instance != null)
                FPSMouseLook.Instance.MouseSensitivity = Mathf.Clamp(magnitude * 2f, 1f, 20f);
        }

        private void BackButtonPressed()
        {
            StartCoroutine(FadeTo(panel, root.Q<VisualElement>("OptionsPanel")));
            clickSound.Play();
        }

    }
}