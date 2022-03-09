using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SceneLoader : UI
    {
        private VisualElement loadingBar;
        private VisualElement progressBar;
        [SerializeField] private float width = 1350f;

        private static SceneLoader instance;
        public static SceneLoader Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;

            Initialize();

            loadingBar = root.Q<VisualElement>("LoadingBar");
            loadingBar.style.width = width;

            progressBar = root.Q<VisualElement>("ProgressBar");
            progressBar.style.width = width;
        }

        public void LoadLevel(string sceneName)
        {
            StartCoroutine(LoadAsync(sceneName));
        }

        private IEnumerator LoadAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            progressBar.style.width = 0f;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                progressBar.style.width = progress * width;

                yield return null;
            }
        }
    }
}
