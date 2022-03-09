using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SceneLoader : UI
    {
        private VisualElement loadingScreen;

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

            loadingBar = root.Q<VisualElement>("LoadingBar");
            loadingBar.style.width = barWidth;

            progressBar = root.Q<VisualElement>("ProgressBar");
            progressBar.style.width = 0f;

            root.style.display = DisplayStyle.None;
        }

        public void LoadLevel(VisualElement from, string sceneName)
        {
            StartCoroutine(LoadAsync(from, sceneName));
        }

        private IEnumerator LoadAsync(VisualElement from, string sceneName)
        {
            if (isLoading)
                yield break;

            isLoading = true;

            root.style.display = DisplayStyle.Flex;
            loadingScreen.style.display= DisplayStyle.None;

            yield return StartCoroutine(FadeTo(from, loadingScreen));

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            progressBar.style.width = 0f;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                progressBar.style.width = progress * barWidth;

                yield return null;
            }

            isLoading = false;
        }
    }
}
