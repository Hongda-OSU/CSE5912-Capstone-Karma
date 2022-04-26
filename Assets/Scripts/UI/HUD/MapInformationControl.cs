using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace CSE5912.PolyGamers
{
    public class MapInformationControl : UI
    {
        [SerializeField] private float displayTime = 3f;

        [SerializeField] private float timeToSee = 4f;

        [SerializeField] private int previousSceneIndex = 0;
        [SerializeField] private int currentSceneIndex = 0;

        [SerializeField] private bool isDisplayed = false;

        private VisualElement mapInformation;
        private Label mapName;

        private VisualElement shade;

        private static MapInformationControl instance;
        public static MapInformationControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            Initialize();

            mapInformation = root.Q<VisualElement>("MapInformation");
            mapName = mapInformation.Q<Label>("MapName");

            shade = root.Q<VisualElement>("FullShade");

            mapInformation.style.display = DisplayStyle.None;
            mapInformation.style.opacity = 0f;

            shade.style.opacity = 0f;
        }

        private void Update()
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex != previousSceneIndex && currentSceneIndex != 0)
            {
                isDisplayed = false;
                StartCoroutine(DisplayMapName());
                previousSceneIndex = currentSceneIndex;
                //DataManager.Instance.Save();
            }
        }

        public IEnumerator DisplayMapName()
        {
            if (isDisplayed)
                yield break;

            isDisplayed = true;

            shade.style.display = DisplayStyle.Flex;
            shade.style.opacity = 1f;

            var timeSince = 0f;
            while (timeSince < timeToSee)
            {
                var delta = Time.deltaTime;
                shade.style.opacity = Mathf.Lerp(1f, 0f, timeSince / timeToSee);
                timeSince += delta;
                yield return new WaitForSeconds(delta);
            }
            shade.style.display = DisplayStyle.None;

            yield return new WaitForSeconds(2f);

            mapName.text = SceneManager.GetActiveScene().name;

            yield return StartCoroutine(FadeIn(mapInformation));

            yield return new WaitForSeconds(displayTime);

            yield return StartCoroutine(FadeOut(mapInformation));
        }
    }
}
