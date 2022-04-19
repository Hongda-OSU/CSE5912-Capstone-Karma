using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SceneLoader : UI
    {
        [SerializeField] private bool isLoading = false;

        [SerializeField] private bool isPositionUpdatedOnLoad = false;
        private Vector3 positionOnLoad;

        private bool isFadingIn = true;

        private VisualElement loadingScreen;

        private VisualElement icon;
        private Label karmicLevelLabel;

        private static SceneLoader instance;
        public static SceneLoader Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            Initialize();

            loadingScreen = root.Q<VisualElement>("LoadingScreen");

            icon = loadingScreen.Q<VisualElement>("Icon");
            karmicLevelLabel = root.Q<Label>("KarmicLevel");

            root.style.display = DisplayStyle.None;
        }

        public void LoadLevel(int sceneIndex, int karmicLevel)
        {
            if (DropoffManager.Instance != null)
                DropoffManager.Instance.ClearDropoffs();

            StartCoroutine(LoadAsync(sceneIndex, karmicLevel));
        }

        private IEnumerator BlinkIcon(float time)
        {
            float totalTime = 0f;

            while (isLoading)
            {
                var opacity = icon.resolvedStyle.opacity;
                if (opacity >= 1f)
                {
                    isFadingIn = false;
                    totalTime = 0f;
                }
                else if (opacity <= 0f)
                {
                    isFadingIn = true;
                    totalTime = 0f;
                }

                totalTime += Time.deltaTime;
                var step = totalTime / time;

                if (isFadingIn)
                    icon.style.opacity = Mathf.Lerp(0f, 1f, step);
                else
                    icon.style.opacity = Mathf.Lerp(1f, 0f, step);

                yield return new WaitForSeconds(Time.deltaTime);
            }

        }
        private IEnumerator LoadAsync(int sceneIndex, int karmicLevel)
        {
            if (isLoading)
                yield break;

            icon.style.opacity = 0f;

            karmicLevelLabel.text = "Karmic Level: " + karmicLevel;

            isLoading = true;

            loadingScreen.style.display = DisplayStyle.None;

            yield return StartCoroutine(FadeIn(root));

            yield return StartCoroutine(FadeIn(loadingScreen));

            StartCoroutine(BlinkIcon(0.5f));
            yield return new WaitForSeconds(2f);

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
            while (!operation.isDone)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }

            root.style.display = DisplayStyle.None;

            isLoading = false;

            if (isPositionUpdatedOnLoad)
            {
                PlayerManager.Instance.Player.transform.position = positionOnLoad;
                isPositionUpdatedOnLoad = false;
            }

            if (sceneIndex == 0)
                yield break;

            PlayerStats.Instance.IsInvincible = false;

            FPSMouseLook.Instance.ResetLook();

            FPSControllerCC.Instance.AllowMoving(true);

            GameStateController.Instance.SetGameState(GameStateController.GameState.InGame);

            BgmControl.Instance.PlayCurrentBgm();
        }

        public void SetPositionOnLoad(Vector3 position)
        {
            isPositionUpdatedOnLoad = true;
            positionOnLoad = position;
        }

    }
}
