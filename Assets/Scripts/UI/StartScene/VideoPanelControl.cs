using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class VideoPanelControl : UI
    {
        [SerializeField] private AudioSource clickSound;

        private VisualElement panel;

        private Button resolution;
        private List<Vector2Int> resolutionList = new List<Vector2Int>();
        private int currentResolutionIndex;

        private Button frameRate;
        private List<int> frameRateList = new List<int>();
        private int currentFrameRateIndex;

        private Button fullScreen;
        [SerializeField] private bool isFullScreened = false;

        private Button vsync;
        [SerializeField] private bool isVsynced = false;

        private Button back;


        private static VideoPanelControl instance;
        public static VideoPanelControl Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            panel = root.Q<VisualElement>("VideoPanel");

            resolution = panel.Q<VisualElement>("Resolution").Q<Button>("Button");
            frameRate = panel.Q<VisualElement>("FrameRate").Q<Button>("Button");
            fullScreen = panel.Q<VisualElement>("FullScreen").Q<Button>("Button");
            vsync = panel.Q<VisualElement>("Vsync").Q<Button>("Button");
            back = panel.Q<Button>("Back");

            resolutionList.Add(new Vector2Int(800, 600));
            resolutionList.Add(new Vector2Int(1024, 768));
            resolutionList.Add(new Vector2Int(1280, 720));
            resolutionList.Add(new Vector2Int(1366, 768));
            resolutionList.Add(new Vector2Int(1600, 900));
            resolutionList.Add(new Vector2Int(1920, 1080));
            resolutionList.Add(new Vector2Int(2560, 1440));
            resolutionList.Add(new Vector2Int(3840, 2160));
            currentResolutionIndex = 4;
            ResolutionButtonPressed();

            frameRateList.Add(30);
            frameRateList.Add(60);
            frameRateList.Add(120);
            currentFrameRateIndex = 0;
            FrameRateButtonPressed();

            isFullScreened = true;
            FullScreenButtonPressed();

            isVsynced = true;
            VsyncButtonPressed();
        }

        private void Start()
        {
            resolution.clicked += ResolutionButtonPressed;
            frameRate.clicked += FrameRateButtonPressed;
            fullScreen.clicked += FullScreenButtonPressed;
            vsync.clicked += VsyncButtonPressed;
            back.clicked += BackButtonPressed;
        }

        private void ResolutionButtonPressed()
        {
            currentResolutionIndex = (currentResolutionIndex + 1) % resolutionList.Count;

            var res = resolutionList[currentResolutionIndex];
            Screen.SetResolution(res.x, res.y, isFullScreened);

            resolution.text = res.x + " X " + res.y;
        }

        private void FrameRateButtonPressed()
        {
            currentFrameRateIndex = (currentFrameRateIndex + 1) % frameRateList.Count;

            var res = frameRateList[currentFrameRateIndex];
            Application.targetFrameRate = res;

            frameRate.text = res.ToString();
        }

        private void FullScreenButtonPressed()
        {
            isFullScreened = !isFullScreened;

            var res = resolutionList[currentResolutionIndex];
            Screen.SetResolution(res.x, res.y, isFullScreened);

            fullScreen.text = isFullScreened.ToString();
        }

        private void VsyncButtonPressed()
        {
            isVsynced = !isVsynced;

            if (isVsynced)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;

            vsync.text = isVsynced.ToString();
        }

        private void BackButtonPressed()
        {
            StartCoroutine(FadeTo(panel, root.Q<VisualElement>("OptionsPanel")));
            clickSound.Play();
        }
    }
}
