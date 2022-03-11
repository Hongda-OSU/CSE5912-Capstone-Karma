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
                LoadLevel("Scenes/Main/Level_2_test");
            }
        }

        public void LoadLevel(string sceneName)
        {
            if (DropoffManager.Instance != null)
                DropoffManager.Instance.ClearDropoffs();

            StartCoroutine(LoadAsync(sceneName));
        }

        private IEnumerator LoadAsync(string sceneName)
        {
            if (isLoading)
                yield break;

            isLoading = true;

            loadingScreen.style.display = DisplayStyle.None;

            yield return StartCoroutine(FadeIn(root));

            yield return StartCoroutine(FadeIn(loadingScreen));

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            progressBar.style.width = 0f;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                progressBar.style.width = progress * barWidth;

                yield return null;
            }

            root.style.display = DisplayStyle.None;

            isLoading = false;
        }
    }
}
