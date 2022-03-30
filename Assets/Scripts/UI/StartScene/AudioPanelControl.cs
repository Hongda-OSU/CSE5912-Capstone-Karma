using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;

namespace CSE5912.PolyGamers
{
    public class AudioPanelControl : UI
    {
        private class VolumeControl
        {
            public Label label;
            public VisualElement volume;
            public Label magnitude;
            public List<VisualElement> thresholdList = new List<VisualElement>();

            public bool isMouseHovering = false;

            public VolumeControl(VisualElement element)
            {
                label = element.Q<Label>("Label");
                volume = element.Q<VisualElement>("Volume");
                magnitude = element.Q<Label>("Magnitude");

                var thresholds = element.Q<VisualElement>("Thresholds");
                for (int i = 0; i < thresholds.childCount; i++)
                {
                    thresholdList.Add(thresholds.Q<VisualElement>(i.ToString()));
                }

            }

        }

        [SerializeField] private AudioMixer mixer;

        private VisualElement audioPanel;

        private VolumeControl master;
        private VolumeControl music;
        private VolumeControl effect;

        private Button backButton;

        private void Awake()
        {
            Initialize();

            audioPanel = root.Q<VisualElement>("AudioPanel");

            master = new VolumeControl(audioPanel.Q<VisualElement>("Master"));
            music = new VolumeControl(audioPanel.Q<VisualElement>("Music"));
            effect = new VolumeControl(audioPanel.Q<VisualElement>("Effect"));

            backButton = audioPanel.Q<Button>("Back");
        }

        private void Start()
        {
            AssignCallback(master);
            AssignCallback(music);
            AssignCallback(effect);

            SetVolume(master, 10);
            SetVolume(music, 10);
            SetVolume(effect, 10);

            backButton.clicked += BackButtonPressed;
        }


        private void AssignCallback(VolumeControl volumeControl)
        {
            var thresholdList = volumeControl.thresholdList;
            for (int i = 0; i < thresholdList.Count; i++)
            {
                int magnitude = i;
                thresholdList[i].RegisterCallback<MouseOverEvent>(evt => StartCoroutine(OnMouseHover(volumeControl, magnitude)));
                thresholdList[i].RegisterCallback<MouseOutEvent>(evt => volumeControl.isMouseHovering = false);
            }
        }

        private IEnumerator OnMouseHover(VolumeControl volumeControl, int magnitude)
        {
            if (volumeControl.isMouseHovering)
                yield break;

            volumeControl.isMouseHovering = true;

            while (volumeControl.isMouseHovering)
            {
                if (Input.GetMouseButton(0))
                {
                    SetVolume(volumeControl, magnitude);
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        private void SetVolume(VolumeControl volumeControl, int magnitude)
        {
            var value = Mathf.Clamp(magnitude / 10f, 0.01f, 1f);
            var volume = volumeControl.volume;

            volume.style.width = value * 400f;

            volumeControl.magnitude.text = magnitude.ToString();

            mixer.SetFloat(volumeControl.label.text, Mathf.Log(value) * 20f);
        }

        private void BackButtonPressed()
        {
            StartCoroutine(FadeTo(audioPanel, root.Q<VisualElement>("OptionsPanel")));
        }
    }
}
