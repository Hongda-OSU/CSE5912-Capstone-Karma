using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SceneLoader : UI
    {
        [SerializeField] private Sprite image;

        private VisualElement loadingScreen;

        private VisualElement loadingImage;

        private VisualElement loadingBar;
        private VisualElement progressBar;
        [SerializeField] private float barWidth = 1350f;

        private bool isLoading = false;

        private static SceneLoader instance;
        public static SceneLoader Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            loadingScreen = root.Q<VisualElement>("LoadingScreen");

            loadingImage = root.Q<VisualElement>("LoadingImage");
            loadingImage.style.backgroundImage = new StyleBackground(image);
            loadingImage.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

            loadingBar = root.Q<VisualElement>("LoadingBar");
            loadingBar.style.width = barWidth;

            progressBar = root.Q<VisualElement>("ProgressBar");
            progressBar.style.width = 0f;

            root.style.display = DisplayStyle.None;
        }

        // test
        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.M))
            {
                LoadLevel(2);
            }

        }

        public void LoadLevel(int sceneIndex)
        {
            if (DropoffManager.Instance != null)
                DropoffManager.Instance.ClearDropoffs();

            StartCoroutine(LoadAsync(sceneIndex));
        }

        private IEnumerator LoadAsync(int sceneIndex)
        {
            if (isLoading)
                yield break;

            isLoading = true;

            loadingScreen.style.display = DisplayStyle.None;

            yield return StartCoroutine(FadeIn(root));

            yield return StartCoroutine(FadeIn(loadingScreen));


            progressBar.style.width = 0f;

            bool isReady = false;
            float timeSince = 0f;
            float time = IngameAudioControl.Instance.FadeoutTime;
            while (!isReady)
            {
                if (timeSince >= time)
                    isReady = true;

                float progress = timeSince / time * 0.5f;

                progressBar.style.width = progress * barWidth;

                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f) * 0.25f + 0.75f;

                progressBar.style.width = progress * barWidth;

                yield return new WaitForSeconds(Time.deltaTime);
            }

            root.style.display = DisplayStyle.None;

            isLoading = false;
        }
    }
}
